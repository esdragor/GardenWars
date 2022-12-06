using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_false_FX : MonoBehaviour
{
    IEnumerator HideFalseFX()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        StartCoroutine(HideFalseFX());
    }
}
