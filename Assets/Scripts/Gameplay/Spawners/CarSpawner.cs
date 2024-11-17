using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : ObjectPooling<CarMoving, CarType>
{
    protected override bool CheckMatchValue(CarType matchType, CarMoving component)
    {
        return (component.CarType == matchType);
    }
}
