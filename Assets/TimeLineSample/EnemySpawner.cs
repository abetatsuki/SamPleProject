using UnityEngine;

/// <summary>
/// Timeline から呼ばれて敵をスポーンするクラス
/// </summary>
public sealed class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;

    /// <summary>
    /// 敵を1体スポーンする
    /// Signal から呼ばれる想定
    /// </summary>
    public void SpawnEnemy()
    {
        Debug.Log("Spawn Enemy");
        Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// 複数体スポーンする
    /// </summary>
    public void SpawnEnemyWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
        }
    }
}
