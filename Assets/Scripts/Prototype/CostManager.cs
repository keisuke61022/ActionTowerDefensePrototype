using UnityEngine;

namespace PrototypeTD
{
    public class CostManager : MonoBehaviour
    {
        public int Current { get; private set; }
        public int Max { get; private set; }

        public void Initialize(int initial, int max)
        {
            Current = initial;
            Max = max;
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
