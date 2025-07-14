using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITakeDamageable
{
    public void BroadcastTakeDamage(float damageAmount, GameObject instigator);

    public void TakeDamage(float damageAmount);
}