using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public GameObject enemyPrefab;
    public float spawnRadius = 1f;
    public float spawnInterval = 2f;

    private void Start() {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies() {
        while (true) {
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            print("Spawn position: " + spawnPosition);

            GameObject newEnemyPrefab = enemyPrefab;

            // Instantiate the enemy prefab and get a reference to the Enemy component
            GameObject newEnemyGameObject = Instantiate(newEnemyPrefab, spawnPosition, Quaternion.identity);
            Enemy newEnemy = newEnemyGameObject.GetComponent<Enemy>();

            // Set the properties of the new enemy
            newEnemy.isActive = true;
            newEnemy.enemyDamage = Random.Range(1, 11);;

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
