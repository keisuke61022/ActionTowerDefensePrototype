using UnityEngine;

namespace PrototypeTD
{
    public class Character3DTestController : MonoBehaviour
    {
        public enum TeamColor { Red, Blue }
        public enum MotionState { Idle, Walk, Attack }

        [Header("Team")]
        [SerializeField] private TeamColor team = TeamColor.Red;
        [SerializeField] private Renderer[] teamRenderers;
        [SerializeField] private Color redColor = new Color(0.85f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color blueColor = new Color(0.2f, 0.4f, 0.9f, 1f);

        private MotionState _motion = MotionState.Idle;
        private Vector3 _basePos;
        private Quaternion _baseRot;
        private GameObject _weaponVisual;
        private float _t;
        private Animator _animator;

        public TeamColor Team => team;
        public MotionState CurrentMotion => _motion;

        private void Awake()
        {
            _basePos = transform.position;
            _baseRot = transform.rotation;

            _animator = GetComponentInChildren<Animator>();
            if (_animator != null) _animator.enabled = false;

            if (teamRenderers == null || teamRenderers.Length == 0)
                teamRenderers = GetComponentsInChildren<Renderer>(true);
            ApplyTeamColor();
        }

        private void Update()
        {
            _t += Time.deltaTime;
            switch (_motion)
            {
                case MotionState.Idle:   SimulateIdle();   break;
                case MotionState.Walk:   SimulateWalk();   break;
                case MotionState.Attack: SimulateAttack(); break;
            }
        }

        // ── fake motions ─────────────────────────────────────────

        private void SimulateIdle()
        {
            var y = Mathf.Sin(_t * 1.5f) * 0.04f;
            transform.position = _basePos + new Vector3(0f, y, 0f);
            transform.rotation = _baseRot;
        }

        private void SimulateWalk()
        {
            var x = Mathf.Sin(_t * 1.8f) * 2.0f;
            var y = Mathf.Abs(Mathf.Sin(_t * 3.6f)) * 0.06f;
            transform.position = _basePos + new Vector3(x, y, 0f);
            var dx = Mathf.Cos(_t * 1.8f);
            if (Mathf.Abs(dx) > 0.01f)
                transform.forward = new Vector3(Mathf.Sign(dx), 0f, 0f);
        }

        private void SimulateAttack()
        {
            transform.position = Vector3.Lerp(transform.position, _basePos, Time.deltaTime * 8f);
            transform.rotation = Quaternion.Slerp(transform.rotation, _baseRot, Time.deltaTime * 8f);

            if (_weaponVisual == null) CreateWeaponVisual();
            var pulse = (Mathf.Sin(_t * 6f) + 1f) * 0.5f;
            _weaponVisual.transform.position = _basePos + new Vector3(0f, 0.8f, 0.3f + pulse * 0.9f);
        }

        private void CreateWeaponVisual()
        {
            _weaponVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _weaponVisual.name = "WeaponVisual";
            _weaponVisual.transform.localScale = Vector3.one * 0.18f;
            Destroy(_weaponVisual.GetComponent<Collider>());
            _weaponVisual.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }

        // ── public API ───────────────────────────────────────────

        public void SetMotion(MotionState state)
        {
            _t = 0f;
            if (state != MotionState.Attack && _weaponVisual != null)
            {
                Destroy(_weaponVisual);
                _weaponVisual = null;
            }
            _motion = state;
        }

        [ContextMenu("Switch Team")]
        public void SwitchTeam() => SetTeam(team == TeamColor.Red ? TeamColor.Blue : TeamColor.Red);

        public void SetTeam(TeamColor t)
        {
            team = t;
            ApplyTeamColor();
        }

        [ContextMenu("Apply Team Color")]
        public void ApplyTeamColor()
        {
            var color = team == TeamColor.Red ? redColor : blueColor;
            var block = new MaterialPropertyBlock();
            foreach (var r in teamRenderers)
            {
                if (r == null) continue;
                r.GetPropertyBlock(block);
                block.SetColor("_BaseColor", color);
                block.SetColor("_Color", color);
                r.SetPropertyBlock(block);
            }
        }
    }
}
