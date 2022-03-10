using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GD3D.ObjectPooling
{
    public class PoolObject : MonoBehaviour
    {
        [SerializeField] protected UnityEvent OnSpawnEvent;

        private Type poolType;
        private object pool;
        private MethodInfo removeMethodInfo;

        /// <summary>
        /// Override this to determine what happens when the object has just been created from the pool (is only called once so it's basically a Start() method)
        /// </summary>
        public virtual void OnCreated()
        {

        }

        /// <summary>
        /// Override this to determine what happens when the object gets spawned from the pool
        /// </summary>
        public virtual void OnSpawn()
        {
            OnSpawnEvent?.Invoke();
        }

        /// <summary>
        /// Override this to determine what happens when the object gets removed from the pool
        /// </summary>
        public virtual void OnRemove()
        {

        }

        /// <summary>
        /// Removes this pool object after waiting for the given <paramref name="time"/>. <para/>
        /// Use <paramref name="onComplete"/> to determine what happens when this timer is finished.
        /// </summary>
        public void RemoveAfterTime(float time, Action onComplete = null)
        {
            StartCoroutine(RemoveAfterTimeCoroutine(time, onComplete));
        }

        /// <summary>
        /// Coroutine for removing this object after time. Used in <see cref="RemoveAfterTime(float, System.Action)"/>
        /// </summary>
        private IEnumerator RemoveAfterTimeCoroutine(float time, Action onComplete)
        {
            yield return new WaitForSeconds(time);

            // Invoke onComplete event
            onComplete?.Invoke();

            Remove();
        }

        /// <summary>
        /// Remove this object from it's pool
        /// </summary>
        public void Remove()
        {
            removeMethodInfo.Invoke(pool, new object[] { this });
        }

        /// <summary>
        /// Sets the poolType and pool for this poolObject
        /// </summary>
        public void SetPool<PoolType, ObjType>(PoolType pool) where PoolType : ObjectPool<ObjType> where ObjType : PoolObject
        {
            poolType = typeof(PoolType);
            this.pool = pool;

            removeMethodInfo = poolType.GetMethod("RemoveFromPool");
        }
    }
}
