using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class ArcherAttackComponent : MonoBehaviour
{
    #region Variables
    public ControllerTracking controllerTracking; //궤적을 추적하는 스크립트 할당
    #endregion

    #region Unity Functions
    private void Start()
    {
        controllerTracking.OnTrackingFinished += CheckGesture;
    }
    #endregion

    #region User Functions
    private void CheckGesture()
    {
        var trail = controllerTracking.Positions;

        if (GestureUtils.IsLineGesture(trail))
            Debug.Log("직선 운동시 - 화살공격진행");
    }
    #endregion
}