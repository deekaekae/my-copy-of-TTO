using UnityEngine;
using System.Collections.Generic;

public class CoinSpawner : MonoBehaviour
{
    [Header("Coin Prefabs")]
    [SerializeField] private GameObject playerCoinPrefab;
    [SerializeField] private GameObject aiCoinPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform aiSpawnPoint;

    private List<GameObject> activeCoins = new List<GameObject>();

    private int movedCoinCount = 0;

    public void ClearCoins(){
        foreach (var coin in activeCoins)
        {
            Destroy(coin);
        }
        activeCoins.Clear();
    }

   public void MoveCoinAside(GameObject coin){
        float spacing = .6f;
        float xOffset = movedCoinCount * spacing;

        Vector3 newPosition = new Vector3(
            playerSpawnPoint.position.x + xOffset,
            playerSpawnPoint.position.y,
            playerSpawnPoint.position.z + 2f
        );

        coin.transform.position = newPosition;

        movedCoinCount++;
    }




    public GameObject SpawnSingleCoin(){
        Vector3 position = playerSpawnPoint.position;
        GameObject coin = Instantiate(playerCoinPrefab, position, Quaternion.identity);
        activeCoins.Add(coin);
        return coin;
    }

    public GameObject SpawnPlayerCoin(){
        GameObject coin = Instantiate(playerCoinPrefab, playerSpawnPoint.position, Quaternion.identity);
        activeCoins.Add(coin);
        return coin;
    }

    public GameObject SpawnAICoin(){
        GameObject coin = Instantiate(aiCoinPrefab, aiSpawnPoint.position, Quaternion.identity);
        activeCoins.Add(coin);
        return coin;
    }

    public void ResetCoinLayout(){
        movedCoinCount = 0;
    }

}