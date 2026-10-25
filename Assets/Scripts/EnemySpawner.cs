
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject enemyPrefab;

    [Header("Four Spawn Points")]
    public Transform[] spawnPoints = new Transform[4];

    [Header("Difficulty")]
    public float startingInterval = 3f;
    public float minimumInterval = 0.5f;
    public float intervalDecrease = 0.2f;
    public float difficultyStepTime = 10f;

    private float spawnTimer;
    private float difficultyTimer;
    private float currentInterval;
    private bool stopped;

    private void Start()
    {
        currentInterval = startingInterval;
        spawnTimer = 0f;
        difficultyTimer = 0f;

        ValidateSetup();
    }

    private void Update()
    {
        if (stopped || GameManager.IsGameOver)
            return;

        difficultyTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        if (difficultyTimer >= difficultyStepTime)
        {
            difficultyTimer = 0f;

            currentInterval = Mathf.Max(
                minimumInterval,
                currentInterval - intervalDecrease
            );
        }

        if (spawnTimer >= currentInterval)
        {
            spawnTimer = 0f;
            SpawnEnemy();
        }
    }

    private void ValidateSetup()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError(
                "EnemySpawner: Assign the enemy PREFAB asset " +
                "from the Project window!",
                this
            );
        }

        if (spawnPoints == null || spawnPoints.Length != 4)
        {
            Debug.LogWarning(
                "EnemySpawner: Assign exactly four spawn points.",
                this
            );
        }
    }

    private void SpawnEnemy()
    {
        if (GameManager.IsGameOver || stopped)
            return;

        if (enemyPrefab == null ||
            spawnPoints == null ||
            spawnPoints.Length == 0)
            return;

        List<Transform> validPoints = new List<Transform>();

        foreach (Transform point in spawnPoints)
        {
            if (point != null)
                validPoints.Add(point);
        }

        if (validPoints.Count == 0)
        {
            Debug.LogError(
                "EnemySpawner: No valid spawn points assigned!",
                this
            );
            return;
        }

        Transform selectedPoint = validPoints[
            Random.Range(0, validPoints.Count)
        ];

        GameObject enemy = Instantiate(
            enemyPrefab,
            selectedPoint.position,
            Quaternion.identity
        );

        ColoredEnemy coloredEnemy =
            enemy.GetComponent<ColoredEnemy>();

        if (coloredEnemy == null)
        {
            Debug.LogError(
                "Enemy prefab needs the ColoredEnemy component!",
                enemy
            );
            Destroy(enemy);
            return;
        }

        ColorState randomColor = (ColorState)
            Random.Range(0, 5);

        coloredEnemy.SetColor(randomColor);
    }

    public void StopSpawning()
    {
        stopped = true;
    }
}
