using UnityEngine;

namespace PrototypeTD
{
    public class CameraAngleSwitcher : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private Transform lookTarget;
        [SerializeField] private Vector3[] positions =
        {
            new(8f, 1.5f, 0f),   // side
            new(6f, 6f, -6f),    // top-down
            new(0f, 2f, -8f)     // front-ish
        };

        private int _current;

        private void Start()
        {
            if (targetCamera == null) targetCamera = GetComponent<Camera>();
            if (lookTarget == null)
            {
                var c = FindObjectOfType<Character3DTestController>();
                if (c != null) lookTarget = c.transform;
            }
            ApplyCurrent();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                _current = (_current + 1) % positions.Length;
                ApplyCurrent();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) { _current = 0; ApplyCurrent(); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { _current = 1; ApplyCurrent(); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { _current = 2; ApplyCurrent(); }
        }

        private void ApplyCurrent()
        {
            if (targetCamera == null || lookTarget == null || positions.Length == 0) return;
            targetCamera.transform.position = positions[_current];
            targetCamera.transform.LookAt(lookTarget.position + Vector3.up * 1.2f);
        }
    }
}
