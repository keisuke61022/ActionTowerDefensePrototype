using System;
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
            _mainCamera.orthographicSize = 9f;
            _mainCamera.transform.position = new Vector3(0f, 0f, -10f);
            _mainCamera.backgroundColor = new Color(0.08f, 0.08f, 0.1f);
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
            PlayerBase = CreateBase("PlayerBase", new Vector2(0f, -6.2f), Color.cyan, 30, false);
            EnemyBase = CreateBase("EnemyBase", new Vector2(0f, 6.5f), Color.red, 30, true);
            Player = CreatePlayer(new Vector2(0f, -4.6f));
        }

        private void CreateBackground()
        {
            var bg = GameObject.CreatePrimitive(PrimitiveType.Quad);
            bg.name = "BattleField";
            Destroy(bg.GetComponent<Collider>());
            bg.transform.position = new Vector3(0f, 1f, 2f);
            bg.transform.localScale = new Vector3(9f, 15f, 1f);
            bg.GetComponent<MeshRenderer>().material.color = new Color(0.13f, 0.2f, 0.13f);
        }

        private BaseController CreateBase(string name, Vector2 position, Color color, int hp, bool enemy)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = new Vector3(2.3f, 0.9f, 1f);
            var collider = go.GetComponent<BoxCollider>();
            collider.isTrigger = true;
            go.GetComponent<MeshRenderer>().material.color = color;

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
            go.GetComponent<MeshRenderer>().material.color = Color.yellow;

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
            go.GetComponent<MeshRenderer>().material.color = new Color(0.8f, 0.2f, 0.9f);

            var col = go.AddComponent<CapsuleCollider>();
            col.isTrigger = true;
            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            var enemy = go.AddComponent<EnemyController>();
            enemy.Initialize(PlayerBase.transform);
            return enemy;
        }

        public void RegisterEnemyKill()
        {
            CostManager.Add(1);
        }

        public void TryPlaceTurret()
        {
            if (IsGameOver || !CostManager.TrySpend(3))
            {
                return;
            }

            var pos = Player.transform.position + Vector3.up * 1.2f;
            pos.x = Mathf.Clamp(pos.x, -3.8f, 3.8f);
            pos.y = Mathf.Clamp(pos.y, -4.8f, 4.2f);
            UnitController.CreateTurret(pos);
        }

        public void OnBaseDestroyed(BaseController baseController)
        {
            if (IsGameOver)
            {
                return;
            }

            IsGameOver = true;
            if (baseController == EnemyBase)
            {
                UIManager.SetMessage("Victory! Enemy base destroyed.");
            }
            else
            {
                UIManager.SetMessage("Defeat... Your base has fallen.");
            }
        }

        public void TryCompleteWavesWin()
        {
            if (IsGameOver || PlayerBase.CurrentHp <= 0)
            {
                return;
            }

            IsGameOver = true;
            UIManager.SetMessage("Victory! You survived all 5 waves.");
        }
    }
}
