using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Direction 
{ 
    Up,
    Down,
    Left,
    Right,
    NoDirection
}

public sealed class Chip : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public event Action<RectTransform> OnClampInsideWorldBounds;
    public event Action OnUpdatedAllowedDirections;

    public event Predicate<Direction> OnGetConnectedNodeRequest;    
    public event Predicate<Direction> OnContainedAllowedDirection;

    public event Func<Vector2, Transform> OnGetClosestNodeRequest;
    public event Func<Vector2, Vector2> OnScreenToLocalInRectangle;
    public event Func<Vector3> OnGetCanvasLossyScale;

    [SerializeField]
    private float directionThreshold = 1f;

    [SerializeField]
    private RectTransform rectTransform;

    private bool isDragging = false;
    private Vector2 startDragPosition;
    private Vector2 lastMousePosition;        
        
    private Direction currentDirection = Direction.NoDirection;

    private void Start()
    {                        
        OnGetClosestNodeRequest(rectTransform.anchoredPosition);
        OnUpdatedAllowedDirections?.Invoke();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {        
        isDragging = true;        
        startDragPosition = eventData.position;
        lastMousePosition = startDragPosition;
        currentDirection = Direction.NoDirection;        
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        currentDirection = Direction.NoDirection;        

        Transform nearNode = OnGetClosestNodeRequest(rectTransform.anchoredPosition);
        if (nearNode != null)
        {
            rectTransform.anchoredPosition = nearNode.GetComponent<RectTransform>().anchoredPosition;            
            OnUpdatedAllowedDirections?.Invoke();
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        //EventSystem
    }

    private void Update()
    {
        if (!isDragging) 
            return;

        Direction guessedDirection = Direction.NoDirection;
        Vector2 currentMousePosition = OnScreenToLocalInRectangle(Input.mousePosition);
        Vector2 previousMousePosition = OnScreenToLocalInRectangle(lastMousePosition);                
        Vector2 deltaCanvas = currentMousePosition - previousMousePosition;
        lastMousePosition = Input.mousePosition;
        
        if (currentDirection == Direction.NoDirection)
        {            
            Vector3 lossyScale = OnGetCanvasLossyScale.Invoke();
            if (lossyScale.x < 0f)
                deltaCanvas.x = -deltaCanvas.x;
            if (lossyScale.y < 0f)
                deltaCanvas.y = -deltaCanvas.y;

            if (deltaCanvas.magnitude > directionThreshold)
            {               
                
                if (Mathf.Abs(deltaCanvas.x) >= Mathf.Abs(deltaCanvas.y))
                {
                    guessedDirection = (deltaCanvas.x >= 0f) ? Direction.Right : Direction.Left;
                }
                else
                {
                    guessedDirection = (deltaCanvas.y >= 0f) ? Direction.Up : Direction.Down;
                }                
                
                if (OnContainedAllowedDirection.Invoke(guessedDirection))
                {                    
                    currentDirection = guessedDirection;                    
                }
                else
                {                    
                    isDragging = false;
                    return;
                }
            }
        }
                
        if (currentDirection != Direction.NoDirection && !OnGetConnectedNodeRequest.Invoke(guessedDirection))
        {
            Vector2 move = Vector2.zero;           

            switch (currentDirection)
            {
                case Direction.Right:
                    move.x = Mathf.Max(0, deltaCanvas.x);
                    break;
                case Direction.Left:
                    move.x = Mathf.Min(0, deltaCanvas.x);
                    break;
                case Direction.Up:
                    move.y = Mathf.Max(0, deltaCanvas.y);
                    break;
                case Direction.Down:
                    move.y = Mathf.Min(0, deltaCanvas.y);
                    break;
            }

            rectTransform.anchoredPosition += move;

            OnClampInsideWorldBounds?.Invoke(rectTransform);            
        }
    }      
}



