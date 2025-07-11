using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordInput : MonoBehaviour
{
    public event System.Action OnPressingFinished;
    public Transform rightControllerTransform;
 
    private bool isPressingAttack = false;
    private Vector3 startPosition;
    private Vector3 endPosition;

    public Vector3 StartPosition => startPosition;
    public Vector3 EndPosition => endPosition;

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isPressingAttack = true;
            startPosition = rightControllerTransform.position;
        }
        else if (context.canceled)
        {
            isPressingAttack = false;
            endPosition = rightControllerTransform.position;
            OnPressingFinished.Invoke();
        }
    }
}