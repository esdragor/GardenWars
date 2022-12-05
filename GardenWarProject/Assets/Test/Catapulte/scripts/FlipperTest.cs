using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipperTest : MonoBehaviour
{
    public FlipperSO flipperSO;
    public Transform StartPoint;
    public Transform EndPoint;

    private bool LaunchFlipper = false;
    private GameObject candyBag = null;
    private Ray ray;
    private Vector3 worldPosition = Vector3.zero;
    private Plane plane = new Plane(Vector3.up, 0);
    private float distance = 0.0f;
    private Vector3 dir;
    private Rigidbody rb;


    private void Update()
    {
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(mouseRay, out var hit)) return;
        
        hit.point = (new Vector3(hit.point.x, 0, hit.point.z));

        dir = (hit.point - StartPoint.position).normalized;
        
        EndPoint.position = StartPoint.position + dir;

        if (Input.GetKeyDown(KeyCode.C))
        {
            candyBag = Instantiate(flipperSO.CandyBagPrefab, EndPoint.position + Vector3.up, Quaternion.identity);
            rb = candyBag.GetComponent<Rigidbody>();
            //candyBag.GetComponent<CandyScript>().Init(flipperSO, rb);
            
        }
    }
}