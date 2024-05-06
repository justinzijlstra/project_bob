using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class JK_ChangeChildTextures : MonoBehaviour
{
    [System.Serializable]
    public class ChildTextureInfo
    {
        public string childObjectName;
        public Texture newMainTexture;
    }

    public List<ChildTextureInfo> childTextureList = new List<ChildTextureInfo>();

#if UNITY_EDITOR
    [CustomEditor(typeof(JK_ChangeChildTextures))]
    public class ChangeChildTexturesEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            JK_ChangeChildTextures script = (JK_ChangeChildTextures)target;

            GUILayout.Space(10);

            if (GUILayout.Button("Change Child Textures"))
            {
                script.ChangeTextures();
            }
        }
    }

    void ChangeTextures()
    {
        int id = -1;
        foreach (var childTextureInfo in childTextureList)
        {
            id++;
            Transform childTransform = FindChildByName(transform, childTextureInfo.childObjectName);

            if (childTransform != null)
            {
                Renderer renderer = childTransform.GetComponent<Renderer>();
                if (renderer != null)
                {

                   if( childTextureInfo.newMainTexture == null)
                    {
                        Debug.Log("No texture found for " + childTextureInfo.childObjectName + ". switched off renderer component");
                        renderer.enabled = false;
                        continue;
                    }

                    renderer.enabled = true;
                    Material originalMaterial = renderer.sharedMaterial;

                    // Create a new material as a copy of the original
                    Material newMaterial = new Material(originalMaterial);
                    newMaterial.mainTexture = childTextureInfo.newMainTexture;

                    // Save the new material as an asset
                    string assetPath = "Assets/Bob popcorn/Bobs props/newMaterials/" + id + "_" + gameObject.name + ".mat";
                    AssetDatabase.CreateAsset(newMaterial, assetPath);

                    // Assign the new material to the renderer
                    renderer.material = newMaterial;

                    // You might want to save the asset if you're modifying a material in the Editor
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(renderer);
                    UnityEditor.AssetDatabase.SaveAssets();
#endif
                }
            }
        }
    }

    Transform FindChildByName(Transform parent, string name)
    {
        Transform child = parent.Find(name);

        if (child != null)
        {
            return child;
        }

        foreach (Transform nestedChild in parent)
        {
            child = FindChildByName(nestedChild, name);
            if (child != null)
            {
                return child;
            }
        }

        return null;
    }
#endif
}
