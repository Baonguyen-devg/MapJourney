using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SolidLineManager : MonoBehaviour
{
    private readonly Vector3 VECTOR3_INF = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

    [Header("Other components"), Space(6)]
    [SerializeField] private LayerMask groundLayerMask;

    [Header("Databases"), Space(6)]
    [SerializeField] private SolidLineSpawner solidLineSpawner;
    [SerializeField] private List<SolidLine> solidLines = new List<SolidLine>();
    [SerializeField] private List<SolidLine> bezierLines = new List<SolidLine>();

    public SolidLineSpawner SolidLineSpawner => solidLineSpawner;
    public List<SolidLine> SolidLines => solidLines;
    public List<SolidLine> BezierLines => bezierLines;

    [Header("Setting for bezier solid line"), Space(6)]
    [SerializeField] private int segmentCount = 20;
    [SerializeField] private float offsetDistance = 0.4f;

    private Dictionary<SolidLine, List<SolidLine>> lineToBezierMap = new Dictionary<SolidLine, List<SolidLine>>();
    private Dictionary<Vector3, List<SolidLine>> beziersAtPoint = new Dictionary<Vector3, List<SolidLine>>();

    #region initialization
    private void Awake() => RegisterEvents();
    private void OnDestroy() => UnRegisterEvents();

    private void RegisterEvents()
    {
        Debug.Log("[SolidLineManager] RegisterEvents | Start register events");
        InputManager.OnLeftMouseDown += CreateRemoveSolidLine;
    }

    private void UnRegisterEvents()
    {
        Debug.Log("[SolidLineManager] UnRegisterEvents | Start unregister events");
        InputManager.OnLeftMouseDown -= CreateRemoveSolidLine;
    }

    private void CreateRemoveSolidLine()
    {
        if (!SetUpsController.Instance.IsSetupSolidLines()) return;
        Debug.Log("[SolidLineManager] CreateRemoveSolidLine | Avaible to create/remove SolidLine");

        //Baodev: Only add/remove solidLine when game in remove solidLine mode
        if (SetUpsController.Instance.SetupSolidLinePanel.IsRemoveSolidLineState()) DetectMouseRayToRemoveSolidLine();
    }
    #endregion

    #region add/remove solidLine methods
    //Baodev: Get start/end point from PointManager, create a solidLine
    public void CreateSolidLine()
    {
        Point startPoint = Enviroment.Instance.PointManager.StartSolidLinePoint;
        Point endPoint = Enviroment.Instance.PointManager.EndSolidLinePoint;

        if (startPoint != null && endPoint != null)
        {
            Vector3 startPosition = startPoint.transform.position;
            Vector3 endPosition = endPoint.transform.position;

            TryCreateSolidAndBezierLine(startPosition, endPosition);
            Enviroment.Instance.PointManager.SelectSolidLineEndPoint();
            TrackAndDisactiveCenterLine();
        }
    }

    public bool TryCreateSolidAndBezierLine(Vector3 startPoint, Vector3 endPoint)
    {
        bool isContainer = IsSolidLineContain(startPoint, endPoint);
        if (!isContainer) CreateSolidAndBezeirLine(startPoint, endPoint);
        return !isContainer;
    }

    public void CreateSolidAndBezeirLine(Vector3 startPoint, Vector3 endPoint)
    {
        SolidLine solidLine = CreateStraightSolidLine(startPoint, endPoint);
        foreach (SolidLine existingLine in solidLines)
        {
            Vector3 sharePoint = FindSharedPoint(existingLine, solidLine);
            if (existingLine == solidLine || sharePoint == VECTOR3_INF) continue;
            InitializeBezierLine(solidLine, existingLine, sharePoint);
        }
    }

    private SolidLine CreateStraightSolidLine(Vector3 startPoint, Vector3 endPoint)
    {
        (Vector3 startMeshGenerate, Vector3 endMeshGenerate) = CalculateStraightPoints(startPoint, endPoint);
        SolidLine solidLine = solidLineSpawner.Spawn(SolidLineType.Straight);
        solidLine.CommonlyInit(startPoint, endPoint);
        solidLine.MeshGenerateInit(startMeshGenerate, endMeshGenerate);
        solidLine.GenerateStraightRoad();
        solidLines.Add(solidLine);
        return solidLine;
    }

    //Baodev: Get closestSolidLine and remove
    private void DetectMouseRayToRemoveSolidLine()
    {
        RaycastHit raycastHit = new RaycastHit();
        if (Physics.Raycast(GetMouseRay(), out raycastHit, Mathf.Infinity, groundLayerMask))
        {
            Debug.Log($"[SolidLineManager] RemoveSolidLine | raycastHitPosition: {raycastHit.point}");
            Vector3 raycastHitPosition = raycastHit.point;
            SolidLine closestSolidLine = FindClosestSolidLine(raycastHitPosition);

            if (closestSolidLine == null) return;
            RemoveSolidLineWithBeziers(closestSolidLine);
        }
    }

    public void RemoveSolidLinesConnectedToPoint(Vector3 position)
    {
        Func<SolidLine, bool> haveConnected = line => line.StartPosition == position || line.EndPosition == position;
        List<SolidLine> solidLinesToRemove = solidLines.Where(haveConnected).ToList();

        foreach (SolidLine solidLine in solidLinesToRemove)
            RemoveSolidLineWithBeziers(solidLine);
    }

    public void RemoveAllSolidLine()
    {
        for (int i = solidLines.Count - 1; i >= 0; i--)
        {
            RemoveSolidLineWithBeziers(solidLines[i]);
        }
    }

    private void RemoveSolidLineWithBeziers(SolidLine solidLine)
    {
        RemoveBezierLinesForSolidLine(solidLine);
        solidLineSpawner.Despawn(solidLine);     
        solidLines.Remove(solidLine);    
    }
    #endregion

    #region add/remove bezierline methods
    public (Vector3 startPoint, Vector3 endPoint) InitializeBezierLine(SolidLine line1, SolidLine line2, Vector3 sharedPoint)
    {
        (Vector3 startPoint, Vector3 endPoint) = CalculateControlPoints(line1, line2, sharedPoint);
        SolidLine bezierSolidLine = CreateBezierSolidLine(startPoint, endPoint, sharedPoint);
        AddBezierLineToMap(line1, line2, bezierSolidLine);
        AddBazierAtPoint(sharedPoint, bezierSolidLine);
        return (startPoint, endPoint);
    }

    private SolidLine CreateBezierSolidLine(Vector3 startPoint, Vector3 endPoint, Vector3 sharedPoint)
    {
        SolidLine bezierSolidLine = SolidLineSpawner.Spawn(SolidLineType.Bezier);
        bezierSolidLine.CommonlyInit(startPoint, endPoint);

        List<Vector3> bezierPoints = new List<Vector3>();
        float totalLength = EstimateBezierCurveLength(startPoint, sharedPoint, endPoint, segmentCount);
        int pointCount = Mathf.Max(2, Mathf.CeilToInt(totalLength / bezierSolidLine.Spacing));

        for (int i = 0; i <= pointCount; i++)
        {
            float interpolationFactor = i / (float)pointCount;
            Vector3 bezierPoint = CalculateQuadraticBezierPoint(interpolationFactor, startPoint, sharedPoint, endPoint);
            bezierPoints.Add(bezierPoint + new Vector3(0, 0.1f, 0));
        }

        Vector3 header = sharedPoint - startPoint;
        Vector3 footer = endPoint - sharedPoint;
        bezierSolidLine.GenerateBezierRoad(bezierPoints, header, footer);
        return bezierSolidLine;
    }

    private float EstimateBezierCurveLength(Vector3 point0, Vector3 point1, Vector3 point2, int segmentCount)
    {
        float length = 0f;
        Vector3 previousPoint = point0;
        for (int i = 1; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            Vector3 currentPoint = CalculateQuadraticBezierPoint(t, point0, point1, point2);
            length += Vector3.Distance(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
        return length;
    }

    private Vector3 CalculateQuadraticBezierPoint(float interpolationFactor, Vector3 point0, Vector3 point1, Vector3 point2)
    {
        float u = 1 - interpolationFactor;
        return u * u * point0 + 2 * u * interpolationFactor * point1
             + interpolationFactor * interpolationFactor * point2;
    }

    private void AddBezierLineToMap(SolidLine line1, SolidLine line2, SolidLine bezierSolidLine)
    {
        if (!lineToBezierMap.ContainsKey(line1))
        {
            lineToBezierMap[line1] = new List<SolidLine>();
        }
        if (!lineToBezierMap.ContainsKey(line2))
        {
            lineToBezierMap[line2] = new List<SolidLine>();
        }

        lineToBezierMap[line1].Add(bezierSolidLine);
        lineToBezierMap[line2].Add(bezierSolidLine);
    }

    private void RemoveBezierLinesForSolidLine(SolidLine solidLineToRemove)
    {
        if (lineToBezierMap.TryGetValue(solidLineToRemove, out List<SolidLine> relatedBeziers))
        {
            List<SolidLine> bezierLinesToRemove = new List<SolidLine>(relatedBeziers);
            foreach (SolidLine bezierLine in bezierLinesToRemove)
            {
                solidLineSpawner.Despawn(bezierLine);
                bezierLines.Remove(bezierLine);
                RemoveBezierAtPoint(solidLineToRemove.StartPosition, bezierLine);
                RemoveBezierAtPoint(solidLineToRemove.EndPosition, bezierLine);

                foreach (var entry in lineToBezierMap)
                {
                    entry.Value.Remove(bezierLine);
                }
            }
            lineToBezierMap.Remove(solidLineToRemove);
        }
    }

    private void AddBazierAtPoint(Vector3 point, SolidLine bezierSolidLine)
    {
        if (!beziersAtPoint.ContainsKey(point))
        {
            beziersAtPoint[point] = new List<SolidLine>();
        }
        beziersAtPoint[point].Add(bezierSolidLine);
    }

    private void RemoveBezierAtPoint(Vector3 point, SolidLine bezierSolidLine)
    {
        if (beziersAtPoint.ContainsKey(point))
        {
            var solidLineList = beziersAtPoint[point];
            solidLineList.Remove(bezierSolidLine);

            if (solidLineList.Count == 0)
            {
                beziersAtPoint.Remove(point);
            }
        }
    }

    private void TrackAndDisactiveCenterLine()
    {
        foreach (var keyValuePair in beziersAtPoint)
        {
            if (keyValuePair.Value.Count < 2) continue;
            foreach (var solidLine in keyValuePair.Value)
            {
                solidLine.SetCenterLineActive(false);
            }
        }
    }

    public List<Vector3> GetBezierPointsBetweenLines(SolidLine line1, SolidLine line2)
    {
        List<Vector3> bezierPoints = new List<Vector3>();

        Vector3 sharedPoint = Enviroment.Instance.SolidLineManager.FindSharedPoint(line1, line2);

        if (sharedPoint == VECTOR3_INF)
        {
            return bezierPoints; 
        }

        (Vector3 startPoint, Vector3 endPoint) = CalculateControlPoints(line1, line2, sharedPoint);

        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            Vector3 bezierPoint = CalculateQuadraticBezierPoint(t, startPoint, sharedPoint, endPoint);
            bezierPoints.Add(bezierPoint);
        }
        return bezierPoints;
    }

    public SolidLine FindSolidLine(Vector3 start, Vector3 end)
    {
        return solidLines.Find(line =>
            (line.StartPosition == start && line.EndPosition == end) ||
            (line.StartPosition == end && line.EndPosition == start));
    }
    #endregion

    #region Other methods
    private (Vector3 startPoint, Vector3 endPoint) CalculateControlPoints(SolidLine line1, SolidLine line2, Vector3 sharedPoint)
    {
        Vector3 startPointOriginal = GetOriginalPoint(line1, sharedPoint);
        Vector3 endPointOriginal = GetOriginalPoint(line2, sharedPoint);

        Vector3 directionStart = (sharedPoint - startPointOriginal).normalized;
        Vector3 directionEnd = (endPointOriginal - sharedPoint).normalized;

        Vector3 fixedLengthVectorStart = directionStart * 5f;
        Vector3 fixedLengthVectorEnd = directionEnd * 5f;

        Vector3 offsetVectorStart = (sharedPoint - startPointOriginal) * offsetDistance;
        Vector3 offsetVectorEnd = (endPointOriginal - sharedPoint) * offsetDistance;

        Vector3 resultStart = GetShorterVector(offsetVectorStart, fixedLengthVectorStart);
        Vector3 resultEnd = GetShorterVector(offsetVectorEnd, fixedLengthVectorEnd);

        Vector3 startPoint = sharedPoint - resultStart;
        Vector3 endPoint = sharedPoint + resultEnd;
        return (startPoint, endPoint);
    }

    private Vector3 GetOriginalPoint(SolidLine line, Vector3 sharedPoint)
    {
        return (line.StartPosition == sharedPoint) ? line.EndPosition : line.StartPosition;
    }

    private Vector3 GetShorterVector(Vector3 vector1, Vector3 vector2)
    {
        return (vector1.magnitude < vector2.magnitude) ? vector1 : vector2;
    }

    private (Vector3 startPoint, Vector3 endPoint) CalculateStraightPoints(Vector3 point1, Vector3 point2)
    {
        Vector3 directionStart = (point2 - point1).normalized;
        Vector3 fixedLengthVectorStart = directionStart * 5f;
        Vector3 minusStart = (point2 - point1) * offsetDistance;

        float lengthMinusStart = minusStart.magnitude;
        float lengthFixedStart = fixedLengthVectorStart.magnitude;
        Vector3 resultStart = (lengthMinusStart < lengthFixedStart) ? minusStart : fixedLengthVectorStart;

        Vector3 startPoint = point1 + resultStart;
        Vector3 endPoint = point2 - resultStart;
        return (startPoint, endPoint);
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

    private bool TryGetClosestSolidLine(out SolidLine closetSolidLine)
    {
        closetSolidLine = null;
        if (Physics.Raycast(GetMouseRay(), out RaycastHit raycastHit, Mathf.Infinity, groundLayerMask))
        {
            closetSolidLine = FindClosestSolidLine(raycastHit.point);
            return true;
        }
        return false;
    }

    private SolidLine FindClosestSolidLine(Vector3 hitPosition)
    {
        SolidLine closestLine = null;
        float minDistance = Mathf.Infinity;

        foreach (SolidLine solidLine in solidLines)
        {
            float distance = DistancePointToLineSegment(hitPosition, solidLine.StartPosition, solidLine.EndPosition);
            if (distance < minDistance && distance < 1f)
            {
                minDistance = distance;
                closestLine = solidLine;
            }
        }
        return closestLine;
    }

    private float DistancePointToLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        float lineLength = lineDirection.magnitude;
        lineDirection.Normalize();

        Vector3 toPoint = point - lineStart;
        float projectionLength = Vector3.Dot(toPoint, lineDirection);

        if (projectionLength <= 0) return Vector3.Distance(point, lineStart);
        else if (projectionLength >= lineLength) return Vector3.Distance(point, lineEnd);

        Vector3 projectionPoint = lineStart + lineDirection * projectionLength;
        return Vector3.Distance(point, projectionPoint);
    }

    private Ray GetMouseRay()
    {
        Vector3 mousePosition = InputManager.Instance.GetMousePosition();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        return ray;
    }

    public bool IsSolidLineContain(Vector3 startPoint, Vector3 endPoint)
    {
        bool isContainer = false;
        foreach (SolidLine line in solidLines)
        {
            if (line.StartPosition == startPoint && line.EndPosition == endPoint
                || line.StartPosition == endPoint && line.EndPosition == startPoint)
                isContainer = true;
        }
        return isContainer;
    }
    #endregion
}
