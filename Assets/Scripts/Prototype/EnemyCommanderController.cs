using UnityEngine;

namespace PrototypeTD
{
    public class EnemyCommanderController : MonoBehaviour
    {
        private float _moveSpeed = 2.2f;
        private float _nextTargetTime;
        private Vector3 _moveTarget;
        private float _nextShoot;
        private readonly Vector2 _extents = new Vector2(0.5f, 0.5f);

        private void Start()
        {
            PickTarget();
        }

        private void Update()
        {
            if (GameManager.Instance.IsGameOver) return;

            MoveAI();
            AttackAI();
        }

        private void MoveAI()
        {
            if (Time.time >= _nextTargetTime || Vector2.Distance(transform.position, _moveTarget) < 0.2f)
            {
                PickTarget();
            }

            var pos = Vector3.MoveTowards(transform.position, _moveTarget, _moveSpeed * Time.deltaTime);
            Rect area = GameManager.Instance.GetEnemyCommanderRect(_extents);
            transform.position = new Vector3(
                Mathf.Clamp(pos.x, area.xMin, area.xMax),
                Mathf.Clamp(pos.y, area.yMin, area.yMax),
                0f
            );
        }

        private void PickTarget()
        {
            Vector3 desired = new Vector3(Random.Range(-2.8f, 2.8f), Random.Range(1.0f, 4.8f), 0f);
            var player = GameManager.Instance.Player;
            if (player != null)
            {
                float d = Vector2.Distance(transform.position, player.transform.position);
                if (d < 4f)
                {
                    Vector3 away = (transform.position - player.transform.position).normalized;
                    desired = transform.position + away * 1.8f;
                }
            }

            EnemyController[] allUnits = FindObjectsOfType<EnemyController>();
            foreach (var unit in allUnits)
            {
                if (unit.IsEnemySide) continue;
                float d = Vector2.Distance(transform.position, unit.transform.position);
                if (d < 2.8f)
                {
                    Vector3 away = (transform.position - unit.transform.position).normalized;
                    desired = transform.position + away * 2f;
                    break;
                }
            }

            _moveTarget = desired;
            _nextTargetTime = Time.time + Random.Range(1.5f, 3.2f);
        }

        private void AttackAI()
        {
            if (Time.time < _nextShoot) return;
            _nextShoot = Time.time + 0.7f;

            Vector3 dir = Vector3.down;
            Transform target = FindNearestTarget();
            if (target != null) dir = (target.position - transform.position).normalized;
            ProjectileController.Create(transform.position + dir * 0.7f, dir, 7f, 1, false);
        }

        private Transform FindNearestTarget()
        {
            Transform best = null;
            float bestDist = 6f;

            if (GameManager.Instance.Player != null)
            {
                float pd = Vector2.Distance(transform.position, GameManager.Instance.Player.transform.position);
                if (pd < bestDist)
                {
                    bestDist = pd;
                    best = GameManager.Instance.Player.transform;
                }
            }

            EnemyController[] units = FindObjectsOfType<EnemyController>();
            foreach (var unit in units)
            {
                if (unit.IsEnemySide) continue;
                float d = Vector2.Distance(transform.position, unit.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = unit.transform;
                }
            }

            return best;
        }
    }
}
