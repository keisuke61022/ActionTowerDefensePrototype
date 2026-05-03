using UnityEngine;

namespace PrototypeTD
{
    public class CameraAngleSwitcher : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private Transform lookTarget;
        [SerializeField] private Vector3[] positions =
        {
            new(8f, 1.5f, 0f),
            new(6f, 6f, -6f),
            new(0f, 2f, -8f)
        };

        private int _current;

        private void Start()
        {
            if (targetCamera == null) targetCamera = GetComponent<Camera>();
            ApplyCurrent();
        }

        public void SetLookTarget(Transform target)
        {
            lookTarget = target;
            ApplyCurrent();
        }

        public void NextAngle()
        {
            if (positions.Length == 0) return;
            _current = (_current + 1) % positions.Length;
            ApplyCurrent();
        }

        public void SetAngleIndex(int index)
        {
            if (positions.Length == 0) return;
            _current = Mathf.Clamp(index, 0, positions.Length - 1);
            ApplyCurrent();
        }

        private void ApplyCurrent()
        {
            if (targetCamera == null || lookTarget == null || positions.Length == 0) return;
            targetCamera.transform.position = positions[_current];
            targetCamera.transform.LookAt(lookTarget.position + Vector3.up * 1.2f);
        }
    }
}
