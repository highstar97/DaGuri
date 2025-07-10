using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    private Slider healthBarSlider;     //체력바 UI 받아오기
    private Transform cameraTransform;  //카메라 위치 받기

    private void Awake()
    {
        healthBarSlider = GetComponentInChildren<Slider>();
    }

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }
        
    void Update()
    {
        transform.LookAt(cameraTransform);
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthBarSlider.value = currentHealth / maxHealth;
    }
}