using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint; // 손끝 위치

    public void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        rb.velocity = firePoint.forward * 20f;

  
    }
}
