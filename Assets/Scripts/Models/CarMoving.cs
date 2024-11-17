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
    [SerializeField] private float speed;
    [SerializeField] private bool doMove = false;

    public void SetPathMovePoints(List<Vector3> pathMovePoints) => this.pathMovePoints = pathMovePoints;
    public CarType CarType => carType;

    public void MoveToStartPoint(Vector3 startPoint)
    {
        transform.position = startPoint;
    }

    public void StartMove()
    {
        if (pathMovePoints.Count == 0) return;
        MoveToStartPoint(pathMovePoints[0]);
        gameObject.SetActive(true);
        StartCoroutine(StartMoveCoroutine());
    }

    private IEnumerator StartMoveCoroutine()
    {
        yield return new WaitForSeconds(1f);
        pathIndex = 0;
        doMove = true;
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
                pathIndex = 0;
                doMove = false;
                return;
            }
            targetPosition = pathMovePoints[pathIndex];
        }

        Vector3 nextPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        RotateFollowNextPosition(nextPosition);
        transform.position = nextPosition;
    }

    private void RotateFollowNextPosition(Vector3 nextPosition)
    {
        Vector3 direction = (nextPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
        }
    }
}
