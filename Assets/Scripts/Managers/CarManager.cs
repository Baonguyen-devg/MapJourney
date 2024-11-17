using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    [Header("Databases"), Space(6)]
    [SerializeField] private CarSpawner carSpawner;
    [SerializeField] private List<CarData> carDatas = new List<CarData>();
    [SerializeField] private List<CarMoving> carMoves = new List<CarMoving>();

    [SerializeField] private int carNumber;
    public int CarNumber => carNumber;

    private int carArrived = 0;

    public static event System.Action EnoughCarArrived;
    public static event Action<List<CarData>> CarDataUpdated;

    public void SetCarNumber(int number)
    {
        carNumber = number;
        carDatas = new List<CarData>(carNumber);
    }

    public void SetAcceleration(float number, int id) => carDatas[id].SetAcceleration(number);
    public void SetInitialSpeed(float number, int id) => carDatas[id].SetInitialSpeed(number);
    public void SetTime(float number, int id) => carDatas[id].SetTime(number);

    private void Awake() => SetupJourneyEndpoints.CarInformationUpdated += UpdateCarDatas;
    private void OnDestroy() => SetupJourneyEndpoints.CarInformationUpdated -= UpdateCarDatas;

    [ContextMenu("Demo car move")]
    public void DemoCarMove()
    {
        carArrived = 0;
        float totalTravelTime = 0f;

        List<Point> startJourneyPoints = Enviroment.Instance.PointManager.StartJourneyPoints;
        Point endJourneyPoint = Enviroment.Instance.PointManager.EndJourneyPoint;

        DestroyAllCarMoving();
        foreach (Point startPoint in startJourneyPoints)
        {
            Debug.Log($"[CarManager] DemoCarMove | From {startPoint} to {endJourneyPoint}");
            List<Vector3> pathMovePoints = Enviroment.Instance.PathFindingManager.FindPathDijkstra(startPoint, endJourneyPoint);

            int carId = startJourneyPoints.IndexOf(startPoint);
            float totalDistance = TotalDistance(pathMovePoints);
            float travelTime = CalculateTravelTime(totalDistance, carDatas[carId].Acceleration, carDatas[carId].InitialSpeed);

            totalTravelTime = totalTravelTime + travelTime;
            carDatas[carId].SetTime(travelTime);
            CarDataUpdated?.Invoke(carDatas);

            CarMoving newCarMoving = CreateCarMoving();
            newCarMoving.SetDistance(totalDistance);
            newCarMoving.Init(pathMovePoints, carDatas[carId].Acceleration, carDatas[carId].InitialSpeed, carDatas[carId].Time);
        }

        float mediumTravelTime = (float)totalTravelTime / carDatas.Count;
        foreach (CarMoving carMoving in carMoves)
        {
            float initialSpeed = CalculateInitialVelocity(carMoving.Distance, mediumTravelTime, carMoving.Acceleration);
            carMoving.SetInitialSpeed(initialSpeed);
            carMoving.SetTime(mediumTravelTime);
         
            int carIndex = carMoves.IndexOf(carMoving);
            carDatas[carIndex].SetInitialSpeed(initialSpeed);
            carDatas[carIndex].SetTime(mediumTravelTime);
        }

        CarDataUpdated?.Invoke(carDatas);
        foreach (CarMoving carMoving in carMoves)
            carMoving.StartMove();
    }

    public void CreaseCarArrived()
    {
        carArrived = carArrived + 1;
        CheckEnoughCarArrived();
    }

    public void CheckEnoughCarArrived()
    {
        if (carArrived != carDatas.Count) return;
        EnoughCarArrived?.Invoke();
    }

    public float CalculateTravelTime(float totalDistance, float acceleration, float initialSpeed)
    {
        Debug.Log($"[CarManager] CalculateTravelTime | totalDistance: {totalDistance}," +
            $" acceleration: {acceleration}, initialSpeed: {initialSpeed}");

        if (acceleration != 0f)
        {
            float discriminant = Mathf.Pow(initialSpeed, 2) + 2 * acceleration * totalDistance;
            if (discriminant < 0) return -1f;
            return (float)(-initialSpeed + (float)Mathf.Sqrt(discriminant)) / acceleration;
        }
        else
        {
            if (initialSpeed == 0f) return -1f;
            return (float) totalDistance / initialSpeed;
        }
    }

    public float CalculateInitialVelocity(float distance, float time, float acceleration)
    {
        float initialVelocity = (distance - 0.5f * acceleration * Mathf.Pow(time, 2)) / time;
        return initialVelocity;
    }

    private float TotalDistance(List<Vector3> pathMovePoints)
    {
        float totalDistance = 0;
        for (int i = 1; i < pathMovePoints.Count; i++)
        {
            float distance = Vector3.Distance(pathMovePoints[i], pathMovePoints[i - 1]);
            totalDistance = totalDistance + distance;
        }
        return totalDistance;
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

    public void UpdateCarDatas(List<CarInformation> carInformations)
    {
        if (carDatas.Count != 0) carDatas.Clear();
        foreach (CarInformation carInformation in carInformations)
        {
            CarData carData = new CarData();
            carData.SetAcceleration(carInformation.AccelerationValue());
            carData.SetInitialSpeed(carInformation.InitialSpeedValue());
            carDatas.Add(carData);
        }
    }
}