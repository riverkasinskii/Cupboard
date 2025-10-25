using System.Collections.Generic;
using UnityEngine;

public sealed class PathChipHighlighter
{
    private readonly List<NodeHighlighter> _nodeHighlighters = new();
    private readonly ChipMover _chipMover;
    private readonly ChipGraphNavigator _chipGraphNavigator;

    private Transform currentNode;
    private GraphBuilder graphBuilder;

    public PathChipHighlighter(ChipMover chipMover, ChipGraphNavigator chipGraphNavigator)
    {
        _chipMover = chipMover;
        _chipGraphNavigator = chipGraphNavigator;
    }
    private void Injector()
    {
        currentNode = _chipMover.CurrentNode;
        graphBuilder = _chipGraphNavigator.GraphBuilder;
    }

    public void ClearHighlights()
    {
        for (int i = 0; i < _nodeHighlighters.Count; i++)
        {
            if (_nodeHighlighters[i])
                _nodeHighlighters[i].Hide();
        }
        _nodeHighlighters.Clear();
    }


    public void HighlightAllowedNodesDeep()
    {
        Injector();
        ClearHighlights();

        if (!currentNode) 
            return;

        if (!graphBuilder.Graph.TryGetValue(currentNode, out var connects)) 
            return;
                
        foreach (var connect in connects)
        {
            if (connect.nextPoint == null) 
                continue;

            WalkAndHighlightChain(currentNode, connect.dir, maxDepth: 64);
        }
    }
        
    private void WalkAndHighlightChain(Transform start, Direction dir, int maxDepth = 64)
    {
        var visited = new HashSet<Transform> 
        { 
            start 
        };

        Transform node = start;
        int depth = 0;

        while (depth < maxDepth)
        {            
            if (!graphBuilder.Graph.TryGetValue(node, out var list)) 
                break;

            Transform next = null;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].dir == dir)
                {
                    next = list[i].nextPoint;
                    break;
                }
            }

            if (next == null) 
                break;         
            
            if (!visited.Add(next)) 
                break;        
                        
            var nextNodeComp = next.GetComponent<Node>();
            bool occupied = nextNodeComp != null && nextNodeComp.Occupied;

            
            if (next.TryGetComponent<NodeHighlighter>(out var highlighter))
            {
                if (!occupied)
                {                    
                    highlighter.ShowAllowed(depthScale: Mathf.Clamp01(1f - depth * 0.15f));
                }
                else
                {
                    highlighter.ShowBlocked();
                }
                _nodeHighlighters.Add(highlighter);
            }
                        
            if (occupied) 
                break;
                        
            node = next;
            depth++;
        }
    }        
}
