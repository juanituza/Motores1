Shader "Custom/StormSkybox"
{
    Properties
    {
        _SkyColorTop ("Sky Top", Color) = (0.02, 0.02, 0.04, 1)
        _SkyColorHorizon ("Sky Horizon", Color) = (0.06, 0.06, 0.08, 1)
        _CloudColor ("Cloud Color", Color) = (0.08, 0.08, 0.1, 1)
        _CloudDensity ("Cloud Density", Range(0,5)) = 0.6
        _CloudSpeed ("Cloud Speed", Float) = 0.01
        _CloudScale ("Cloud Scale", Float) = 3.0
        _LightningColor ("Lightning Color", Color) = (0.85, 0.9, 1.0, 1)
        _LightningIntensity ("Lightning Intensity", Float) = 8.0
        _LightningSpeed ("Lightning Speed", Float) = 0.7
        _LightningThreshold ("Lightning Threshold", Range(0.5,1.0)) = 0.978
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "RenderPipeline"="UniversalPipeline" }
        Cull Off
        ZWrite Off
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" "RenderPipeline"="UniversalPipeline" }
        Cull Off
        ZWrite Off
        ZTest LEqual  // ← agregá esta línea
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos   : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _SkyColorTop, _SkyColorHorizon;
                float4 _CloudColor;
                float  _CloudDensity, _CloudSpeed, _CloudScale;
                float4 _LightningColor;
                float  _LightningIntensity, _LightningSpeed, _LightningThreshold;
            CBUFFER_END

            // ── Noise helpers ──────────────────────────────────────────
            float2 hash2(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)),
                           dot(p, float2(269.5, 183.3)));
                return frac(sin(p) * 43758.5453);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(dot(hash2(i),              f - float2(0,0)),
                                 dot(hash2(i + float2(1,0)), f - float2(1,0)), u.x),
                            lerp(dot(hash2(i + float2(0,1)), f - float2(0,1)),
                                 dot(hash2(i + float2(1,1)), f - float2(1,1)), u.x), u.y);
            }

            // FBM: varias octavas de ruido → nubes detalladas
            float fbm(float2 p)
            {
                float v = 0.0, a = 0.5;
                for (int i = 0; i < 5; i++)
                {
                    v += a * noise(p);
                    p  = p * 2.0 + float2(3.1, 1.7);
                    a *= 0.5;
                }
                return v;
            }

            // ── Relámpago ──────────────────────────────────────────────
            // Genera un "evento" de relámpago cada cierto tiempo
            float lightning(float time)
            {
                // Ciclo de ~4-8 segundos entre relámpagos
                float cycle = frac(time * _LightningSpeed * 0.13);
                float trigger = step(_LightningThreshold, frac(sin(floor(time * _LightningSpeed * 0.13) * 127.4) * 4375.5));

                // Flash rápido con doble pulso (más realista)
                float flash  = exp(-cycle * 18.0) + exp(-(cycle - 0.08) * 22.0) * 0.4;
                return flash * trigger;
            }

            // ── Vertex ────────────────────────────────────────────────
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldPos   = IN.positionOS.xyz;
                return OUT;
            }

            // ── Fragment ──────────────────────────────────────────────
            half4 frag(Varyings IN) : SV_Target
            {
                float3 dir = normalize(IN.worldPos);
                float  t   = _Time.y;

                // Gradiente vertical
                float horizon = saturate(dir.y * 2.5 + 0.15);
                float3 sky = lerp(_SkyColorHorizon.rgb, _SkyColorTop.rgb, horizon);

                // Nubes con movimiento
                float2 cloudUV = float2(atan2(dir.x, dir.z), dir.y) * _CloudScale;
                cloudUV.x += t * _CloudSpeed;

                float cloud = fbm(cloudUV);
                cloud = smoothstep(1.0 - _CloudDensity, 1.0, cloud);
                cloud *= saturate(dir.y * 4.0 + 0.5); // desaparecen en horizonte

                sky = lerp(sky, _CloudColor.rgb, cloud);

                // Relámpago
                float bolt = lightning(t);
                // Las nubes amplifican el destello (iluminación desde adentro)
                float cloudAmp = lerp(0.3, 1.0, cloud);
                sky += _LightningColor.rgb * bolt * _LightningIntensity * cloudAmp;

                return half4(sky, 1.0);
            }
            ENDHLSL
        }
    }
}