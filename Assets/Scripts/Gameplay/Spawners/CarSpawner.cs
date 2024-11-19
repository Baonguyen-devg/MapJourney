public class CarSpawner : ObjectPooling<CarMovement, CarType>
{
    protected override bool CheckMatchValue(CarType matchType, CarMovement component)
    {
        return (component.CarType == matchType);
    }
}
