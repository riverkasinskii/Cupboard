using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Direction 
{ 
    Up,
    Down,
    Left,
    Right
}

public sealed class Chip : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action<Chip> OnCurrentChipRequestDirection;
    public event Action<Chip, Vector2> OnEndDragHandler;    
    public List<Direction> AllowedDirections { get; set; } = new();

    [SerializeField]
    private float directionThreshold = 25f;

    [field: SerializeField]
    public RectTransform RectTransform { get; private set; }

    [SerializeField]
    private PulsingAnimation _animation;

    private Vector2 startDragPosition;
    private Vector2 startPosition;
    private Direction? currentDirection = null;    
                
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        startDragPosition = RectTransform.anchoredPosition;
        startPosition = RectTransform.position;
        currentDirection = null;
        _animation.Highlight(true);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        OnCurrentChipRequestDirection?.Invoke(this);
        Vector2 delta = eventData.position - eventData.pressPosition;
                
        if (currentDirection == null && delta.magnitude > directionThreshold)
        {
            currentDirection = DetectDirection(delta);
                        
            if (!AllowedDirections.Contains(currentDirection.Value))
            {
                currentDirection = null;
            }               
        }

        if (currentDirection != null)
        {
            Vector2 newPosition = startDragPosition;

            switch (currentDirection)
            {
                case Direction.Up:
                    newPosition.y = startDragPosition.y + Mathf.Max(0, delta.y);
                    break;
                case Direction.Down:
                    newPosition.y = startDragPosition.y + Mathf.Min(0, delta.y);                    
                    break;
                case Direction.Left:
                    newPosition.x = startDragPosition.x + Mathf.Min(0, delta.x);
                    break;
                case Direction.Right:
                    newPosition.x = startDragPosition.x + Mathf.Max(0, delta.x);
                    break;
            }
            RectTransform.anchoredPosition = newPosition;
        }
    }
        
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        OnEndDragHandler?.Invoke(this, startPosition);
        _animation.Highlight(false);
    }

    private Direction DetectDirection(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            return delta.x > 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            return delta.y > 0 ? Direction.Up : Direction.Down;
        }
    }        
}

