using UnityEngine;
using System.Collections.Generic;

public sealed class GraphBuilder : MonoBehaviour
{
    [SerializeField]
    private List<RectTransform> nodes;

    [SerializeField]
    private float connectionThreshold = 500f;

    private int countPairs;
        
    public Dictionary<Transform, List<(Transform nextPoint, Direction dir)>> Graph { get; private set; } = new();

    
    public void Install()
    {
        countPairs = GameInitializer.Config.countPairs;
        for (int i = 0; i < nodes.Count; i++)
        {
            if (countPairs < i)
            {
                nodes.RemoveAt(i);
            }
        }

        BuildGraph();
    }

    public void BuildGraph()
    {
        Graph.Clear();               
        
        foreach (var node in nodes)
        {
            List<(Transform, Direction)> connections = new();

            foreach (var other in nodes)
            {
                if (other == node) continue;

                Vector2 diff = other.anchoredPosition - node.anchoredPosition;

                if (Mathf.Abs(diff.x) <= connectionThreshold && Mathf.Abs(diff.y) < 5f && diff.x > 0)
                {
                    connections.Add((other, Direction.Right));
                }
                else if (Mathf.Abs(diff.x) <= connectionThreshold && Mathf.Abs(diff.y) < 5f && diff.x < 0)
                {
                    connections.Add((other, Direction.Left));
                }
                else if (Mathf.Abs(diff.y) <= connectionThreshold && Mathf.Abs(diff.x) < 5f && diff.y > 0)
                {
                    connections.Add((other, Direction.Up));
                }
                else if (Mathf.Abs(diff.y) <= connectionThreshold && Mathf.Abs(diff.x) < 5f && diff.y < 0)
                {
                    connections.Add((other, Direction.Down));
                }
            }
            Graph[node] = connections;
        }

        RemoveEdges();
    }

    private void RemoveEdges()
    {
        Transform firstNode = null;
        Transform secondNode = null;
        Transform seventhNode = null;
        Transform eighthNode = null;
        foreach (var node in nodes)
        {
            if (node.TryGetComponent(out Node nodeData))
            {
                switch (nodeData.NodeName)
                {
                    case NodeName.FirstNode:
                        firstNode = node;
                        break;
                    case NodeName.SecondNode:
                        secondNode = node;
                        Remove(firstNode, secondNode);
                        break;
                    case NodeName.ThirdNode:
                        Transform thirdNode = node;
                        Remove(secondNode, thirdNode);
                        break;
                    case NodeName.SeventhNode:
                        seventhNode = node;
                        break;
                    case NodeName.EighthNode:
                        eighthNode = node;
                        Remove(seventhNode, eighthNode);
                        break;
                    case NodeName.NinthNode:
                        Transform ninthNode = node;
                        Remove(eighthNode, ninthNode);
                        break;
                }
            }
        }
    }

    private void Remove(Transform firstNode, Transform secondNode)
    {
        if (firstNode != null && secondNode != null)
        {
            Graph[firstNode].RemoveAll(e => e.nextPoint == secondNode);
            Graph[secondNode].RemoveAll(e => e.nextPoint == firstNode);
        }
    }
}

