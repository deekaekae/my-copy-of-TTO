
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [SerializeField] private List<Upgrade> availableUpgrades;

    private Dictionary<Upgrade.UpgradeType, int> playerUpgrades = new();

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

    public int GetUpgradeCount(Upgrade.UpgradeType type)
    {
        return playerUpgrades.ContainsKey(type) ? playerUpgrades[type] : 0;
    }

    public bool TryPurchaseUpgrade(Upgrade upgrade)
    {
        if (RewardManager.Instance.GetCurrentCash() >= upgrade.cost)
        {
            RewardManager.Instance.SpendCash(upgrade.cost);

            if (playerUpgrades.ContainsKey(upgrade.type))
                playerUpgrades[upgrade.type]++;
            else
                playerUpgrades[upgrade.type] = 1;

            Debug.Log($"Purchased upgrade: {upgrade.upgradeName} (Now owns {playerUpgrades[upgrade.type]})");
            return true;
        }
        return false;
    }

    public bool HasUpgrade(Upgrade.UpgradeType type)
    {
        return playerUpgrades.ContainsKey(type) && playerUpgrades[type] > 0;
    }

    public void UseOneTimeUpgrade(Upgrade.UpgradeType type)
    {
        if (playerUpgrades.ContainsKey(type) && playerUpgrades[type] > 0)
        {
            playerUpgrades[type]--;
            Debug.Log($"Used one-time upgrade: {type} (Remaining: {playerUpgrades[type]})");

            if (playerUpgrades[type] <= 0)
                playerUpgrades.Remove(type);
        }
    }
}
