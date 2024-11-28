using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjects;
    public Transform[] spawnPoints;

    public float maxSpawnDelay;
    public float curSpawnDelay;

    void Update()
    {
        curSpawnDelay += Time.deltaTime;
        if (curSpawnDelay > maxSpawnDelay)
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0;
        }
    }

    void SpawnEnemy()
    {
        int enemyIndex = Random.Range(0, enemyObjects.Length);
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(enemyObjects[enemyIndex], spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
    }

}
