using System;
using TMPro;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class XerUlt : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;
    public TMP_Text jaugeText;

    public XerathUltimateSO capacitySo;

    private float Animation;
    private float height = 5.0f;
    private float ReduceSpeed = 5.0f;
    private int nbBounce = 0;
    private bool finish = false;
    private float radiusRandom = 0f;
    private Vector3 dir = Vector3.zero;
    private float hextechDistance = 0.0f;
    private bool isHextech = false;
    private bool PositiveJaugeHextech = true;

    private bool launchXerathUlt = false;

    private Plane plane = new Plane(Vector3.up, 0);
    private float distance = 0.0f;

    public class ParabolaClass
    {
        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }
    }

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


    // Start is called before the first frame update
    void Start()
    {
        dir = (EndPoint.position - StartPoint.position).normalized;
        height = capacitySo.height;
        ReduceSpeed = 10.1f - capacitySo.SpeedOnAir;
        if (capacitySo.speedByNbCandy) ReduceSpeed -= capacitySo.nbCandy;

        nbBounce = capacitySo.nbBounce;
        radiusRandom = capacitySo.RandomizeZoneRadius;
        isHextech = capacitySo.IsHextechFlash;
        hextechDistance = capacitySo.MinDistanceHFlash;
        
        if (capacitySo.RandomizeZone)
            EndPoint.position += (UnityEngine.Random.insideUnitSphere * radiusRandom);
        EndPoint.position = new Vector3(EndPoint.position.x, 0, EndPoint.position.z);
    }

    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            launchXerathUlt = true;

        #region SimpleXerath

        if (!isHextech && launchXerathUlt)
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


            if (capacitySo.ScaleBagByNbCandy)
                transform.localScale = Vector3.one * capacitySo.nbCandy;
            Animation += Time.deltaTime;
            Animation %= ReduceSpeed;
            transform.position =
                ParabolaClass.Parabola(StartPoint.position, EndPoint.position, height, Animation / ReduceSpeed);

        }

        #endregion

        else if (isHextech)
        {
            switch (capacitySo.hextechMode)
            {
                case HextechMode.hold:
                    if (Input.GetKeyDown(KeyCode.A))
                        hextechDistance = capacitySo.MinDistanceHFlash;
                    if (Input.GetKey(KeyCode.A))
                    {
                        if (hextechDistance < capacitySo.MaxDistanceHFlash)
                            hextechDistance += Time.deltaTime * capacitySo.HextechFlashSpeedScale;

                        jaugeText.text = "Jauge : " + hextechDistance.ToString("F2");

                        EndPoint.position = StartPoint.position + getDirByMousePosition().normalized;
                    }

                    if (Input.GetKeyUp(KeyCode.A))
                    {
                        EndPoint.position = StartPoint.position + getDirByMousePosition().normalized * hextechDistance;
                        isHextech = false;
                    }

                    break;

                case HextechMode.jauge:
                    if (PositiveJaugeHextech)
                    {
                        if (hextechDistance < capacitySo.MaxDistanceHFlash)
                            hextechDistance += Time.deltaTime * capacitySo.HextechFlashSpeedScale;
                        else
                            PositiveJaugeHextech = false;
                    }
                    else
                    {
                        if (hextechDistance > capacitySo.MinDistanceHFlash)
                            hextechDistance -= Time.deltaTime * capacitySo.HextechFlashSpeedScale;
                        else
                            PositiveJaugeHextech = true;
                    }

                    EndPoint.position = StartPoint.position + getDirByMousePosition().normalized;

                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        EndPoint.position = StartPoint.position + getDirByMousePosition().normalized * hextechDistance;
                        isHextech = false;
                    }

                    jaugeText.text = "Jauge : " + hextechDistance.ToString("F2");
                    return;

                case HextechMode.mouseDistance:

                    EndPoint.position = StartPoint.position + getDirByMousePosition().normalized;

                    float mouseDist = Vector3.Distance(StartPoint.position, getDirByMousePosition());

                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        if (capacitySo.RatioMouseDistance != 0)
                            mouseDist /= capacitySo.RatioMouseDistance;

                        EndPoint.position = StartPoint.position + getDirByMousePosition().normalized * mouseDist;
                        isHextech = false;
                    }

                    return;
            }
        }
    }
}