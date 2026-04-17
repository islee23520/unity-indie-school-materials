using UnityEngine;
using UnityEngine.Pool;

namespace Metroidvania.Pooling
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float lifetime = 3f;
        private IObjectPool<Bullet> pool;
        private float spawnTime;

        public void SetPool(IObjectPool<Bullet> pool)
        {
            this.pool = pool;
        }

        private void OnEnable()
        {
            spawnTime = Time.time;
        }

        private void Update()
        {
            if (Time.time - spawnTime >= lifetime)
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Damage logic here
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            pool?.Release(this);
        }
    }
}
