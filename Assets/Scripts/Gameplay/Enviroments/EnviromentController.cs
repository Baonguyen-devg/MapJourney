using Dreamteck.Splines;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentController : MonoBehaviour
{
    private readonly Vector3 VECTOR3_INF = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

    [SerializeField] private PointSpawner pointSpawner;
    [SerializeField] private SolidLineSpawner solidLineSpawner;
    [SerializeField] private SplineComputer roadSplineComputer;

    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private SetUpsController setUpsController;

    [SerializeField] private List<Point> points = new List<Point>();
    [SerializeField] private List<SolidLine> solidLines = new List<SolidLine>();
    [SerializeField] private List<SolidLine> bezierLines = new List<SolidLine>();

    [Header("Setting for bezier solid line"), Space(6)]
    [SerializeField] private int segmentCount = 20;
    [SerializeField] private float offsetDistance = 0.4f;

    public PointSpawner PointSpawner => pointSpawner;
    public SolidLineSpawner SolidLineSpawner => solidLineSpawner;

    private Point clickedPoint;
    private Point targetPoint;

    private Dictionary<SolidLine, List<SolidLine>> lineToBezierMap = new Dictionary<SolidLine, List<SolidLine>>();

    private void Awake()
    {
        InputManager.OnLeftMouseDown += SetUpPoint;
        InputManager.OnLeftMouseDown += SetUpSolidLine;
        InputManager.OnRightMouseDown += ResetPoints;
    }
    
    private void SetUpPoint()
    {
        if (!setUpsController.IsSetupPoint()) return;
        if (setUpsController.SetUpPointPanel.IsAddPointState()) AddPoint();
        if (setUpsController.SetUpPointPanel.IsRemovePointState()) RemovePoint();
    }

    private void SetUpSolidLine()
    {
        if (!setUpsController.IsSetupSolidLine()) return;
        if (setUpsController.SetupSolidLinePanel.IsAddSolidLineState()) ConnectPoints();
        if (setUpsController.SetupSolidLinePanel.IsRemoveSolidLineState()) RemoveLine();
    }
    
    private void ResetPoints()
    {
        if (!setUpsController.IsSetupSolidLine()) return;
        if (clickedPoint != null) clickedPoint.OnUnselected();
        if (targetPoint != null) targetPoint.OnUnselected();
        clickedPoint = null;
        targetPoint = null;
    }

    private void AddPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMousePosition());
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, groundLayerMask))
        {
            Vector3 hitPosition = raycastHit.point;
            Point point = pointSpawner.Spawn(PointType.Default);
            point.transform.position = hitPosition;
            point.Init(hitPosition.x, hitPosition.y, hitPosition.z);
            points.Add(point);
        }
    }

    private void RemovePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMousePosition());
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, groundLayerMask))
        {
            Vector3 hitPosition = raycastHit.point;
            Point closestPoint = FindClosestPoint(hitPosition);

            if (closestPoint != null)
            {
                RemovePointWithSolidLines(closestPoint);
            }
        }
    }

    private void RemovePointWithSolidLines(Point pointToRemove)
    {
        List<SolidLine> solidLinesToRemove = new List<SolidLine>();
        foreach (SolidLine solidLine in solidLines)
        {
            if (solidLine.StartPosition == pointToRemove.transform.position ||
                solidLine.EndPosition == pointToRemove.transform.position)
            {
                solidLinesToRemove.Add(solidLine);
            }
        }

        foreach (SolidLine solidLine in solidLinesToRemove)
        {
            RemoveSolidLineWithBezier(solidLine);
        }
        pointSpawner.Despawn(pointToRemove);
        points.Remove(pointToRemove);
    }

    private void RemoveLine()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMousePosition());
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, groundLayerMask))
        {
            Vector3 hitPosition = raycastHit.point;
            SolidLine closestLine = null;
            float minDistance = Mathf.Infinity;

            foreach (SolidLine solidLine in solidLines)
            {
                float distance = DistancePointToLineSegment(hitPosition, solidLine.StartPosition, solidLine.EndPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestLine = solidLine;
                }
            }

            if (closestLine != null && minDistance < 1.0f)
            {
                RemoveSolidLineWithBezier(closestLine);
            }
        }
    }

    private float DistancePointToLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        float lineLength = lineDirection.magnitude;
        lineDirection.Normalize();

        Vector3 toPoint = point - lineStart;
        float projectionLength = Vector3.Dot(toPoint, lineDirection);

        if (projectionLength <= 0)
        {
            return Vector3.Distance(point, lineStart); 
        }
        else if (projectionLength >= lineLength)
        {
            return Vector3.Distance(point, lineEnd); 
        }

        Vector3 projectionPoint = lineStart + lineDirection * projectionLength;
        return Vector3.Distance(point, projectionPoint);
    }

    public void ConverLineToSpline()
    {
        roadSplineComputer.gameObject.SetActive(true);
        roadSplineComputer.SetPoints(new SplinePoint[0]);

        List<SplinePoint> splinePoints = new List<SplinePoint>();
        foreach (SolidLine solidLine in solidLines)
        {
            float segmentLength = Vector3.Distance(solidLine.StartPosition, solidLine.EndPosition);

            Vector3 start = solidLine.StartPosition + (solidLine.EndPosition - solidLine.StartPosition).normalized * 0.5f;
            Vector3 end = solidLine.EndPosition - (solidLine.EndPosition - solidLine.StartPosition).normalized * 0.5f;

            splinePoints.Add(new SplinePoint(start));
            splinePoints.Add(new SplinePoint(end));
        }
        roadSplineComputer.SetPoints(splinePoints.ToArray());
    }

    private void ConnectPoints()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMousePosition());
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, groundLayerMask))
        {
            Vector3 hitPosition = raycastHit.point;
            Point closestPoint = FindClosestPoint(hitPosition);
            if (closestPoint == null) return;

            if (clickedPoint == null)
            {
                clickedPoint = closestPoint;
                clickedPoint.OnSelected();
                return;
            }
            else if (clickedPoint == closestPoint)
            {
                clickedPoint.OnUnselected();
                clickedPoint = null;
                return;
            }

            if (clickedPoint != null)
            {
                targetPoint = closestPoint;
                targetPoint.OnSelected();
            }
            else if (targetPoint == clickedPoint)
            {
                targetPoint.OnUnselected();
                targetPoint = null;
            }

            if (clickedPoint != null && targetPoint != null)
            {
                bool isContainer = false;
                foreach (SolidLine line in solidLines)
                {
                    if (line.StartPosition == clickedPoint.transform.position && line.EndPosition == targetPoint.transform.position
                        || line.StartPosition == targetPoint.transform.position && line.EndPosition == clickedPoint.transform.position)
                        isContainer = true;
                }

                if (!isContainer) 
                {
                    SolidLine solidLine = solidLineSpawner.Spawn(SolidLineType.Default);
                    solidLine.Init(clickedPoint.transform.position, targetPoint.transform.position);
                    solidLines.Add(solidLine);

                    foreach (SolidLine existingLine in solidLines)
                    {
                        if (existingLine == solidLine) continue;
                        CreateBezierLine(solidLine, existingLine);
                    }
                }

                clickedPoint.OnUnselected();
                clickedPoint = targetPoint;
                targetPoint = null;
                clickedPoint.OnSelected();
            }
        }
    }

    private Point FindClosestPoint(Vector3 position)
    {
        Point closestPoint = null;
        float minDistance = 1f;

        foreach (Point point in points)
        {
            float distance = Vector3.Distance(position, point.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = point;
            }
        }
        return closestPoint;
    }

    public void RemoveAllPoint()
    {
        for (int i = points.Count - 1; i >= 0; i--)
        {
            RemovePointWithSolidLines(points[i]);
        }
    }

    public void RemoveAllSolidLine()
    {
        for (int i = solidLines.Count - 1; i >= 0; i--)
        {
            RemoveSolidLineWithBezier(solidLines[i]);
        }
    }

    void CreateBezierLine(SolidLine solidLine1, SolidLine solidLine2)
    {
        Vector3 sharedPoint = FindSharedPoint(solidLine1, solidLine2);
        if (sharedPoint == VECTOR3_INF)
        {
            Debug.Log($"[EnviromentController] CreateBezierLine | Don't have sharepoint");
            return;
        }

        Vector3 startPointOriginal = (solidLine1.StartPosition == sharedPoint) ? solidLine1.EndPosition : solidLine1.StartPosition;
        Vector3 endPointOriginal = (solidLine2.StartPosition == sharedPoint) ? solidLine2.EndPosition : solidLine2.StartPosition;

        Vector3 startPoint = sharedPoint - (sharedPoint - startPointOriginal) * offsetDistance;
        Vector3 endPoint = sharedPoint + (endPointOriginal - sharedPoint) * offsetDistance;

        SolidLine bezierSolidLine = SolidLineSpawner.Spawn(SolidLineType.BezierSolidLine);
        bezierSolidLine.LineRenderer.positionCount = segmentCount + 1;
        bezierSolidLine.SetPosition(startPointOriginal, endPointOriginal);
        bezierSolidLine.SetLineWidth(0.3f);

        for (int i = 0; i <= segmentCount; i++)
        {
            float interpolationFactor = i / (float)segmentCount;
            Vector3 bezierPoint = CalculateQuadraticBezierPoint(interpolationFactor, startPoint, sharedPoint, endPoint);
            bezierSolidLine.LineRenderer.SetPosition(i, bezierPoint);
        }

        if (!lineToBezierMap.ContainsKey(solidLine1))
        {
            lineToBezierMap[solidLine1] = new List<SolidLine>();
        }
        if (!lineToBezierMap.ContainsKey(solidLine2))
        {
            lineToBezierMap[solidLine2] = new List<SolidLine>();
        }
        lineToBezierMap[solidLine1].Add(bezierSolidLine);
        lineToBezierMap[solidLine2].Add(bezierSolidLine);
    }

    private void RemoveSolidLineWithBezier(SolidLine solidLineToRemove)
    {
        if (lineToBezierMap.TryGetValue(solidLineToRemove, out List<SolidLine> relatedBeziers))
        {
            List<SolidLine> bezierLinesToRemove = new List<SolidLine>(relatedBeziers);
            foreach (SolidLine bezierLine in bezierLinesToRemove)
            {
                solidLineSpawner.Despawn(bezierLine);
                bezierLines.Remove(bezierLine);

                foreach (var entry in lineToBezierMap)
                {
                    entry.Value.Remove(bezierLine);
                }
            }
            lineToBezierMap.Remove(solidLineToRemove);
        }
        solidLineSpawner.Despawn(solidLineToRemove);
        solidLines.Remove(solidLineToRemove);
    }

    public Vector3 FindSharedPoint(SolidLine line1, SolidLine line2)
    {
        if (line1.StartPosition == line2.StartPosition || line1.StartPosition == line2.EndPosition)
        {
            Debug.Log($"[EnviromentController] FindSharedPoint | Sharedpoint: {line1.StartPosition}");
            return line1.StartPosition;
        }
        else if (line1.EndPosition == line2.StartPosition || line1.EndPosition == line2.EndPosition)
        {
            Debug.Log($"[EnviromentController] FindSharedPoint | Sharedpoint: {line1.EndPosition}");
            return line1.EndPosition;
        }
        return VECTOR3_INF;
    }

    Vector3 CalculateQuadraticBezierPoint(float interpolationFactor, Vector3 point0, Vector3 point1, Vector3 point2)
    {
        float u = 1 - interpolationFactor;
        return u * u * point0 + 2 * u * interpolationFactor * point1 
             + interpolationFactor * interpolationFactor * point2;
    }
}
