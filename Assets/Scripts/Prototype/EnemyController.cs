using UnityEngine;

namespace PrototypeTD
{
    public class EnemyController : MonoBehaviour
    {
        private Transform _target;
        private int _hp = 3;
        private float _speed = 1.5f;

        public void Initialize(Transform target)
        {
            _target = target;
        }

        private void Update()
        {
            if (GameManager.Instance.IsGameOver) return;
            if (_target == null) return;

            var next = Vector3.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);
            transform.position = new Vector3(next.x, next.y, 0f);
        }

        public void TakeDamage(int amount)
        {
            _hp -= amount;
            if (_hp <= 0)
            {
                GameManager.Instance.RegisterEnemyKill();
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<BaseController>(out var baseController) && !baseController.IsEnemy)
            {
                baseController.TakeDamage(2);
                Destroy(gameObject);
                return;
            }

            if (other.TryGetComponent<PlayerController>(out var player))
            {
                player.TakeDamage(1);
            }
        }
    }
}
