using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ParabolaLaunch : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;

    public XerathUltimateSO capacitySo;

    private float Animation;
    private float height = 5.0f;
    private float ReduceSpeed = 5.0f;
    private int nbBounce = 0;
    private bool finish = false;
    private float radiusRandom = 0f;
    private Vector3 dir = Vector3.zero;

    public class ParabolaClass
    {
        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        dir = (EndPoint.position - StartPoint.position).normalized;
        height = capacitySo.height;
        ReduceSpeed = capacitySo.ReduceSpeed;
        nbBounce = capacitySo.nbBounce;
        radiusRandom = capacitySo.RandomizeZoneRadius;
        if (capacitySo.RandomizeZone)
            EndPoint.position += (UnityEngine.Random.insideUnitSphere * radiusRandom);
        EndPoint.position = new Vector3(EndPoint.position.x, 0, EndPoint.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (finish) return;
        if (Vector3.Distance(gameObject.transform.position, EndPoint.position) < 0.3f)
        {
            if (finish == false)
            {
                finish = true;
                if (nbBounce > 0)
                {
                    finish = false;
                    Animation = 0f;
                    height /= 1.5f;
                    radiusRandom /= 2f;
                    StartPoint.position = gameObject.transform.position;
                    EndPoint.position = new Vector3(EndPoint.position.x + nbBounce * dir.x, EndPoint.position.y,
                        EndPoint.position.z + nbBounce * dir.z);
                    if (capacitySo.RandomizeZone)
                        EndPoint.position += (UnityEngine.Random.insideUnitSphere * radiusRandom);
                    EndPoint.position = new Vector3(EndPoint.position.x, 0, EndPoint.position.z);
                    nbBounce--;
                    ReduceSpeed *= 1.1f;
                }
            }

            return;
        }


        Animation += Time.deltaTime;

        Animation = Animation % ReduceSpeed;

        transform.position =
            ParabolaClass.Parabola(StartPoint.position, EndPoint.position, height, Animation / ReduceSpeed);
    }
}