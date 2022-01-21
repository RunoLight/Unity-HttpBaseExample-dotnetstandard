using System;
using System.Collections.Concurrent;

using UnityEngine;

namespace Light.Unity.Extensions
{
    public class ThreadHelper : MonoBehaviour
    {
        private static ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Update()
        {
            while (queue.TryDequeue(out var action))
            {
                action();
            }
        }

        public static void AddAction(Action action)
        {
            queue.Enqueue(action);
        }
    }
}
