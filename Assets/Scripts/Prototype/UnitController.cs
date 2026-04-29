using UnityEngine;

namespace PrototypeTD
{
    public class UnitController : MonoBehaviour
    {
        private float _range = 3.8f;
        private float _fireInterval = 0.9f;
        private float _nextShotTime;

        public static void CreateTurret(Vector3 position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = "TurretUnit1";
            go.transform.position = position;
            go.transform.localScale = new Vector3(0.5f, 0.35f, 0.5f);
            go.GetComponent<MeshRenderer>().material.color = new Color(0.2f, 0.7f, 1f);
            Destroy(go.GetComponent<Collider>());

            go.AddComponent<UnitController>();
        }

        private void Update()
        {
            if (GameManager.Instance.IsGameOver || Time.time < _nextShotTime) return;

            EnemyController nearest = FindNearestEnemy();
            if (nearest == null) return;

            Vector3 direction = (nearest.transform.position - transform.position).normalized;
            ProjectileController.Create(transform.position + direction * 0.6f, direction, 6f, 1, true);
            _nextShotTime = Time.time + _fireInterval;
        }

        private EnemyController FindNearestEnemy()
        {
            EnemyController[] enemies = FindObjectsOfType<EnemyController>();
            EnemyController best = null;
            float bestDistance = float.MaxValue;
            foreach (var enemy in enemies)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance <= _range && distance < bestDistance)
                {
                    bestDistance = distance;
                    best = enemy;
                }
            }

            return best;
        }
    }
}
