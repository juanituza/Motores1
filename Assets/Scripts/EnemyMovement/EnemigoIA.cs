using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemigoIA : MonoBehaviour
{
    public enum EstadoEnemigo { Patrullando, Persiguiendo, Cooldown }
    [Header("Estados")]
    public EstadoEnemigo estadoActual = EstadoEnemigo.Patrullando;

    [Header("Componentes")]
    private NavMeshAgent agent;
    public Transform jugador;

    [Header("Movimiento")]
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4.5f;

    [Header("Patrulla (Puntos por la casa)")]
    public List<Transform> puntosPatrulla;
    private int indicePatrullaActual = 0;
    private bool yendoHaciaAdelante = true;
    private bool esperandoEnPunto = false;

    [Header("Comportamiento Hello Neighbor")]
    [Tooltip("Tiempo mínimo que se queda quieto investigando el punto")]
    public float tiempoEsperaMin = 2f;
    [Tooltip("Tiempo máximo que se queda quieto investigando el punto")]
    public float tiempoEsperaMax = 4.5f;

    [Header("Detección Visual (Cono de Visión)")]
    public float rangoVision = 10f;
    [Range(0, 360)] public float anguloVision = 60f;
    public LayerMask capaJugador;
    public LayerMask capaObstaculos;

    [Header("Detección Auditiva")]
    public float rangoOido = 4f;

    [Header("Ataque y Cooldown")]
    public float distanciaAtaque = 1.5f;
    public float tiempoCooldown = 2.5f;
    private bool estaEnCooldown = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (puntosPatrulla.Count > 0)
        {
            // Va directo al primer punto asignado al iniciar
            agent.destination = puntosPatrulla[indicePatrullaActual].position;
        }
    }

    void Update()
    {
        if (estaEnCooldown) return;

        DeteccionPorOido();

        switch (estadoActual)
        {
            case EstadoEnemigo.Patrullando:
                ComportamientoPatrulla();
                EvaluarVision();
                break;

            case EstadoEnemigo.Persiguiendo:
                ComportamientoPersecucion();
                break;
        }

        // Llamada para controlar animaciones (opcional)
        ControlarAnimaciones();
    }

    // --- LÓGICA DE PATRULLA ---
    void ComportamientoPatrulla()
    {
        // Si está esperando quieto e investigando, detenemos la lógica de avance
        if (esperandoEnPunto) return;

        agent.speed = velocidadPatrulla;

        // Si llegó cerca del punto actual, inicia la pausa de investigación
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            StartCoroutine(EsperarEInvestigarPunto());
        }
    }

    IEnumerator EsperarEInvestigarPunto()
    {
        esperandoEnPunto = true;
        agent.isStopped = true; // Se detiene el agente

        // Elige un tiempo de espera aleatorio en el rango configurado
        float tiempoEspera = Random.Range(tiempoEsperaMin, tiempoEsperaMax);
        yield return new WaitForSeconds(tiempoEspera);

        // Si cambió de estado (nos vio/escuchó) durante la espera, cancelamos el flujo de patrulla
        if (estadoActual != EstadoEnemigo.Patrullando)
        {
            esperandoEnPunto = false;
            yield break;
        }

        // Una vez que esperó, calcula el siguiente punto en reversa o avance
        CalcularSiguienteIndice();

        // Asigna el nuevo destino y reanuda el movimiento
        if (puntosPatrulla.Count > 0)
        {
            agent.isStopped = false;
            agent.destination = puntosPatrulla[indicePatrullaActual].position;
        }

        esperandoEnPunto = false;
    }

    void CalcularSiguienteIndice()
    {
        if (puntosPatrulla.Count <= 1) return;

        if (yendoHaciaAdelante)
        {
            indicePatrullaActual++;
            // Si supera el límite de la lista, rebota hacia atrás
            if (indicePatrullaActual >= puntosPatrulla.Count)
            {
                yendoHaciaAdelante = false;
                indicePatrullaActual = Mathf.Max(0, puntosPatrulla.Count - 2);
            }
        }
        else
        {
            indicePatrullaActual--;
            // Si baja de 0, vuelve a avanzar hacia adelante
            if (indicePatrullaActual < 0)
            {
                yendoHaciaAdelante = true;
                indicePatrullaActual = Mathf.Min(puntosPatrulla.Count - 1, 1);
            }
        }
    }

    // --- LÓGICA DE DETECCIÓN ---
    void EvaluarVision()
    {
        Vector3 direccionAlJugador = (jugador.position - transform.position).normalized;
        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);

        if (distanciaAlJugador <= rangoVision)
        {
            if (Vector3.Angle(transform.forward, direccionAlJugador) < anguloVision / 2)
            {
                if (!Physics.Raycast(transform.position + Vector3.up, direccionAlJugador, distanciaAlJugador, capaObstaculos))
                {
                    DetectarAlJugador();
                }
            }
        }
    }

    void DeteccionPorOido()
    {
        if (estadoActual == EstadoEnemigo.Persiguiendo) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);
        if (distanciaAlJugador <= rangoOido)
        {
            DetectarAlJugador();
        }
    }

    void DetectarAlJugador()
    {
        // Si estaba quieto esperando en el punto de patrulla, cancela la espera inmediatamente
        if (esperandoEnPunto)
        {
            StopAllCoroutines();
            esperandoEnPunto = false;
            agent.isStopped = false;
        }

        estadoActual = EstadoEnemigo.Persiguiendo;
    }

    // --- LÓGICA DE PERSECUCIÓN Y GOLPE ---
    void ComportamientoPersecucion()
    {
        agent.speed = velocidadPersecucion;
        agent.destination = jugador.position;

        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);

        if (distanciaAlJugador <= distanciaAtaque)
        {
            StartCoroutine(RealizarAtaque());
        }

        // CORRECCIÓN: Si el jugador se sale del rango de visión, se pierde el rastro.
        // Anteriormente era * 1.5f, lo cual generaba el comportamiento de "freeze" en el borde.
        if (distanciaAlJugador > rangoVision)
        {
            VolverAPatrulla();
        }
    }

    IEnumerator RealizarAtaque()
    {
        estaEnCooldown = true;
        estadoActual = EstadoEnemigo.Cooldown;
        agent.isStopped = true;

        Debug.Log("¡HIT! El enemigo te golpeó.");

        yield return new WaitForSeconds(tiempoCooldown);

        agent.isStopped = false;
        estaEnCooldown = false;

        VolverAPatrulla();
    }

    void VolverAPatrulla()
    {
        estadoActual = EstadoEnemigo.Patrullando;

        // Al perder de vista al jugador, retoma la ruta desde el punto donde quedó configurado
        if (puntosPatrulla.Count > 0)
        {
            agent.destination = puntosPatrulla[indicePatrullaActual].position;
        }
    }

    // --- LÓGICA DE ANIMACIONES (Opcional) ---
    void ControlarAnimaciones()
    {
        // Ejemplo simple si ya tienes un Animator y quieres configurarlo
        // Animator anim = GetComponent<Animator>();
        // if(anim != null){
        //     anim.SetFloat("velocidadActual", agent.velocity.magnitude);
        //     anim.SetBool("estaEnCooldown", estaEnCooldown);
        //     anim.SetInteger("estadoActual", (int)estadoActual);
        // }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangoOido);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoVision);

        Vector3 fovLine1 = Quaternion.AngleAxis(anguloVision / 2, Vector3.up) * transform.forward;
        Vector3 fovLine2 = Quaternion.AngleAxis(-anguloVision / 2, Vector3.up) * transform.forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + Vector3.up, fovLine1 * rangoVision);
        Gizmos.DrawRay(transform.position + Vector3.up, fovLine2 * rangoVision);
    }
}