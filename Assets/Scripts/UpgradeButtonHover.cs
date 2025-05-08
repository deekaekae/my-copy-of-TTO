using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UpgradeButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Upgrade upgradeData;  // Assigned when button is created

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (upgradeData != null)
        {
            UIManager.Instance.ShowUpgradeDescription(upgradeData.description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.ClearUpgradeDescription();
    }
}

