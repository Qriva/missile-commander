using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PrefabPooler : Singleton<ProjectilePooler>
    {
        // NOTE: Can be converted to property exposed in inspector
        public const int MIN_STACK_CAPACITY = 16;
        public const int INITIAL_HIERARCHY_CAPACITY = 64;
        /// <summary>
        /// Objects segregated by their prefab instance id.
        /// </summary>
        private Dictionary<int, Stack<IPoolable>> pool;

        protected override void InitSingleton()
        {
            pool = new Dictionary<int, Stack<IPoolable>>();
            // Set initial capacity to a bit higher value, so there is no need to resize it all the time
            transform.hierarchyCapacity = INITIAL_HIERARCHY_CAPACITY;
        }

        /// <summary>
        /// Returns copy of given poolable object
        /// </summary>
        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component, IPoolable
        {
            // NOTE: Debug.Log(prefab.GetPooledObject().GetInstanceID());
            // GetInstanceID returns unique value, so it can be used as id
            int prefabId = prefab.GetInstanceID();

            // First check if there is spare instance in the pool
            if (pool.TryGetValue(prefabId, out Stack<IPoolable> stack) && stack.Count > 0)
            {
                // Return instance from the stack
                IPoolable instance = stack.Pop();
                Transform instanceTransform = instance.AsComponent().transform;
                // Reparent and set position when not active to avoid dirting transform hierarchy twice
                instanceTransform.SetParent(parent);
                instanceTransform.SetPositionAndRotation(position, rotation);
                // Allow to restore instance values before use
                instance.Restore(prefab);
                // Enable instance gameobject
                instanceTransform.gameObject.SetActive(true);
                // Cas must be valid as it's ensured by signature of methods
                return (T)instance;
            }

            // It is required to create new instance
            return CreateInstance(prefab, position, rotation, parent);
        }

        /// <summary>
        /// Instantiates and ensures pooler manages this type of objects
        /// </summary>
        private T CreateInstance<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component, IPoolable
        {
            int prefabId = prefab.GetInstanceID();
            // Ensure the pool is ready to manage this kind of prefab/object
            if (!pool.ContainsKey(prefabId))
            {
                // Create new key with empty stack
                pool.Add(prefabId, new Stack<IPoolable>(MIN_STACK_CAPACITY));
            }

            // Create instance
            T instance = Instantiate<T>(prefab, position, rotation, parent);
            // Set pooler id to know later how to return object to pool
            instance.SetPoolerId(prefabId);
            return instance;
        }

        /// <summary>
        /// Returns instance to the pool
        /// </summary>
        /// <param name="instance">Returned instance</param>
        /// <param name="destroyOnFail">When instance cannot be returned it will be destroyed when set to true</param>
        /// <returns>True on success or false when this type of object is not managed by pooler</returns>
        public bool Release<T>(T instance, bool destroyOnFail = false) where T : Component, IPoolable
        {
            // Check if instance has been created by pooler
            if (instance.TryGetPoolerId(out int id))
            {
                // First disable instance gameobject
                instance.gameObject.SetActive(false);
                // Reparent as child of this pooler to be sure it's not destroyed in the future
                instance.transform.SetParent(this.transform);
                // Return instance to the pool
                pool[id].Push(instance);
                return true;
            }
            else if (destroyOnFail)
            {
                Destroy(instance.gameObject);
            }
            // This instance is not managed by pooler
            return false;
        }
    }

    /// <summary>
    /// Interface meant to be used by components that should be poolable by PrefabPooler
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Returns this object as Component
        /// </summary>
        public Component AsComponent();
        /// <summary>
        /// Restores object to initial state as if it have been instantiated just now
        /// </summary>
        /// <param name="original">Original object/prefab</param>
        public void Restore(IPoolable original);
        /// <summary>
        /// Sets pooler id - instance id of original prefab
        /// </summary>
        public void SetPoolerId(int id);
        /// <summary>
        /// Tries to get the id of designated pool
        /// </summary>
        /// <param name="id">Pool id, valid only if method returns true</param>
        /// <returns>True when id has been designated by PrefabPooler</returns>
        public bool TryGetPoolerId(out int id);
    }
}
