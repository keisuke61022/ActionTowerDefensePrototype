using UnityEngine;

namespace PrototypeTD
{
    public class UnitController : MonoBehaviour
    {
        private float _fireInterval = 0.9f;
        private float _nextShotTime;
        private bool _playerSide;

        public static void CreateTurret(Vector3 position, bool playerSide)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = playerSide ? "PlayerTurret" : "EnemyTurret";
            go.transform.position = position;
            go.transform.localScale = new Vector3(0.36f, 0.24f, 0.36f);
            go.GetComponent<MeshRenderer>().material.color = playerSide ? new Color(0.2f, 0.7f, 1f) : new Color(0.95f, 0.45f, 0.45f);
            Destroy(go.GetComponent<Collider>());

            var unit = go.AddComponent<UnitController>();
            unit._playerSide = playerSide;
        }

        private void Update()
        {
            if (GameManager.Instance.IsGameOver || Time.time < _nextShotTime) return;

            Vector3 direction = _playerSide ? Vector3.up : Vector3.down;
            ProjectileController.Create(transform.position + direction * 0.45f, direction, 6f, 1, _playerSide);
            _nextShotTime = Time.time + _fireInterval;
        }
    }
}
