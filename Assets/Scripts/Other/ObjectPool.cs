using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GD3D.ObjectPooling
{
    /// <summary>
    /// Class that contains data for a pool of objects
    /// </summary>
    public class ObjectPool<T> where T : PoolObject
    {
        public T OriginalObject;
        public int Size = 100;

        private Queue<T> _queue;
        public Queue<T> Queue => _queue;

        #region SpawnFromPool Methods
        /// <summary>
        /// "Spawns" (actually reuses) an object from the pool and returns it
        /// </summary>
        public T SpawnFromPool()
        {
            // Return null if the pool is empty
            if (IsEmpty())
            {
                return null;
            }

            T poolObj = _queue.Dequeue();

            poolObj.gameObject.SetActive(true);

            // Call the OnSpawn() method on the pool object
            poolObj.OnSpawn();

            // Return the pool object so other scripts can make use of it
            return poolObj;
        }
        /// <summary>
        /// "Spawns" (actually reuses) an object from the pool and returns it
        /// </summary>
        public T SpawnFromPool(Vector3 position)
        {
            // Return null if the pool is empty
            if (IsEmpty())
            {
                return null;
            }

            T poolObj = _queue.Dequeue();

            // Set transform
            Transform objTransform = poolObj.transform;
            objTransform.position = position;

            poolObj.gameObject.SetActive(true);

            // Call the OnSpawn() method on the pool object
            poolObj.OnSpawn();

            // Return the pool object so other scripts can make use of it
            return poolObj;
        }
        /// <summary>
        /// "Spawns" (actually reuses) an object from the pool and returns it
        /// </summary>
        public T SpawnFromPool(Vector3 position, Quaternion rotation)
        {
            // Return null if the pool is empty
            if (IsEmpty())
            {
                return null;
            }

            T poolObj = _queue.Dequeue();

            // Set transform
            Transform objTransform = poolObj.transform;
            objTransform.position = position;
            objTransform.rotation = rotation;

            poolObj.gameObject.SetActive(true);

            // Call the OnSpawn() method on the pool object
            poolObj.OnSpawn();

            // Return the pool object so other scripts can make use of it
            return poolObj;
        }
        /// <summary>
        /// "Spawns" (actually reuses) an object from the pool and returns it
        /// </summary>
        public T SpawnFromPool(Vector3 position, Quaternion rotation, Transform parent)
        {
            // Return null if the pool is empty
            if (IsEmpty())
            {
                return null;
            }

            T poolObj = _queue.Dequeue();

            // Set transform
            Transform objTransform = poolObj.transform;
            objTransform.position = position;
            objTransform.rotation = rotation;
            objTransform.SetParent(parent);

            poolObj.gameObject.SetActive(true);

            // Call the OnSpawn() method on the pool object
            poolObj.OnSpawn();

            // Return the pool object so other scripts can make use of it
            return poolObj;
        }
        #endregion

        #region RemoveFromPool
        /// <summary>
        /// Removes the given pool <paramref name="obj"/>, disables it and adds it back to the queue
        /// </summary>
        public void RemoveFromPool(T obj)
        {
            // Call the OnRemove() method on the pool object
            obj.OnRemove();

            // Disable the object
            obj.gameObject.SetActive(false);

            // Add object back to queue
            _queue.Enqueue(obj);
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Expands or shrinks the pool to have the same size as the given <paramref name="newSize"/>. <para/>
        /// Warning! Not very performant during runtime.
        /// </summary>
        public void SetSize(int newSize)
        {
            // Return if the queue is already the new size
            if (_queue.Count == newSize)
            {
                return;
            }

            int difference = _queue.Count - newSize;

            // Shrink bool (old size is bigger than new size)
            bool shrink = _queue.Count > newSize;

            // Loop for amount of difference
            int absDifference = Mathf.Abs(difference);
            for (int i = 0; i < absDifference; i++)
            {
                // Shrink and destroy old objects
                if (shrink)
                {
                    // Remove and get object from queue
                    T poolObject = _queue.Dequeue();

                    Object.Destroy(poolObject.gameObject);
                }
                // Expand and create new objects
                else
                {
                    // Get object from queue
                    T poolObject = _queue.Peek();
                    Transform objTransform = poolObject.transform;

                    // Copy object
                    GameObject newObj = Object.Instantiate(poolObject.gameObject, objTransform.position, objTransform.rotation, objTransform.parent);
                    T newPoolObject = newObj.GetComponent<T>();
                    _queue.Enqueue(newPoolObject);
                }
            }
        }

        /// <summary>
        /// True if all the objects have been used in the pool, leaving it empty :(
        /// </summary>
        public bool IsEmpty()
        {
            return _queue.Count <= 0;
        }
        #endregion

        #region Create New Pool Methods
        /// <summary>
        /// Creates a new pool and automatically adds it to the object pooler. <para/>
        /// Set <paramref name="callOnObjSpawned"/> to create custom code when a new object is created for the pool
        /// </summary>
        public ObjectPool(T originalObject, int size = 100, Action<T> callOnObjSpawned = null)
        {
            OriginalObject = originalObject;
            Size = size;

            Create(callOnObjSpawned);
        }

        /// <summary>
        /// Creates a new pool and automatically adds it to the object pooler. <para/>
        /// Set <paramref name="callOnObjSpawned"/> to create custom code when a new object is created for the pool
        /// </summary>
        public ObjectPool(GameObject originalObject, int size = 100, Action<T> callOnObjSpawned = null)
        {
            OriginalObject = originalObject.GetComponent<T>();
            Size = size;

            Create(callOnObjSpawned);
        }

        /// <summary>
        /// Creates the pool
        /// </summary>
        private void Create(Action<T> callOnObjSpawned = null)
        {
            // Create and fill queue
            _queue = new Queue<T>();

            for (int i = 0; i < Size; i++)
            {
                // Create object
                GameObject newObj = Object.Instantiate(OriginalObject.gameObject);
                newObj.name = $"{OriginalObject.name} ({i})";

                // Disable the new object
                newObj.SetActive(false);

                // Get and setup pool object component
                T poolObject = newObj.GetComponent<T>();
                poolObject.SetPool<ObjectPool<T>, T>(this);

                // Add object to the queue
                _queue.Enqueue(poolObject);

                // Invoke OnCreated() method and the callOnObjSpawned event
                poolObject.OnCreated();

                callOnObjSpawned?.Invoke(poolObject);
            }
        }
        #endregion
    }
}
