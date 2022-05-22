using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    enum BarrierType { Normal, Rollable }

    [SerializeField] private BarrierType barrierType = BarrierType.Normal;
    [SerializeField] private float rollSpeed = 1.0f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (barrierType == BarrierType.Rollable)
        {
            StartCoroutine(RollBarrier());
        }
    }

    IEnumerator RollBarrier()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            rb.AddTorque(Vector3.left * rollSpeed, ForceMode.Impulse);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
