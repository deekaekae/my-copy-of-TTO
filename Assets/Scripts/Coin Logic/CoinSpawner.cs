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

    public void ClearCoins()
    {
        foreach (var coin in activeCoins)
        {
            Destroy(coin);
        }
        activeCoins.Clear();
    }

    public void MoveCoinAside(GameObject coin)
    {
        int usedIndex = activeCoins.IndexOf(coin);
        float offset = (usedIndex - 0.5f) * .75f;
        coin.transform.position = new Vector3(coin.transform.position.x + offset, coin.transform.position.y, coin.transform.position.z + 2f);
    }

    public GameObject SpawnSingleCoin()
    {
        Vector3 position = playerSpawnPoint.position;
        GameObject coin = Instantiate(playerCoinPrefab, position, Quaternion.identity);
        activeCoins.Add(coin);
        return coin;
    }

    public GameObject SpawnPlayerCoin()
    {
        GameObject coin = Instantiate(playerCoinPrefab, playerSpawnPoint.position, Quaternion.identity);
        activeCoins.Add(coin);
        return coin;
    }

    public GameObject SpawnAICoin()
    {
        GameObject coin = Instantiate(aiCoinPrefab, aiSpawnPoint.position, Quaternion.identity);
        activeCoins.Add(coin);
        return coin;
    }
}