using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    #region Variables
    public float moveSpeed = 3f;

    public StatComponent ownerStat;

    public System.Action<FireBall> OnFireBallCollapsed;
    #endregion

    #region Unity Functions
    private void Update()
    {
        this.transform.position += this.transform.forward * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss") == false) return;

        ITakeDamageable victim = other.GetComponent<ITakeDamageable>();
        if (victim == null) return;

        victim.TakeDamage(ownerStat.attack.BaseValue);
        OnFireBallCollapsed.Invoke(this);
    }
    #endregion
}