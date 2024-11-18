using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PopupManager : Singleton<PopupManager>
{
    public enum PopupType
    {
        Setting,
        Alert,
        Credit,
    }

    [SerializeField] private List<BasePopup> popupPrefabs = new List<BasePopup>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void CloseAllPopup()
    {
        foreach (BasePopup basePopup in popupPrefabs)
        {
            basePopup.gameObject.SetActive(false);
        }
    }

    public BasePopup GetPopup(PopupType popupType)
    {
        foreach (BasePopup basePopup in popupPrefabs)
        {
            if (basePopup.PopupType == popupType)
                return basePopup;
        }
        return null;
    }

    public void OpenPopup<T>(PopupType popupType) where T : BasePopup
    {
        BasePopup basePopup = GetPopup(popupType);
        T popup = basePopup as T;
        popup?.Open();
    }

    public void ClosePopup<T>(PopupType popupType) where T : BasePopup
    {
        BasePopup basePopup = Instance.GetPopup(popupType);
        T popup = basePopup as T;
        popup?.Close();
    }
}
