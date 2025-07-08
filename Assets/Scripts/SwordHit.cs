using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHit : MonoBehaviour
{
    private SwordAttack swordAttack;
    private SwordInput swordInput;

    void Start()
    {
        swordAttack = GetComponentInParent<SwordAttack>();
        swordInput = GetComponentInParent<SwordInput>();

        if (swordAttack == null) Debug.LogError("❌ SwordAttack 못 찾음");
        if (swordInput == null) Debug.LogWarning("⚠️ SwordInput 못 찾음 (사용 안하면 무시 가능)");
       
    }

    void OnTriggerEnter(Collider other)
    {

        Debug.Log("Something entered trigger: " + other.name);
        if (other.CompareTag("Target"))
        {
            if (swordAttack != null && swordInput != null)
            {
                if (swordAttack.IsSwinging && swordInput.isPressingAttack)
                {
                    Debug.Log("💥 Attack Success!");
                }
            }
        }
    }
}
