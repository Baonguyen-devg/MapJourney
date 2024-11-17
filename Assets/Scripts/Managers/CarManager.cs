using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    [Header("Databases"), Space(6)]
    [SerializeField] private Enviroment enviroment;
    [SerializeField] private CarSpawner carSpawner;
    [SerializeField] private List<CarMoving> carMoves = new List<CarMoving>();

    [SerializeField] private int carNumber;

    public void SetCarNumber(int number) => carNumber = number;
    public int CarNumber => carNumber;

    [ContextMenu("Demo car move")]
    public void DemoCarMove()
    {
        List<Point> startJourneyPoints = enviroment.PointManager.StartJourneyPoints;
        Point endJourneyPoint = enviroment.PointManager.EndJourneyPoint;

        DestroyAllCarMoving();
        foreach (Point startPoint in startJourneyPoints)
        {
            Debug.Log($"[CarManager] DemoCarMove | From {startPoint} to {endJourneyPoint}");
            List<Vector3> pathMovePoints = enviroment.PathFindingManager.FindPathDijkstra(startPoint, endJourneyPoint);

            CarMoving newCarMoving = CreateCarMoving();
            newCarMoving.SetPathMovePoints(pathMovePoints);
            newCarMoving.StartMove();
        }
    }

    public CarMoving CreateCarMoving()
    {
        CarMoving carMoving = carSpawner.Spawn(CarType.Default);
        carMoves.Add(carMoving);
        return carMoving;
    }

    public void DestroyCarMoving(CarMoving carMoving)
    {
        if (!carMoves.Contains(carMoving)) return;
        carMoves.Remove(carMoving);
        carSpawner.Despawn(carMoving);
    }

    public void DestroyAllCarMoving()
    {
        for (int i = carMoves.Count - 1; i >= 0; i--)
            DestroyCarMoving(carMoves[i]);
    }
}
