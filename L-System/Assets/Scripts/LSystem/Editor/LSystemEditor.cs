#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LSystem))]
public class LSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LSystem lSystem = (LSystem) target;

        if (Application.isPlaying)
        {
            GUI.enabled = true;
            
            lSystem.iterations = EditorGUILayout.IntSlider("Iterations", lSystem.iterations, 1, 10);
            lSystem.length = EditorGUILayout.IntSlider("Length", lSystem.length, 1, 10);
            lSystem.angle = EditorGUILayout.IntSlider("Angle", lSystem.angle, 1, 90);
            lSystem.width = EditorGUILayout.Slider("Width", lSystem.width, 0.01f, 1);
        }
        else
        {
            DrawDefaultInspector();
            
            GUI.enabled = false;
        }

        // these buttons are intractable only when application is playing
        if (GUILayout.Button("Generate New Tree"))
            lSystem.Generate();
        
        if (GUILayout.Button("Simulate Construction of Current Tree"))
        {
            // TODO : implement step by step construction
        }
    }
    
}

#endif