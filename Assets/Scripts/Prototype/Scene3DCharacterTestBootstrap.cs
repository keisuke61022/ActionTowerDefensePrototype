using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrototypeTD
{
    public class Scene3DCharacterTestBootstrap : MonoBehaviour
    {
        [Header("Character (set Quaternius FBX prefab in Inspector)")]
        [SerializeField] private GameObject quaterniusCharacterPrefab;
        [SerializeField] private Character3DTestController characterPrefabOverride;

        [Header("Camera")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera spriteRenderCamera;

        private void Awake()
        {
            if (SceneManager.GetActiveScene().name != "Scene_3DCharacterTest") return;

            var existing = FindObjectOfType<Character3DTestController>();
            if (existing == null)
            {
                SpawnCharacter();
            }

            EnsureCameras();
        }

        private void SpawnCharacter()
        {
            if (characterPrefabOverride != null)
            {
                Instantiate(characterPrefabOverride, Vector3.zero, Quaternion.identity);
                return;
            }

            if (quaterniusCharacterPrefab != null)
            {
                var go = Instantiate(quaterniusCharacterPrefab, Vector3.zero, Quaternion.identity);
                if (go.GetComponent<Character3DTestController>() == null)
                {
                    go.AddComponent<Character3DTestController>();
                }
                return;
            }

            var fallback = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            fallback.name = "UMM_TestCharacter_Fallback";
            fallback.transform.position = new Vector3(0f, 1f, 0f);
            fallback.AddComponent<Character3DTestController>();
        }

        private void EnsureCameras()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (mainCamera != null && mainCamera.GetComponent<CameraAngleSwitcher>() == null)
            {
                mainCamera.gameObject.AddComponent<CameraAngleSwitcher>();
            }

            if (spriteRenderCamera == null)
            {
                var spriteCamGo = GameObject.Find("SpriteRenderCamera");
                if (spriteCamGo == null)
                {
                    spriteCamGo = new GameObject("SpriteRenderCamera");
                    spriteRenderCamera = spriteCamGo.AddComponent<Camera>();
                }
                else
                {
                    spriteRenderCamera = spriteCamGo.GetComponent<Camera>();
                    if (spriteRenderCamera == null) spriteRenderCamera = spriteCamGo.AddComponent<Camera>();
                }
            }

            spriteRenderCamera.fieldOfView = 20f;
            spriteRenderCamera.transform.position = new Vector3(4f, 2.5f, -4f);
            var target = FindObjectOfType<Character3DTestController>();
            if (target != null) spriteRenderCamera.transform.LookAt(target.transform.position + Vector3.up * 1.2f);
            spriteRenderCamera.enabled = false;
        }
    }
}
