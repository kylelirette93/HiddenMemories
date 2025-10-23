using TMPro;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public int EnemiesKilled { get { return enemiesKilled; } }
    int enemiesKilled;

    public void EnemyKilled()
    {
        enemiesKilled++;
    }
    public void Reset()
    {
        enemiesKilled = 0;
    }
}
