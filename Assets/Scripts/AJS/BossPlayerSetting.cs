using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class BossPlayerSetting : MonoBehaviourPunCallbacks
{
    [Header("XR Origin")]
    [SerializeField] private XROrigin xrOrigin;
    [SerializeField] private XRInputModalityManager xRInputModalityManager;

    [Header("Main Camera")]
    [SerializeField] private Camera camera; // XR Origin의 Main Camera (Head)
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private TrackedPoseDriver trackedPoseDriver;
    [SerializeField] private UniversalAdditionalCameraData universalAdditionalCameraData;

    [Header("Left Controller")]
    // XR Interaction Toolkit의 Input Action Manager, Locomotion System 등
    [SerializeField] private ActionBasedControllerManager leftActionBasedControllerManager;
    [SerializeField] private ActionBasedController leftXRController;
    [SerializeField] private XRInteractionGroup leftXRInteractionGroup;

    [Header("In Childeren")]
    [SerializeField] private GameObject leftHandPresence;
    [SerializeField] private GameObject leftDirectInteractor;

    [Header("Left Hand Physics")]
    [SerializeField] private BossHandPhysics leftBossHandPhysics;
    [SerializeField] private Rigidbody leftRgidbody;

    [Header("Right Controller")]
    [SerializeField] private ActionBasedControllerManager rightActionBasedControllerManager;
    [SerializeField] private ActionBasedController rightXRController;
    [SerializeField] private XRInteractionGroup rightXRInteractionGroup;

    [Header("In Childeren")]
    [SerializeField] private GameObject rightHandPresence;
    [SerializeField] private GameObject rightDirectInteractor;

    [Header("Right Hand Physics")]
    [SerializeField] private BossHandPhysics rightBossHandPhysics;
    [SerializeField] private Rigidbody rightrRgidbody;

    [Header("Controller Stabilized")]
    [SerializeField] private GameObject leftStabilized;
    [SerializeField] private GameObject rightStabilized;

    [Header("Locomotion Sytem")]
    [SerializeField] private GameObject locomotionSytem;

    void Awake()
    {
        // 로컬 플레이어: 모든 XR 관련 컴포넌트 활성화
        if (photonView.IsMine)
        {
            // XR Origin
            xrOrigin.enabled = true;
            xRInputModalityManager.enabled = true;

            // 카메라 활성화 (다른 플레이어의 카메라는 비활성화)
            camera.enabled = true;
            audioListener.enabled = true;
            trackedPoseDriver.enabled = true;
            universalAdditionalCameraData.enabled = true;

            // LeftController
            leftActionBasedControllerManager.enabled = true;
            leftXRController.enabled = true;
            leftXRInteractionGroup.enabled = true;

            leftHandPresence.SetActive(true);
            leftDirectInteractor.SetActive(true);

            // Hack : 동기화 테스트
            leftBossHandPhysics.enabled = true;
            leftRgidbody.isKinematic = false; // 다른 서버에서는 위치로만 동기화

            // RightController
            rightActionBasedControllerManager.enabled = true;
            rightXRController.enabled = true;
            rightXRInteractionGroup.enabled = true;

            // Hack : 동기화 테스트
            rightBossHandPhysics.enabled = true;
            rightrRgidbody.isKinematic = false; // 다른 서버에서는 위치로만 동기화

            rightHandPresence.SetActive(true);
            rightDirectInteractor.SetActive(true);

            leftStabilized.SetActive(true);
            rightStabilized.SetActive(true);

            locomotionSytem.SetActive(true);
        }
        else
        {

        }
    }
}
