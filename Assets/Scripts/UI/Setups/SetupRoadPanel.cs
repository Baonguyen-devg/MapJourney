using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetupRoadPanel : MonoBehaviour
{
    [SerializeField] private EnviromentController enviromentController;

    private void OnEnable()
    {
        enviromentController.ConverLineToSpline();
    }
}
