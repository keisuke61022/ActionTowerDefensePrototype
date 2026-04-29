using UnityEngine;

namespace PrototypeTD
{
    public class PlayerController : MonoBehaviour
    {
        public int CurrentHp { get; private set; } = 10;

        private float _speed = 4f;
        private float _shootCooldown = 0.25f;
        private float _nextShoot;

        private void Update()
        {
            if (GameManager.Instance.IsGameOver) return;

            Move();
            HandleActions();
        }

        private void Move()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector2 stick = GameManager.Instance.UIManager.GetStickDirection();

            Vector3 dir = new Vector3(horizontal + stick.x, vertical + stick.y, 0f);
            if (dir.sqrMagnitude > 1f) dir.Normalize();

            transform.position += dir * (_speed * Time.deltaTime);
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -4f, 4f),
                Mathf.Clamp(transform.position.y, -7f, 7f),
                0f);
        }

        private void HandleActions()
        {
            bool shootPressed = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0) || GameManager.Instance.UIManager.ShootPressed;
            if (shootPressed && Time.time >= _nextShoot)
            {
                _nextShoot = Time.time + _shootCooldown;
                ProjectileController.Create(transform.position + Vector3.up * 0.7f, Vector3.up, 8f, 1, true);
            }

            if (Input.GetKeyDown(KeyCode.E) || GameManager.Instance.UIManager.PlaceUnit1Pressed)
            {
                GameManager.Instance.TryPlaceTurret();
            }
        }

        public void TakeDamage(int amount)
        {
            CurrentHp = Mathf.Max(0, CurrentHp - amount);
            if (CurrentHp <= 0)
            {
                GameManager.Instance.OnBaseDestroyed(GameManager.Instance.PlayerBase);
                Destroy(gameObject);
            }
        }
    }
}
