using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatProperty
{
    public float baseValue;

    public System.Action<float> OnValueChanged = (baseValue) => { Debug.Log($"Value Changed : {baseValue}"); };

    public float BaseValue => baseValue;

    public void SetBaseValue(float baseValue)
    {
        this.baseValue = baseValue;
        OnValueChanged.Invoke(this.baseValue);
    }
}

public class StartComponent : MonoBehaviour, ITakeDamaged
{
    public StatProperty attack = new();
    public StatProperty attackSpeed = new();
    public StatProperty criticalRate = new();
    public StatProperty criticalCoefficient = new();
    public StatProperty maxHealth = new();
    public StatProperty currentHealth = new();
    public StatProperty moveVelocity = new();


    public System.Action OnCurrentHealthBeZero = () => { Debug.Log("Character's current health set zero."); };
    // Start is called before the first frame update
    void Start()
    {
        InitStatProperty();
    }
    private void InitStatProperty()
    {
        attack.SetBaseValue(100.0f);
        attackSpeed.SetBaseValue(1.0f);
        criticalRate.SetBaseValue(0.0f);
        criticalCoefficient.SetBaseValue(2.0f);
        maxHealth.SetBaseValue(100.0f);
        currentHealth.SetBaseValue(maxHealth.BaseValue);
        moveVelocity.SetBaseValue(2.0f);
    }

    public void TakeDamage(float damageAmount)
    {
        float remainingCurrentHealth = currentHealth.BaseValue - damageAmount;

        if (remainingCurrentHealth < 0.0f || Mathf.Approximately(remainingCurrentHealth, 0.0f))
        {
            currentHealth.SetBaseValue(0.0f);
            OnCurrentHealthBeZero.Invoke();
            return;
        }

        currentHealth.SetBaseValue(remainingCurrentHealth);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
