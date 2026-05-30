using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Enemy[] _usableEnemies = new Enemy[0];
    private List<Enemy> _activeEnemies = new List<Enemy>();
    private bool _isSpawning = false;

    [Header("Spawn")]
    [SerializeField] private Transform _spawnLineCenter;
    [SerializeField] private float _spawnLineWidth = 20f;

    private float _spawnInterval = 2f;
    private float _timer;
    private int _maxEnemies = 999;

    public float SpawnInterval
    {
        get => _spawnInterval;
        set => _spawnInterval = value;
    }

    public void SetMaxEnemies(int max) => _maxEnemies = max;
    public void SetEnemies(Enemy[] enemies) => _usableEnemies = enemies;

    public void SetSpawning(bool active)
    {
        _isSpawning = active;
        _timer = 0f;
    }

    private void Update()
    {
        if (!_isSpawning || _usableEnemies.Length == 0) return;

        _timer += Time.deltaTime;
        if (_timer >= _spawnInterval)
        {
            _timer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        _activeEnemies.RemoveAll(e => e == null || !e.IsAlive);
        if (_activeEnemies.Count >= _maxEnemies) return;

        Vector3 spawnPos = _spawnLineCenter.position + Vector3.right * Random.Range(-_spawnLineWidth / 2f, _spawnLineWidth / 2f);
        Enemy prefab = _usableEnemies[Random.Range(0, _usableEnemies.Length)];
        Enemy instance = Instantiate(prefab, spawnPos, Quaternion.identity);
        _activeEnemies.Add(instance);
    }

    public void KillAllEnemies()
    {
        foreach (Enemy enemy in _activeEnemies)
        {
            if (enemy != null) enemy.Kill();
        }
        _activeEnemies.Clear();
    }

    public void SpawnBoss(Enemy bossPrefab)
    {
        if (bossPrefab == null) return;
        Enemy instance = Instantiate(bossPrefab, _spawnLineCenter.position, Quaternion.identity);
        _activeEnemies.Add(instance);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        _activeEnemies.Remove(enemy);
    }
}
