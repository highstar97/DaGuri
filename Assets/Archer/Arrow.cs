using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float arrowSpeed = 8f; //화살 속도
    public StatComponent ownerStat;

    public System.Action<Arrow> OnArrowCollapsed;
    void Update()
    {
        this.transform.position += this.transform.forward * arrowSpeed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss") == false) return;
        ITakeDamaged victim = other.GetComponent<ITakeDamaged>();
        if(victim == null) return;

        victim.TakeDamage(ownerStat.attack.baseValue);
        OnArrowCollapsed.Invoke(this);
    }
}
