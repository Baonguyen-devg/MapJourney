using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SolidLineType
{
    Default,
}

public class SolidLine : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    public LineRenderer LineRenderer => lineRenderer;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private SolidLineType solidLineType = SolidLineType.Default;

    public Vector3 StartPosition => startPosition;
    public Vector3 EndPosition => endPosition;
    public SolidLineType SolidLineType => solidLineType;

    public void Init(Vector3 startPosition, Vector3 endPosition, SolidLineType solidLineType = default)
    {
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        this.solidLineType = solidLineType;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
    }
}
