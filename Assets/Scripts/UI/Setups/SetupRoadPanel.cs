using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetupRoadPanel : MonoBehaviour
{
    private void Start()
    {
        Enviroment.Instance.ConvertLineToSpline();
    }
}
