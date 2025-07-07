using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordInput : MonoBehaviour
{
   public bool isPressingAttack = false;

    public void OnAttack(InputAction.CallbackContext context)
    {

        Debug.Log("🗡️ Attack 입력 감지됨!");
        if (context.performed)
            isPressingAttack = true;
        else if (context.canceled)
            isPressingAttack = false;
    }

   
}
