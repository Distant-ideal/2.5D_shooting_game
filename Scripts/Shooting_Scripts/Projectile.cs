using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float shootSpeed;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifetime;

    public LayerMask collisionLayerMash;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * shootSpeed * Time.deltaTime);
        CheckCollision();
    }

    private void CheckCollision()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo; //结构体类型：射线击中目标的具体信息
        if(Physics.Raycast(ray, out hitInfo, shootSpeed * Time.deltaTime, collisionLayerMash, QueryTriggerInteraction.Collide))
        {
            HitEnemy(hitInfo);
        }
    }

    private void HitEnemy(RaycastHit _hitInfo)
    {
        IDamageable damageable = _hitInfo.collider.GetComponent<IDamageable>();
        if(damageable != null)
        {
            damageable.TakenDamage(damage);
        }
        Destroy(gameObject); //当子弹击中敌人后，子弹需要被销毁
    }
}
