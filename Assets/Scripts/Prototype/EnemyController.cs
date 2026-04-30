using UnityEngine;

namespace PrototypeTD
{
    public class EnemyController : MonoBehaviour
    {
        public bool IsEnemySide { get; private set; }

        private Transform _targetBase;
        private int _hp = 4;
        private float _speed = 1.7f;

        public void Initialize(Transform targetBase, bool isEnemySide)
        {
            _targetBase = targetBase;
            IsEnemySide = isEnemySide;
        }

        private void Update()
        {
            if (GameManager.Instance.IsGameOver || _targetBase == null) return;

            Transform target = GetPriorityTarget() ?? _targetBase;
            var next = Vector3.MoveTowards(transform.position, target.position, _speed * Time.deltaTime);
            transform.position = new Vector3(next.x, next.y, 0f);
        }

        private Transform GetPriorityTarget()
        {
            EnemyController[] units = FindObjectsOfType<EnemyController>();
            EnemyController closest = null;
            float best = 1.4f;
            foreach (var unit in units)
            {
                if (unit == this || unit.IsEnemySide == IsEnemySide) continue;
                float d = Vector2.Distance(transform.position, unit.transform.position);
                if (d < best)
                {
                    best = d;
                    closest = unit;
                }
            }

            return closest != null ? closest.transform : null;
        }

        public void TakeDamage(int amount)
        {
            _hp -= amount;
            if (_hp <= 0)
            {
                if (IsEnemySide) GameManager.Instance.RegisterEnemyKill();
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<EnemyController>(out var otherUnit) && otherUnit.IsEnemySide != IsEnemySide)
            {
                otherUnit.TakeDamage(1);
                TakeDamage(1);
                return;
            }

            if (IsEnemySide)
            {
                if (other.TryGetComponent<BaseController>(out var playerBase) && !playerBase.IsEnemy)
                {
                    playerBase.TakeDamage(2);
                    Destroy(gameObject);
                    return;
                }

                if (other.TryGetComponent<PlayerController>(out var player))
                {
                    player.TakeDamage(1);
                }
            }
            else
            {
                if (other.TryGetComponent<BaseController>(out var enemyBase) && enemyBase.IsEnemy)
                {
                    enemyBase.TakeDamage(1);
                    Destroy(gameObject);
                }
            }
        }
    }
}
