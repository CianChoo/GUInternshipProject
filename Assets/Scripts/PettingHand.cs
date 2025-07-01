using System;
using UnityEngine;

public class PettingHand : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Reset()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }
}
