using System.Collections.Generic;
using UnityEngine;

public sealed class ChipController : MonoBehaviour
{
    [SerializeField]
    private RectTransform _worldBoundRectTransform;

    [SerializeField]
    private GraphBuilder _graphBuilder;

    [SerializeField]
    private Canvas _canvas;
        
    [SerializeField]
    private List<Chip> chips = new();

    [SerializeField]
    private ConnectionsBetweenPoints connectionsBetweenPoints;

    private readonly List<Direction> allowedDirections = new();
    private Transform currentNode;
    private Transform targetNode;        

    private void OnEnable()
    {
        foreach (var chip in chips)
        {
            chip.OnClampInsideWorldBounds += ClampInsideWorldBound;
            chip.OnGetConnectedNodeRequest += GetConnectedNode;
            chip.OnGetClosestNodeRequest += GetClosestNode;
            chip.OnUpdatedAllowedDirections += UpdateAllowedDirections;
            chip.OnContainedAllowedDirection += ContainsAllowedDirections;
            chip.OnScreenToLocalInRectangle += ScreenToLocalInRectangle;
            chip.OnGetCanvasLossyScale += GetCanvasLossyScale;
        }
    }

    private void OnDisable()
    {
        foreach (var chip in chips)
        {
            chip.OnClampInsideWorldBounds -= ClampInsideWorldBound;
            chip.OnGetConnectedNodeRequest -= GetConnectedNode;
            chip.OnGetClosestNodeRequest -= GetClosestNode;
            chip.OnUpdatedAllowedDirections -= UpdateAllowedDirections;
            chip.OnContainedAllowedDirection -= ContainsAllowedDirections;
            chip.OnScreenToLocalInRectangle -= ScreenToLocalInRectangle;
            chip.OnGetCanvasLossyScale -= GetCanvasLossyScale;
        }
    }

    private Vector3 GetCanvasLossyScale() => _canvas.transform.lossyScale;

    private void ClampInsideWorldBound(RectTransform rectTransform)
    {
        Vector2 localPos = _worldBoundRectTransform.InverseTransformPoint(rectTransform.position);

        Rect mapBounds = _worldBoundRectTransform.rect;

        Vector2 chipSizeWorld = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);
        Vector2 mapScaleInv = new(
            1f / Mathf.Max(0.0001f, _worldBoundRectTransform.lossyScale.x),
            1f / Mathf.Max(0.0001f, _worldBoundRectTransform.lossyScale.y)
        );
        Vector2 chipSizeInMap = Vector2.Scale(chipSizeWorld, mapScaleInv);
        Vector2 half = chipSizeInMap * 0.5f;

        localPos.x = Mathf.Clamp(localPos.x, mapBounds.xMin + half.x, mapBounds.xMax - half.x);
        localPos.y = Mathf.Clamp(localPos.y, mapBounds.yMin + half.y, mapBounds.yMax - half.y);

        rectTransform.position = _worldBoundRectTransform.TransformPoint(localPos);
    }

    private bool GetConnectedNode(Direction direction)
    {
        if (_graphBuilder.Graph.TryGetValue(currentNode, out var connects))
        {
            foreach (var connect in connects)
            {
                if (connect.dir == direction)
                {
                    targetNode = connect.nextPoint;
                    return true;
                }                   
            }                
        }
        return false;
    }

    private void UpdateAllowedDirections()
    {
        allowedDirections.Clear();

        if (currentNode == null || !_graphBuilder.Graph.ContainsKey(currentNode))
            return;

        foreach (var node in _graphBuilder.Graph[currentNode])
            allowedDirections.Add(node.dir);
    }

    private Transform GetClosestNode(Vector2 position)
    {
        float minDistance = float.MaxValue;
        
        foreach (var node in _graphBuilder.Graph.Keys)
        {
            float distance = Vector2.Distance(position, node.GetComponent<RectTransform>().anchoredPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                currentNode = node;
            }
        }        
        return currentNode;
    }

    private bool ContainsAllowedDirections(Direction direction)
    {
        return allowedDirections.Contains(direction);
    }

    private Vector2 ScreenToLocalInRectangle(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            screenPosition,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out Vector2 localPosition
        );
        return localPosition;
    }
}
