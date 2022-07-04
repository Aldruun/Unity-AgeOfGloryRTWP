using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AoG.Core
{
    public class CoroutineRunner : MonoBehaviour
    {
        public static CoroutineRunner Instance;

        private void Awake()
        {
            StopAllCoroutines();

            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }

            Instance = this;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}