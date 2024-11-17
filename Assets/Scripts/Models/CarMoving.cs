using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum CarType
{
    Default,
}

public class CarMoving : MonoBehaviour
{
    [SerializeField] private List<Vector3> pathMovePoints = new List<Vector3>();
    [SerializeField] private CarType carType;
    [SerializeField] private int pathIndex = 0;
    [SerializeField] private bool doMove = false;

    [SerializeField] private float acceleration = 2f; 
    [SerializeField] private float initialSpeed = 5f;
    [SerializeField] private float time = 0f;
    [SerializeField] private float distance = 0f;

    public void SetAcceleration(float number) => acceleration = number;
    public void SetInitialSpeed(float number) => initialSpeed = number;
    public void SetTime(float number) => time = number;
    public void SetDistance(float number) => distance = number;

    private float currentSpeed = 0f;
    private float totalTime = 0f;

    public CarType CarType => carType;
    public float InitialSpeed => initialSpeed;
    public float Acceleration => acceleration;
    public float TimeArrived => time;
    public float Distance => distance;

    public void Init(List<Vector3> pathMovePoints, float acceleration = 2f, float initialSpeed = 5f, float time = 0f)
    {
        this.acceleration = acceleration;
        this.initialSpeed = initialSpeed;
        this.time = time;
        this.pathMovePoints = pathMovePoints;
    }

    public void StartMove()
    {
        if (pathMovePoints.Count == 0) return;
        ResetCarStatus();
        StartCoroutine(StartMoveCoroutine());
    }

    public void MoveToStartPoint(Vector3 startPoint) => transform.position = startPoint;
    private void ResetCarStatus()
    {
        MoveToStartPoint(pathMovePoints[0]);
        gameObject.SetActive(true);
    }

    private IEnumerator StartMoveCoroutine()
    {
        yield return new WaitForSeconds(1f);
        (pathIndex, totalTime, doMove) = (0, 0, true);
    }

    private void Update()
    {
        if (!doMove) return;

        Vector3 targetPosition = pathMovePoints[pathIndex];
        float distance = Vector3.Distance(targetPosition, transform.position);
        if (distance <= 0.1f)
        {
            pathIndex = pathIndex + 1;
            if (pathIndex == pathMovePoints.Count)
            {
                Enviroment.Instance.CarManager.CreaseCarArrived();
                pathIndex = 0;
                doMove = false;
                return;
            }
            targetPosition = pathMovePoints[pathIndex];
        }

        CalculateCurrentSpeed();
        MoveFollowTargetPosition(targetPosition);
    }

    private void CalculateCurrentSpeed()
    {
        totalTime = totalTime + Time.deltaTime;
        currentSpeed = initialSpeed + acceleration * totalTime;
        currentSpeed = Mathf.Max(0, currentSpeed);
    }

    private Vector3 MoveFollowTargetPosition(Vector3 targetPosition)
    {
        Vector3 nextPosition = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
        RotateFollowNextPosition(nextPosition);
        transform.position = nextPosition;
        return nextPosition;
    }

    private void RotateFollowNextPosition(Vector3 nextPosition)
    {
        Vector3 direction = (nextPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * currentSpeed);
        }
    }
}
