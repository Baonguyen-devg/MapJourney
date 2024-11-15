using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PointType
{
    Default,
}

public class Point : MonoBehaviour
{
    [SerializeField] private float xCoordinate;  
    [SerializeField] private float yCoordinate;  
    [SerializeField] private float zCoordinate;

    public float XCoordinate => xCoordinate;
    public float YCoordinate => yCoordinate;
    public float ZCoordinate => zCoordinate;

    [SerializeField] private PointType pointType = PointType.Default;
    [SerializeField] private Renderer pointRenderer;

    public PointType PointType => pointType;
    public Renderer PointRenderer => pointRenderer;

    public void Init(float xCoordinate, float yCoordinate, float zCoordinate, PointType pointType = default)
    {
        this.xCoordinate = xCoordinate;
        this.yCoordinate = yCoordinate;
        this.zCoordinate = zCoordinate;
        this.pointType = pointType;
        OnNormalPoint();
    }

    public void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one * 2, 0.2f).SetEase(Ease.Linear);
    }

    public void DisActive()
    {
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear)
           .OnComplete(() => gameObject.SetActive(false));
    }

    public void OnSelected() => transform.DOScale(Vector3.one * 2.5f, 0.2f).SetEase(Ease.Linear);
    public void OnUnselected() => transform.DOScale(Vector3.one * 2, 0.2f).SetEase(Ease.Linear);

    public void OnJourneyStartPoint() => pointRenderer.material.DOColor(Color.red, 0.2f);
    public void OnJourneyEndPoint() => pointRenderer.material.DOColor(Color.green, 0.2f);
    public void OnNormalPoint()
    {
        pointRenderer.material.DOColor(Color.white, 0.2f);
        transform.localScale = Vector3.one * 2;
    }
}
