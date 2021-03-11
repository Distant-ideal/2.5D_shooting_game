using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Wave[] waves; //有多少波敌人

    private Wave currentwave; //当前波
    private int currentIndex; //当前波在数组中的序号，索引Index

    public int waitSpawnNum; //这一波还剩下多少敌人没有生成，等于0以后不再生成新的敌人
    public int spawnAliveNum;//这一波的敌人还生存了多少个，少于0的话触发下一波敌人的逻辑
    public float nextspawntime;

    private void Start()
    {
        NextWave();
    }

    private void NextWave()
    {
        currentIndex++;
        Debug.Log(string.Format("Current Wave : {0}", currentIndex));

        if(currentIndex -1 < waves.Length)
        {
            currentwave = waves[currentIndex - 1];
            waitSpawnNum = currentwave.enmuNum;
            spawnAliveNum = currentwave.enmuNum;
        }
    }

    private void Update()
    {
        if(waitSpawnNum > 0 && Time.time > nextspawntime)
        {
            waitSpawnNum--;//等待生成的敌人-1
            GameObject spawnEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity); //生成新敌人
            //每当生成了新的敌人就要将这个敌人的阵亡事件处理器订阅到事件onDeath上
            spawnEnemy.GetComponent<Enemy>().onDeath += EnemyDeath;

            nextspawntime = Time.time + currentwave.timeBtwSpawn; //Time.time游戏开始后的时间 currentwave.timeBtwSpawn距离上一次距离timeBtwSpawns
        }
    }

    //当敌人阵亡时，spawnAliveNum - 1， spawnAliveNum = 0时，下一波 
    private void EnemyDeath()
    {
        spawnAliveNum--;
        if(spawnAliveNum <= 0)
        {
            NextWave();
        }
    }
}

