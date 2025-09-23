using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Start()
    {
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("EnemySpawn");
        spawnPoints = new Transform[spawnPointObjects.Length];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[i] = spawnPointObjects[i].transform;
        }
        SpawnEnemies();
    }
    public void SpawnEnemies()
    {
        if (spawnPoints != null)
        {
            foreach (Transform spawn in spawnPoints)
            {
                SpawnEnemy(spawn);
            }
        }
    }

    public void SpawnEnemy(Transform spawn)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawn.position, Quaternion.identity);
        spawnedEnemies.Add(enemy);
    }
}
