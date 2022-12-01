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


    public Vector3 getDirByMousePosition()
    {
        float dist;
        Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray2, out dist))
        {
            Vector3 worldPosition = ray2.GetPoint(dist);
            return (new Vector3(worldPosition.x, 0, worldPosition.z) - StartPoint.position);
        }

        return Vector3.zero;
    }


    private void Update()
    {
        dir = getDirByMousePosition().normalized;
        EndPoint.position = StartPoint.position + dir;

        if (Input.GetKeyDown(KeyCode.A))
        {
            candyBag = Instantiate(flipperSO.CandyBagPrefab, StartPoint.position + Vector3.up, Quaternion.identity);
            rb = candyBag.GetComponent<Rigidbody>();
            candyBag.GetComponent<CandyScript>().Init(flipperSO, rb);

            float TotalForce = flipperSO.CandyBagSpeed;
            if (flipperSO.speedByNbCandy) TotalForce -= flipperSO.nbCandy;
            
            if (flipperSO.ScaleBagByNbCandy)
            candyBag.transform.localScale = Vector3.one * flipperSO.nbCandy;
            
            rb.AddForce(dir * TotalForce, ForceMode.Impulse);
            LaunchFlipper = true;
        }
    }
}