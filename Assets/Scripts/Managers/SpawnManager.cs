using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject gunPickupPrefab;
    public Transform[] spawnPoints;
    public Transform[] gunSpawns;
    public GameObject[] guns;
    public List<GameObject> pickups = new List<GameObject>();
    public List<GameObject> spawnedEnemies = new List<GameObject>();
    public GameObject[] keys;

    private void Start()
    {
        StateActions.Reset += DespawnEnemies;
        StateActions.Reset += DespawnObjects;
        StateActions.Start += SpawnEnemies;
        StateActions.Start += SpawnGuns;

        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("EnemySpawn");
        spawnPoints = new Transform[spawnPointObjects.Length];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[i] = spawnPointObjects[i].transform;
        }
    }

    private void OnDestroy()
    {
        StateActions.Reset -= DespawnEnemies;
        StateActions.Reset -= DespawnObjects;
        StateActions.Start -= SpawnEnemies;
        StateActions.Start -= SpawnGuns;
    }

    public void ClearPickups()
    {
        pickups.Clear();
    }

    public void RebindGunSpawnEvent()
    {
        StateActions.Start += SpawnGuns;
    }
    public void SpawnGuns()
    {
        // Only spawn guns if none in scene.
        if (pickups.Count == 0)
        { 
            for (int i = 0; i < gunSpawns.Length; i++)
            {
                if (guns[i] != null)
                {
                    GameObject gun = Instantiate(guns[i], gunSpawns[i].position, Quaternion.identity);
                    pickups.Add(gun);
                }
            }
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
        Destroy(enemy);
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
        for (int i = pickups.Count - 1; i >= 0; i--)
        {
            if (pickups[i] == null)
            {
                // If a gun was picked up, don't try to respawn it.
                pickups.RemoveAt(i);
            }
        }
        if (pickups.Count == 0)
        {
            StateActions.Start -= SpawnGuns;
        }
    }

    public void RespawnKeys()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            GameObject key = Instantiate(keys[i], keys[i].transform.position, Quaternion.identity);
        }
    }

    public void SpawnEnemy(Transform spawn)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawn.position, Quaternion.identity);
        spawnedEnemies.Add(enemy);
    }
}
