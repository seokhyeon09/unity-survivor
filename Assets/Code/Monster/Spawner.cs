using UnityEngine;

[System.Serializable]
public struct SpawnData
{
    public int spriteIndex;
    public float spawnTime;
    public float health;
    public float speed;
}

public class Spawner : MonoBehaviour
{
    [Header("# Spawn Settings")]
    public SpawnData[] spawnData;
    public GameObject[] enemyPrefabs;
    public GameObject bossPrefab;

    [Header("# Difficulty & Optimization")]
    public int bossSpawnLimit = 5;
    public int maxMonsterCount = 50;
    public float timeToMaxDifficulty = 600f;

    [Header("# Scaling Difficulty")]
    public float healthScalePerMinute = 0.1f; // 시간당 체력 증가폭

    GameObject player;
    float timer;
    int level;
    bool isBossSpawned = false;

    public static int killCount;
    public static int currentMonsterCount;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        killCount = 0;
        currentMonsterCount = 0;
    }

    void Update()
    {
        if (player == null) return;

        level = Mathf.Min(Mathf.FloorToInt(Time.time / 10f), spawnData.Length - 1);
        timer += Time.deltaTime;

        float timeModifier = Mathf.Lerp(1f, 0.5f, Time.time / timeToMaxDifficulty);
        float actualSpawnTime = spawnData[level].spawnTime * timeModifier;

        if (timer > actualSpawnTime && currentMonsterCount < maxMonsterCount)
        {
            timer = 0f;
            Spawn();
        }

        if (killCount >= bossSpawnLimit && !isBossSpawned)
        {
            SpawnBoss();
        }
    }

    void Spawn()
    {
        if (player == null || enemyPrefabs.Length == 0) return;

        Vector3 spawnDir = Random.insideUnitCircle.normalized;
        float spawnDistance = Random.Range(12f, 15f);
        Vector3 pos = player.transform.position + (spawnDir * spawnDistance);

        int index = Random.Range(0, enemyPrefabs.Length);
        GameObject enemy = Instantiate(enemyPrefabs[index], pos, Quaternion.identity);
        currentMonsterCount++;

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            SpawnData currentData = spawnData[level];

            // [기존 유지] 시간에 따른 일반 몹 체력 강화
            float healthMultiplier = 1f + (healthScalePerMinute * (Time.time / 60f));
            currentData.health *= healthMultiplier;

            enemyScript.Init(currentData);
            enemyScript.target = player.GetComponent<Rigidbody2D>();
        }
    }

    void SpawnBoss()
    {
        if (bossPrefab == null) return;

        isBossSpawned = true;
        Vector3 bossPos = player.transform.position + Vector3.up * 10f;
        GameObject boss = Instantiate(bossPrefab, bossPos, Quaternion.identity);

        Boss bossScript = boss.GetComponent<Boss>();
        if (bossScript != null)
        {
            bossScript.target = player.GetComponent<Rigidbody2D>();

            // [수정된 보스 강화 로직] 
            // 1. 시간에 따른 기본 강화 + 2. 킬 수에 따른 추가 강화 (killCount / 500f)
            // 킬 카운트가 많을수록 보스가 더 단단해집니다.
            float timeBonus = healthScalePerMinute * 2f * (Time.time / 60f);
            float killBonus = killCount / 500f;
            float bossStrength = 1f + timeBonus + killBonus;

            bossScript.Init(bossStrength);
        }
        Debug.Log($"⚠ 보스 소환! (강화 배율: {1f + (healthScalePerMinute * 2f * (Time.time / 60f)) + (killCount / 500f):F2}배)");
    }

    public void ResetBossStatus()
    {
        isBossSpawned = false;
        killCount = 0; // 보스 소환용 킬 카운트 리셋
        Debug.Log("보스 사망! 다음 보스를 위해 킬 카운트 리셋.");
    }
}