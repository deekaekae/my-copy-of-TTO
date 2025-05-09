using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private CoinSpawner coinSpawner;
    [SerializeField] private Announcer announcer;
    [SerializeField] private PlayerController player;
    [SerializeField] private AIController ai;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private CameraRigController cameraRig; 
    [SerializeField] private GameObject announcerCoin;


    private List<bool> targetSequence = new List<bool>();
    private List<GameObject> currentCoins = new List<GameObject>();
    private List<GameObject> playerCoinsThisRound = new();


    private int currentRound = 1;
    private int coinsPerRound;
    private int currentCoinIndex = 0;

    private bool playerTurnComplete = false;
    private bool aiTurnComplete = false;
    private int playerAttemptsRemaining = 2;
    private int playerFlipCount = 0;

    
    private bool lastFlipMatched; 
    public int GetPlayerAttemptsRemaining() => playerAttemptsRemaining;

    private enum GameState
    {
        Idle,
        Announce,
        PlayerTurn,
        AITurn,
        EndRound
    }

    private GameState currentState = GameState.Idle;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        uiManager.ShowGameStart(() => StartCoroutine(DelayedStartRound()));
    }

    public void OnPlayerFinishedTurn(bool success){
        if (currentCoinIndex >= targetSequence.Count){
            Debug.LogWarning("OnPlayerFinishedTurn called after round ended.");
            return;
        }

        playerTurnComplete = true;
        currentState = GameState.AITurn;

        cameraRig.MoveToAI();  // AI turn camera move
        ai.StartTurn(targetSequence[currentCoinIndex]);

        CheckTurnComplete();
    }

    private List<bool> GenerateSequence(int length){
        List<bool> sequence = new List<bool>();
        for (int i = 0; i < length; i++){
            sequence.Add(Random.value < 0.5f);
        }
        return sequence;
    }

    public void StartRound(){
        //tracking for putting round coins aside
        coinSpawner.ResetCoinLayout();
        currentCoinIndex = 0;

        playerCoinsThisRound.Clear();
        playerFlipCount = 0;
        

        coinsPerRound = currentRound;
        targetSequence = GenerateSequence(coinsPerRound);

        coinSpawner.ClearCoins();
        currentCoins.Clear();

        uiManager?.ShowRound(currentRound);
        uiManager?.ClearFlipResults();

        ResetPlayerAttempts();

        cameraRig.MoveToAnnouncer();  
        ProceedToNextCoin();
    }

    public void ProceedToNextCoin(){
        if (currentCoinIndex >= coinsPerRound){
            EndRound();
            return;
        }

        playerTurnComplete = false;
        aiTurnComplete = false;
        
        bool nextCall = targetSequence[currentCoinIndex];
        StartCoroutine(AnnounceAndWait(nextCall));

        // If there's a previous coin, move it aside BEFORE spawning a new one
       if (playerCoinsThisRound.Count > 0){
            GameObject lastPlayerCoin = playerCoinsThisRound[playerCoinsThisRound.Count - 1];
            coinSpawner.MoveCoinAside(lastPlayerCoin);
        }

        GameObject playerCoin = coinSpawner.SpawnPlayerCoin();
        playerCoinsThisRound.Add(playerCoin);
        GameObject aiCoin = coinSpawner.SpawnAICoin();
        currentCoins.Add(playerCoin);

        player.SetCurrentCoin(playerCoin);
        ai.SetCurrentCoin(aiCoin);

        cameraRig.SetPlayerLookTarget(playerCoin.transform);
        cameraRig.SetAILookTarget(aiCoin.transform);

        CoinClickTarget clickTarget = playerCoin.GetComponent<CoinClickTarget>();
        if (clickTarget != null)
            clickTarget.SetActive(true);
        

        currentState = GameState.Announce;
        //COME BACK HERE AFTER ANIMATION TO ADJUST INTRO TIME 
        StartCoroutine(TransitionToPlayerTurn(5f));
        RewardManager.Instance.ResetCoinRewardFlag();
    }

    private System.Collections.IEnumerator TransitionToPlayerTurn(float delay){
        yield return new WaitForSeconds(delay);

        if (currentCoinIndex >= targetSequence.Count){
            Debug.LogWarning("Tried to start player turn after sequence ended.");
            yield break;
        }

        bool expected = targetSequence[currentCoinIndex];
        //this is tilt 
        UpgradeEffects.ApplyAllEffects(ref expected, currentCoinIndex);
        currentState = GameState.PlayerTurn;

        cameraRig.MoveToPlayer();  
        player.StartTurn(expected);
    }

    public void OnAIFinishedTurn(bool success){
        if (currentCoinIndex >= targetSequence.Count){
            Debug.LogWarning("OnAIFinishedTurn called after round ended.");
            return;
        }

        aiTurnComplete = true;
        cameraRig.MoveToAnnouncer(); 
        CheckTurnComplete();
    }

    private void CheckTurnComplete(){
        if (playerTurnComplete && aiTurnComplete){
            // Move the current coin BEFORE incrementing the index
            GameObject completedCoin = currentCoins[currentCoinIndex];
            coinSpawner.MoveCoinAside(completedCoin);

            // Disable  click target
            CoinClickTarget clickTarget = completedCoin.GetComponent<CoinClickTarget>();
            if (clickTarget != null)
                clickTarget.SetActive(false);

            currentCoinIndex++;
            ProceedToNextCoin();
        }
    }


    private void EndRound(){
        currentState = GameState.EndRound;
        Debug.Log($"Round {currentRound} Complete.");

        UIManager.Instance.ShowBuyPhase();
        currentRound++; 
    }

    private System.Collections.IEnumerator NextRoundDelay(float delay){
        yield return new WaitForSeconds(delay);
        StartRound();
    }

    public void UsePlayerAttempt(){
        if (playerAttemptsRemaining > 0)
            playerAttemptsRemaining--;

        UIManager.Instance?.ShowAttempts(playerAttemptsRemaining);
    }

    public void AddExtraAttempt(){
        playerAttemptsRemaining++;
        UIManager.Instance?.ShowAttempts(playerAttemptsRemaining);
    }

    public void ResetPlayerAttempts(int value = 2){
        playerAttemptsRemaining = value;

        //Mercy effect type not yet implemented
        foreach (var upgrade in UpgradeManager.Instance.GetPlayerUpgrades()){
            if (upgrade.effectType == UpgradeEffectType.Mercy &&
                upgrade.upgradeName == "Extra Attempt" &&
                upgrade.category == UpgradeCategory.Passive)
            {
                int bonus = Mathf.RoundToInt(upgrade.effectStrength);
                playerAttemptsRemaining += bonus;
                Debug.Log($"[GameManager] Mercy upgrade added {bonus} extra attempt(s).");
            }
        }

        if (UIManager.Instance != null)
            UIManager.Instance.ShowAttempts(playerAttemptsRemaining);
        else
            Debug.LogWarning("UIManager.Instance was null when resetting attempts.");
    }


    private System.Collections.IEnumerator DelayedStartRound(){
        while (UIManager.Instance == null){
            yield return null;
        }

        StartRound();
    }

    public void EndGameDueToPlayerFailure(){
        StopAllCoroutines();
        currentState = GameState.Idle;
        uiManager.ShowGameOver(RestartGame);
    }

    private void RestartGame(){
        Debug.Log("Restarting game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator AnnounceAndWait(bool call){
        cameraRig.MoveToAnnouncer();
        yield return new WaitForSeconds(1.5f); 

        announcer.Announce(call);

        // Flip the announcer's coin with animation
        if (announcerCoin != null){
            var flipper = announcerCoin.GetComponent<CoinFlipper>();
            if (flipper != null){
                flipper.ResetState();  
                flipper.Flip(call);    
            }
        
        }

        yield return new WaitForSeconds(3.5f); // wait for animation to finish
    }

    public void SetLastFlipMatched(bool matched){
        lastFlipMatched = matched;
    }

    public bool LastFlipMatched(){
        return lastFlipMatched;
    }
    public bool IsPlayerTurn(){
        return currentState == GameState.PlayerTurn;
    }

    


}
