using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFindingManager : MonoBehaviour
{
    [SerializeField] private Enviroment enviroment;

    public List<Vector3> FindPathDijkstra(Point startPoint, Point endPoint)
    {
        if (startPoint == null || endPoint == null) return null;

        Dictionary<Point, float> distances = new Dictionary<Point, float>();
        Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();
        List<Point> openList = new List<Point>(); 
        HashSet<Point> visited = new HashSet<Point>();

        distances[startPoint] = 0f;
        openList.Add(startPoint);

        while (openList.Count > 0)
        {
            Point currentPoint = GetPointWithSmallestDistance(openList, distances);
            if (currentPoint == endPoint)
            {
                List<Vector3> retracePath = RetracePath(cameFrom, startPoint, endPoint);
                return GetBezierPathFromRetrace(retracePath);
            }

            openList.Remove(currentPoint);
            visited.Add(currentPoint);

            foreach (SolidLine line in enviroment.SolidLineManager.SolidLines)
            {
                Point neighborPoint = enviroment.PointManager.GetNeighborPoint(line.StartPosition, line.EndPosition, currentPoint);
                if (neighborPoint != null && !visited.Contains(neighborPoint))
                {
                    float lineWeight = Vector3.Distance(line.StartPosition, line.EndPosition);
                    float newDistance = distances[currentPoint] + lineWeight;

                    if (!distances.ContainsKey(neighborPoint) || newDistance < distances[neighborPoint])
                    {
                        distances[neighborPoint] = newDistance;
                        cameFrom[neighborPoint] = currentPoint;

                        if (!openList.Contains(neighborPoint))
                        {
                            openList.Add(neighborPoint);
                        }
                    }
                }
            }
        }
        return null;
    }

    private Point GetPointWithSmallestDistance(List<Point> openList, Dictionary<Point, float> distances)
    {
        Point smallestPoint = openList[0];
        float smallestDistance = distances[smallestPoint];

        foreach (Point point in openList)
        {
            if (distances[point] < smallestDistance)
            {
                smallestPoint = point;
                smallestDistance = distances[point];
            }
        }

        return smallestPoint;
    }

    public List<Vector3> GetBezierPathFromRetrace(List<Vector3> retracePath)
    {
        List<Vector3> bezierPath = new List<Vector3>();

        bezierPath.Add(retracePath[0]);
        for (int i = 0; i < retracePath.Count - 1; i++)
        {
            Vector3 currentPoint = retracePath[i];
            Vector3 nextPoint = retracePath[i + 1];

            SolidLine line1 = enviroment.SolidLineManager.FindSolidLine(currentPoint, nextPoint);
            if (line1 != null && i < retracePath.Count - 2)
            {
                Vector3 nextNextPoint = retracePath[i + 2];
                SolidLine line2 = enviroment.SolidLineManager.FindSolidLine(nextPoint, nextNextPoint);

                if (line2 != null)
                {
                    List<Vector3> bezierPoints = enviroment.SolidLineManager.GetBezierPointsBetweenLines(line1, line2);
                    bezierPath.AddRange(bezierPoints);
                }
            }
        }

        bezierPath.Add(retracePath.Last());
        return bezierPath;
    }

    private List<Vector3> RetracePath(Dictionary<Point, Point> cameFrom, Point startPoint, Point endPoint)
    {
        List<Vector3> path = new List<Vector3>();
        Point currentPoint = endPoint;
        path.Add(currentPoint.transform.position);

        while (currentPoint != null)
        {
            Point parentPoint = cameFrom.ContainsKey(currentPoint) ? cameFrom[currentPoint] : null;

            if (parentPoint != null)
            {
                path.Add(parentPoint.transform.position);
            }
            currentPoint = parentPoint;
        }

        path.Add(startPoint.transform.position);
        path.Reverse();
        return path;
    }
}
