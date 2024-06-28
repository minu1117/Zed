using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyGenerator : MonoBehaviour
{
    public List<Enemy> enemies;
    public int poolSize;
    private List<GameObject> poolObjects;
    private List<IObjectPool<Enemy>> enemyPools;
    private int createIndex = 0;

    public void Awake()
    {
        enemyPools = new();
        poolObjects = new();

        for (int i = 0; i < enemies.Count; i++)
        {
            poolObjects.Add(new GameObject($"{enemies[createIndex].data.charactorName} Pool"));

            IObjectPool<Enemy> pool;
            pool = new ObjectPool<Enemy>
            (
                CreateEnemy,
                GetEnemy,
                ReleaseEnemy,
                DestroyEnemy,
                maxSize : poolSize
            );

            enemyPools.Add(pool);
            createIndex++;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            enemyPools[0].Get();
        }
    }

    private Enemy CreateEnemy()
    {
        // Create Index = Count - 1
        int index = createIndex - 1;
        var enemyobj = Instantiate(enemies[index].gameObject, poolObjects[index].transform);
        var enemy = enemyobj.GetComponent<Enemy>();
        var hpController = enemy.GetHPController();

        enemy.Init();
        hpController.SetMaxHP();
        enemy.SetPool(enemyPools[index]);
        enemy.transform.position = Vector3.zero;

        return enemy;
    }
    private void GetEnemy(Enemy enemy)
    {
        enemy.transform.position = Vector3.zero;
        var hpController = enemy.GetHPController();
        hpController.SetMaxHP();
        enemy.gameObject.SetActive(true);
    }
    private void ReleaseEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }
    private void DestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }
}
