using UnityEngine;
using UnityEngine.InputSystem;

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
            Vector2 moveInput = Vector2.zero;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveInput.x -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveInput.x += 1f;
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveInput.y += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveInput.y -= 1f;
            }

            Vector2 stick = GameManager.Instance.UIManager.GetStickDirection();
            Vector3 dir = new Vector3(moveInput.x + stick.x, moveInput.y + stick.y, 0f);
            if (dir.sqrMagnitude > 1f) dir.Normalize();

            transform.position += dir * (_speed * Time.deltaTime);
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -4f, 4f), Mathf.Clamp(transform.position.y, -7f, 7f), 0f);
        }

        private void HandleActions()
        {
            bool keyboardShoot = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;
            bool mouseShoot = Mouse.current != null && Mouse.current.leftButton.isPressed;
            bool shootPressed = keyboardShoot || mouseShoot || GameManager.Instance.UIManager.ShootPressed;
            if (shootPressed && Time.time >= _nextShoot)
            {
                _nextShoot = Time.time + _shootCooldown;
                ProjectileController.Create(transform.position + Vector3.up * 0.7f, Vector3.up, 8f, 1, true);
            }

            bool placePressed = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
            if (placePressed || GameManager.Instance.UIManager.PlaceUnit1Pressed)
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
