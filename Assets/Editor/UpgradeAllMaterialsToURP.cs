using UnityEngine;
using UnityEditor;

public class UpgradeAllMaterialsToURP : EditorWindow
{
    [MenuItem("Tools/Upgrade All Materials to URP Lit")]
    static void UpgradeAllMaterials()
    {
        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");

        if (urpLit == null)
        {
            Debug.LogError("No se encontró el shader URP/Lit. ¿Está instalado URP?");
            return;
        }

        // Busca todos los materiales en el proyecto
        string[] guids = AssetDatabase.FindAssets("t:Material");
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat == null) continue;

            // Opcional: saltear los que ya tienen URP
            if (mat.shader == urpLit) continue;

            mat.shader = urpLit;
            EditorUtility.SetDirty(mat);
            count++;

            Debug.Log($"Actualizado: {path}");
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"✅ {count} materiales actualizados a URP/Lit");
    }
}