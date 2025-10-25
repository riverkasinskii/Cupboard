using UnityEngine;

public sealed class ChipMover
{    
    private readonly RectTransform _rect;
    private readonly float _snapDistance;
    private readonly ChipGraphNavigator _navigator;
    private readonly NodeOccupancy _occupancy;

    public Transform CurrentNode { get; private set; }
    private Transform targetNode;
    private Direction? currentDirection;

    public ChipMover(RectTransform rect, float snapDistance,
        ChipGraphNavigator navigator, NodeOccupancy occupancy)
    {
        _rect = rect;
        _snapDistance = snapDistance;
        _navigator = navigator;
        _occupancy = occupancy;
    }

    public void InitializeStartNode()
    {
        CurrentNode = _navigator.FindNearestNode(_rect.anchoredPosition);
        _occupancy.Mark(CurrentNode, true);        
    }

    public void TryStartMove(Direction direction)
    {
        var neighbor = _navigator.GetFreeNeighbor(CurrentNode, direction);
        if (neighbor != null)
        {
            currentDirection = direction;
            targetNode = neighbor;
        }
    }

    public void ContinueMove(Vector2 delta)
    {
        if (currentDirection == null || targetNode == null) 
            return;

        Vector2 move = _navigator.FilterMovement(delta, currentDirection.Value);
        Vector2 next = _rect.anchoredPosition + move;

        Vector2 firstPos = CurrentNode.GetComponent<RectTransform>().anchoredPosition;
        Vector2 secondPos = targetNode.GetComponent<RectTransform>().anchoredPosition;

        _rect.anchoredPosition = ClampToSegment(next, firstPos, secondPos);

        if (Vector2.Distance(_rect.anchoredPosition, secondPos) <= _snapDistance)
        {
            _occupancy.Mark(CurrentNode, false);
            CurrentNode = targetNode;
            _occupancy.Mark(CurrentNode, true);

            _rect.anchoredPosition = secondPos;
            currentDirection = null;
            targetNode = null;            
        }
    }

    public void SnapToCurrentNode()
    {
        if (CurrentNode)
            _rect.anchoredPosition = CurrentNode.GetComponent<RectTransform>().anchoredPosition;
    }

    private Vector2 ClampToSegment(Vector2 pos, Vector2 firstPos, Vector2 secondPos)
    {
        Vector2 deltaPos = secondPos - firstPos;
        float magnitude = deltaPos.magnitude;
        if (magnitude < 0.001f) 
            return firstPos;

        Vector2 direction = deltaPos / magnitude;
        float temp = Mathf.Clamp(Vector2.Dot(pos - firstPos, direction), 0, magnitude);
        return firstPos + direction * temp;
    }        
}

