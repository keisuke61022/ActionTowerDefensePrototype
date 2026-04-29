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
            go.transform.localScale = Vector3.one * 0.2f;
            go.GetComponent<MeshRenderer>().material.color = playerSide ? Color.white : Color.magenta;

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

            Destroy(go, 3f);
        }

        private void Update()
        {
            transform.position += _direction * (_speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_playerSide)
            {
                if (other.TryGetComponent<EnemyController>(out var enemy))
                {
                    enemy.TakeDamage(_damage);
                    Destroy(gameObject);
                }

                if (other.TryGetComponent<BaseController>(out var targetBase) && targetBase.IsEnemy)
                {
                    targetBase.TakeDamage(_damage);
                    Destroy(gameObject);
                }
            }
        }
    }
}
