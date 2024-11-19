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

    public int CarID => carId;

    public void SetCarId(int id) => carId = id;

    public void SetTime(float time) => timeInput.text = time.ToString();
    public void SetAcceleration(float acceleration) => accelerationInput.text = acceleration.ToString();
    public void SetInitialSpeed(float initialSpeed) => initialSpeedInput.text = initialSpeed.ToString();

    public float TimeValue => float.TryParse(timeInput.text, out float time) ? time : 0;

    public float AccelerationValue()
    {
        return float.TryParse(accelerationInput.text, out float acceleration) ? acceleration : 0;
    }

    public float InitialSpeedValue()
    {
        return float.TryParse(initialSpeedInput.text, out float initialSpeed) ? initialSpeed : 0;
    }

    public void Init(int carId, float acceleration, float initialSpeed)
    {
        this.carId = carId;
        accelerationInput.text = acceleration.ToString();
        initialSpeedInput.text = initialSpeed.ToString();
        carNameText.text = "Car " + carId.ToString();
    }

    public void OnAccelerationChange()
    {
        if (accelerationInput != null && float.TryParse(accelerationInput.text, out float acceleration))
        {
            if (Enviroment.Instance != null && Enviroment.Instance.CarManager != null)
            {
                Enviroment.Instance.CarManager.SetAcceleration(acceleration, carId);
            }
            else
            {
                Debug.LogError("Enviroment.Instance or CarManager is null.");
            }
        }
        else
        {
            Debug.LogWarning("Acceleration input is invalid.");
        }
    }

    public void OnInitialSpeedChange()
    {
        if (initialSpeedInput != null && float.TryParse(initialSpeedInput.text, out float initialSpeed))
        {
            if (Enviroment.Instance != null && Enviroment.Instance.CarManager != null)
            {
                Enviroment.Instance.CarManager.SetInitialSpeed(initialSpeed, carId);
            }
            else
            {
                Debug.LogError("Enviroment.Instance or CarManager is null.");
            }
        }
        else
        {
            Debug.LogWarning("Initial speed input is invalid.");
        }
    }

    public void OnTimeChange()
    {
        if (timeInput != null && float.TryParse(timeInput.text, out float time))
        {
            if (Enviroment.Instance != null && Enviroment.Instance.CarManager != null)
            {
                Enviroment.Instance.CarManager.SetTime(time, carId);
            }
            else
            {
                Debug.LogError("Enviroment.Instance or CarManager is null.");
            }
        }
        else
        {
            Debug.LogWarning("Time input is invalid.");
        }
    }
}
