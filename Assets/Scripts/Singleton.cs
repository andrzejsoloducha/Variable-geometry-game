using UnityEngine;
public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (!_instance)
                    {
                        SetupInstance();
                    }
                }

                return _instance;
            }
        }

        public virtual void Awake()
        {
            RemoveDuplicates();
        }

        private void RemoveDuplicates()
        {
            if (!_instance)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private static void SetupInstance()
        {
            _instance = (T)FindObjectOfType(typeof(T));

            if (!_instance)
            {
                var gameObj = new GameObject
                {
                    name = typeof(T).Name
                };
                _instance = gameObj.AddComponent<T>();
                DontDestroyOnLoad(gameObj);
            }
        }
        
    }