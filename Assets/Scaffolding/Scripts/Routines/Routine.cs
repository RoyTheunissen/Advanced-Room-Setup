using System.Collections;
using UnityEngine;

namespace RoyTheunissen.Scaffolding.Routines
{
    /// <summary>
    /// Helps start routines in a way that's separated from the hierarchy.
    /// </summary>
    public static class Routine 
    {
        public class RoutineHolder : MonoBehaviour { }

        private const string HolderName = "Routine Holder";

        private static RoutineHolder holder;
        public static RoutineHolder Holder
        {
            get
            {
                if (holder == null)
                {
                    GameObject gameObject = new GameObject(HolderName);
                    holder = gameObject.AddComponent<RoutineHolder>();
                }
                return holder;
            }
        }

        public static Coroutine Start(IEnumerator enumerator)
        {
            return Holder.StartCoroutine(enumerator);
        }

        public static void Start(ref Coroutine coroutine, IEnumerator enumerator)
        {
            Stop(ref coroutine);
            coroutine = Start(enumerator);
        }

        public static void Stop(Coroutine coroutine)
        {
            if (coroutine == null)
                return;
            Holder.StopCoroutine(coroutine);
        }

        public static void Stop(ref Coroutine coroutine)
        {
            Stop(coroutine);
            coroutine = null;
        }

        public static void Stop(IEnumerator enumerator)
        {
            if (enumerator == null)
                return;
            Holder.StopCoroutine(enumerator);
        }

        public static void Stop(ref IEnumerator enumerator)
        {
            Stop(enumerator);
            enumerator = null;
        }
    }
}
