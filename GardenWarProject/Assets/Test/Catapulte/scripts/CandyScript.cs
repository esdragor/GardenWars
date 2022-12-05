using System;
using System.Collections;
using System.Collections.Generic;
using GameStates;
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
    
    public void Init(FlipperSO flipperSO, Vector3 dir)
    {
        if (flipperSO.StopByRebound)
            nbRebound = flipperSO.MaxRebound;
        if (flipperSO.StopByTimer)
            timer = flipperSO.MaxTimer;
        rb = GetComponent<Rigidbody>();;
        Deceleration = flipperSO.ForceDecelerate;
        DecelerateDuring = flipperSO.decreaseSpeedDuring;
        NoDeceleration = flipperSO.StopBagWithoutDelay;
        
        float TotalForce = flipperSO.CandyBagSpeed;
        if (flipperSO.speedByNbCandy) TotalForce -= flipperSO.nbCandy;
            
        if (flipperSO.ScaleBagByNbCandy) transform.localScale = Vector3.one * flipperSO.nbCandy;
            
        rb.AddForce(dir * TotalForce, ForceMode.Impulse);
        GameStateMachine.Instance.OnTick += MoveCandy;
    }

    public void DecelerateCandy()
    {
        if (NoDeceleration)
        {
            rb.velocity = Vector3.zero;
            GameStateMachine.Instance.OnTick -= MoveCandy;
            return;
        }
        rb.AddForce((rb.velocity * -1).normalized * Deceleration, ForceMode.Force);
        if (rb.velocity.magnitude < 0.1f)
        {
            rb.velocity = Vector3.zero;
            GameStateMachine.Instance.OnTick -= MoveCandy;
        }
    }
    
    private void MoveCandy()
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
