using System;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance { get; private set; }

    [Header("Reward Settings")]
    [SerializeField] private int baseCash = 10;
    [SerializeField] private UIManager uiManager;

    private int currentStreak = 0;
    private int multiplier = 1;
    private int totalCash = 0;
    private int aiStreak = 0;
    private int aiMultiplier = 1;
    private int aiCash = 0;

    private bool rewardAlreadyGivenThisCoin = false;

    public event Action<int, int> OnRewardUpdated;
    private int lastRewardAmount = 0;
    public int GetLastRewardAmount() => lastRewardAmount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdatePlayerRewards(){
        rewardAlreadyGivenThisCoin = true;

        currentStreak++;
        multiplier++;

        int before = totalCash;
        totalCash = (int)(MathF.Max(1, totalCash) * multiplier);
        lastRewardAmount = totalCash - before; // Store amount earned this flip

        uiManager.UpdateRewardUI(true, totalCash, multiplier);
        Debug.Log($"[DEBUG] Updated Player Cash: {totalCash}, Mult: x{multiplier}, Earned: ${lastRewardAmount}");
    }

    //AI not fully implememented
    public void UpdateAIRewards(){
        aiStreak++;
        aiMultiplier = 1 + (aiStreak / 2);
        int reward = baseCash * aiMultiplier;
        aiCash += reward;

        Debug.Log($"AI REWARD TRIGGERED - Cash: {aiCash}, Mult: {aiMultiplier}");
        uiManager.UpdateRewardUI(false, aiCash, aiMultiplier);
    }

    public void ResetRewards(){
        currentStreak = 0;
        multiplier = 1;
        totalCash = 10;
        OnRewardUpdated?.Invoke(totalCash, multiplier);
    }

    public void ResetCoinRewardFlag(){
        rewardAlreadyGivenThisCoin = false;
    }

    public void OnPlayerMissed(){
        Debug.Log("Streak broken. Resetting multiplier and scaling cash loss.");

        currentStreak = 0;

        // Determine loss % based on multiplier 
        float lossFactor = 0.05f;  // 5% per x1
        float lossPercent = multiplier * lossFactor;
        lossPercent = Mathf.Clamp(lossPercent, 0f, 0.9f);  // cap at 90% 

        int lostCash = Mathf.FloorToInt(totalCash * lossPercent);
        totalCash -= lostCash;

        multiplier = 1;
        rewardAlreadyGivenThisCoin = true;

        uiManager.UpdateRewardUI(true, totalCash, multiplier);
    }

    public bool SpendCash(int amount){
        if (totalCash >= amount)
        {
            totalCash -= amount;
            uiManager.UpdateRewardUI(true, totalCash, multiplier);
            Debug.Log($"[RewardManager] Spent ${amount}. Remaining: ${totalCash}");
            return true;
        }

        Debug.LogWarning("Not enough cash to spend.");
        return false;
    }
    
    public void ApplyMultiplierBonus(int amount){
        multiplier += amount;
        Debug.Log($"[RewardManager] Multiplier increased by {amount}. New multiplier: {multiplier}");
            uiManager.UpdateRewardUI(true, totalCash, multiplier);

    }

    public void AddCash(int amount){
        if (amount <= 0){
            return;
        }

        totalCash += amount;
        uiManager.UpdateRewardUI(true, totalCash, multiplier);
    }

    public int GetCurrentCash() => totalCash;
    public int GetMultiplier() => multiplier;
    public bool CanAfford(int amount){
        return totalCash >= amount;
    }
    




}
