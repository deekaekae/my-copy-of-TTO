using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuyPhaseUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;

    private void OnEnable()
    {
        ClearButtons();

        if (buttonPrefab == null){
            return;
        }

        if (buttonContainer == null){
            return;
        }

        Debug.Log("Instantiating upgrade buttons...");

        foreach (var upgrade in UpgradeManager.Instance.GetAvailableUpgrades()){
            if (upgrade == null){
                continue;
            }

            Debug.Log($"Loaded Upgrade: {upgrade.upgradeName}");
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            var text = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            
            if (text == null){
                Debug.LogError("No TextMeshProUGUI found in button.");
                continue;
            }

            text.text = $"{upgrade.upgradeName} - ${upgrade.cost}";

            //hover for description
            var hover = btnObj.GetComponent<UpgradeButtonHover>();
            if (hover != null)
                hover.upgradeData = upgrade;

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null){
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

    private void ClearButtons(){
        foreach (Transform child in buttonContainer){
            Destroy(child.gameObject);
        }
    }
}

