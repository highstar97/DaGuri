//#define LOCAL_TEST
#define PHOTON

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
public class HandAnimation : MonoBehaviourPun
{
    [Header("Animator Component")]
    [SerializeField] private Animator handAnimator;

    [Header("Input Actions")]
    public InputActionReference triggerInput;
    public InputActionReference gripInput;

    private float gripValue = 0f;
    private float triggerValue = 0f;

    private void Update()
    {
#if LOCAL_TEST
        bool isLocalPlayer = true;
#elif PHOTON
        bool isLocalPlayer = photonView.IsMine;
#endif

        if (handAnimator != null)
        {
            handAnimator.SetFloat("Trigger", triggerValue);
            handAnimator.SetFloat("Grip", gripValue);
        }

        if (!isLocalPlayer) return;

        if (triggerInput != null && triggerInput.action != null && triggerInput.action.enabled)
        {
            triggerValue = triggerInput.action.ReadValue<float>();
        }

        if (gripInput != null && gripInput.action != null && gripInput.action.enabled)
        {
            gripValue = gripInput.action.ReadValue<float>();
        }
    }
}
