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
    [SerializeField] private CameraRigController cameraRig;  // 👈 Camera rig reference

    private List<bool> targetSequence = new List<bool>();
    private List<GameObject> currentCoins = new List<GameObject>();

    private int currentRound = 1;
    private int coinsPerRound;
    private int currentCoinIndex = 0;

    private bool playerTurnComplete = false;
    private bool aiTurnComplete = false;
    private int playerAttemptsRemaining = 2;

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

    public void OnPlayerFinishedTurn(bool success)
    {
        if (currentCoinIndex >= targetSequence.Count)
        {
            Debug.LogWarning("OnPlayerFinishedTurn called after round ended.");
            return;
        }

        playerTurnComplete = true;
        currentState = GameState.AITurn;

        cameraRig.MoveToAI();  // 👈 Move to AI view after player finishes
        ai.StartTurn(targetSequence[currentCoinIndex]);

        CheckTurnComplete();
    }

    private List<bool> GenerateSequence(int length)
    {
        List<bool> sequence = new List<bool>();
        for (int i = 0; i < length; i++)
        {
            sequence.Add(Random.value < 0.5f);
        }
        return sequence;
    }

    public void StartRound()
    {
        currentCoinIndex = 0;
        coinsPerRound = currentRound;
        targetSequence = GenerateSequence(coinsPerRound);

        coinSpawner.ClearCoins();
        currentCoins.Clear();

        Debug.Log($"Starting Round {currentRound} with {coinsPerRound} coins.");

        uiManager?.ShowRound(currentRound);
        uiManager?.ClearFlipResults();

        ResetPlayerAttempts();

        cameraRig.MoveToAnnouncer();  // 👈 Start at announcer's view
        ProceedToNextCoin();
    }

    public void ProceedToNextCoin()
    {
        if (currentCoinIndex >= coinsPerRound)
        {
            Debug.Log("All coins played. Ending round.");
            EndRound();
            return;
        }

        playerTurnComplete = false;
        aiTurnComplete = false;

        bool nextCall = targetSequence[currentCoinIndex];
        StartCoroutine(AnnounceAndWait(nextCall));

        GameObject newCoin = coinSpawner.SpawnSingleCoin();
        currentCoins.Add(newCoin);

        CoinClickTarget clickTarget = newCoin.GetComponent<CoinClickTarget>();
        if (clickTarget != null)
            clickTarget.SetActive(true);

        player.SetCurrentCoin(newCoin);
        ai.SetCurrentCoin(newCoin);

        currentState = GameState.Announce;
        //COME BACK HERE AFTER ANIMATION TO ADJUST INTRO TIME 
        StartCoroutine(TransitionToPlayerTurn(5f));
        RewardManager.Instance.ResetCoinRewardFlag();
    }

    private System.Collections.IEnumerator TransitionToPlayerTurn(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentCoinIndex >= targetSequence.Count)
        {
            Debug.LogWarning("Tried to start player turn after sequence ended.");
            yield break;
        }

        bool expected = targetSequence[currentCoinIndex];
        currentState = GameState.PlayerTurn;

        cameraRig.MoveToPlayer();  // 👈 Move to player camera for turn
        player.StartTurn(expected);
    }

    public void OnAIFinishedTurn(bool success)
    {
        if (currentCoinIndex >= targetSequence.Count)
        {
            Debug.LogWarning("OnAIFinishedTurn called after round ended.");
            return;
        }

        aiTurnComplete = true;

        cameraRig.MoveToAnnouncer();  // 👈 Return to announcer after AI finishes
        CheckTurnComplete();
    }

    private void CheckTurnComplete()
    {
        if (playerTurnComplete && aiTurnComplete)
        {
            coinSpawner.MoveCoinAside(currentCoins[currentCoinIndex]);

            CoinClickTarget clickTarget = currentCoins[currentCoinIndex].GetComponent<CoinClickTarget>();
            if (clickTarget != null)
                clickTarget.SetActive(false);

            currentCoinIndex++;
            ProceedToNextCoin();
        }
    }

    private void EndRound()
    {
        currentState = GameState.EndRound;
        Debug.Log($"Round {currentRound} Complete.");
        currentRound++;

        StartCoroutine(NextRoundDelay(2f));
    }

    private System.Collections.IEnumerator NextRoundDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartRound();
    }

    public void UsePlayerAttempt()
    {
        if (playerAttemptsRemaining > 0)
            playerAttemptsRemaining--;

        UIManager.Instance?.ShowAttempts(playerAttemptsRemaining);
    }

    public void ResetPlayerAttempts(int value = 2)
    {
        playerAttemptsRemaining = value;
        if (UIManager.Instance != null)
            UIManager.Instance.ShowAttempts(playerAttemptsRemaining);
        else
            Debug.LogWarning("UIManager.Instance was null when resetting attempts.");
    }

    private System.Collections.IEnumerator DelayedStartRound()
    {
        while (UIManager.Instance == null)
        {
            Debug.Log("Waiting for UIManager to initialize...");
            yield return null;
        }

        StartRound();
    }

    public void EndGameDueToPlayerFailure()
    {
        Debug.Log("Game Over: Player failed with no attempts left.");
        StopAllCoroutines();
        currentState = GameState.Idle;
        uiManager.ShowGameOver(RestartGame);
    }

    private void RestartGame()
    {
        Debug.Log("Restarting game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator AnnounceAndWait(bool call)
    {
        yield return new WaitForSeconds(5f); // pause on announcer
        announcer.Announce(call);
        yield return new WaitForSeconds(3.5f); // let the flip play out later
    }

}
