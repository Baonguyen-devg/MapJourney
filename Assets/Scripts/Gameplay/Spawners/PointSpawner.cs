using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSpawner : ObjectPooling<Point, PointType>
{
    protected override bool CheckMatchValue(PointType matchType, Point component)
    {
        return (component.PointType == matchType);
    }

    public override void Despawn(Point component)
    {
        component.DisAtive();
        components.Add(component);
    }
}
