using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class BasePopup : MonoBehaviour
{
    [SerializeField] protected PopupManager.PopupType popupType;
    [SerializeField] protected Button closePopupButton;
    [SerializeField] protected Transform board;
    [SerializeField] protected Image backgroundImage;

    public PopupManager.PopupType PopupType => popupType;

    protected virtual void Awake() => closePopupButton?.onClick.AddListener(Close);

    public virtual void Open()
    {
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.Popup);
        gameObject.SetActive(true);

        if (board == null) return;

        board.localScale = Vector3.zero;
        board.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear);
        backgroundImage.DOFade(0.95f, 0.2f).SetEase(Ease.Linear);
    }
    public virtual void Close()
    {
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.Popdown);

        if (board == null)
        {
            gameObject.SetActive(false);
            return;
        }

        backgroundImage.DOFade(0, 0.2f).SetEase(Ease.Linear);
        board.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
