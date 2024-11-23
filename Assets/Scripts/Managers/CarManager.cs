using System;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    [Header("Databases"), Space(6)]
    [SerializeField] private CarSpawner carSpawner;
    [SerializeField] private List<CarData> carDatas = new List<CarData>();
    [SerializeField] private List<CarMovement> carMovements = new List<CarMovement>();

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

        DestroyAllCarMovements();
        totalTravelTime = CreateCarMovementsAt(totalTravelTime, startJourneyPoints, endJourneyPoint);

        float mediumTravelTime = (float)totalTravelTime / carDatas.Count;
        ApplyMediumTravelTime(mediumTravelTime);
        StartDemoCarMovement();
    }

    private void StartDemoCarMovement()
    {
        CarDataUpdated?.Invoke(carDatas);
        CameraManager.Instance.CreateVirtualCameraFollowCar(carMovements);
        foreach (CarMovement carMovement in carMovements)
            carMovement.StartMove();
    }

    private void ApplyMediumTravelTime(float mediumTravelTime)
    {
        if (carMovements.Count != 1)
        {
            foreach (CarMovement carMoving in carMovements)
            {
                float initialSpeed = CalculateInitialVelocity(carMoving.Distance, mediumTravelTime, carMoving.Acceleration);
                carMoving.SetInitialSpeed(initialSpeed);
                carMoving.SetTime(mediumTravelTime);

                int carIndex = carMovements.IndexOf(carMoving);
                carDatas[carIndex].SetInitialSpeed(initialSpeed);
                carDatas[carIndex].SetTime(mediumTravelTime);
            }
        }
    }

    private float CreateCarMovementsAt(float totalTravelTime, List<Point> startJourneyPoints, Point endJourneyPoint)
    {
        foreach (Point startPoint in startJourneyPoints)
        {
            Debug.Log($"[CarManager] DemoCarMove | From {startPoint} to {endJourneyPoint}");
            List<Vector3> pathMovePoints = PathFinding.FindPathDijkstra(startPoint, endJourneyPoint);

            int carId = startJourneyPoints.IndexOf(startPoint);
            float totalDistance = TotalDistance(pathMovePoints);
            float travelTime = CalculateTravelTime(totalDistance, carDatas[carId].Acceleration, carDatas[carId].InitialSpeed);

            totalTravelTime = totalTravelTime + travelTime;
            carDatas[carId].SetTime(travelTime);

            CarMovement newCarMoving = CreateCarMoving();
            newCarMoving.SetDistance(totalDistance);
            newCarMoving.Init(pathMovePoints, carDatas[carId].Acceleration, carDatas[carId].InitialSpeed, carDatas[carId].Time);
        }

        return totalTravelTime;
    }

    public void CreaseCarArrived()
    {
        carArrived = carArrived + 1;
        CheckEnoughCarArrived();
    }

    public void CheckEnoughCarArrived()
    {
        if (!IsEnoughCarArrived()) return;
        EnoughCarArrived?.Invoke();
    }

    public bool IsEnoughCarArrived()
    {
        return carArrived == carDatas.Count;
    }

    public float CalculateTravelTime(float totalDistance, float acceleration, float initialSpeed)
    {
        Debug.Log($"[CarManager] CalculateTravelTime | totalDistance: {totalDistance}, " +
                  $"acceleration: {acceleration}, initialSpeed: {initialSpeed}");

        double distance = totalDistance;
        double accel = acceleration;
        double initSpeed = initialSpeed;

        if (Math.Abs(accel) > Mathf.Epsilon)
        {
            double discriminant = Math.Pow(initSpeed, 2) + 2 * accel * distance;
            if (discriminant < 0)
                return -1f; 

            return (float)((-initSpeed + Math.Sqrt(discriminant)) / accel); 
        }
        else
        {
            if (Math.Abs(initSpeed) < Mathf.Epsilon)
                return -1f; 
            return (float)(distance / initSpeed);
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

    public CarMovement CreateCarMoving()
    {
        CarMovement carMovement = carSpawner.Spawn(CarType.Default);
        carMovements.Add(carMovement);
        return carMovement;
    }

    public void DestroyCarMoving(CarMovement carMovement)
    {
        if (!carMovements.Contains(carMovement)) return;
        carMovements.Remove(carMovement);
        carSpawner.Despawn(carMovement);
    }

    public void DestroyAllCarMovements()
    {
        for (int i = carMovements.Count - 1; i >= 0; i--)
            DestroyCarMoving(carMovements[i]);
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