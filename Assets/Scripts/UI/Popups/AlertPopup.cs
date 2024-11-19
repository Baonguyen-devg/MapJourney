using TMPro;
using UnityEngine;

public class AlertPopup : BasePopup
{
    public static readonly string ALERT_DO_NOT_ENOUGH_JOURNEY_POINT = "You have not set up enough journey points";
    public static readonly string ALERT_IN_DEMO_MODE = "You are in demo mode";

    [SerializeField] private TextMeshProUGUI contentText;

    public void ChangeContentText(string content)
    {
        Debug.Log("[AlertPopup] ChangeContentText | " + content);
        contentText.text = content;
    }
}
