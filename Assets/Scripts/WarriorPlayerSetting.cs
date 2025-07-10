using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class WarriorPlayerSetting : MonoBehaviourPunCallbacks
{
    [Header("XR Origin")]
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private XROrigin xrOrigin;

    [Header("Main Camera")]
    [SerializeField] private Camera camera; // XR Origin의 Main Camera (Head)
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private TrackedPoseDriver trackedPoseDriver;
    [SerializeField] private UniversalAdditionalCameraData universalAdditionalCameraData;

    [Header("Right Controller")]
    [SerializeField] private TrackedPoseDriver RightControllertrackedPoseDriver;

    private void Start()
    {
        // 로컬 플레이어: 모든 XR 관련 컴포넌트 활성화
        if (photonView.IsMine)
        {
            // XR Origin
            capsuleCollider.enabled = true;
            rigidBody.isKinematic = false;
            xrOrigin.enabled = true;

            // 카메라 활성화 (다른 플레이어의 카메라는 비활성화)
            camera.enabled = true;
            audioListener.enabled = true;
            trackedPoseDriver.enabled = true;
            universalAdditionalCameraData.enabled = true;

            // RightController
            RightControllertrackedPoseDriver.enabled = true;
        }
    }
}