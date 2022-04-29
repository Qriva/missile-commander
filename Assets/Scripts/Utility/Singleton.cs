using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }
        [SerializeField] protected bool persistOnSceneLoad = true;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                if (persistOnSceneLoad)
                {
                    DontDestroyOnLoad(this.gameObject);
                }
                Instance = this as T;
                InitSingleton();
            }
        }

        protected abstract void InitSingleton();
    }
}
