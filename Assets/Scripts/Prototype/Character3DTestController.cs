using UnityEngine;

namespace PrototypeTD
{
    public class Character3DTestController : MonoBehaviour
    {
        public enum TeamColor { Red, Blue }

        [Header("Team")]
        [SerializeField] private TeamColor team = TeamColor.Red;
        [SerializeField] private Renderer[] teamRenderers;
        [SerializeField] private Color redColor = new(0.85f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color blueColor = new(0.2f, 0.4f, 0.9f, 1f);

        [Header("State Simulation")]
        [SerializeField] private Animator animator;
        [SerializeField] private Transform attackPivot;
        [SerializeField] private float moveRadius = 2f;
        [SerializeField] private float moveSpeed = 1.8f;
        [SerializeField] private float attackInterval = 1.2f;

        private static readonly int SpeedParam = Animator.StringToHash("Speed");
        private static readonly int AttackParam = Animator.StringToHash("Attack");

        private Vector3 _startPos;
        private float _nextAttackTime;

        public TeamColor Team
        {
            get => team;
            set
            {
                team = value;
                ApplyTeamColor();
            }
        }

        private void Awake()
        {
            _startPos = transform.position;
            if (teamRenderers == null || teamRenderers.Length == 0) teamRenderers = GetComponentsInChildren<Renderer>(true);
            if (animator == null) animator = GetComponentInChildren<Animator>();
            if (attackPivot == null)
            {
                var p = transform.Find("AttackPivot");
                if (p != null) attackPivot = p;
            }
            ApplyTeamColor();
        }

        private void Update()
        {
            SimulateMove();
            SimulateAttack();
        }

        [ContextMenu("Switch Team")]
        public void SwitchTeam()
        {
            Team = team == TeamColor.Red ? TeamColor.Blue : TeamColor.Red;
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
            var dir = target - transform.position;
            if (dir.sqrMagnitude > 0.001f) transform.forward = Vector3.Lerp(transform.forward, dir.normalized, Time.deltaTime * 8f);
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 3f);

            if (animator != null) animator.SetFloat(SpeedParam, dir.magnitude);
        }

        private void SimulateAttack()
        {
            var attackPhase = Time.time >= _nextAttackTime && Time.time < _nextAttackTime + 0.25f;
            if (Time.time >= _nextAttackTime + attackInterval) _nextAttackTime = Time.time;

            if (animator != null) animator.SetBool(AttackParam, attackPhase);

            if (attackPivot == null) return;
            if (attackPhase) attackPivot.localRotation = Quaternion.Euler(-35f, 0f, 0f);
            else attackPivot.localRotation = Quaternion.Lerp(attackPivot.localRotation, Quaternion.identity, Time.deltaTime * 12f);
        }
    }
}
