using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PrototypeTD
{
    public class Scene3DCharacterTestBootstrap : MonoBehaviour
    {
        [SerializeField] private GameObject quaterniusCharacterPrefab;
        [SerializeField] private Camera mainCamera;

        private static readonly string[] GameplayObjectNameKeywords =
        {
            "PlayerBase", "EnemyBase", "PlayerCommander", "EnemyCommander", "GameCanvas", "EnemyUnit", "EnemyBullet",
            "Turret", "Cost", "Shoot", "Wave", "Spawner", "GameManager", "Commander", "UIManager"
        };

        private CameraAngleSwitcher _cameraSwitcher;
        private Character3DTestController _character;

        private void Awake()
        {
            if (SceneManager.GetActiveScene().name != "Scene_3DCharacterTest") return;

            RemoveGameplayObjects();
            EnsureMainCamera();
            EnsureDirectionalLight();
            EnsureFloor();
            SpawnCharacter();
            EnsureUi();
        }

        private void RemoveGameplayObjects()
        {
            var scene = SceneManager.GetActiveScene();
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root == gameObject || root.name == "Main Camera" || root.name == "Directional Light" || root.name == "TestFloor")
                {
                    continue;
                }

                if (root.GetComponent<Character3DTestController>() != null || root.GetComponentInChildren<Character3DTestController>(true) != null)
                {
                    continue;
                }

                if (ShouldRemoveAsGameplay(root.name) || root.GetComponentInChildren<Canvas>(true) != null)
                {
                    Destroy(root);
                }
            }

            // Also remove gameplay singletons that may live in DontDestroyOnLoad.
            foreach (var obj in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (obj == null || obj.scene.IsValid()) continue;
                if (obj.transform.parent != null) continue;
                if (ShouldRemoveAsGameplay(obj.name))
                {
                    Destroy(obj);
                }
            }
        }

        private static bool ShouldRemoveAsGameplay(string objectName)
        {
            for (var i = 0; i < GameplayObjectNameKeywords.Length; i++)
            {
                if (objectName.Contains(GameplayObjectNameKeywords[i])) return true;
            }

            return false;
        }

        private void EnsureMainCamera()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (mainCamera == null)
            {
                var go = new GameObject("Main Camera");
                go.tag = "MainCamera";
                mainCamera = go.AddComponent<Camera>();
                go.AddComponent<AudioListener>();
            }
            _cameraSwitcher = mainCamera.GetComponent<CameraAngleSwitcher>();
            if (_cameraSwitcher == null) _cameraSwitcher = mainCamera.gameObject.AddComponent<CameraAngleSwitcher>();
        }

        private static void EnsureDirectionalLight()
        {
            if (GameObject.Find("Directional Light") != null) return;
            var go = new GameObject("Directional Light");
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void EnsureFloor()
        {
            if (GameObject.Find("TestFloor") != null) return;
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "TestFloor";
            floor.transform.localScale = new Vector3(1.4f, 1f, 1.4f);
            floor.transform.position = Vector3.zero;
        }

        private void SpawnCharacter()
        {
            var existing = FindObjectOfType<Character3DTestController>();
            if (existing != null)
            {
                _character = existing;
            }
            else
            {
#if UNITY_EDITOR
                if (quaterniusCharacterPrefab == null)
                {
                    quaterniusCharacterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ThirdParty/Quaternius/UltimateModularMen/Humanoid Rig/Individual Characters/FBX/Adventurer.fbx");
                }
#endif
                if (quaterniusCharacterPrefab != null)
                {
                    var go = Instantiate(quaterniusCharacterPrefab, Vector3.zero, Quaternion.identity);
                    go.name = "UMM_Adventurer";
                    go.transform.localScale = Vector3.one * 1.1f;
                    _character = go.GetComponent<Character3DTestController>();
                    if (_character == null) _character = go.AddComponent<Character3DTestController>();
                }
            }

            if (_character != null)
            {
                _cameraSwitcher.SetLookTarget(_character.transform);
            }
        }

        private void EnsureUi()
        {
            if (GameObject.Find("CharacterTestCanvas") != null) return;
            var canvasGo = new GameObject("CharacterTestCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CreateButton(canvasGo.transform, "Cam Side", new Vector2(90, 40), () => _cameraSwitcher.SetAngleIndex(0));
            CreateButton(canvasGo.transform, "Cam Top", new Vector2(90, 80), () => _cameraSwitcher.SetAngleIndex(1));
            CreateButton(canvasGo.transform, "Cam Front", new Vector2(90, 120), () => _cameraSwitcher.SetAngleIndex(2));
            CreateButton(canvasGo.transform, "Team Red/Blue", new Vector2(110, 170), () => _character?.SwitchTeam());
        }

        private static void CreateButton(Transform parent, string label, Vector2 pos, UnityEngine.Events.UnityAction action)
        {
            var go = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(0, 1); rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = pos; rt.sizeDelta = new Vector2(150, 32);
            go.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f, 0.8f);
            go.GetComponent<Button>().onClick.AddListener(action);

            var textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(go.transform, false);
            var text = textGo.GetComponent<Text>();
            text.text = label; text.alignment = TextAnchor.MiddleCenter; text.color = Color.white; text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero; textRt.anchorMax = Vector2.one; textRt.offsetMin = Vector2.zero; textRt.offsetMax = Vector2.zero;
        }
    }
}
