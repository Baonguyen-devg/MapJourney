using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SolidLineType
{
    Default,
    BezierSolidLine,
}

public class SolidLine : MonoBehaviour
{
    private const float WIDTH_DEFAULT = 0.5f;

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
        SetPosition(startPosition, endPosition);
        this.solidLineType = solidLineType;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
        SetLineWidth();
    }

    public void SetPosition(Vector3 startPosition, Vector3 endPosition)
    {
        this.startPosition = startPosition;
        this.endPosition = endPosition;
    }

    public void SetLineWidth(float widht = WIDTH_DEFAULT)
    {
        lineRenderer.startWidth = widht;
        lineRenderer.endWidth = widht;
    }
}
