using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyScript : MonoBehaviour
{
    private int nbRebound = -1;
    private Rigidbody rb;
    FlipperSO SO;
    
    public void Init(FlipperSO flipperSO, Rigidbody _rb)
    {
        if (flipperSO.StopByRebound)
            nbRebound = flipperSO.MaxRebound;
        rb = _rb;
        SO = flipperSO;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (--nbRebound == -1)
            rb.AddForce((rb.velocity * -1).normalized * SO.ForceDecelerate, ForceMode.Force);
        Debug.Log("Collision");
    }
}
