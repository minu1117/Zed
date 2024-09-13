using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyGenerator : MonoBehaviour
{
    public List<EnemyBase> enemies;                     // 생성할 몬스터 List
    public int poolSize;
    private List<GameObject> poolObjects;               // 생성된 몬스터를 종류마다 담아둘 부모 오브젝트의 List
    private List<IObjectPool<EnemyBase>> enemyPools;    // 몬스터 종류 별 오브젝트 풀 List
    private int createIndex = 0;                        // 생성 시 설정될 풀의 Index

    public void Awake()
    {
        enemyPools = new();
        poolObjects = new();

        // 생성될 몬스터 List 순회
        for (int i = 0; i < enemies.Count; i++)
        {
            var poolObj = new GameObject($"{enemies[i].data.charactorName} Pool");    // 몬스터 오브젝트를 담아둘 부모 오브젝트 생성
            poolObj.transform.position = transform.position;                          // 부모 오브젝트를 생성기의 위치로 이동 (몬스터가 이상한 위치에서 생성되지 않기 위함)
            poolObjects.Add(poolObj);

            IObjectPool<EnemyBase> pool;        // 오브젝트 풀 생성
            pool = new ObjectPool<EnemyBase>    // 오브젝트 풀 초기화
            (
                CreateEnemy,
                GetEnemy,
                ReleaseEnemy,
                DestroyEnemy,
                maxSize : poolSize
            );

            enemyPools.Add(pool);   // 몬스터 풀 List에 오브젝트 풀 추가
        }
    }

    public void Update()
    {
        // 임시 코드
        // F1 키 입력으로 랜덤 몬스터 소환
        if (Input.GetKeyDown(KeyCode.F1))
        {
            var index = Random.Range(0, enemyPools.Count);
            createIndex = index;
            enemyPools[createIndex].Get();
        }
    }

    // 오브젝트 풀의 Create 메서드
    private EnemyBase CreateEnemy()
    {
        var enemyobj = Instantiate(enemies[createIndex].gameObject, poolObjects[createIndex].transform);
        enemyobj.transform.position = transform.position;

        var enemy = enemyobj.GetComponent<EnemyBase>();
        var hpController = enemy.GetHPController();

        enemy.Init();
        hpController.SetMaxValue();
        enemy.SetPool(enemyPools[createIndex]);

        return enemy;
    }

    // 오브젝트 풀의 Get 메서드
    private void GetEnemy(EnemyBase enemy)
    {
        enemy.transform.position = transform.position;
        enemy.SetIsPatrol(true);
        var hpController = enemy.GetHPController();
        hpController.SetMaxValue();
        enemy.gameObject.SetActive(true);
    }

    // 오브젝트 풀의 Release 메서드
    private void ReleaseEnemy(EnemyBase enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    // 오브젝트 풀의 Destroy 메서드
    private void DestroyEnemy(EnemyBase enemy)
    {
        Destroy(enemy.gameObject);
    }
}
