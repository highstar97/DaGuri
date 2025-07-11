using Photon.Pun;
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

public class StatComponent : MonoBehaviour, IPunObservable, ITakeDamageable
{
    #region Variables
    public StatProperty attack = new();
    public StatProperty attackSpeed = new();
    public StatProperty criticalRate = new();
    public StatProperty criticalCoefficient = new();
    public StatProperty maxHealth = new();
    public StatProperty currentHealth = new();
    public StatProperty moveVelocity = new();

    public System.Action OnCurrentHealthBeZero = () => { Debug.Log("Character's current health set zero."); GameEndManager.Instance.NotifyAdventureDied(); };

    public HealthBarUI healthBarUI;        //체력바 UI
    #endregion

    #region Unity Functions
    private void Start()
    {
        currentHealth.OnValueChanged += UpdateCurrentHealthUI;
        maxHealth.OnValueChanged += UpdateMaxHealthUI;

        InitStatProperty();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
        }
        else
        {
            this.currentHealth.SetBaseValue((float)stream.ReceiveNext());
        }
    }
    #endregion

    #region User Functions
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
            if(this.gameObject.CompareTag("Boss"))
            {
                GameEndManager.Instance.NotifyBossDied();
            }
            return;
        }

        currentHealth.SetBaseValue(remainingCurrentHealth);
    }

    [PunRPC]
    public void RequestTakeDamage(float damageAmount)
    {
        
        if (PhotonNetwork.IsMasterClient)
        {
            // 이 TakeDamage 메서드는 체력 감소 및 0 처리 로직을 가지고 있습니다.
            TakeDamage(damageAmount);

      
            // if (currentHealth.BaseValue <= 0 && photonView.IsMine)
            // {
            //     PhotonNetwork.Destroy(this.gameObject);
            // }
        }
    }


    private void UpdateHealthUI()
    {
        healthBarUI.UpdateHealth(currentHealth.BaseValue, maxHealth.BaseValue);
    }

    private void UpdateCurrentHealthUI(float currentHealthAmount)
    {
        healthBarUI.UpdateHealth(currentHealthAmount, maxHealth.BaseValue);
    }

    private void UpdateMaxHealthUI(float maxHealthAmout)
    {
        currentHealth.SetBaseValue(Mathf.Min(currentHealth.BaseValue, maxHealthAmout));
        healthBarUI.UpdateHealth(currentHealth.BaseValue, maxHealthAmout);
    }
    #endregion
}