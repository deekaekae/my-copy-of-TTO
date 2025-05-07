using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;



public class UIManager : MonoBehaviour
{
    
    [Header("Flip Results")]
    [SerializeField] private TextMeshProUGUI playerResultText;
    [SerializeField] private TextMeshProUGUI aiResultText;

    [Header("Player Stats")]
    [SerializeField] private TextMeshProUGUI attemptText;
    [SerializeField] private TextMeshProUGUI roundText;
    [Header("Cash and Multiplier")]
    [SerializeField] private TextMeshProUGUI playerCashText;
    [SerializeField] private TextMeshProUGUI playerMultiplierText;
    [SerializeField] private TextMeshProUGUI aiCashText;
    [SerializeField] private TextMeshProUGUI aiMultiplierText;
    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private GameObject gameStartPanel;
    [Header("Buttons")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button startGameButton;
    [Header("Upgrade Debug Panel")]
    [SerializeField] private GameObject upgradeDebugPanel;
    [SerializeField] private TextMeshProUGUI upgradeDebugText;  
    [SerializeField] private GameObject buyPhasePanel;
    [SerializeField] private TextMeshProUGUI upgradeConfirmText;


    private List<GameObject> previouslyActivePanels = new();
    private bool isDebugViewActive = false;


    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
 
    public void UpdateUpgradeDebugPanel()
    {
        if (upgradeDebugPanel == null || upgradeDebugText == null)
            return;

        upgradeDebugPanel.SetActive(true); // Ensure it's visible when updating

        string upgrades = "Upgrades:\n";
        foreach (var upg in UpgradeManager.Instance.GetAvailableUpgrades())
        {
            int count = UpgradeManager.Instance.GetUpgradeCount(upg.upgradeName);
            if (count > 0)
                upgrades += $"- {upg.upgradeName} ({count}) [{upg.effectStrength * 100f}%]\n";

        }

        upgrades += "\nStats:\n";
        upgrades += $"- Attempts Left: {GameManager.Instance.GetPlayerAttemptsRemaining()}\n";
        upgrades += $"- Multiplier: x{RewardManager.Instance.GetMultiplier()}\n";
        upgrades += $"- Cash: ${RewardManager.Instance.GetCurrentCash()}\n";
        upgrades += "\nProbabilities:\n";
        upgrades += $"- % Heads Chance: {UpgradeEffects.GetChanceToLandHeads()}%\n";
        upgrades += $"- % Tails Chance: {UpgradeEffects.GetChanceToLandTails()}%\n";
        upgrades += $"- % Match Target: {UpgradeEffects.GetChanceToMatchExpected()}%\n";

        upgradeDebugText.text = upgrades;
    }

    public void ShowBuyPhase()
    {
        buyPhasePanel.SetActive(true);
    }

    public void HideBuyPhaseAndContinue()
    {
        buyPhasePanel.SetActive(false);
        GameManager.Instance.StartRound();  // resume game
    }

 
    // ----- Flip Results -----
    public void ShowPlayerResult(string message)
    {
        if (playerResultText != null)
            playerResultText.text = message;
    }

    public void ShowAIResult(string message)
    {
        if (aiResultText != null)
            aiResultText.text = message;
    }

    public void ClearFlipResults()
    {
        playerResultText.text = "";
        aiResultText.text = "";
    }

    // ----- Player Stats -----
    public void ShowAttempts(int remaining)
    {
        attemptText.text = "Mercy Flips: " + remaining;
    }

    public void ShowRound(int round)
    {
        if (roundText != null){
            roundText.text = "Round: " + round;
            Debug.Log("UIManager: Updated round text to Round: " + round);
        }
        else{
            Debug.LogWarning("UIManager: roundText is NULL");
        }
            
    }

    public void ClearAll()
    {
        ClearFlipResults();
        attemptText.text = "";
        //roundText.text = "";
    }

    public void ShowPlayerMessage(string message){
        if (playerResultText != null)
            playerResultText.text = message;
    }

    public void UpdateRewardUI(bool isPlayer, int cash, int multiplier)
    {
        if (isPlayer)
        {
            if (playerCashText != null)
                playerCashText.text = $"Cash: ${cash}";

            if (playerMultiplierText != null)
                playerMultiplierText.text = $"Multiplier: x{multiplier}";
        }
        else
        {
            if (aiCashText != null)
                aiCashText.text = $"Cash: ${cash}";

            if (aiMultiplierText != null)
                aiMultiplierText.text = $"Multiplier: x{multiplier}";
        }
        Debug.Log($"[DEBUG UI] Showing Cash: {cash}, Mult: x{multiplier}");

    }

    public void ShowGameOver(Action onRetry)
    {
        gameOverPanel.SetActive(true);
        retryButton.onClick.RemoveAllListeners();
        retryButton.onClick.AddListener(() => {
            gameOverPanel.SetActive(false);
            onRetry?.Invoke();
        });
    }

    public void ShowGameStart(System.Action onStart)
    {
        gameStartPanel.SetActive(true);
        startGameButton.onClick.RemoveAllListeners();
        startGameButton.onClick.AddListener(() => {
            backgroundPanel.SetActive(true);
            gameStartPanel.SetActive(false);
            onStart?.Invoke();
        });
    }

    public void ToggleUpgradeDebugPanel()
    {
        if (upgradeDebugPanel == null) return;

        isDebugViewActive = !isDebugViewActive;

        if (isDebugViewActive)
        {
            // Hide all active sibling panels except the debug panel
            previouslyActivePanels.Clear();

            foreach (Transform child in upgradeDebugPanel.transform.parent)
            {
                GameObject panel = child.gameObject;

                // Skip self and any inactive objects
                if (panel == upgradeDebugPanel || !panel.activeSelf || panel.name == "StatsButton")
                    continue;

                previouslyActivePanels.Add(panel);
                panel.SetActive(false);
            }

            upgradeDebugPanel.SetActive(true);
            UpdateUpgradeDebugPanel();
        }
        else
        {
            // Restore previously hidden panels
            foreach (var panel in previouslyActivePanels)
            {
                if (panel != null)
                    panel.SetActive(true);
            }

            upgradeDebugPanel.SetActive(false);
        }
    }

    public void ShowUpgradeConfirmation(string upgradeName)
    {
        if (upgradeConfirmText == null) return;

        StopAllCoroutines(); // cancel any previous animations
        upgradeConfirmText.text = $"{upgradeName} Purchased!";
        upgradeConfirmText.alpha = 1f;
        StartCoroutine(FadeOutText(upgradeConfirmText, 3f));
    }

    private IEnumerator FadeOutText(TextMeshProUGUI text, float duration)
    {
        float startAlpha = 1f;
        float time = 0f;

        while (time < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            text.alpha = alpha;
            time += Time.deltaTime;
            yield return null;
        }

        text.alpha = 0f;
    }




}

//test