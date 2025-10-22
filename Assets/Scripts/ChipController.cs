using System.Collections.Generic;
using UnityEngine;

public sealed class ChipController : MonoBehaviour
{
    [SerializeField]
    private float endDragOffset = 40f;

    [SerializeField]
    private List<Chip> chips = new();

    [SerializeField]
    private ConnectionsBetweenPoints connectionsBetweenPoints;

    private void OnEnable()
    {
        foreach (var chip in chips)
        {
            chip.OnCurrentChipRequestDirection += CurrentChipRequestDirection;
            chip.OnEndDragHandler += EndDragHandler;
        }
    }
        
    private void OnDisable()
    {
        foreach (var chip in chips)
        {
            chip.OnCurrentChipRequestDirection -= CurrentChipRequestDirection;
            chip.OnEndDragHandler -= EndDragHandler;
        }
    }

    private void CurrentChipRequestDirection(Chip chip)
    {
        List<Direction> directions = connectionsBetweenPoints.GetAllowedDirections(chip.RectTransform.position);
        if (directions != null)
        {
            chip.AllowedDirections = directions;
        }        
    }

    private void EndDragHandler(Chip chip, Vector2 startPosition)
    {        
        List<Point> points = connectionsBetweenPoints.Points;
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 screenPositionA = RectTransformUtility.WorldToScreenPoint(null, chip.RectTransform.position);
            Vector2 screenPositionB = RectTransformUtility.WorldToScreenPoint(null, points[i].RectTransform.position);
            if (points.Count - 1 == i)
            {
                chip.RectTransform.position = startPosition;
            }
            if (Vector2.Distance(screenPositionA, screenPositionB) <= endDragOffset)
            {
                chip.RectTransform.position = points[i].RectTransform.position;
                break;
            }           
        }           
    }
}
