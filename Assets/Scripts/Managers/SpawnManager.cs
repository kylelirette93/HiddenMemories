using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Prefabs")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject rangedEnemyPrefab;
    [SerializeField] GameObject gunPickupPrefab;

    [Header("Enemy Spawning")]
    GameObject[] enemySpawnPointObjects; // Enemy spawn points in scene.
    [SerializeField] Transform[] enemySpawnPoints; // Enemy spawn locations.
    [SerializeField] List<GameObject> spawnedEnemies = new List<GameObject>(); // Enemies currently spawned in scene.

    [Header("Gun Spawning")]
    [SerializeField] Transform[] gunSpawnPoints; // Gun spawn locations.
    [SerializeField] GameObject[] guns; // Guns to be spawned.
    [SerializeField] List<GameObject> pickedUpWeapons = new List<GameObject>(); // Weapons that have been picked up.

    [Header("Keys and Doors")]
    [SerializeField] GameObject[] doors; // Doors in the scene, to be closed when resetting.
    [SerializeField] GameObject[] keys; // Keys pickups in the scene, to be respawned if not picked up.
    List<GameObject> spawnedKeys = new List<GameObject>(); // Keys currently spawned in scene.

    
    EnemyData enemyData; // Enemy data that's applied to spawned enemies.
    GameObject player; // Player ref used with enemy spawning logic.
    DataPersistenceManager dataPersistenceManager => GameManager.Instance.dataPersistenceManager;

    private void Start()
    {
        // Subscribe to state actions.
        StateActions.Reset += DespawnEnemies;
        StateActions.Reset += DespawnObjects;
        StateActions.Start += SpawnEnemies;

        enemyData = new EnemyData();
        // Find each enemy spawn in scene and store positions.
        enemySpawnPointObjects = GameObject.FindGameObjectsWithTag("EnemySpawn");
        enemySpawnPoints = new Transform[enemySpawnPointObjects.Length];
        for (int i = 0; i < enemySpawnPoints.Length; i++)
        {
            enemySpawnPoints[i] = enemySpawnPointObjects[i].transform;
        }
    }

    private void OnDestroy()
    {
        // Cleanup subscriptions.
        StateActions.Reset -= DespawnEnemies;
        StateActions.Reset -= DespawnObjects;
        StateActions.Start -= SpawnEnemies;
    }

    public void ClearPickups()
    {
        // To be called when starting new game plus or new game.
        pickedUpWeapons.Clear();
        spawnedKeys.Clear();
    }
    public void SpawnGuns()
    {
        // Only spawn guns if none in scene.
        if (pickedUpWeapons.Count == 0)
        { 
            GameData data = dataPersistenceManager.GetGameData();
            for (int i = 0; i < gunSpawnPoints.Length; i++)
            {
                if (guns[i] != null)
                {
                    var interactable = guns[i].GetComponent<Interactable>();
                    if (interactable == null) continue;
                    if (data != null && data.inventoryData.Count > 0 && data.inventoryData[0].weaponIDs.Contains(interactable.itemData.name))
                    {
                        // Player already has this gun, don't spawn it.
                        continue;
                    }
                    GameObject gun = Instantiate(guns[i], gunSpawnPoints[i].position, Quaternion.identity);
                    pickedUpWeapons.Add(gun);
                }
            }
        }
    }
    public void SpawnEnemies()
    {
        spawnedEnemies.Clear();
        if (enemySpawnPoints != null)
        {
            // Spawn enemies at their spawn points.
            foreach (Transform spawn in enemySpawnPoints)
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
        Destroy(enemy, 3f);
    }

    public void DespawnEnemies()
    {
        // Despawn all enemies on reset.
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
        // Despawn picked up weapons on reset.
        for (int i = pickedUpWeapons.Count - 1; i >= 0; i--)
        {
            if (pickedUpWeapons[i] == null)
            {
                // If a gun was picked up, don't try to respawn it, this is merely across scenes.
                pickedUpWeapons.RemoveAt(i);
            }
        }
        if (pickedUpWeapons.Count == 0)
        {
            // If all guns are spawned, unsubscribe.
            StateActions.Start -= SpawnGuns;
        }
    }

    public void SpawnKeys()
    {
        if (keys.Length == spawnedKeys.Count)
        {
            Debug.LogError("Keys: " + keys.Length + "Spawned keys: " + spawnedKeys.Count);
            return;
        }
        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i] != null)
            {
                GameData data = GameManager.Instance.dataPersistenceManager.GetGameData();
                KeyDataSO keyData = keys[i].GetComponent<Interactable>().itemData as KeyDataSO;
                PlayerInventory inventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
                if (data != null && data.inventoryData.Count >= 0)
                {
                    if (data.doorsOpened[keyData.doorNumber] || inventory.Keys.Contains(keyData))
                    {
                        continue;
                    }
                }
                //Debug.Log("Spawning key: " + keys[i].name);
                GameObject key = Instantiate(keys[i], keys[i].transform.position, keys[i].transform.rotation);
                spawnedKeys.Add(key);
            }
            else
            {
                //Debug.Log("Key is null.");
            }
        }
    }

    public void RespawnKeys()
    {
        // Applying same filtering logic as spawn keys to avoid duplicates.
        if (keys.Length == spawnedKeys.Count)
        {
            Debug.LogError("Keys: " + keys.Length + "Spawned keys: " + spawnedKeys.Count);
            return;
        }
        for (int i = 0; i < keys.Length; i++)
        {
            GameObject key = Instantiate(keys[i], keys[i].transform.position, keys[i].transform.rotation);
            spawnedKeys.Add(key);
        }
    }

    public void RespawnEnemyAtPosition(Vector3 position, float delay)
    {
        StartCoroutine(RespawnAtPositionAfterDelay(position, delay));
    }
    IEnumerator RespawnAtPositionAfterDelay(Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);
        float minSpawnDistance = 15f;
        // Max wait time to avoid scenario where player camps spawn.
        float waitTime = 0f;
        float maxWaitTime = 30f;
        player = GameObject.FindGameObjectWithTag("Player");

        while (player != null && Vector3.Distance(player.transform.position, position) < minSpawnDistance && waitTime < maxWaitTime)
        {
            // Check every half a second.
            yield return new WaitForSeconds(0.5f);
            waitTime += 0.5f;
        }

        GameData data = dataPersistenceManager.GetGameData();
        if (data != null)
        {
            enemyData.navSpeed = data.enemyData.navSpeed;
            enemyData.timeBetweenAttacks = data.enemyData.timeBetweenAttacks;
            enemyData.attackDamage = data.enemyData.attackDamage;
        }
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        EnemyController ec = enemy.GetComponent<EnemyController>();
        ec.Initialize(enemyData);
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }

    public void CloseDoors()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            var door = doors[i].GetComponentInChildren<Door>();
            door.SetDoorState(false);
            if (door != null)
            {
                Interactable doorInteractable = door.GetComponent<Interactable>();
                doorInteractable.CanInteract = true;
            }
        }
    }

    public void SpawnEnemy(Transform spawn)
    {
        // Set enemy data from save data.
        GameData data = dataPersistenceManager.GetGameData();
        if (data != null)
        {
            enemyData.navSpeed = data.enemyData.navSpeed;
            enemyData.timeBetweenAttacks = data.enemyData.timeBetweenAttacks;
            enemyData.attackDamage = data.enemyData.attackDamage;
        }
        GameObject enemy;
        if (Random.value < 0.4f)
        {
            enemy = Instantiate(rangedEnemyPrefab, spawn.position, Quaternion.identity);
        }
        else
        {
            enemy = Instantiate(enemyPrefab, spawn.position, Quaternion.identity);
        }
        spawnedEnemies.Add(enemy);
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        //Debug.Log($"Initializing enemy with navSpeed: {enemyData.navSpeed}");
        enemyController.Initialize(enemyData);
    }
}
