using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject gunPickupPrefab;
    public Transform[] spawnPoints;
    public Transform pistolPickup;
    public List<GameObject> pickups = new List<GameObject>();
    public List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Start()
    {
        StateActions.Reset += DespawnEnemies;
        StateActions.Reset += DespawnObjects;
        StateActions.Start += SpawnEnemies;
        StateActions.Start += SpawnGun;

        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("EnemySpawn");
        spawnPoints = new Transform[spawnPointObjects.Length];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[i] = spawnPointObjects[i].transform;
        }
    }

    private void SpawnGun()
    {
        pickups.Clear();
        pistolPickup = GameObject.FindGameObjectWithTag("GunSpawn")?.transform;
        if (pistolPickup != null && gunPickupPrefab != null)
        {
            GameObject gun = Instantiate(gunPickupPrefab, pistolPickup.position, Quaternion.identity);
            pickups.Add(gun);
        }
    }
    public void SpawnEnemies()
    {
        spawnedEnemies.Clear();
        if (spawnPoints != null)
        {
            foreach (Transform spawn in spawnPoints)
            {
                SpawnEnemy(spawn);
            }
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
        }
    }

    public void DespawnEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
    }

    public void DespawnObjects()
    {
        foreach (GameObject pickup in pickups)
        {
            if (pickup != null)
            {
                Destroy(pickup);
            }
        }
        pickups.Clear();
    }

    private void OnDestroy()
    {
        StateActions.Reset -= DespawnEnemies;
        StateActions.Reset -= DespawnObjects;
        StateActions.Start -= SpawnEnemies;
        StateActions.Start -= SpawnGun;
    }
    public void SpawnEnemy(Transform spawn)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawn.position, Quaternion.identity);
        spawnedEnemies.Add(enemy);
    }
}
