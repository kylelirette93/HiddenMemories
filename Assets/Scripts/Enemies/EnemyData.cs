[System.Serializable]
public class EnemyData
{
    public float timeBetweenAttacks;
    public float navSpeed;
    public int attackDamage;

    public EnemyData()
    {
        timeBetweenAttacks = 1.0f;
        navSpeed = 2.5f;
        attackDamage = 10;
    }
}
