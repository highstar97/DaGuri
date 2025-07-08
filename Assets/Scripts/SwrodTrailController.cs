using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwrodTrailController : MonoBehaviour
{

    private TrailRenderer trailRenderer;
    private SwordAttack swordAttack;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        swordAttack = GetComponent<SwordAttack>();

        trailRenderer.enabled = false;
    }

    void Update()
    {
        if (swordAttack != null && swordAttack.IsSwinging)
        {
            if (!trailRenderer.enabled)
                trailRenderer.enabled = true;
        }
        else
        {
            if (trailRenderer.enabled)
                trailRenderer.enabled = false;
        }
    }
}
