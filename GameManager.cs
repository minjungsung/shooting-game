using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public int stage;
    public Animator stageAnimator;
    public Animator clearAnimator;
    public Animator fadeAnimator;
    public Transform playerPos;
    public string[] enemyObjects;
    public Transform[] spawnPoints;

    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public Text scoreText;
    public Image[] lifeImages;
    public Image[] boomImages;
    public GameObject gameOverSet;
    public ObjectManager objectManager;

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjects = new string[] { "EnemyS", "EnemyM", "EnemyL", "Boss" };
        StageStart();
    }

    void ReadSpawnFile()
    {
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        TextAsset textFile = Resources.Load("Stage " + stage.ToString()) as TextAsset;
        StringReader reader = new StringReader(textFile.text);

        while (reader != null)
        {
            string line = reader.ReadLine();
            if (line == null) break;

            Spawn spawn = new Spawn();

            spawn.delay = float.Parse(line.Split(',')[0]);
            spawn.type = line.Split(',')[1];
            spawn.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawn);
        }
        reader.Close();
        nextSpawnDelay = spawnList[0].delay;
    }

    public void StageStart()
    {
        Debug.Log("StageStart");
        stageAnimator.SetTrigger("On");
        stageAnimator.GetComponent<Text>().text = "Stage " + stage + "\nStart";
        ReadSpawnFile();
        fadeAnimator.SetTrigger("In");
    }

    public void StageEnd()
    {
        Debug.Log("StageEnd");
        clearAnimator.SetTrigger("On");
        clearAnimator.GetComponent<Text>().text = "Stage " + stage + "\nClear";
        fadeAnimator.SetTrigger("Out");
        player.transform.position = playerPos.position;
        stage++;
        StartCoroutine(HandleStageTransition());

    }

    private IEnumerator HandleStageTransition()
    {
        // Wait for the clear message to be visible
        yield return new WaitForSeconds(6f);

        if (stage > 2)
        {
            GameOver();
        }
        else
        {
            StageStart();
        }
    }

    void Update()
    {
        curSpawnDelay += Time.deltaTime;
        if (curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            curSpawnDelay = 0;
        }

        if (player != null)
        {
            Player playerLogic = player.GetComponent<Player>();
            scoreText.text = string.Format("{0:n0}", playerLogic.score);
        }
    }

    void SpawnEnemy()
    {
        int enemyIndex = 0;
        switch (spawnList[spawnIndex].type)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            case "Boss":
                enemyIndex = 3;
                break;
        }


        int enemyPoint = spawnList[spawnIndex].point;

        GameObject enemy = objectManager.MakeObj(enemyObjects[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;
        enemyLogic.gameManager = this;
        enemyLogic.objectManager = objectManager;
        if (enemyPoint == 5 || enemyPoint == 6)
        {
            enemy.transform.Rotate(Vector3.back * 90);
            rigid.velocity = new Vector2(enemyLogic.speed * -1, -1);
        }
        else if (enemyPoint == 7 || enemyPoint == 8)
        {
            enemy.transform.Rotate(Vector3.forward * 90);
            rigid.velocity = new Vector2(enemyLogic.speed * 1, -1);
        }
        else
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed * -1);
        }

        spawnIndex++;
        if (spawnIndex >= spawnList.Count)
        {
            spawnEnd = true;
            return;
        }

        nextSpawnDelay = spawnList[spawnIndex].delay;
    }

    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 2f);
    }

    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }

    public void UpdateLifeIcon(int life)
    {
        for (int i = 0; i < 3; i++)
        {
            lifeImages[i].color = new Color(1, 1, 1, 0);
        }

        for (int i = 0; i < life; i++)
        {
            lifeImages[i].color = new Color(1, 1, 1, 1);
        }
    }

    public void CallExplosion(Vector3 pos, string type)
    {
        GameObject explosion = objectManager.MakeObj("Explosion");
        Explosion explosionLogic = explosion.GetComponent<Explosion>();

        explosion.transform.position = pos;
        explosionLogic.StartExplosion(type);
    }

    public void UpdateBoomIcon(int boom)
    {
        for (int i = 0; i < 3; i++)
        {
            boomImages[i].color = new Color(1, 1, 1, 0);
        }

        for (int i = 0; i < boom; i++)
        {
            boomImages[i].color = new Color(1, 1, 1, 1);
        }
    }


    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
}


