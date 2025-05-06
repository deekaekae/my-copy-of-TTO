using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;


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


}

//test