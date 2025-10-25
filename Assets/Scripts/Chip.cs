using System.Collections.Generic;
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
    [SerializeField] 
    private Canvas canvas;

    [SerializeField]
    private GraphBuilder graphBuilder;

    [SerializeField] 
    private float directionThreshold = 18f;

    [SerializeField] 
    private float nodeSnapDistance = 14f;

    private ChipInputHandler inputHandler;
    private ChipMover mover;
    private ChipGraphNavigator navigator;
    private NodeOccupancy occupancy;
    private ChipHighlighter highlighter;
    private PathChipHighlighter pathChipHighlighter;

    public void OnBeginDrag(PointerEventData eventData)
    {
        inputHandler.OnBeginDrag(eventData);
        pathChipHighlighter.HighlightAllowedNodesDeep();
    }

    public void OnDrag(PointerEventData eventData)
    {
        inputHandler.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inputHandler.OnEndDrag(eventData);
        pathChipHighlighter.ClearHighlights();
    }

    private void Awake()
    {
        var rect = GetComponent<RectTransform>();
        highlighter = GetComponent<ChipHighlighter>();
        occupancy = new NodeOccupancy();
        navigator = new ChipGraphNavigator(graphBuilder, occupancy);
        mover = new ChipMover(rect, nodeSnapDistance, navigator, occupancy);        
        inputHandler = new ChipInputHandler(canvas, directionThreshold, highlighter, mover, navigator);
        pathChipHighlighter = new PathChipHighlighter(mover, navigator);
    }

    private void Start()
    {
        mover.InitializeStartNode();
    }

    private void Update()
    {
        inputHandler.Update();
    }
}
