using UnityEngine;

namespace Orvnge
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = (T) FindObjectOfType(typeof(T), true);
                if (_instance != null) return _instance;

                _instance = new GameObject().AddComponent<T>();
                _instance.name = typeof(T).ToString();
                // Debug.Log($"Create instance of {typeof(T)})");
                return _instance;
            }
            protected set => _instance = value;
        }

        private void Awake()
        {
            if (null != _instance && _instance != this)
            {
                Debug.Log(gameObject.name + " Destroy repeat.");
                Destroy(gameObject);
                return;
            }

            OnAwakeInit();
        }
        
        public virtual void OnAwakeInit()
        {
            Debug.Log(gameObject.name + " OnAwakeInit");
        }

        public virtual void Initialize()
        {
        }
        
        public virtual void DeInit()
        {
        }

        private void OnDestroy()
        {
            _instance = null;
        }
    }
}