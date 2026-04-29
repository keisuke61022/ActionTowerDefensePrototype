using UnityEngine;

namespace PrototypeTD
{
    public class BaseController : MonoBehaviour
    {
        public int CurrentHp { get; private set; }
        public int MaxHp { get; private set; }
        public bool IsEnemy { get; private set; }

        public void Initialize(int hp, bool isEnemy)
        {
            MaxHp = hp;
            CurrentHp = hp;
            IsEnemy = isEnemy;
        }

        public void TakeDamage(int amount)
        {
            if (GameManager.Instance.IsGameOver)
            {
                return;
            }

            CurrentHp = Mathf.Max(0, CurrentHp - amount);
            if (CurrentHp <= 0)
            {
                GameManager.Instance.OnBaseDestroyed(this);
            }
        }
    }
}
