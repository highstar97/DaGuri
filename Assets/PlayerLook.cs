using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviourPun
{
    [Header("XR Rig Root")]
    [SerializeField] private Transform playerRoot; // 회전시킬 오브젝트 (보통 XR Origin 또는 Player 프리팹 루트)

    [Header("Turn Settings")]
    [SerializeField] private float turnSpeed = 60f; // 도/초

    private float turnInput;

    public void OnTurn(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;

        Vector2 turnAxis = context.ReadValue<Vector2>();
        turnInput = turnAxis.x; // 좌우 입력 (왼쪽: -1, 오른쪽: 1)
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        if (Mathf.Abs(turnInput) > 0.1f)
        {
            playerRoot.Rotate(Vector3.up, turnInput * turnSpeed * Time.deltaTime);
        }
    }
}
