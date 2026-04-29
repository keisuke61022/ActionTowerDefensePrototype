using System.Collections;
using UnityEngine;

namespace PrototypeTD
{
    public class WaveManager : MonoBehaviour
    {
        public int CurrentWave { get; private set; }

        private GameManager _gameManager;

        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            yield return new WaitForSeconds(1.5f);
            for (int wave = 1; wave <= 5; wave++)
            {
                CurrentWave = wave;
                int enemies = 3 + wave;
                for (int i = 0; i < enemies; i++)
                {
                    if (_gameManager.IsGameOver) yield break;
                    float spawnX = Random.Range(-3.5f, 3.5f);
                    _gameManager.SpawnEnemy(new Vector2(spawnX, 7f));
                    yield return new WaitForSeconds(0.7f);
                }

                yield return new WaitForSeconds(2f);
            }

            yield return new WaitForSeconds(5f);
            _gameManager.TryCompleteWavesWin();
        }
    }
}
