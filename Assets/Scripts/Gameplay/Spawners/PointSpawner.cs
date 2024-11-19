public class PointSpawner : ObjectPooling<Point, PointType>
{
    protected override bool CheckMatchValue(PointType matchType, Point component)
    {
        return (component.PointType == matchType);
    }

    public override void Despawn(Point component)
    {
        component.DisActive();
        components.Add(component);
    }
}
