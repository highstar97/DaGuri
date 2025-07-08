using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArcherAttackComponent : MonoBehaviour
{
    public Transform ArrowOffset; //화살 발사 지점
    public Bow bow;

    public ArrowSpawner arrowSpawner; //화살 스포너
    public ControllerTracking controllerTracking; //컨트롤러 트래킹

    public InputActionProperty aBtnAction; //a버튼 액션 등록

  

 
    private void Start()
    {
        if (controllerTracking != null)
        {
            controllerTracking.OnTrackingFinished += CheckGesture;
        }
        else
        {
            this.enabled = false;
        }
    }

    private void CheckGesture()
    {
        var trail = controllerTracking.Positions;

        if (GestureUtils.IsLineGesture(trail))
        {
            arrowSpawner.SpawnArrow(ArrowOffset.position, this.transform.forward, this.gameObject);
        }
    }
    


}