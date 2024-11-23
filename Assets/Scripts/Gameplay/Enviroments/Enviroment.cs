using UnityEngine;

public class Enviroment : Singleton<Enviroment>
{
    [Header("Managers:"), Space(6)]    
    [SerializeField] private PointManager pointManager;
    [SerializeField] private SolidLineManager solidLineManager;
    [SerializeField] private CarManager carManager;

    public PointManager PointManager => pointManager;
    public SolidLineManager SolidLineManager => solidLineManager;
    public CarManager CarManager => carManager;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void ResetGame()
    {
        pointManager.RemoveAllPoint();
        solidLineManager.RemoveAllSolidLine();
        carManager.DestroyAllCarMovements();
    }
}
