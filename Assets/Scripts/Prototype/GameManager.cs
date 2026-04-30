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
        public EnemyCommanderController EnemyCommander { get; private set; }
        public CostManager CostManager { get; private set; }
        public CostManager EnemyCostManager { get; private set; }
        public UIManager UIManager { get; private set; }

        [SerializeField] private Rect _playableRect = new Rect(-4f, -2.1f, 8f, 9.1f);
        [SerializeField] private float _playablePadding = 0.05f;

        private float _nextEnemySpawnTime;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupSystems();
            BuildArena();
            UIManager.BuildUI();
            UIManager.SetMessage("CPU対戦プロトタイプ\nWASD/Arrows: Move  Space/Click: Shoot  E: Turret");
        }

        private void SetupSystems()
        {
            CostManager = gameObject.AddComponent<CostManager>();
            EnemyCostManager = gameObject.AddComponent<CostManager>();
            UIManager = gameObject.AddComponent<UIManager>();
            CostManager.Initialize(5, 10, 1f);
            EnemyCostManager.Initialize(5, 10, 1f);
        }

        private void Update()
        {
            if (IsGameOver) return;
            EnemyCpuSpawnTick();
        }

        private void BuildArena()
        {
            PlayerBase = CreateBase("PlayerBase", new Vector2(0f, -5.7f), new Color(0.2f, 0.5f, 1f), 100, false);
            EnemyBase = CreateBase("EnemyBase", new Vector2(0f, 6.1f), new Color(1f, 0.3f, 0.3f), 120, true);
            Player = CreatePlayer(new Vector2(0f, -4.1f));
            EnemyCommander = CreateEnemyCommander(new Vector2(0f, 4.6f));
        }

        private BaseController CreateBase(string name, Vector2 position, Color color, int hp, bool enemy)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = new Vector3(2.3f, 0.9f, 1f);
            go.GetComponent<BoxCollider>().isTrigger = true;
            go.GetComponent<MeshRenderer>().material.color = color;
            var b = go.AddComponent<BaseController>();
            b.Initialize(hp, enemy);
            return b;
        }

        private PlayerController CreatePlayer(Vector2 pos)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = pos;
            go.GetComponent<MeshRenderer>().material.color = new Color(0.2f, 0.95f, 1f);
            var col = go.AddComponent<SphereCollider>(); col.isTrigger = true;
            var rb = go.AddComponent<Rigidbody>(); rb.useGravity = false; rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            return go.AddComponent<PlayerController>();
        }

        private EnemyCommanderController CreateEnemyCommander(Vector2 pos)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "EnemyCommander";
            go.transform.position = pos;
            go.GetComponent<CapsuleCollider>().isTrigger = true;
            go.GetComponent<MeshRenderer>().material.color = new Color(1f, 0.45f, 0.45f);
            var rb = go.AddComponent<Rigidbody>(); rb.useGravity = false; rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            return go.AddComponent<EnemyCommanderController>();
        }

        public EnemyController SpawnUnit(Vector2 position, bool enemySide)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = enemySide ? "EnemyUnit" : "AllyUnit";
            go.transform.position = position;
            go.transform.localScale = new Vector3(0.8f, 0.9f, 0.8f);
            go.GetComponent<MeshRenderer>().material.color = enemySide ? new Color(0.9f, 0.25f, 0.45f) : new Color(0.25f, 0.7f, 1f);
            go.GetComponent<CapsuleCollider>().isTrigger = true;
            var rb = go.AddComponent<Rigidbody>(); rb.useGravity = false; rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            var unit = go.AddComponent<EnemyController>();
            unit.Initialize(enemySide ? PlayerBase.transform : EnemyBase.transform, enemySide);
            return unit;
        }

        // Backward-compatible API used by WaveManager.
        public EnemyController SpawnEnemy(Vector2 position)
        {
            if (IsGameOver) return null;
            return SpawnUnit(position, true);
        }

        private void EnemyCpuSpawnTick()
        {
            if (Time.time < _nextEnemySpawnTime || EnemyCostManager.Current < 3) return;
            int enemyUnitCount = 0;
            foreach (var u in FindObjectsOfType<EnemyController>()) if (u.IsEnemySide) enemyUnitCount++;
            if (enemyUnitCount >= 6) return;
            if (Random.value > 0.65f) return;

            Vector3 anchor = EnemyCommander != null ? EnemyCommander.transform.position : EnemyBase.transform.position;
            Vector2 spawn = new Vector2(Mathf.Clamp(anchor.x + Random.Range(-1.4f, 1.4f), -3.6f, 3.6f), Mathf.Clamp(anchor.y - 0.7f, 2.2f, 5.2f));
            if (EnemyCostManager.TrySpend(3)) SpawnUnit(spawn, true);
            _nextEnemySpawnTime = Time.time + Random.Range(2f, 4f);
        }

        public void RegisterEnemyKill() => CostManager.Add(1);

        public void TryPlaceTurret()
        {
            if (IsGameOver) return;
            if (!CostManager.TrySpend(3)) { UIManager.SetMessage("コスト不足"); return; }
            var pos = Player.transform.position + Vector3.up * 1.2f;
            var placeRect = GetPlayableRectForExtents(new Vector2(0.45f, 0.45f));
            pos.x = Mathf.Clamp(pos.x, placeRect.xMin, placeRect.xMax);
            pos.y = Mathf.Clamp(pos.y, placeRect.yMin, placeRect.yMax);
            UnitController.CreateTurret(pos);
            SpawnUnit(Player.transform.position + Vector3.up * 0.8f, false);
        }

        public Rect GetPlayableRectForExtents(Vector2 extents) => Rect.MinMaxRect(_playableRect.xMin + extents.x + _playablePadding, _playableRect.yMin + extents.y + _playablePadding, _playableRect.xMax - extents.x - _playablePadding, _playableRect.yMax - extents.y - _playablePadding);
        public Rect GetEnemyCommanderRect(Vector2 extents)
        {
            var baseRect = GetPlayableRectForExtents(extents);
            return Rect.MinMaxRect(baseRect.xMin, 0.3f, baseRect.xMax, baseRect.yMax);
        }

        public void OnBaseDestroyed(BaseController baseController)
        {
            if (IsGameOver) return;
            IsGameOver = true;
            UIManager.SetMessage(baseController == EnemyBase ? "Victory! Enemy base destroyed." : "Defeat... Your base has fallen.");
        }

        // Backward-compatible API used by WaveManager.
        public void TryCompleteWavesWin()
        {
            if (IsGameOver) return;

            foreach (var unit in FindObjectsOfType<EnemyController>())
            {
                if (unit != null && unit.IsEnemySide)
                {
                    return;
                }
            }

            IsGameOver = true;
            UIManager.SetMessage("Victory! All waves cleared.");
        }
    }
}
