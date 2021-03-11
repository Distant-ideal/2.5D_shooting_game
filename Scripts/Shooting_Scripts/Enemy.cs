using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    private NavMeshAgent navMeshAgent;
    private Transform target; //追击

    [SerializeField] private float updateRate; //每三秒一次新的反应追击敌人

    protected override void Start()
    {
        base.Start();
        navMeshAgent = GetComponent<NavMeshAgent>();
        //敌人生成时player可能已经阵亡就不会追击但我们邮箱获取他的trans组件所以要进行判断
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }

        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        //navMeshAgent.SetDestination(target.position); //锲而不舍类AI
    }

    //AI 摸鱼类AI 【反应迟钝的敌人】
    IEnumerator UpdatePath()
    {
        //float updateRate = 3.0f; //每三秒一次新的反应追击敌人

        while(target != null)
        {
            Vector3 preTargetPos = new Vector3(target.position.x, 0, target.position.z);
            navMeshAgent.SetDestination(preTargetPos);

            yield return new WaitForSeconds(updateRate); //每过三秒确认一次目标的位置。
        }

    }
}
