using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField]
    public Vector2 MoveSpeed { get; private set; } = new Vector2(3, 5);

    Vector3 m_inputDir;
    Rigidbody m_rb;
    Animator m_anim;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_anim = GetComponentInChildren<Animator>();
    }
    public void Update()
    {
        var x = Input.GetAxisRaw("Horizontal");
        var z = Input.GetAxisRaw("Vertical");

        m_inputDir.x = x;
        m_inputDir.z = z;
        m_inputDir.Normalize();

        m_anim.SetFloat("MoveX",m_rb.velocity.x);
        m_anim.SetFloat("MoveZ", m_rb.velocity.z);
    }

    private void FixedUpdate()
    {
        m_rb.velocity = m_inputDir * MoveSpeed.x;
    }
}
