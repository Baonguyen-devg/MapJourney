using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarInformation : MonoBehaviour
{
    [Header("InputFields"), Space(6)]
    [SerializeField] private TMP_InputField accelerationInput;
    [SerializeField] private TMP_InputField initialSpeedInput;
    [SerializeField] private TMP_InputField timeInput;

    [SerializeField] private TextMeshProUGUI carNameText;
    [SerializeField] private int carId;

    public void SetCarId(int id) => carId = id;
    public int CarID => carId;

    public void SetTime(float time) => timeInput.text = time.ToString();
    public void SetAcceleration(float acceleration) => accelerationInput.text = acceleration.ToString();
    public void SetInitialSpeed(float initialSpeed) => initialSpeedInput.text = initialSpeed.ToString();
    public float TimeValue => float.Parse(timeInput.text);

    public float AccelerationValue()
    {
        if (float.TryParse(accelerationInput.text, out float acceleration))
            return acceleration;
        return 0;
    }

    public float InitialSpeedValue()
    {
        if (float.TryParse(initialSpeedInput.text, out float InitialSpeed))
            return InitialSpeed;
        return 0;
    }

    public void Init(int carId, float accleration, float initialSpeed)
    {
        this.carId = carId;
        this.accelerationInput.text = accleration.ToString();
        this.initialSpeedInput.text = initialSpeed.ToString();
        carNameText.text = "Car " + carId.ToString();
    }

    public void OnAccelerationChange()
    {
        if (float.TryParse(accelerationInput.text, out float acceleration))
        {
            Enviroment.Instance.CarManager.SetAcceleration(acceleration, carId);
        }
    }

    public void OnInitialSpeedChange()
    {
        if (float.TryParse(initialSpeedInput.text, out float InitialSpeed))
        {
            Enviroment.Instance.CarManager.SetInitialSpeed(InitialSpeed, carId);
        }
    }

    public void OnTimeChange()
    {
        if (float.TryParse(timeInput.text, out float time))
        {
            Enviroment.Instance.CarManager.SetTime(time, carId);
        }
    }
}
