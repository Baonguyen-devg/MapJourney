using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class EnviromentController : MonoBehaviour
{
    [SerializeField] private PointSpawner pointSpawner;
    [SerializeField] private SolidLineSpawner solidLineSpawner;

    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private SetUpsController setUpsController;

    [SerializeField] private List<Point> points = new List<Point>();
    [SerializeField] private List<SolidLine> solidLines = new List<SolidLine>();

    public PointSpawner PointSpawner => pointSpawner;
    public SolidLineSpawner SolidLineSpawner => solidLineSpawner;

    private Point clickedPoint;
    private Point targetPoint;

    private void Awake()
    {
        InputManager.OnLeftMouseDown += SetUpPoint;
        InputManager.OnLeftMouseDown += SetUpSolidLine;
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
                points.Remove(closestPoint);
                pointSpawner.Despawn(closestPoint);
            }
        }
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
                SolidLineSpawner.Despawn(closestLine);
                solidLines.Remove(closestLine);
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
                }

                clickedPoint.OnUnselected();
                targetPoint.OnUnselected();
                clickedPoint = null;
                targetPoint = null;
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
            pointSpawner.Despawn(points[i]);
            points.Remove(points[i]);
        }
    }

    public void RemoveAllSolidLine()
    {
        for (int i = solidLines.Count - 1; i >= 0; i--)
        {
            SolidLineSpawner.Despawn(solidLines[i]);
            solidLines.Remove(solidLines[i]);
        }
    }
}
