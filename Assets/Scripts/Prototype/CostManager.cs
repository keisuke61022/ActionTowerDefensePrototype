using UnityEngine;

namespace PrototypeTD
{
    public class CostManager : MonoBehaviour
    {
        public int Current { get; private set; }
        public int Max { get; private set; }

        private float _regenInterval = 1f;
        private float _nextRegenTime;

        public void Initialize(int initial, int max, float regenInterval = 1f)
        {
            Current = initial;
            Max = max;
            _regenInterval = Mathf.Max(0.1f, regenInterval);
            _nextRegenTime = Time.time + _regenInterval;
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
            if (Time.time < _nextRegenTime) return;

            Add(1);
            _nextRegenTime = Time.time + _regenInterval;
        }

        public bool TrySpend(int amount)
        {
            if (Current < amount) return false;
            Current -= amount;
            return true;
        }

        public void Add(int amount)
        {
            Current = Mathf.Min(Max, Current + amount);
        }
    }
}
