using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetupRoadPanel : MonoBehaviour
{
    [SerializeField] private Enviroment enviromentController;

    private void OnEnable()
    {
        enviromentController.ConvertLineToSpline();
    }
}
