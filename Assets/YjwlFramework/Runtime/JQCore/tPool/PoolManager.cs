/// <Licensing>
/// ?2011 (Copyright) Path-o-logical Games, LLC
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

//using System.Diagnostics;

namespace JQCore.tPool
{
    /// <description>
    ///     PoolManager v2.0
    ///     - PoolManager.Pools is not a complete implimentation of the IDictionary interface
    ///     Which enabled:
    ///     * Much more acurate and clear errorCode handling
    ///     * Member access protection so you can't run anything you aren't supposed to.
    ///     - Moved all functions for working with Pools from PoolManager to PoolManager.Pools
    ///     which enabled shorter names to reduces the length of lines of code.
    ///     Online Docs: http://docs.poolmanager2.path-o-logical.com
    /// </description>
    public static class PoolManager
    {
        public static readonly SpawnPoolsDict Pools = new();
    }


    public static class PoolManagerUtils
    {
        internal static void SetActive(GameObject obj, bool state)
        {
#if (UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0)
            obj.SetActiveRecursively(state);
#else
            if (state) obj.SetActive(state);
#endif
        }

        internal static bool activeInHierarchy(GameObject obj)
        {
#if (UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0)
            return obj.active;
#else
            return obj.activeInHierarchy;
#endif
        }
    }


    public class SpawnPoolsDict : IDictionary<string, SpawnPool>
    {
        #region Event Handling

        public delegate void OnCreatedDelegate(SpawnPool pool);

        internal Dictionary<string, OnCreatedDelegate> onCreatedDelegates = new();

        public void AddOnCreatedDelegate(string poolName, OnCreatedDelegate createdDelegate)
        {
            // Assign first delegate "just in time"
            if (!onCreatedDelegates.ContainsKey(poolName))
            {
                onCreatedDelegates.Add(poolName, createdDelegate);
                return;
            }

            onCreatedDelegates[poolName] += createdDelegate;
        }

        public void RemoveOnCreatedDelegate(string poolName, OnCreatedDelegate createdDelegate)
        {
            if (!onCreatedDelegates.ContainsKey(poolName))
                throw new KeyNotFoundException
                (
                    "No OnCreatedDelegates found for pool name '" + poolName + "'."
                );

            onCreatedDelegates[poolName] -= createdDelegate;
        }

        #endregion Event Handling

        #region Public Custom Memebers

        /// <summary>
        ///     Creates a new GameObject with a SpawnPool Component which registers itself
        ///     with the PoolManager.Pools dictionary. The SpawnPool can then be accessed
        ///     directly via the return value of this function or by via the PoolManager.Pools
        ///     dictionary using a 'key' (string : the name of the pool, SpawnPool.poolName).
        /// </summary>
        /// <param name="poolName">
        ///     The name for the new SpawnPool. The GameObject will have the word "Pool"
        ///     Added at the end.
        /// </param>
        /// <returns>A reference to the new SpawnPool component</returns>
        public SpawnPool Create(string poolName)
        {
            // Add "Pool" to the end of the poolName to make a more user-friendly
            //   GameObject name. This gets stripped back out in SpawnPool Awake()
            var owner = new GameObject(poolName + "Pool");
            owner.transform.position = new Vector3(-10000, 0, -10000);
            return owner.AddComponent<SpawnPool>();
        }


        /// <summary>
        ///     Creates a SpawnPool Component on an 'owner' GameObject which registers
        ///     itself with the PoolManager.Pools dictionary. The SpawnPool can then be
        ///     accessed directly via the return value of this function or via the
        ///     PoolManager.Pools dictionary.
        /// </summary>
        /// <param name="poolName">
        ///     The name for the new SpawnPool. The GameObject will have the word "Pool"
        ///     Added at the end.
        /// </param>
        /// <param name="owner">A GameObject to add the SpawnPool Component</param>
        /// <returns>A reference to the new SpawnPool component</returns>
        public SpawnPool Create(string poolName, GameObject owner)
        {
            if (!assertValidPoolName(poolName))
                return null;

            // When the SpawnPool is created below, there is no way to set the poolName
            //   before awake runs. The SpawnPool will use the rootGameObject name by default
            //   so a try statement is used to temporarily change the parent's name in a
            //   safe way. The finally block will always run, even if there is an errorCode.
            var ownerName = owner.gameObject.name;

            try
            {
                owner.gameObject.name = poolName;

                // Note: This will use SpawnPool.Awake() to Finish initUI and self-add the pool
                return owner.AddComponent<SpawnPool>();
            }
            finally
            {
                // Runs no matter what
                owner.gameObject.name = ownerName;
            }
        }


        /// <summary>
        ///     Used to ensure a name is valid before creating anything.
        /// </summary>
        /// <param name="poolName">The name to test</param>
        /// <returns>True if sucessful, false if failed.</returns>
        private bool assertValidPoolName(string poolName)
        {
            // Cannot request a name with the word "Pool" in it. This would be a 
            //   rundundant naming convention and is a reserved word for GameObject
            //   defaul naming
            string tmpPoolName;
            tmpPoolName = poolName.Replace("Pool", "");
            if (tmpPoolName != poolName) // Warn if "Pool" was used in poolName
            {
                // Log a warning and continue on with the fixed name
                var msg = string.Format("'{0}' has the word 'Pool' in it. " +
                                        "This word is reserved for GameObject defaul naming. " +
                                        "The pool name has been changed to '{1}'",
                    poolName, tmpPoolName);

                Debug.LogWarning(msg);
                poolName = tmpPoolName;
            }

            if (ContainsKey(poolName))
            {
                Debug.Log(string.Format("A pool with the name '{0}' already exists",
                    poolName));
                return false;
            }

            return true;
        }


        /// <summary>
        ///     Returns a formatted string showing all the pool names
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Get a string[] array of the keys for formatting with join()
            var keysArray = new string[_pools.Count];
            _pools.Keys.CopyTo(keysArray, 0);

            // Return a comma-sperated list inside square brackets (Pythonesque)
            return string.Format("[{0}]", string.Join(", ", keysArray));
        }


        /// <summary>
        ///     Destroy an entire SpawnPool, including its GameObject and all children.
        ///     You can also just destroy the GameObject directly to achieve the same result.
        ///     This is really only here to make it easier when a reference isn't at hand.
        /// </summary>
        /// <param name="spawnPool"></param>
        public bool Destroy(string poolName)
        {
            // Use TryGetValue to avoid KeyNotFoundException.
            //   This is faster than Contains() and then accessing the dictionary
            SpawnPool spawnPool;
            if (!_pools.TryGetValue(poolName, out spawnPool))
            {
                Debug.LogError(
                    string.Format("PoolManager: Unable to destroy '{0}'. Not in PoolManager",
                        poolName));
                return false;
            }

            // The rest of the logic will be handled by OnDestroy() in SpawnPool
            Object.Destroy(spawnPool.gameObject);

            // Remove it from the dict in case the user re-creates a SpawnPool of the 
            //  same name later
            //this._pools.Remove(spawnPool.poolName);

            return true;
        }

        /// <summary>
        ///     Destroy ALL SpawnPools, including their GameObjects and all children.
        ///     You can also just destroy the GameObjects directly to achieve the same result.
        ///     This is really only here to make it easier when a reference isn't at hand.
        /// </summary>
        /// <param name="spawnPool"></param>
        public void DestroyAll()
        {
            foreach (var pair in _pools)
                Object.Destroy(pair.Value);

            // Clear the dict in case the user re-creates a SpawnPool of the same name later
            _pools.Clear();
        }

        #endregion Public Custom Memebers


        #region Dict Functionality

        // Internal (wrapped) dictionary
        private readonly Dictionary<string, SpawnPool> _pools = new();

        /// <summary>
        ///     Used internally by SpawnPools to add themseleves on Awake().
        ///     Use PoolManager.CreatePool() to create an entirely new SpawnPool GameObject
        /// </summary>
        /// <param name="spawnPool"></param>
        internal void Add(SpawnPool spawnPool)
        {
            // Don't let two pools with the same name be added. See errorCode below for details
            if (ContainsKey(spawnPool.poolName))
            {
                Debug.LogError(string.Format("A pool with the name '{0}' already exists. " +
                                             "This should only happen if a SpawnPool with " +
                                             "this name is added to a scene twice.",
                    spawnPool.poolName));
                return;
            }

            _pools.Add(spawnPool.poolName, spawnPool);

            if (onCreatedDelegates.ContainsKey(spawnPool.poolName))
                onCreatedDelegates[spawnPool.poolName](spawnPool);
        }

        // Keeping here so I remember we have a NotImplimented overload (original signature)
        public void Add(string key, SpawnPool value)
        {
            var msg = "SpawnPools add themselves to PoolManager.Pools when created, so " +
                      "there is no need to Add() them explicitly. Create pools using " +
                      "PoolManager.Pools.Create() or add a SpawnPool component to a " +
                      "GameObject.";
            throw new NotImplementedException(msg);
        }


        /// <summary>
        ///     Used internally by SpawnPools to remove themseleves on Destroy().
        ///     Use PoolManager.Destroy() to destroy an entire SpawnPool GameObject.
        /// </summary>
        /// <param name="spawnPool"></param>
        internal bool Remove(SpawnPool spawnPool)
        {
            if (!ContainsKey(spawnPool.poolName) && Application.isPlaying)
            {
                Debug.LogError(string.Format("PoolManager: Unable to remove '{0}'. " +
                                             "Pool not in PoolManager",
                    spawnPool.poolName));
                return false;
            }

            _pools.Remove(spawnPool.poolName);
            return true;
        }

        // Keeping here so I remember we have a NotImplimented overload (original signature)
        public bool Remove(string poolName)
        {
            var msg = "SpawnPools can only be destroyed, not removed and kept alive" +
                      " outside of PoolManager. There are only 2 legal ways to destroy " +
                      "a SpawnPool: Destroy the GameObject directly, if you have a " +
                      "reference, or use PoolManager.Destroy(string poolName).";
            throw new NotImplementedException(msg);
        }

        /// <summary>
        ///     Get the number of SpawnPools in PoolManager
        /// </summary>
        public int Count => _pools.Count;

        /// <summary>
        ///     Returns true if a pool exists with the passed pool name.
        /// </summary>
        /// <param name="poolName">The name to look for</param>
        /// <returns>True if the pool exists, otherwise, false.</returns>
        public bool ContainsKey(string poolName)
        {
            return _pools.ContainsKey(poolName);
        }

        /// <summary>
        ///     Used to get a SpawnPool when the user is not sure if the pool name is used.
        ///     This is faster than checking IsPool(poolName) and then accessing Pools][poolName.]
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string poolName, out SpawnPool spawnPool)
        {
            return _pools.TryGetValue(poolName, out spawnPool);
        }


        #region Not Implimented

        public bool Contains(KeyValuePair<string, SpawnPool> item)
        {
            var msg = "Use PoolManager.Pools.Contains(string poolName) instead.";
            throw new NotImplementedException(msg);
        }

        public SpawnPool this[string key]
        {
            get
            {
                SpawnPool pool;
                try
                {
                    pool = _pools[key];
                }
                catch (KeyNotFoundException)
                {
                    var msg = string.Format("A Pool with the name '{0}' not found. " +
                                            "\nPools={1}",
                        key, ToString());
                    throw new KeyNotFoundException(msg);
                }

                return pool;
            }
            set
            {
                var msg = "Cannot set PoolManager.Pools[key] directly. " +
                          "SpawnPools add themselves to PoolManager.Pools when created, so " +
                          "there is no need to set them explicitly. Create pools using " +
                          "PoolManager.Pools.Create() or add a SpawnPool component to a " +
                          "GameObject.";
                throw new NotImplementedException(msg);
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                var msg = "If you need this, please request it.";
                throw new NotImplementedException(msg);
            }
        }


        public ICollection<SpawnPool> Values
        {
            get
            {
                var msg = "If you need this, please request it.";
                throw new NotImplementedException(msg);
            }
        }


        #region ICollection<KeyValuePair<string,SpawnPool>> Members

        private bool IsReadOnly => true;
        bool ICollection<KeyValuePair<string, SpawnPool>>.IsReadOnly => true;

        public void Add(KeyValuePair<string, SpawnPool> item)
        {
            var msg = "SpawnPools add themselves to PoolManager.Pools when created, so " +
                      "there is no need to Add() them explicitly. Create pools using " +
                      "PoolManager.Pools.Create() or add a SpawnPool component to a " +
                      "GameObject.";
            throw new NotImplementedException(msg);
        }

        public void Clear()
        {
            var msg = "Use PoolManager.Pools.DestroyAll() instead.";
            throw new NotImplementedException(msg);
        }

        private void CopyTo(KeyValuePair<string, SpawnPool>[] array, int arrayIndex)
        {
            var msg = "PoolManager.Pools cannot be copied";
            throw new NotImplementedException(msg);
        }

        void ICollection<KeyValuePair<string, SpawnPool>>.CopyTo(KeyValuePair<string, SpawnPool>[] array, int arrayIndex)
        {
            var msg = "PoolManager.Pools cannot be copied";
            throw new NotImplementedException(msg);
        }

        public bool Remove(KeyValuePair<string, SpawnPool> item)
        {
            var msg = "SpawnPools can only be destroyed, not removed and kept alive" +
                      " outside of PoolManager. There are only 2 legal ways to destroy " +
                      "a SpawnPool: Destroy the GameObject directly, if you have a " +
                      "reference, or use PoolManager.Destroy(string poolName).";
            throw new NotImplementedException(msg);
        }

        #endregion ICollection<KeyValuePair<string, SpawnPool>> Members

        #endregion Not Implimented


        #region IEnumerable<KeyValuePair<string,SpawnPool>> Members

        public IEnumerator<KeyValuePair<string, SpawnPool>> GetEnumerator()
        {
            return _pools.GetEnumerator();
        }

        #endregion


        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _pools.GetEnumerator();
        }

        #endregion

        #endregion Dict Functionality
    }
}