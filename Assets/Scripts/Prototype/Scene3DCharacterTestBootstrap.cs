using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrototypeTD
{
    public static class Scene3DCharacterTestBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            if (SceneManager.GetActiveScene().name != "Scene_3DCharacterTest") return;
            if (Object.FindObjectOfType<Character3DTestController>() != null) return;

            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.position = Vector3.zero;
            floor.transform.localScale = new Vector3(1.2f, 1f, 1.2f);

            var characterRoot = new GameObject("UMM_TestCharacter");
            characterRoot.transform.position = new Vector3(0f, 0f, 0f);
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "BodyPlaceholder";
            body.transform.SetParent(characterRoot.transform, false);
            body.transform.localPosition = new Vector3(0f, 1f, 0f);

            var weapon = GameObject.CreatePrimitive(PrimitiveType.Cube);
            weapon.name = "WeaponDummy";
            weapon.transform.SetParent(characterRoot.transform, false);
            weapon.transform.localPosition = new Vector3(0.3f, 1.2f, 0.45f);
            weapon.transform.localScale = new Vector3(0.1f, 0.1f, 0.6f);

            var ctrl = characterRoot.AddComponent<Character3DTestController>();

            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            camGo.AddComponent<AudioListener>();

            var switcher = camGo.AddComponent<CameraAngleSwitcher>();

            var spriteCamGo = new GameObject("SpriteRenderCamera");
            var spriteCam = spriteCamGo.AddComponent<Camera>();
            spriteCam.orthographic = false;
            spriteCam.fieldOfView = 20f;
            spriteCam.transform.position = new Vector3(4f, 2.5f, -4f);
            spriteCam.transform.LookAt(new Vector3(0f, 1.2f, 0f));
            spriteCam.enabled = false;

            // Configure via serialized fields fallback with reflection-free defaults by SendMessage-like pattern
            characterRoot.SendMessage("ApplyTeamColor", SendMessageOptions.DontRequireReceiver);

            // Minimal setup for inspector-time swapping: add renderer array/weapon refs manually in editor if needed.
        }
    }
}
