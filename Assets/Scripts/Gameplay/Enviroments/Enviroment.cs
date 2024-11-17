using Dreamteck.Splines;
using System.Collections.Generic;
using UnityEngine;

public class Enviroment : Singleton<Enviroment>
{
    [Header("Managers:"), Space(6)]    
    [SerializeField] private PointManager pointManager;
    [SerializeField] private SolidLineManager solidLineManager;
    [SerializeField] private PathFindingManager pathFindingManager;
    [SerializeField] private CarManager carManager;

    public PointManager PointManager => pointManager;
    public SolidLineManager SolidLineManager => solidLineManager;
    public PathFindingManager PathFindingManager => pathFindingManager;
    public CarManager CarManager => carManager;

    [Header("Other components:"), Space(6)]    
    [SerializeField] private SetUpsController setUpsController;
    [SerializeField] private SplineComputer roadSplineComputer;

    public SetUpsController SetUpsController => setUpsController;

    [ContextMenu("Convert solidLine to spline")]
    public void ConvertLineToSpline()
    {
        roadSplineComputer.gameObject.SetActive(true);
        roadSplineComputer.SetPoints(new SplinePoint[0]);

        List<SplinePoint> splinePoints = new List<SplinePoint>();
        foreach (SolidLine solidLine in solidLineManager.SolidLines)
        {
            float segmentLength = Vector3.Distance(solidLine.StartPosition, solidLine.EndPosition);

            Vector3 start = solidLine.StartPosition + (solidLine.EndPosition - solidLine.StartPosition).normalized * 0.5f;
            Vector3 end = solidLine.EndPosition - (solidLine.EndPosition - solidLine.StartPosition).normalized * 0.5f;

            splinePoints.Add(new SplinePoint(start));
            splinePoints.Add(new SplinePoint(end));
        }
        roadSplineComputer.SetPoints(splinePoints.ToArray());
    }
}
