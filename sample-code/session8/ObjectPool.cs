using UnityEngine;
using UnityEngine.Pool;
using System;

namespace Metroidvania.Pooling
{
    public class SimpleObjectPool<T> where T : Component
    {
        private readonly T prefab;
        private readonly ObjectPool<T> pool;

        public SimpleObjectPool(T prefab, int defaultCapacity = 10, int maxSize = 50)
        {
            this.prefab = prefab;
            this.pool = new ObjectPool<T>(
                createFunc: CreateInstance,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDestroy,
                collectionCheck: true,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }

        private T CreateInstance()
        {
            return UnityEngine.Object.Instantiate(prefab);
        }

        private void OnGet(T instance)
        {
            instance.gameObject.SetActive(true);
        }

        private void OnRelease(T instance)
        {
            instance.gameObject.SetActive(false);
        }

        private void OnDestroy(T instance)
        {
            UnityEngine.Object.Destroy(instance.gameObject);
        }

        public T Get() => pool.Get();
        public void Release(T instance) => pool.Release(instance);
    }
}
