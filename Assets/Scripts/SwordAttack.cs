using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public float swingThreshold = 2.5f;
    public float swingDurationRequired = 0.1f;

    public bool IsSwinging { get; private set; } = false;

    private Vector3 prevPos;

    void Start()
    {
        prevPos = transform.position;
    }

    void Update()
    {
        Vector3 velocity = (transform.position - prevPos) / Time.deltaTime;
        prevPos = transform.position;

        IsSwinging = velocity.magnitude > swingThreshold;

        if (IsSwinging) {// Debug.Log("🔺 Swinging! (속도: " + velocity.magnitude.ToString("F2") + ")");
        }


       
        
       
    }
}
