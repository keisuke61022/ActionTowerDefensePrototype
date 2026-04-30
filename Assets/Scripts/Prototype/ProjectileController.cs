using UnityEngine;

namespace PrototypeTD
{
    public class ProjectileController : MonoBehaviour
    {
        private Vector3 _direction;
        private float _speed;
        private int _damage;
        private bool _playerSide;

        public static void Create(Vector3 position, Vector3 direction, float speed, int damage, bool playerSide)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = playerSide ? "PlayerBullet" : "EnemyBullet";
            go.transform.position = position;
            go.transform.localScale = Vector3.one * 0.14f;
            go.GetComponent<MeshRenderer>().material.color = playerSide ? new Color(0.45f, 0.85f, 1f) : new Color(1f, 0.3f, 0.3f);

            var col = go.AddComponent<SphereCollider>();
            col.isTrigger = true;
            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            var projectile = go.AddComponent<ProjectileController>();
            projectile._direction = direction.normalized;
            projectile._speed = speed;
            projectile._damage = damage;
            projectile._playerSide = playerSide;

            Object.Destroy(go, 3f);
        }

        private void Update() => transform.position += _direction * (_speed * Time.deltaTime);

        private void OnTriggerEnter(Collider other)
        {
            if (_playerSide)
            {
                if (other.TryGetComponent<EnemyController>(out var enemy) && enemy.IsEnemySide)
                {
                    enemy.TakeDamage(_damage);
                    Destroy(gameObject);
                    return;
                }
                if (other.TryGetComponent<EnemyCommanderController>(out _))
                {
                    Destroy(gameObject);
                    return;
                }
                if (other.TryGetComponent<BaseController>(out var targetBase) && targetBase.IsEnemy)
                {
                    targetBase.TakeDamage(1);
                    Destroy(gameObject);
                }
            }
            else
            {
                if (other.TryGetComponent<PlayerController>(out var player))
                {
                    player.TakeDamage(_damage);
                    Destroy(gameObject);
                    return;
                }
                if (other.TryGetComponent<EnemyController>(out var ally) && !ally.IsEnemySide)
                {
                    ally.TakeDamage(_damage);
                    Destroy(gameObject);
                    return;
                }
                if (other.TryGetComponent<BaseController>(out var myBase) && !myBase.IsEnemy)
                {
                    myBase.TakeDamage(_damage);
                    Destroy(gameObject);
                }
            }
        }
    }
}
