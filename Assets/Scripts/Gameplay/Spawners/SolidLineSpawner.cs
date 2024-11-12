using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidLineSpawner : ObjectPooling<SolidLine, SolidLineType>
{
    protected override bool CheckMatchValue(SolidLineType matchType, SolidLine component)
    {
        return (component.SolidLineType == matchType);
    }
}
