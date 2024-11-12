using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentController : MonoBehaviour
{
    [SerializeField] private PointSpawner pointSpawner;
    [SerializeField] private LayerMask groundLayerMask;

    public PointSpawner PointSpawner => pointSpawner;

    private void Awake()
    {
        InputManager.OnLeftMouseDown += CreatePoint;
    }
    
    private void CreatePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMousePosition());
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, groundLayerMask))
        {
            Vector3 hitPosition = raycastHit.point;
            Point point = pointSpawner.Spawn(PointType.Default);
            point.transform.position = hitPosition;
        }
    }
}
