using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

public class SceneExporter
{
    // Adds a new menu item to Unity's top toolbar
    [MenuItem("Tools/Export Scene Structure")]
    public static void Export()
    {
        StringBuilder sb = new StringBuilder();
        Scene activeScene = SceneManager.GetActiveScene();
        
        sb.AppendLine($"=== Scene: {activeScene.name} ===");
        
        // Iterate through all root objects in the active scene
        foreach (GameObject rootObj in activeScene.GetRootGameObjects())
        {
            DumpGameObject(rootObj, sb, "");
        }
        
        // Save the output to a text file in the project's root directory
        string path = "SceneStructure.txt";
        File.WriteAllText(path, sb.ToString());
        Debug.Log($"Scene exported successfully to: {path}");
        // Opens the text file automatically using the OS default text editor
        EditorUtility.OpenWithDefaultApp(path);
    }

    // Recursive function to process children and their attached components
    static void DumpGameObject(GameObject go, StringBuilder sb, string indent)
    {
        sb.AppendLine($"{indent}- {go.name}");
        
        // Get all components attached to the current GameObject
        Component[] components = go.GetComponents<Component>();
        foreach (var comp in components)
        {
            // Skip the Transform component since every GameObject has one by default
            if (comp != null && comp.GetType() != typeof(Transform)) 
            {
                sb.AppendLine($"{indent}  [Component: {comp.GetType().Name}]");
            }
        }
        
        // Recursively process all child objects
        foreach (Transform child in go.transform)
        {
            DumpGameObject(child.gameObject, sb, indent + "  ");
        }
    }
}