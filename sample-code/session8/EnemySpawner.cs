using UnityEngine;
using System.Collections.Generic;
using Metroidvania.Combat;
using R3;

namespace Metroidvania.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyFactory enemyFactory;
        [SerializeField] private float spawnInterval = 3f;
        [SerializeField] private int maxEnemies = 10;
        [SerializeField] private EnemyType defaultEnemyType = EnemyType.Slime;

        private readonly List<GameObject> activeEnemies = new();
        private readonly CompositeDisposable disposables = new();

        private void Start()
        {
            Observable.Interval(TimeSpan.FromSeconds(spawnInterval))
                .Subscribe(_ => SpawnEnemy())
                .AddTo(disposables);
        }

        private void SpawnEnemy()
        {
            if (activeEnemies.Count >= maxEnemies) return;

            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 5f;
            GameObject enemy = enemyFactory.SpawnEnemy(defaultEnemyType, spawnPos);

            if (enemy != null)
            {
                activeEnemies.Add(enemy);

                Health health = enemy.GetComponent<Health>();
                if (health != null)
                {
                    health.OnDeath
                        .Take(1)
                        .Subscribe(_ => OnEnemyDeath(enemy))
                        .AddTo(disposables);
                }
            }
        }

        private void OnEnemyDeath(GameObject enemy)
        {
            activeEnemies.Remove(enemy);
            enemyFactory.DespawnEnemy(defaultEnemyType, enemy);
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}
