using UnityEngine;
using UnityEngine.UI;

public class SpawnNotifier : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Image timer;
    
    private float spawnDelay = 3f;
    private float accumulatedTime = 0f;

    void Update()
    {
        accumulatedTime += Time.deltaTime;
        timer.fillAmount = accumulatedTime / spawnDelay;
        if (accumulatedTime > spawnDelay)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
