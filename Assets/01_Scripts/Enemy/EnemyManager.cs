using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Prefabs ennemis")]
    [SerializeField] private Enemy[] _enemyPrefabs;
    private Enemy[] _usableEnemies;

    [Header("Spawn")]
    [SerializeField] private Transform _spawnLineCenter;
    [SerializeField] private float _spawnLineWidth = 20f;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private float _minSpawnInterval = 0.4f;

    public float SpawnInterval
    {
        get { return _minSpawnInterval; }
        set { _minSpawnInterval = value; }
    }
    public void SetEnemies(Enemy[] enemies) => _usableEnemies = enemies;
    
    private float _timer;
    private float _elapsed;

    private void Update()
    {
        _elapsed += Time.deltaTime;
        _timer += Time.deltaTime;

        float currentInterval = Mathf.Lerp(_spawnInterval, _minSpawnInterval, _elapsed);

        if (_timer >= currentInterval)
        {
            _timer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (_usableEnemies.Length == 0) return;

        Vector3 spawnPos = _spawnLineCenter.position + Vector3.right * Random.Range(-_spawnLineWidth / 2f, _spawnLineWidth / 2f);
        Enemy prefab = _usableEnemies[Random.Range(0, _usableEnemies.Length)];
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
