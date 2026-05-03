using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PrototypeTD
{
    public class Scene3DCharacterTestBootstrap : MonoBehaviour
    {
        [SerializeField] private GameObject quaterniusCharacterPrefab;
        [SerializeField] private Camera mainCamera;

        private static readonly string[] GameplayObjectNameKeywords =
        {
            "PlayerBase", "EnemyBase", "PlayerCommander", "EnemyCommander", "GameCanvas", "EnemyUnit", "EnemyBullet",
            "Turret", "Cost", "Shoot", "Wave", "Spawner", "GameManager", "Commander", "UIManager", "DontDestroyOnLoad"
        };

        private CameraAngleSwitcher _cameraSwitcher;
        private Character3DTestController _character;

        private void Awake()
        {
            if (SceneManager.GetActiveScene().name != "Scene_3DCharacterTest") return;

            EnsureEventSystem();
            RemoveGameplayObjects();
            EnsureMainCamera();
            EnsureDirectionalLight();
            EnsureFloor();
            SpawnCharacter();
            EnsureUi();
        }

        // ── scene setup ──────────────────────────────────────────

        private static void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null) return;
            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            DontDestroyOnLoad(go);
        }

        private void RemoveGameplayObjects()
        {
            var scene = SceneManager.GetActiveScene();
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root == gameObject ||
                    root.name == "Main Camera" ||
                    root.name == "Directional Light" ||
                    root.name == "TestFloor") continue;

                if (root.GetComponent<Character3DTestController>() != null ||
                    root.GetComponentInChildren<Character3DTestController>(true) != null) continue;

                if (ShouldRemoveAsGameplay(root.name) || root.GetComponentInChildren<Canvas>(true) != null)
                    Destroy(root);
            }

            foreach (var obj in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (obj == null || obj.scene.IsValid()) continue;
                if (obj.transform.parent != null) continue;
                if (ShouldRemoveAsGameplay(obj.name)) Destroy(obj);
            }
        }

        private static bool ShouldRemoveAsGameplay(string objectName)
        {
            foreach (var kw in GameplayObjectNameKeywords)
                if (objectName.Contains(kw)) return true;
            return false;
        }

        private void EnsureMainCamera()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (mainCamera == null)
            {
                var go = new GameObject("Main Camera") { tag = "MainCamera" };
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
                    quaterniusCharacterPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
                        "Assets/ThirdParty/Quaternius/UltimateModularMen/Humanoid Rig/Individual Characters/FBX/Adventurer.fbx");
                }
#endif
                if (quaterniusCharacterPrefab == null)
                {
                    Debug.LogWarning("Scene3DCharacterTestBootstrap: Quaternius Character Prefab is not assigned. Character spawn is skipped.");
                    return;
                }

                var go = Instantiate(quaterniusCharacterPrefab, Vector3.zero, Quaternion.identity);
                go.name = "UMM_Adventurer";
                go.transform.localScale = Vector3.one * 1.1f;
                _character = go.GetComponent<Character3DTestController>();
                if (_character == null) _character = go.AddComponent<Character3DTestController>();
            }

            if (_character != null)
                _cameraSwitcher.SetLookTarget(_character.transform);
        }

        // ── UI ───────────────────────────────────────────────────

        private void EnsureUi()
        {
            if (GameObject.Find("CharacterTestCanvas") != null) return;

            var canvasGo = new GameObject("CharacterTestCanvas",
                typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // ── Left: Camera ──────────────────────
            CreateLabel(canvasGo.transform, "[ Camera ]",
                new Vector2(0, 1), new Vector2(10, -12));
            CreateButton(canvasGo.transform, "Cam Front",
                new Vector2(0, 1), new Vector2(10, -40),
                () => _cameraSwitcher?.SetAngleIndex(2));
            CreateButton(canvasGo.transform, "Cam Side",
                new Vector2(0, 1), new Vector2(10, -84),
                () => _cameraSwitcher?.SetAngleIndex(0));
            CreateButton(canvasGo.transform, "Cam Top",
                new Vector2(0, 1), new Vector2(10, -128),
                () => _cameraSwitcher?.SetAngleIndex(1));

            // ── Left: Team ────────────────────────
            CreateLabel(canvasGo.transform, "[ Team ]",
                new Vector2(0, 1), new Vector2(10, -186));
            CreateButton(canvasGo.transform, "Team Red",
                new Vector2(0, 1), new Vector2(10, -214),
                () => _character?.SetTeam(Character3DTestController.TeamColor.Red),
                new Color(0.75f, 0.15f, 0.15f, 0.9f));
            CreateButton(canvasGo.transform, "Team Blue",
                new Vector2(0, 1), new Vector2(10, -258),
                () => _character?.SetTeam(Character3DTestController.TeamColor.Blue),
                new Color(0.15f, 0.3f, 0.75f, 0.9f));

            // ── Right: Motion ─────────────────────
            CreateLabel(canvasGo.transform, "[ Motion ]",
                new Vector2(1, 1), new Vector2(-138, -12));
            CreateButton(canvasGo.transform, "Idle",
                new Vector2(1, 1), new Vector2(-138, -40),
                () => _character?.SetMotion(Character3DTestController.MotionState.Idle));
            CreateButton(canvasGo.transform, "Walk",
                new Vector2(1, 1), new Vector2(-138, -84),
                () => _character?.SetMotion(Character3DTestController.MotionState.Walk));
            CreateButton(canvasGo.transform, "Attack",
                new Vector2(1, 1), new Vector2(-138, -128),
                () => _character?.SetMotion(Character3DTestController.MotionState.Attack));
        }

        private static void CreateLabel(Transform parent, string text, Vector2 anchor, Vector2 pos)
        {
            var go = new GameObject(text, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchor; rt.anchorMax = anchor; rt.pivot = anchor;
            rt.anchoredPosition = pos; rt.sizeDelta = new Vector2(128, 22);
            var t = go.GetComponent<Text>();
            t.text = text;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = new Color(1f, 1f, 0.6f, 1f);
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = 14;
            t.fontStyle = FontStyle.Bold;
        }

        private static void CreateButton(Transform parent, string label, Vector2 anchor, Vector2 pos,
            UnityEngine.Events.UnityAction action, Color? bgColor = null)
        {
            var bg = bgColor ?? new Color(0.12f, 0.12f, 0.12f, 0.88f);

            var go = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchor; rt.anchorMax = anchor; rt.pivot = anchor;
            rt.anchoredPosition = pos; rt.sizeDelta = new Vector2(128, 38);
            go.GetComponent<Image>().color = bg;
            go.GetComponent<Button>().onClick.AddListener(action);

            var textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(go.transform, false);
            var textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero; textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero; textRt.offsetMax = Vector2.zero;
            var t = textGo.GetComponent<Text>();
            t.text = label;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.white;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = 16;
            t.fontStyle = FontStyle.Bold;
        }
    }
}
