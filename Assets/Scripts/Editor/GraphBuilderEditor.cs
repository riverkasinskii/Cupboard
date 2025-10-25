using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GraphBuilder))]
public sealed class GraphBuilderEditor : Editor
{
    private void OnSceneGUI()
    {
        GraphBuilder builder = (GraphBuilder)target;
                
        Handles.color = Color.cyan;

        foreach (var kvp in builder.Graph)
        {
            Transform from = kvp.Key;
            if (from == null)
            {
                continue;
            }

            foreach (var connection in kvp.Value)
            {
                Transform to = connection.nextPoint;
                if (to == null) 
                {
                    continue; 
                }
                                
                Handles.DrawLine(from.position, to.position);
                                
                Vector3 mid = (from.position + to.position) / 2f;
                Vector3 dir = (to.position - from.position).normalized * 0.2f;
                Handles.ArrowHandleCap(0, mid, Quaternion.LookRotation(Vector3.forward, dir), 0.2f, EventType.Repaint);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GraphBuilder builder = (GraphBuilder)target;

        if (GUILayout.Button("Построить граф"))
        {
            builder.BuildGraph();
        }
                
        if (builder.Graph.Count > 0)
        {
            GUILayout.Label($"Узлов в графе: {builder.Graph.Count}", EditorStyles.boldLabel);
        }
    }
}

