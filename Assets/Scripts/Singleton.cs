using System;
using UnityEngine;

namespace Scripts
{
    public class Singleton<T> : MonoBehaviour where T : class
    {
        public static T Instance {get; private set;}

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}