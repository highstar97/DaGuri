using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimation : MonoBehaviour
{
    [Header("Animator Component")]
    [SerializeField] private Animator handAnimator;

    [Header("Input Actions")]
    public InputActionReference triggerInput;
    public InputActionReference gripInput;

    private void Update()
    {
        float triggerValue = 0f;
        if (triggerInput != null && triggerInput.action != null && triggerInput.action.enabled)
        {
            triggerValue = triggerInput.action.ReadValue<float>();
        }

        float gripValue = 0f;
        if (gripInput != null && gripInput.action != null && gripInput.action.enabled)
        {
            gripValue = gripInput.action.ReadValue<float>();
        }

        if (handAnimator != null)
        {
            handAnimator.SetFloat("Trigger", triggerValue);
            handAnimator.SetFloat("Grip", gripValue);
        }
    }
}
