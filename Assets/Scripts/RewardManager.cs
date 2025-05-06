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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdatePlayerRewards()
    {
        if (rewardAlreadyGivenThisCoin) return;
        rewardAlreadyGivenThisCoin = true;

        currentStreak++;
        
        //OG non-compound logic
        /*
        multiplier++;
        int reward = baseCash * multiplier;
        totalCash += reward;
        */

        multiplier++;
        totalCash = (int)(MathF.Max(1, totalCash) * multiplier);
        uiManager.UpdateRewardUI(true, totalCash, multiplier);
        Debug.Log($"[DEBUG] Updated Player Cash: {totalCash}, Mult: x{multiplier}");

    }

    public void UpdateAIRewards()
    {
        aiStreak++;
        aiMultiplier = 1 + (aiStreak / 2);
        int reward = baseCash * aiMultiplier;
        aiCash += reward;

        Debug.Log($"AI REWARD TRIGGERED - Cash: {aiCash}, Mult: {aiMultiplier}");
        uiManager.UpdateRewardUI(false, aiCash, aiMultiplier);
    }

    public void ResetRewards()
    {
        currentStreak = 0;
        multiplier = 1;
        totalCash = 10;
        OnRewardUpdated?.Invoke(totalCash, multiplier);
    }

    public void ResetCoinRewardFlag()
    {
        rewardAlreadyGivenThisCoin = false;
    }

    public int GetCurrentCash() => totalCash;
    public int GetMultiplier() => multiplier;

    public void OnPlayerMissed()
    {
        Debug.Log("[DEBUG] Streak broken. Resetting multiplier and scaling cash loss.");

        currentStreak = 0;

        // Determine loss % based on multiplier (e.g., 5% per multiplier level)
        float lossFactor = 0.05f;  // 5% per x1
        float lossPercent = multiplier * lossFactor;
        lossPercent = Mathf.Clamp(lossPercent, 0f, 0.9f);  // cap at 90% to avoid total wipe

        int lostCash = Mathf.FloorToInt(totalCash * lossPercent);
        totalCash -= lostCash;

        Debug.Log($"[DEBUG] Player lost {lossPercent * 100f}% (${lostCash}). New cash: {totalCash}");

        multiplier = 1;
        rewardAlreadyGivenThisCoin = true;

        uiManager.UpdateRewardUI(true, totalCash, multiplier);
    }
}
