public class SolidLineSpawner : ObjectPooling<SolidLine, SolidLineType>
{
    protected override bool CheckMatchValue(SolidLineType matchType, SolidLine component)
    {
        return (component.SolidLineType == matchType);
    }
}
