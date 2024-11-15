using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    private const float CLOSEST_DISTANCE = 0.1f;

    [Header("Other components"), Space(6)]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Enviroment enviroment;

    [Header("Databases"), Space(6)]
    [SerializeField] private PointSpawner pointSpawner;
    [SerializeField] private List<Point> points = new List<Point>();

    public PointSpawner PointSpawner => pointSpawner;
    public List<Point> Points => points;

    private Point solidLineStartPoint;
    private Point solidLineEndPoint;

    public Point StartSolidLinePoint => solidLineStartPoint;
    public Point EndSolidLinePoint => solidLineEndPoint;

    [SerializeField] private Point startJourneyPoint;
    [SerializeField] private Point endJourneyPoint;

    public Point StartJourneyPoint => startJourneyPoint;
    public Point EndJourneyPoint => endJourneyPoint;

    #region initialization
    private void Awake() => RegisterEvents();
    private void OnDestroy() => UnRegisterEvents();

    private void RegisterEvents()
    {
        Debug.Log("[PointManager] RegisterEvents | Start register events");
        InputManager.OnLeftMouseDown += AddRemovePoint;
        InputManager.OnLeftMouseDown += SetJourneyPoints;
        InputManager.OnLeftMouseDown += SetupSolidLinePoints;
        InputManager.OnRightMouseDown += ResetSolidLinePoints;
    }

    private void UnRegisterEvents()
    {
        Debug.Log("[PointManager] UnRegisterEvents | Start unregister events");
        InputManager.OnLeftMouseDown -= AddRemovePoint;
        InputManager.OnLeftMouseDown -= SetJourneyPoints;
        InputManager.OnLeftMouseDown -= SetupSolidLinePoints;
        InputManager.OnRightMouseDown -= ResetSolidLinePoints;
    }

    private void AddRemovePoint()
    {
        if (!enviroment.SetUpsController.IsSetupPoints()) return;
        Debug.Log("[PointManager] AddRemovePoint | Avaible to create/remove point");

        //Baodev: Only add/remove point when game in add/remove point mode
        if (enviroment.SetUpsController.SetUpPointPanel.IsAddPointMode()) DetectMouseRayToAddPoint();
        if (enviroment.SetUpsController.SetUpPointPanel.IsRemovePointMode()) DetectMouseRayToRemovePoint();
    }

    private void SetJourneyPoints()
    {
        if (!enviroment.SetUpsController.IsSetupJourneyEndpoints()) return;
        Debug.Log("[PointManager] SetJourneyPoints | Avaible to set journey start/end point");

        //Baodev: Only set start/end point when game in set journey start/end point mode
        if (enviroment.SetUpsController.SetupJourneyEndpoints.IsSetStartPointMode()) DetectMouseRayToSetStartJourney();
        if (enviroment.SetUpsController.SetupJourneyEndpoints.IsSetEndPointMode()) DetectMouseRayToSetEndJourney();
    }

    private void SetupSolidLinePoints()
    {
        if (!enviroment.SetUpsController.IsSetupSolidLines()) return;
        Debug.Log("[PointManager] SetupSolidLinePoints | Avaible to create SolidLine");

        //Baodev: Only add/remove solidLine when game in add/remove solidLine mode
        if (enviroment.SetUpsController.SetupSolidLinePanel.IsAddSolidLineState()) DetectMouseRayToSetSolidLinePoints();
    }
    #endregion

    #region add/remove point methods
    //Baodev: Get mouse raycast position, create a newly point in this position
    private void DetectMouseRayToAddPoint()
    {
        RaycastHit raycastHit = new RaycastHit();
        if (Physics.Raycast(GetMouseRay(), out raycastHit, Mathf.Infinity, groundLayerMask))
        {
            Debug.Log($"[PointManager] DetectMouseRaycastToAddPoint | raycastHitPosition: {raycastHit.point}");
            Vector3 raycastHitPosition = raycastHit.point;
            SpawnNewPointAt(raycastHitPosition);
        }
    }

    //Baodev: Get mouse raycast position, find closet point, remove point and solidline connected
    private void DetectMouseRayToRemovePoint()
    {
        var haveClosetPoint = TryGetClosestPoint(out Point closestPoint) && closestPoint != null;
        if (haveClosetPoint)
        {
            Debug.Log($"[PointManager] DetectMouseRaycastToRemovePoint | closestPoint: {closestPoint}");
            RemovePoint(closestPoint);
        }
    }

    private void SpawnNewPointAt(Vector3 raycastHitPosition)
    {
        Point newPoint = pointSpawner.Spawn(PointType.Default);
        newPoint.Init(raycastHitPosition.x, raycastHitPosition.y, raycastHitPosition.z);
        newPoint.transform.position = raycastHitPosition;
        points.Add(newPoint);
    }

    private void RemovePoint(Point closestPoint)
    {
        enviroment.SolidLineManager.RemoveSolidLinesConnectedToPoint(closestPoint.transform.position);
        pointSpawner.Despawn(closestPoint);
        points.Remove(closestPoint);
    }

    public void RemoveAllPoint()
    {
        for (int i = points.Count - 1; i >= 0; i--)
            RemovePoint(points[i]);
    }

    public void UnSelecteAllPoints()
    {
        for (int i = points.Count - 1; i >= 0; i--)
            points[i].OnUnselected();
    }
    #endregion

    #region solidLine start/end point methods
    //Baodev: Get mouse raycast position, find closet point, set for solidline start/end point
    private void DetectMouseRayToSetSolidLinePoints()
    {
        var haveClosetPoint = TryGetClosestPoint(out Point closestPoint) && closestPoint != null;
        if (haveClosetPoint)
        {
            SetSolidLineStartPoint(closestPoint);
            SetSolidLineEndPoint(closestPoint);
            enviroment.SolidLineManager.CreateSolidLine();
        }
    }

    //Baodev: Set closestPoint for end point When the previous starting point is found
    private void SetSolidLineEndPoint(Point closestPoint)
    {
        if (solidLineStartPoint != null)
        {
            Debug.Log($"[PointManager] SetSolidLineEndPoint | solidLineEndPoint: {closestPoint}");
            solidLineEndPoint = closestPoint;
            solidLineEndPoint.OnSelected();
        }
    }

    //Baodev: Set closestPoint for start point When the previous starting point is not found
    private void SetSolidLineStartPoint(Point closestPoint)
    {
        if (solidLineStartPoint == null)
        {
            Debug.Log($"[PointManager] SetSolidLineStartPoint | solidLineStartPoint: {closestPoint}");
            solidLineStartPoint = closestPoint;
            solidLineStartPoint.OnSelected();
        }
    }

    public void SelectSolidLineEndPoint()
    {
        solidLineStartPoint.OnUnselected();
        solidLineStartPoint = solidLineEndPoint;
        solidLineEndPoint = null;
        solidLineStartPoint.OnSelected();
    }

    public void ResetSolidLinePoints()
    {
        if (!enviroment.SetUpsController.IsSetupSolidLines()) return;
        if (solidLineStartPoint != null) solidLineStartPoint.OnUnselected();
        if (solidLineEndPoint != null) solidLineEndPoint.OnUnselected();
        solidLineStartPoint = null;
        solidLineEndPoint = null;
    }
    #endregion

    #region journey start/end point methods
    //Baodev: Get mouse raycast position, set startJourneyPoint
    public void DetectMouseRayToSetStartJourney()
    {
        var haveClosetPoint = TryGetClosestPoint(out Point closestPoint) && closestPoint != null;
        if (haveClosetPoint)
        {
            if (startJourneyPoint != null) startJourneyPoint.OnNormalPoint();
            Debug.Log($"[PointManager] DetectMouseRayToSetStartJourney | startJourneyPoint: {closestPoint}");

            closestPoint.OnJourneyStartPoint();
            startJourneyPoint = closestPoint;
        }
    }

    //Baodev: Get mouse raycast position, set endJourneyPoint
    public void DetectMouseRayToSetEndJourney()
    {
        var haveClosetPoint = TryGetClosestPoint(out Point closestPoint) && closestPoint != null;
        if (haveClosetPoint)
        {
            if (endJourneyPoint != null) endJourneyPoint.OnNormalPoint();
            Debug.Log($"[PointManager] DetectMouseRayToSetEndJourney | endJourneyPoint: {closestPoint}");

            closestPoint.OnJourneyEndPoint();
            endJourneyPoint = closestPoint;
        }
    }

    public void ResetAllJourneyPoint()
    {
        if (!enviroment.SetUpsController.IsSetupJourneyEndpoints()) return;
        if (startJourneyPoint != null) startJourneyPoint.OnNormalPoint();
        if (endJourneyPoint != null) endJourneyPoint.OnNormalPoint();
        startJourneyPoint = null;
        endJourneyPoint = null;
    }
    #endregion

    #region other methods
    private bool TryGetClosestPoint(out Point closestPoint)
    {
        closestPoint = null;
        if (Physics.Raycast(GetMouseRay(), out RaycastHit raycastHit, Mathf.Infinity, groundLayerMask))
        {
            closestPoint = FindClosestPoint(raycastHit.point);
            return true;
        }
        return false;
    }

    public Point FindClosestPoint(Vector3 position)
    {
        var pointDistances = points.OrderBy(point => Vector3.Distance(position, point.transform.position));
        var closetPoint = pointDistances.FirstOrDefault(point => Vector3.Distance(position, point.transform.position) < 1);
        return closetPoint;
    }

    public Point GetNeighborPoint(Vector3 endLinePosition, Vector3 startLinePosition, Point currentPoint)
    {
        if (startLinePosition == currentPoint.transform.position)
        {
            return points.Find(p => p.transform.position == endLinePosition);
        }
        else if (endLinePosition == currentPoint.transform.position)
        {
            return points.Find(p => p.transform.position == startLinePosition);
        }
        return null;
    }

    private Ray GetMouseRay()
    {
        Vector3 mousePosition = InputManager.Instance.GetMousePosition();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        return ray;
    }
    #endregion
}
