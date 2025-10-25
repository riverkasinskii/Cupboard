using UnityEngine;
using UnityEngine.EventSystems;

public sealed class ChipInputHandler : IBeginDragHandler, IEndDragHandler, IDragHandler
{    
    private readonly Canvas _canvas;
    private readonly float _threshold;
    private readonly ChipHighlighter _highlighter;
    private readonly ChipMover _mover;
    private readonly ChipGraphNavigator _navigator;

    private bool isDragging;
    private Vector2 dragStart, dragLast;
    private Direction? fixedDirection;

    public ChipInputHandler(Canvas canvas, float threshold,
        ChipHighlighter highlighter, ChipMover mover, ChipGraphNavigator navigator)
    {
        _canvas = canvas;
        _threshold = threshold;
        _highlighter = highlighter;
        _mover = mover;
        _navigator = navigator;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        dragStart = eventData.position;
        dragLast = eventData.position;
        fixedDirection = null;
        _highlighter.Highlight(true);       
    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        fixedDirection = null;
        _highlighter.Highlight(false);
        _mover.SnapToCurrentNode();        
    }

    public void Update()
    {
        if (!isDragging) 
            return;

        Vector2 curr = ScreenToLocal(Input.mousePosition);
        Vector2 prev = ScreenToLocal(dragLast);
        Vector2 delta = curr - prev;
        dragLast = Input.mousePosition;

        if (fixedDirection == null)
        {
            Vector2 total = curr - ScreenToLocal(dragStart);
            if (total.magnitude >= _threshold)
            {
                fixedDirection = GuessDirection(total);
                _mover.TryStartMove(fixedDirection.Value);
            }
        }

        _mover.ContinueMove(delta);
    }

    private Direction GuessDirection(Vector2 total)
    {
        return Mathf.Abs(total.x) > Mathf.Abs(total.y)
            ? (total.x > 0 ? Direction.Right : Direction.Left)
            : (total.y > 0 ? Direction.Up : Direction.Down);
    }
        
    private Vector2 ScreenToLocal(Vector2 screen)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            screen,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out Vector2 local
        );
        return local;
    }            
}

