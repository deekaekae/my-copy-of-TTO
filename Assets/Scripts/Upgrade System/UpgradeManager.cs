
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [SerializeField] private List<Upgrade> availableUpgrades;

    private List<Upgrade> playerUpgrades = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //InitializeUpgrades();
    }

    /*
    private void InitializeUpgrades()
    {
       

        availableUpgrades.Clear();
        /*
        availableUpgrades.Add(new Upgrade("Double or Nothing", "Double your reward on success.", 20, Upgrade.UpgradeType.DoubleOrNothing, true));
        availableUpgrades.Add(new Upgrade("Tilt", "Increase odds of HEADS or TAILS.", 15, Upgrade.UpgradeType.Tilt, false));
        availableUpgrades.Add(new Upgrade("Streaks", "Boost your multiplier more per streak.", 25, Upgrade.UpgradeType.Streaks, false));
        availableUpgrades.Add(new Upgrade("Thief", "Steal one of the opponent's upgrades.", 30, Upgrade.UpgradeType.Thief, true));
        availableUpgrades.Add(new Upgrade("Stun", "Skip opponent turn on success.", 35, Upgrade.UpgradeType.Stun, true));
        availableUpgrades.Add(new Upgrade("Mercy", "Permanently gain an extra flip attempt.", 40, Upgrade.UpgradeType.Mercy, false));
        
    }
    */
    public List<Upgrade> GetAvailableUpgrades()
    {
        return availableUpgrades;
    }

    public int GetUpgradeCount(string upgradeName)
    {
        int count = 0;
        foreach (var upgrade in playerUpgrades)
        {
            if (upgrade.upgradeName == upgradeName)
                count++;
        }
        return count;
    }



    public bool TryPurchaseUpgrade(Upgrade upgrade)
    {
        if (RewardManager.Instance.GetCurrentCash() >= upgrade.cost)
        {
            RewardManager.Instance.SpendCash(upgrade.cost);

            playerUpgrades.Add(upgrade);

            int count = GetUpgradeCount(upgrade.upgradeName);
           Debug.Log($"Purchased upgrade: {upgrade.upgradeName} (Now owns {count})");
            return true;
        }
        return false;
    }

    public bool HasUpgradeNamed(string upgradeName)
    {
        foreach (var upgrade in GetPlayerUpgrades())
        {
            if (upgrade.upgradeName == upgradeName)
                return true;
        }
        return false;
    }


    public void UseOneTimeUpgrade(string upgradeName)
    {
        for (int i = 0; i < playerUpgrades.Count; i++)
        {
            if (playerUpgrades[i].upgradeName == upgradeName &&
                playerUpgrades[i].category == UpgradeCategory.OneTimeUse)
            {
                playerUpgrades.RemoveAt(i);
                Debug.Log($"Used one-time upgrade: {upgradeName}");
                return;
            }
        }
    }

    public List<Upgrade> GetPlayerUpgrades()
    {
        return playerUpgrades;
    }


}
