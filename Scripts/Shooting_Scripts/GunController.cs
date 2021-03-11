using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform firePoint;
    public GameObject projectilePrefab;
    [SerializeField] private float fireRate = 0.5f;

    private float timer;

    private void Update()
    {
        if(Input.GetMouseButton(0)) //一直按住鼠標
        {
            Shot();
        }
    }

    public void Shot()
    {
        timer += Time.deltaTime;
        if(timer > fireRate)
        {
            timer = 0;
            GameObject spawnProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }
}
