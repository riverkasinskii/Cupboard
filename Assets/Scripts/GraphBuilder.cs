using UnityEngine;
using System.Collections.Generic;

public sealed class GraphBuilder : MonoBehaviour
{    
    [field: SerializeField]
    public Transform NodesParent { get; private set; }

    public Dictionary<Transform, List<(Transform nextPoint, Direction dir)>> Graph { get; private set; } = new();

    [SerializeField]
    private float connectionThreshold = 120f;
        
    private void Awake()
    {
        BuildGraph();
    }

    public void BuildGraph()
    {
        Graph.Clear();
        List<RectTransform> nodes = new();

        foreach (Transform child in NodesParent)
        {
            nodes.Add(child.GetComponent<RectTransform>());
        }

        foreach (var node in nodes)
        {
            List<(Transform, Direction)> connections = new();

            foreach (var other in nodes)
            {
                if (other == node) continue;

                Vector2 diff = other.anchoredPosition - node.anchoredPosition;

                if (Mathf.Abs(diff.x) <= connectionThreshold && Mathf.Abs(diff.y) < 5f && diff.x > 0)
                    connections.Add((other, Direction.Right));
                else if (Mathf.Abs(diff.x) <= connectionThreshold && Mathf.Abs(diff.y) < 5f && diff.x < 0)
                    connections.Add((other, Direction.Left));
                else if (Mathf.Abs(diff.y) <= connectionThreshold && Mathf.Abs(diff.x) < 5f && diff.y > 0)
                    connections.Add((other, Direction.Up));
                else if (Mathf.Abs(diff.y) <= connectionThreshold && Mathf.Abs(diff.x) < 5f && diff.y < 0)
                    connections.Add((other, Direction.Down));
            }

            Graph[node] = connections;
        }

        Debug.Log($"Граф собран: {Graph.Count} узлов");

        Transform firstPoint = NodesParent.Find("FirstPoint");
        Transform secondPoint = NodesParent.Find("SecondPoint");
        Transform thirdPoint = NodesParent.Find("ThirdPoint");
        Transform seventhPoint = NodesParent.Find("SeventhPoint");
        Transform eighthPoint = NodesParent.Find("EighthPoint");
        Transform ninthPoint = NodesParent.Find("NinthPoint");
        
        RemoveEdges(firstPoint, secondPoint);
        RemoveEdges(secondPoint, thirdPoint);
        RemoveEdges(seventhPoint, eighthPoint);
        RemoveEdges(eighthPoint, ninthPoint);
    }

    private void RemoveEdges(Transform firstPoint, Transform secondPoint)
    {        
        if (firstPoint != null && secondPoint != null)
        {
            Graph[firstPoint].RemoveAll(e => e.nextPoint == secondPoint);
            Graph[secondPoint].RemoveAll(e => e.nextPoint == firstPoint);            
        }
    }
}

