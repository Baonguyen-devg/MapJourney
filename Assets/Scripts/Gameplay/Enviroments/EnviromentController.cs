using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class EnviromentController : MonoBehaviour
{
    [SerializeField] private PointSpawner pointSpawner;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private SetUpsController setUpsController;
    [SerializeField] private List<Point> pointSpawned = new List<Point>();

    public PointSpawner PointSpawner => pointSpawner;

    private void Awake()
    {
        InputManager.OnLeftMouseDown += SetUpPoint;
    }
    
    private void SetUpPoint()
    {
        if (setUpsController.SetUpPointPanel.IsAddPointState()) AddPoint();
        if (setUpsController.SetUpPointPanel.IsRemovePointState()) RemovePoint();
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
            pointSpawned.Add(point);
        }
    }

    private void RemovePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMousePosition());
        RaycastHit raycastHit;

        float minDistance = 1f;
        Point pointToRemove = null;

        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, groundLayerMask))
        {
            Vector3 hitPosition = raycastHit.point;
            foreach (Point point in pointSpawned)
            {
                float distanceToHit = Vector3.Distance(hitPosition, point.transform.position);
                if (distanceToHit < minDistance)
                {
                    minDistance = distanceToHit;
                    pointToRemove = point;
                }
            }

            if (pointToRemove != null)
            {
                pointSpawned.Remove(pointToRemove);
                pointSpawner.Despawn(pointToRemove);
            }
        }
    }

    public void RemoveAllPoint()
    {
        for (int i = pointSpawned.Count - 1; i >= 0; i--)
        {
            pointSpawner.Despawn(pointSpawned[i]);
            pointSpawned.Remove(pointSpawned[i]);
        }
    }
}
