using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    void Update()
    {
        Debug.Log($"[카메라 위치] {transform.position} / [회전] {transform.rotation.eulerAngles}");
    }
}
