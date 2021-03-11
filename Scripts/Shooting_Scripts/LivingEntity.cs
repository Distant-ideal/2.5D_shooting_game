using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这个脚本是敌人和玩家的父类
//C#不支持多继承，只能继承一个父类但是可以用接口配合父类来代替多继承
public class LivingEntity : MonoBehaviour, IDamageable
{
    public float maxHealth;
    [SerializeField] 
    protected float health; //外界不能访问这个变量只有子类可以进行访问
    protected bool isDead;
    //事件的简略声明格式： public/... + event key + Delagate + eventName(on + Foo)
    public event Action onDeath;
    /*
    private void Start()
    {
        health = maxHealth;
    }
    */

    protected virtual void Start()
    {
        health = maxHealth;
    }

    //事件的触发是由事件的拥有者内部逻辑处罚的
    protected void Die()
    {
        isDead = true;
        Destroy(gameObject);
        if(onDeath != null)
        {
            onDeath();
        }
    }

    //当人物或者敌人受伤时，就会扣血，会触发内部逻辑的事件
    //人物：GameOver敌人取消追击
    //敌人：计数生成下一波敌人
    public void TakenDamage(float _damageAmount)
    {
        health -= _damageAmount;
        if(health <= 0 && isDead == false)
        {
            Die();
        }
    }
}
