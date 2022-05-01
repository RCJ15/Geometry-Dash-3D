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

        [Space]
        [SerializeField] protected bool playAnimOnSpawn;
        protected Animator anim;

        private Type _poolType;
        private object _pool;
        private MethodInfo _removeMethodInfo;

        /// <summary>
        /// Override this to determine what happens when the object has just been created from the pool (is only called once so it's basically a Start() method)
        /// </summary>
        public virtual void OnCreated()
        {
            // Get animator
            anim = GetComponent<Animator>();

            // If that fails, get animator from children
            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
        }

        /// <summary>
        /// Override this to determine what happens when the object gets spawned from the pool
        /// </summary>
        public virtual void OnSpawn()
        {
            OnSpawnEvent?.Invoke();

            // Play anim on spawn
            if (playAnimOnSpawn)
            {
                PlayAnim();
            }
        }

        /// <summary>
        /// Will play the Reset animation
        /// </summary>
        public void PlayAnim()
        {
            // Play reset anim
            if (anim != null)
            {
                anim.SetTrigger("Reset");
            }
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
        /// Coroutine for removing this object after time. Used in <see cref="RemoveAfterTime"/>
        /// </summary>
        private IEnumerator RemoveAfterTimeCoroutine(float time, Action onComplete)
        {
            yield return Helpers.GetWaitForSeconds(time);

            // Invoke onComplete event
            onComplete?.Invoke();

            Remove();
        }

        /// <summary>
        /// Remove this object from it's pool
        /// </summary>
        public void Remove()
        {
            _removeMethodInfo.Invoke(_pool, new object[] { this });
        }

        /// <summary>
        /// Sets the poolType and pool for this poolObject
        /// </summary>
        public void SetPool<PoolType, ObjType>(PoolType pool) where PoolType : ObjectPool<ObjType> where ObjType : PoolObject
        {
            _poolType = typeof(PoolType);
            _pool = pool;

            _removeMethodInfo = _poolType.GetMethod("RemoveFromPool");
        }
    }
}
