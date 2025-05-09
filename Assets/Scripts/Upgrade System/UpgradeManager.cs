
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [SerializeField] private List<Upgrade> availableUpgrades;

    private List<Upgrade> playerUpgrades = new();
    private List<Upgrade> oneTimeUseUpgrades = new();
    


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        
    }

   
    public List<Upgrade> GetAvailableUpgrades()
    {
        return availableUpgrades;
    }

    public int GetUpgradeCount(string upgradeName){
        int count = 0;
        foreach (var upgrade in playerUpgrades){
            if (upgrade.upgradeName == upgradeName)
                count++;
        }
        foreach (var upgrade in oneTimeUseUpgrades){
            if (upgrade.upgradeName == upgradeName)
                count++;
        }
        return count;
    }

    public bool TryPurchaseUpgrade(Upgrade upgrade){
        if (RewardManager.Instance.GetCurrentCash() >= upgrade.cost){
            RewardManager.Instance.SpendCash(upgrade.cost);

            if (upgrade.category == UpgradeCategory.OneTimeUse){
                oneTimeUseUpgrades.Add(upgrade);
            }
            else{
                playerUpgrades.Add(upgrade);
            }

            int count = GetUpgradeCount(upgrade.upgradeName);
            Debug.Log($"Purchased upgrade: {upgrade.upgradeName} (Now owns {count})");
            return true;
        }
        return false;
    }


    public bool HasUpgradeNamed(string upgradeName){
        foreach (var upgrade in GetPlayerUpgrades()){
            if (upgrade.upgradeName == upgradeName)
                return true;
        }
        return false;
    }


    public void UseOneTimeUpgrade(string upgradeName){
        for (int i = 0; i < oneTimeUseUpgrades.Count; i++){
            if (oneTimeUseUpgrades[i].upgradeName == upgradeName){
                Debug.Log($"Used one-time upgrade: {upgradeName}");
                oneTimeUseUpgrades.RemoveAt(i);
                return;
            }
        }
    }


    public List<Upgrade> GetPlayerUpgrades(){
        return playerUpgrades;
    }
    public List<Upgrade> GetOneTimeUseUpgrades(){
        return oneTimeUseUpgrades;
    }

    




}
