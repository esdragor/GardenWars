using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyScript : MonoBehaviour
{
    private int nbRebound = -1;
    private Rigidbody rb;
    private float Deceleration = 0.1f;
    private float timer = -1f;
    private bool Decelerate = false;
    private bool DecelerateDuring = false;
    private bool NoDeceleration = false;
    
    public void Init(FlipperSO flipperSO, Rigidbody _rb)
    {
        if (flipperSO.StopByRebound)
            nbRebound = flipperSO.MaxRebound;
        if (flipperSO.StopByTimer)
            timer = flipperSO.MaxTimer;
        rb = _rb;
        Deceleration = flipperSO.ForceDecelerate;
        DecelerateDuring = flipperSO.decreaseSpeedDuring;
        NoDeceleration = flipperSO.StopBagWithoutDelay;
    }

    public void DecelerateCandy()
    {
        if (NoDeceleration)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        rb.AddForce((rb.velocity * -1).normalized * Deceleration, ForceMode.Force);
    }
    
    private void Update()
    {
        if (Decelerate && rb.velocity != Vector3.zero)      
            DecelerateCandy();
        else if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                Decelerate = true;
            }
        }
        if (DecelerateDuring)
            rb.AddForce((rb.velocity * -1).normalized * Deceleration, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (--nbRebound == -1)
            Decelerate = true;
        Debug.Log("Collision");
    }
}
