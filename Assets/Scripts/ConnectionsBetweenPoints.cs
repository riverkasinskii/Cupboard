using System.Collections.Generic;
using UnityEngine;

public enum PointName : byte
{
    FirstPoint,
    SecondPoint,
    ThirdPoint,
    FourthPoint,
    FifthPoint,
    SixthPoint,
    SeventhPoint,
    EighthPoint,
    NinthPoint,
}

public sealed class ConnectionsBetweenPoints : MonoBehaviour
{
    [field: SerializeField]
    public List<Point> Points { get; private set; } = new();

    private readonly Dictionary<int, List<PointName>> pairsPoints = new();

    private void Start()
    {        
        pairsPoints.Add(1, new List<PointName> { PointName.FirstPoint, PointName.FourthPoint });
        pairsPoints.Add(2, new List<PointName> { PointName.SecondPoint, PointName.FifthPoint });
        pairsPoints.Add(3, new List<PointName> { PointName.ThirdPoint, PointName.SixthPoint });
        pairsPoints.Add(4, new List<PointName> { PointName.FourthPoint, PointName.FifthPoint });
        pairsPoints.Add(5, new List<PointName> { PointName.FifthPoint, PointName.SixthPoint });
        pairsPoints.Add(6, new List<PointName> { PointName.FourthPoint, PointName.SeventhPoint });
        pairsPoints.Add(7, new List<PointName> { PointName.FifthPoint, PointName.EighthPoint });
        pairsPoints.Add(8, new List<PointName> { PointName.SixthPoint, PointName.NinthPoint });        
    }

    public List<Direction> GetAllowedDirections(Vector2 position)
    {        
        foreach (var point in Points)
        {
            if (Vector2.Distance(position, point.RectTransform.position) <= 0.5f)
            {                
                return GetPoints(point);
            }
        }
        return null;
    }

    private List<Direction> GetPoints(Point point)
    {
        List<PointName> currentPointNames = new();
        foreach(var pairs in pairsPoints.Values)
        {
            if (pairs.Contains(point.Name))
            {
                currentPointNames.AddRange(pairs);
                currentPointNames.Remove(point.Name);                               
            }
        }
        return GetListDirections(currentPointNames, point.Name);        
    }

    private List<Direction> GetListDirections(List<PointName> directionPoints, PointName currentPointName)
    {
        List<Direction> directions = new();
        foreach (var directionPointName in directionPoints)
        {
            Vector2 directionPointPosition = Vector2.zero;
            Vector2 currentPointPosition = Vector2.zero;
            foreach (var point in Points)
            {                
                if (directionPointName == point.Name)
                {
                    directionPointPosition = point.transform.position;
                }
                if (currentPointName == point.Name)
                {
                    currentPointPosition = point.transform.position;
                }                
            }
            directions.Add(GetDirection(currentPointPosition, directionPointPosition));            
        }
        return directions;
    }
        
    private Direction GetDirection(Vector2 currentPosition, Vector2 directionPosition)
    {
        Vector2 difference = directionPosition - currentPosition;

        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
        {
            return difference.x > 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            return difference.y > 0 ? Direction.Up : Direction.Down;
        }
    }

}
