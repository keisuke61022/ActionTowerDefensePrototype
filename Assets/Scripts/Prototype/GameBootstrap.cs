using UnityEngine;
using UnityEngine.EventSystems;

namespace PrototypeTD
{
    public static class GameBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (Object.FindObjectOfType<GameManager>() != null)
            {
                return;
            }

            EnsureEventSystem();
            var root = new GameObject("PrototypeGameRoot");
            root.AddComponent<GameManager>();
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Object.DontDestroyOnLoad(eventSystemObject);
        }
    }
}
