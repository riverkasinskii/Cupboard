using UnityEngine;

public sealed class ChipGraphNavigator
{    
    public GraphBuilder GraphBuilder { get; private set; }

    private readonly NodeOccupancy _occupancy;    

    public ChipGraphNavigator(GraphBuilder graphBuilder, NodeOccupancy occupancy)
    {
        GraphBuilder = graphBuilder;
        _occupancy = occupancy;        
    }

    public Transform GetFreeNeighbor(Transform node, Direction direction)
    {
        if (!GraphBuilder.Graph.TryGetValue(node, out var connections)) 
            return null;

        foreach (var item in connections)
        {
            if (item.dir != direction) 
                continue;

            var neighbor = item.nextPoint?.GetComponent<Node>();
            if (neighbor == null || !neighbor.Occupied)
                return item.nextPoint;
        }
        return null;
    }
        
    public Vector2 FilterMovement(Vector2 delta, Direction direction)
    {
        return direction switch
        {
            Direction.Right => new Vector2(Mathf.Max(0, delta.x), 0),
            Direction.Left => new Vector2(Mathf.Min(0, delta.x), 0),
            Direction.Up => new Vector2(0, Mathf.Max(0, delta.y)),
            Direction.Down => new Vector2(0, Mathf.Min(0, delta.y)),
            _ => Vector2.zero
        };
    }

    public Transform FindNearestNode(Vector2 pos)
    {
        float min = float.MaxValue;
        Transform nearest = null;
        foreach (var kv in GraphBuilder.Graph)
        {
            var rect = kv.Key as RectTransform;
            float d = Vector2.Distance(pos, rect.anchoredPosition);
            if (d < min)
            {
                min = d;
                nearest = kv.Key;
            }
        }
        return nearest;
    }        
}

