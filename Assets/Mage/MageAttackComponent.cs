using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UIElements;

public class MageAttackComponent : MonoBehaviour
{
    #region Variables
    public Transform magicOffset;                   // 마법 시작지점

    public ProjectileSpawner projectileSpawner;     // 파이어볼 스포너

    public ControllerTracking controllerTracking;   // 컨트롤러 트래킹
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

        if (GestureUtils.IsCircleGesture(trail))
        {
            Debug.Log("원 제스처 인식 → 원형 마법 발동");
            projectileSpawner.SpawnProjectile("FireBall", magicOffset.position, this.transform.forward, this.gameObject);
        }
        else if (GestureUtils.IsStabDownGesture(trail))
        {
            Debug.Log("찌르기 제스처 인식 → 낙뢰 발동");
            projectileSpawner.SpawnProjectile("Thunder", magicOffset.position, this.transform.forward, this.gameObject);
        }
    }
    #endregion
}