using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using Metroidvania.Enemy;

namespace Metroidvania.Enemy
{
    public class EnemyFactory : MonoBehaviour
    {
        [System.Serializable]
        public class EnemyPoolData
        {
            public EnemyType Type;
            public GameObject Prefab;
            public int PoolSize = 10;
        }

        [SerializeField] private List<EnemyPoolData> enemyPools;
        private readonly Dictionary<EnemyType, ObjectPool<GameObject>> pools = new();

        private void Awake()
        {
            foreach (var data in enemyPools)
            {
                pools[data.Type] = new ObjectPool<GameObject>(
                    createFunc: () => Instantiate(data.Prefab),
                    actionOnGet: obj => obj.SetActive(true),
                    actionOnRelease: obj => obj.SetActive(false),
                    actionOnDestroy: Destroy,
                    defaultCapacity: data.PoolSize
                );
            }
        }

        public GameObject SpawnEnemy(EnemyType type, Vector2 position)
        {
            if (!pools.TryGetValue(type, out var pool)) return null;

            GameObject enemy = pool.Get();
            enemy.transform.position = position;

            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            ai?.Initialize();

            return enemy;
        }

        public void DespawnEnemy(EnemyType type, GameObject enemy)
        {
            if (pools.TryGetValue(type, out var pool))
            {
                pool.Release(enemy);
            }
        }
    }
}
