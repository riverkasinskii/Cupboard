using UnityEngine;

public sealed class NodeOccupancy
{
    public void Mark(Transform node, bool value)
    {
        var occupiedNode = node ? node.GetComponent<Node>() : null;
        if (occupiedNode == null)
            return;

        if (value)        
            occupiedNode.SetOccupied(null);
        
        else        
            occupiedNode.Clear();        
    }
}

