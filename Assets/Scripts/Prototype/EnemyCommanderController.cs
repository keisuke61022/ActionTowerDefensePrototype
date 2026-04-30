using UnityEngine;

namespace PrototypeTD
{
    public class EnemyCommanderController : MonoBehaviour
    {
        private float _moveSpeed = 2.2f;
        private float _nextTargetTime;
        private Vector3 _moveTarget;
        private float _nextShoot;

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
            Rect area = GameManager.Instance.GetEnemyCommanderRect(GetVisualExtents());
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
            _nextShoot = Time.time + 0.75f;

            Vector3 dir = Vector3.down;
            ProjectileController.Create(transform.position + dir * 0.55f, dir, 7f, 1, false);
        }

        private Vector2 GetVisualExtents()
        {
            var col = GetComponent<Collider>();
            if (col != null) return col.bounds.extents;

            var renderer = GetComponent<Renderer>();
            if (renderer != null) return renderer.bounds.extents;

            return new Vector2(0.3f, 0.45f);
        }
    }
}
