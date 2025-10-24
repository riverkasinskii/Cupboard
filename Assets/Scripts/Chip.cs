using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public enum Direction 
{ 
    Up,
    Down,
    Left,
    Right
}

public sealed class Chip : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Canvas Canvas { get; set; }

    public GraphBuilder GraphBuilder { get; set; }
        
    [SerializeField]
    private float directionThreshold = 1f;

    private RectTransform rectTransform;
    private bool isDragging = false;
    private Vector2 startDragPosition;
    private Vector2 lastMousePosition;
    private Transform currentNode;

    private readonly List<Direction> allowedDirections = new();
    private Direction? currentDirection = null;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
        if (currentNode == null)
            currentNode = GetClosestNode();

        UpdateAllowedDirections();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {        
        isDragging = true;
        startDragPosition = eventData.position;
        lastMousePosition = startDragPosition;
        currentDirection = null;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        currentDirection = null;
                
        Transform nearest = GetClosestNode();
        if (nearest != null)
        {
            rectTransform.anchoredPosition = nearest.GetComponent<RectTransform>().anchoredPosition;
            currentNode = nearest;
            UpdateAllowedDirections();
        }
    }

    private void Update()
    {
        if (!isDragging) 
            return;
                
        Vector2 currentMousePosition = ScreenToLocalInRectangle(Input.mousePosition);
        Vector2 previousMousePosition = ScreenToLocalInRectangle(lastMousePosition);                
        Vector2 deltaCanvas = currentMousePosition - previousMousePosition;
        lastMousePosition = Input.mousePosition;
        
        if (currentDirection == null)
        {            
            Vector3 lossyScale = Canvas.transform.lossyScale;
            if (lossyScale.x < 0f)
                deltaCanvas.x = -deltaCanvas.x;
            if (lossyScale.y < 0f)
                deltaCanvas.y = -deltaCanvas.y;

            if (deltaCanvas.magnitude > directionThreshold)
            {
                Direction guessedDirection;
                if (Mathf.Abs(deltaCanvas.x) >= Mathf.Abs(deltaCanvas.y))
                {
                    guessedDirection = (deltaCanvas.x >= 0f) ? Direction.Right : Direction.Left;
                }
                else
                {
                    guessedDirection = (deltaCanvas.y >= 0f) ? Direction.Up : Direction.Down;
                }

                Debug.Log(guessedDirection);
                // Проверяем, разрешено ли это направление графом
                if (allowedDirections.Contains(guessedDirection))
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

        // Если направление выбрано — двигаем фишку только по этой оси
        if (currentDirection != null)
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
        }
    }

    private Vector2 ScreenToLocalInRectangle(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Canvas.transform as RectTransform,
            screenPosition,
            Canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Canvas.worldCamera,
            out Vector2 localPosition
        );
        return localPosition;
    }


    // Получаем список разрешённых направлений
    private void UpdateAllowedDirections()
    {
        allowedDirections.Clear();

        if (currentNode == null || !GraphBuilder.Graph.ContainsKey(currentNode))
            return;

        foreach (var node in GraphBuilder.Graph[currentNode])
            allowedDirections.Add(node.dir);
    }

    // Находим ближайший узел (для фиксации при отпускании)
    private Transform GetClosestNode()
    {
        float minDist = float.MaxValue;
        Transform nearest = null;

        foreach (var node in GraphBuilder.Graph.Keys)
        {
            float d = Vector2.Distance(rectTransform.anchoredPosition, node.GetComponent<RectTransform>().anchoredPosition);
            if (d < minDist)
            {
                minDist = d;
                nearest = node;
            }
        }

        return nearest;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        //EventSystem
    }
}



