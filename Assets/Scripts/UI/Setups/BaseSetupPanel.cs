using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SetupPanel : MonoBehaviour
{
    protected readonly Color UNSELECTED_COLOR = new Color(0.2f, 0.2f, 0.2f, 1);
    protected readonly Color SELECTED_COLOR = new Color(0.13f, 0.13f, 0.13f, 1);

    [SerializeField] protected Image boardImage;

    public virtual void OnSelected()
    {
        boardImage.DOColor(SELECTED_COLOR, 0.3f);
    }

    public virtual void OnUnselected()
    {
        boardImage.DOColor(UNSELECTED_COLOR, 0.3f);
    }
}
