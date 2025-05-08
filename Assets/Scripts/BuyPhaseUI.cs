using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuyPhaseUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;
    //[SerializeField] private GameObject BuyPhasePanel;

    private void OnEnable()
    {
        ClearButtons();

        if (buttonPrefab == null)
        {
            Debug.LogError("[BuyPhaseUI] Button Prefab is NOT assigned!");
            return;
        }

        if (buttonContainer == null)
        {
            Debug.LogError("[BuyPhaseUI] Button Container is NOT assigned!");
            return;
        }

        Debug.Log("[BuyPhaseUI] Instantiating upgrade buttons...");

        foreach (var upgrade in UpgradeManager.Instance.GetAvailableUpgrades())
        {
            if (upgrade == null)
            {
                Debug.LogWarning("[BuyPhaseUI] Skipped null upgrade entry.");
                continue;
            }

            Debug.Log($"Loaded Upgrade: {upgrade.upgradeName}");
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            var text = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text == null)
            {
                Debug.LogError("[BuyPhaseUI] No TextMeshProUGUI found in button.");
                continue;
            }

            text.text = $"{upgrade.upgradeName} - ${upgrade.cost}";

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                var upgradeCopy = upgrade;
                btn.onClick.AddListener(() =>
                {
                    bool success = UpgradeManager.Instance.TryPurchaseUpgrade(upgradeCopy);
                    if (success){
                        UIManager.Instance.ShowUpgradeConfirmation(upgradeCopy.upgradeName);
                        UIManager.Instance.UpdateBuyPhaseCashDisplay();
                        UIManager.Instance.UpdateRewardUI(true, RewardManager.Instance.GetCurrentCash(), RewardManager.Instance.GetMultiplier());

                    }
                });
            }
        }

    }


    private void ClearButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
    }
}

