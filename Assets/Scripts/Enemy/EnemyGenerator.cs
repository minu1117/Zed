using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyGenerator : MonoBehaviour
{
    public List<EnemyBase> enemies;
    public int poolSize;
    private List<GameObject> poolObjects;
    private List<IObjectPool<EnemyBase>> enemyPools;
    private int createIndex = 0;

    public void Awake()
    {
        enemyPools = new();
        poolObjects = new();

        for (int i = 0; i < enemies.Count; i++)
        {
            var poolObj = new GameObject($"{enemies[createIndex].data.charactorName} Pool");
            poolObj.transform.position = transform.position;
            poolObjects.Add(poolObj);

            IObjectPool<EnemyBase> pool;
            pool = new ObjectPool<EnemyBase>
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

    private EnemyBase CreateEnemy()
    {
        // Create Index = Count - 1
        int index = createIndex - 1;
        var enemyobj = Instantiate(enemies[index].gameObject, poolObjects[index].transform);
        enemyobj.transform.position = transform.position;

        var enemy = enemyobj.GetComponent<EnemyBase>();
        var hpController = enemy.GetHPController();

        enemy.Init();
        hpController.SetMaxValue();
        enemy.SetPool(enemyPools[index]);

        return enemy;
    }
    private void GetEnemy(EnemyBase enemy)
    {
        enemy.transform.position = transform.position;
        enemy.SetIsPatrol(true);
        var hpController = enemy.GetHPController();
        hpController.SetMaxValue();
        enemy.gameObject.SetActive(true);
    }
    private void ReleaseEnemy(EnemyBase enemy)
    {
        enemy.gameObject.SetActive(false);
    }
    private void DestroyEnemy(EnemyBase enemy)
    {
        Destroy(enemy.gameObject);
    }
}
