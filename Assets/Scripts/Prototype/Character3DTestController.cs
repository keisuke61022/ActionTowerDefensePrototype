using UnityEngine;

namespace PrototypeTD
{
    public class Character3DTestController : MonoBehaviour
    {
        public enum TeamColor
        {
            Red,
            Blue
        }

        [Header("Team")]
        [SerializeField] private TeamColor team = TeamColor.Red;
        [SerializeField] private Renderer[] teamRenderers;
        [SerializeField] private Color redColor = new(0.85f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color blueColor = new(0.2f, 0.4f, 0.9f, 1f);

        [Header("Animation-like Test")]
        [SerializeField] private Transform weaponDummy;
        [SerializeField] private float moveRadius = 2f;
        [SerializeField] private float moveSpeed = 1.8f;
        [SerializeField] private float attackInterval = 1.2f;

        private Vector3 _startPos;
        private float _nextAttackTime;

        private void Awake()
        {
            _startPos = transform.position;
            if (teamRenderers == null || teamRenderers.Length == 0)
            {
                teamRenderers = GetComponentsInChildren<Renderer>();
            }

            if (weaponDummy == null)
            {
                var weapon = transform.Find("WeaponDummy");
                if (weapon != null) weaponDummy = weapon;
            }

            ApplyTeamColor();
        }

        private void Update()
        {
            SimulateMove();
            SimulateAttack();
        }

        [ContextMenu("Apply Team Color")]
        public void ApplyTeamColor()
        {
            var color = team == TeamColor.Red ? redColor : blueColor;
            foreach (var r in teamRenderers)
            {
                if (r == null || r.sharedMaterial == null) continue;
                r.material.color = color;
            }
        }

        private void SimulateMove()
        {
            var x = Mathf.Sin(Time.time * moveSpeed) * moveRadius;
            var z = Mathf.Cos(Time.time * moveSpeed * 0.7f) * moveRadius * 0.4f;
            var target = _startPos + new Vector3(x, 0f, z);
            var dir = (target - transform.position);
            if (dir.sqrMagnitude > 0.001f)
            {
                transform.forward = Vector3.Lerp(transform.forward, dir.normalized, Time.deltaTime * 8f);
            }
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 3f);
        }

        private void SimulateAttack()
        {
            if (weaponDummy == null) return;

            var attackPhase = Time.time >= _nextAttackTime && Time.time < _nextAttackTime + 0.25f;
            if (Time.time >= _nextAttackTime + attackInterval)
            {
                _nextAttackTime = Time.time;
            }

            if (attackPhase)
            {
                weaponDummy.localRotation = Quaternion.Euler(-35f, 0f, 0f);
            }
            else
            {
                weaponDummy.localRotation = Quaternion.Lerp(weaponDummy.localRotation, Quaternion.identity, Time.deltaTime * 12f);
            }
        }
    }
}
