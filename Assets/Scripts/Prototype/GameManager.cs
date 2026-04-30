using UnityEngine;

namespace PrototypeTD
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool IsGameOver { get; private set; }
        public BaseController PlayerBase { get; private set; }
        public BaseController EnemyBase { get; private set; }
        public PlayerController Player { get; private set; }
        public CostManager CostManager { get; private set; }
        public WaveManager WaveManager { get; private set; }
        public UIManager UIManager { get; private set; }

        private Camera _mainCamera;

        public Rect PlayableRect => new Rect(-4f, -5.2f, 8f, 12.2f);

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupCamera();
            CleanupLegacySceneObjects();
            SetupSystems();
            BuildArena();
            UIManager.BuildUI();
            UIManager.SetMessage("Defend your base!\nWASD/Arrows: Move  Space/Click: Shoot  E: Turret");
        }

        private void SetupCamera()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                _mainCamera = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }

            _mainCamera.orthographic = true;
            _mainCamera.orthographicSize = 8f;
            _mainCamera.transform.position = new Vector3(0f, 0f, -10f);
            _mainCamera.backgroundColor = new Color(0.05f, 0.07f, 0.12f);
        }

        private void CleanupLegacySceneObjects()
        {
            foreach (var canvas in FindObjectsOfType<Canvas>())
            {
                if (canvas.name != "GameCanvas") Destroy(canvas.gameObject);
            }
        }

        private void SetupSystems()
        {
            CostManager = gameObject.AddComponent<CostManager>();
            WaveManager = gameObject.AddComponent<WaveManager>();
            UIManager = gameObject.AddComponent<UIManager>();

            CostManager.Initialize(5, 10);
            WaveManager.Initialize(this);
        }

        private void BuildArena()
        {
            CreateBackground();
            PlayerBase = CreateBase("PlayerBase", new Vector2(0f, -5.7f), new Color(0.12f, 0.85f, 0.9f), 50, false);
            EnemyBase = CreateBase("EnemyBase", new Vector2(0f, 6.1f), new Color(0.95f, 0.25f, 0.25f), 45, true);
            Player = CreatePlayer(new Vector2(0f, -4.1f));
        }

        private void CreateBackground()
        {
            var bg = GameObject.CreatePrimitive(PrimitiveType.Quad);
            bg.name = "BattleField";
            Destroy(bg.GetComponent<Collider>());
            bg.transform.position = new Vector3(0f, 0.6f, 2f);
            bg.transform.localScale = new Vector3(8.6f, 12.8f, 1f);
            PaintMesh(bg.GetComponent<MeshRenderer>(), new Color(0.12f, 0.2f, 0.16f));
        }

        private BaseController CreateBase(string name, Vector2 position, Color color, int hp, bool enemy)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = new Vector3(2.3f, 0.9f, 1f);
            go.GetComponent<BoxCollider>().isTrigger = true;
            PaintMesh(go.GetComponent<MeshRenderer>(), color);

            var baseController = go.AddComponent<BaseController>();
            baseController.Initialize(hp, enemy);
            return baseController;
        }

        private PlayerController CreatePlayer(Vector2 position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Player";
            go.transform.position = position;
            go.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            PaintMesh(go.GetComponent<MeshRenderer>(), new Color(1f, 0.88f, 0.2f));

            var col = go.AddComponent<SphereCollider>();
            col.isTrigger = true;
            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            return go.AddComponent<PlayerController>();
        }

        public EnemyController SpawnEnemy(Vector2 position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Enemy";
            go.transform.position = position;
            go.transform.localScale = new Vector3(0.8f, 0.9f, 0.8f);
            PaintMesh(go.GetComponent<MeshRenderer>(), new Color(0.92f, 0.35f, 1f));

            go.GetComponent<CapsuleCollider>().isTrigger = true;
            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            var enemy = go.AddComponent<EnemyController>();
            enemy.Initialize(PlayerBase.transform);
            return enemy;
        }

        private void PaintMesh(Renderer renderer, Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            var mat = new Material(shader);
            mat.color = color;
            renderer.material = mat;
        }

        public void RegisterEnemyKill() => CostManager.Add(1);
        public void TryPlaceTurret()
        {
            if (IsGameOver) return;
            if (!CostManager.TrySpend(3))
            {
                UIManager.SetMessage("コスト不足");
                return;
            }
            var pos = Player.transform.position + Vector3.up * 1.2f;
            pos.x = Mathf.Clamp(pos.x, -3.8f, 3.8f);
            pos.y = Mathf.Clamp(pos.y, -4.8f, 4.2f);
            UnitController.CreateTurret(pos);
        }

        public void OnBaseDestroyed(BaseController baseController)
        {
            if (IsGameOver) return;
            IsGameOver = true;
            UIManager.SetMessage(baseController == EnemyBase ? "Victory! Enemy base destroyed." : "Defeat... Your base has fallen.");
        }

        public void TryCompleteWavesWin()
        {
            if (IsGameOver || PlayerBase.CurrentHp <= 0) return;
            IsGameOver = true;
            UIManager.SetMessage("Victory! You survived all 5 waves.");
        }
    }
}
