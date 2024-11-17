using System;
using System.Collections;
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
        if (!Enviroment.Instance.SetUpsController.IsSetupSolidLines()) return;
        Debug.Log("[SolidLineManager] CreateRemoveSolidLine | Avaible to create/remove SolidLine");

        //Baodev: Only add/remove solidLine when game in remove solidLine mode
        if (Enviroment.Instance.SetUpsController.SetupSolidLinePanel.IsRemoveSolidLineState()) DetectMouseRayToRemoveSolidLine();
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
        SolidLine solidLine = InitializeSolidLine(startPoint, endPoint);
        foreach (SolidLine existingLine in solidLines)
        {
            if (existingLine == solidLine || FindSharedPoint(existingLine, solidLine) == VECTOR3_INF) continue;
            InitializeBezierLine(solidLine, existingLine);
        }
    }

    private SolidLine InitializeSolidLine(Vector3 startPoint, Vector3 endPoint)
    {
        SolidLine solidLine = solidLineSpawner.Spawn(SolidLineType.Default);
        solidLine.Init(startPoint, endPoint);
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
    public void InitializeBezierLine(SolidLine line1, SolidLine line2)
    {
        Vector3 sharedPoint = FindSharedPoint(line1, line2);
        (Vector3 startPoint, Vector3 endPoint) = CalculateControlPoints(line1, line2, sharedPoint);
        SolidLine bezierSolidLine = CreateBezierSolidLine(startPoint, endPoint, sharedPoint);

        AddBezierLineToMap(line1, line2, bezierSolidLine);
    }

    private SolidLine CreateBezierSolidLine(Vector3 startPoint, Vector3 endPoint, Vector3 sharedPoint)
    {
        SolidLine bezierSolidLine = SolidLineSpawner.Spawn(SolidLineType.BezierSolidLine);
        bezierSolidLine.LineRenderer.positionCount = segmentCount + 1;
        bezierSolidLine.SetPosition(startPoint, endPoint);
        bezierSolidLine.SetLineWidth(0.3f);

        for (int i = 0; i <= segmentCount; i++)
        {
            float interpolationFactor = i / (float)segmentCount;
            Vector3 bezierPoint = CalculateQuadraticBezierPoint(interpolationFactor, startPoint, sharedPoint, endPoint);
            bezierSolidLine.LineRenderer.SetPosition(i, bezierPoint);
        }

        return bezierSolidLine;
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

                foreach (var entry in lineToBezierMap)
                {
                    entry.Value.Remove(bezierLine);
                }
            }
            lineToBezierMap.Remove(solidLineToRemove);
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
        Vector3 startPointOriginal = (line1.StartPosition == sharedPoint) ? line1.EndPosition : line1.StartPosition;
        Vector3 endPointOriginal = (line2.StartPosition == sharedPoint) ? line2.EndPosition : line2.StartPosition;

        Vector3 startPoint = sharedPoint - (sharedPoint - startPointOriginal) * offsetDistance;
        Vector3 endPoint = sharedPoint + (endPointOriginal - sharedPoint) * offsetDistance;
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

    private Vector3 CalculateQuadraticBezierPoint(float interpolationFactor, Vector3 point0, Vector3 point1, Vector3 point2)
    {
        float u = 1 - interpolationFactor;
        return u * u * point0 + 2 * u * interpolationFactor * point1
             + interpolationFactor * interpolationFactor * point2;
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
            if (distance < minDistance && distance < 0.1f)
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
