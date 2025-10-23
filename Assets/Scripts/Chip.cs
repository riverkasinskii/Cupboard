using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Collections.Generic;

public enum Direction 
{ 
    Up,
    Down,
    Left,
    Right
}

//public class Chip : MonoBehaviour, IBeginDragHandler, IEndDragHandler
//{
//    [field: SerializeField]
//    public RectTransform RectTransform { get; private set; }
//    public List<Direction> AllowedDirections { get; set; } = new();

//    public event Action<Chip> OnCurrentChipRequestDirection;
//    public event Action<Chip, Vector2> OnEndDragHandler; 

//    public GraphBuilder graphBuilder;
//    public Transform currentNode;
//    public float moveSpeed = 300f;

//    private RectTransform rect;
//    private Vector2 dragStartPos;
//    private bool isMoving = false;

//    private void Start()
//    {
//        rect = GetComponent<RectTransform>();

//        if (currentNode == null && graphBuilder != null)
//        {
//            float minDist = float.MaxValue;
//            foreach (var node in graphBuilder.Graph.Keys)
//            {
//                float d = Vector2.Distance(rect.anchoredPosition, node.GetComponent<RectTransform>().anchoredPosition);
//                if (d < minDist)
//                {
//                    minDist = d;
//                    currentNode = node;
//                }
//            }
//        }
//    }

//    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
//    {
//        if (isMoving) return;
//        dragStartPos = eventData.position;
//    }

//    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
//    {
//        if (isMoving || graphBuilder == null || currentNode == null) return;

//        Vector2 dragEndPos = eventData.position;
//        Vector2 delta = dragEndPos - dragStartPos;

//        if (delta.magnitude < 20f) return; // защита от случайных кликов

//        Direction direction;

//        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
//            direction = delta.x > 0 ? Direction.Right : Direction.Left;
//        else
//            direction = delta.y > 0 ? Direction.Up : Direction.Down;

//        TryMove(direction);
//    }

//    private void TryMove(Direction direction)
//    {
//        if (!graphBuilder.Graph.ContainsKey(currentNode)) return;

//        var connections = graphBuilder.Graph[currentNode];

//        foreach (var connection in connections)
//        {
//            if (connection.dir == direction)
//            {
//                StartCoroutine(MoveTo(connection.nextPoint));
//                return;
//            }
//        }

//        Debug.Log($"Нет пути {direction} из {currentNode.name}");
//    }

//    private IEnumerator MoveTo(Transform nextNode)
//    {
//        isMoving = true;

//        Vector2 start = rect.anchoredPosition;
//        Vector2 target = nextNode.GetComponent<RectTransform>().anchoredPosition;

//        while (Vector2.Distance(rect.anchoredPosition, target) > 0.1f)
//        {
//            rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, target, moveSpeed * Time.deltaTime);
//            yield return null;
//        }

//        rect.anchoredPosition = target;
//        currentNode = nextNode;
//        isMoving = false;
//    }
//}


public sealed class Chip : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action<Chip> OnCurrentChipRequestDirection;
    public event Action<Chip, Vector2> OnEndDragHandler;
    public List<Direction> AllowedDirections { get; set; } = new();

    [SerializeField]
    private float directionThreshold = 25f;

    [field: SerializeField]
    public RectTransform RectTransform { get; private set; }

    public GraphBuilder GraphBuilder { get; set; }

    [SerializeField]
    private PulsingAnimation _animation;

    [SerializeField]
    private float moveSpeed = 300f;

    private Vector2 startDragPosition;
    private Vector2 startPosition;
    private readonly Direction? currentDirection = null;
    private Transform currentNode;
    private bool isMoving = false;


    private void Start()
    {
        RectTransform = GetComponent<RectTransform>();

        if (currentNode == null && GraphBuilder != null)
        {
            float minDist = float.MaxValue;
            foreach (var node in GraphBuilder.Graph.Keys)
            {
                float d = Vector2.Distance(RectTransform.anchoredPosition, node.GetComponent<RectTransform>().anchoredPosition);
                if (d < minDist)
                {
                    minDist = d;
                    currentNode = node;
                }
            }
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        //startDragPosition = RectTransform.anchoredPosition;
        //startPosition = RectTransform.position;
        //currentDirection = null;
        if (isMoving) 
            return;
        startPosition = eventData.position;
        _animation.Highlight(true);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        //OnCurrentChipRequestDirection?.Invoke(this);
        //Vector2 delta = eventData.position - eventData.pressPosition;

        //if (currentDirection == null && delta.magnitude > directionThreshold)
        //{
        //    currentDirection = DetectDirection(delta);

        //    if (!AllowedDirections.Contains(currentDirection.Value))
        //    {
        //        currentDirection = null;
        //    }
        //}

        //if (currentDirection != null)
        //{
        //    Vector2 newPosition = startDragPosition;

        //    switch (currentDirection)
        //    {
        //        case Direction.Up:
        //            newPosition.y = startDragPosition.y + Mathf.Max(0, delta.y);
        //            break;
        //        case Direction.Down:
        //            newPosition.y = startDragPosition.y + Mathf.Min(0, delta.y);
        //            break;
        //        case Direction.Left:
        //            newPosition.x = startDragPosition.x + Mathf.Min(0, delta.x);
        //            break;
        //        case Direction.Right:
        //            newPosition.x = startDragPosition.x + Mathf.Max(0, delta.x);
        //            break;
        //    }
        //    RectTransform.anchoredPosition = newPosition;
        //}
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        //OnEndDragHandler?.Invoke(this, startPosition);
        if (isMoving || GraphBuilder == null || currentNode == null) return;

        Vector2 dragEndPos = eventData.position;
        Vector2 delta = dragEndPos - startPosition;

        if (delta.magnitude < 20f) 
            return;

        Direction direction;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            direction = delta.x > 0 ? Direction.Right : Direction.Left;
        else
            direction = delta.y > 0 ? Direction.Up : Direction.Down;

        TryMove(direction);
        _animation.Highlight(false);
    }

    private void TryMove(Direction direction)
    {
        if (!GraphBuilder.Graph.ContainsKey(currentNode)) 
            return;

        var connections = GraphBuilder.Graph[currentNode];

        foreach (var connection in connections)
        {
            if (connection.dir == direction)
            {
                StartCoroutine(MoveTo(connection.nextPoint));
                return;
            }
        }
    }

    private IEnumerator MoveTo(Transform nextNode)
    {
        isMoving = true;

        Vector2 start = RectTransform.anchoredPosition;
        Vector2 target = nextNode.GetComponent<RectTransform>().anchoredPosition;

        while (Vector2.Distance(RectTransform.anchoredPosition, target) > 0.1f)
        {
            RectTransform.anchoredPosition = Vector2.MoveTowards(RectTransform.anchoredPosition, target, moveSpeed * Time.deltaTime);
            yield return null;
        }

        RectTransform.anchoredPosition = target;
        currentNode = nextNode;
        isMoving = false;
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

