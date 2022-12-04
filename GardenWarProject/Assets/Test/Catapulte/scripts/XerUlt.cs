using System;
using TMPro;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class XerUlt : MonoBehaviour
{
    public Transform Caster;
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
    private Vector3 StartPoint;
    public Vector3 EndPoint;

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
            return (new Vector3(worldPosition.x, 0, worldPosition.z) - StartPoint);
        }

        return Vector3.zero;
    }


    // Start is called before the first frame update
    void Start()
    {
        StartPoint = Caster.position;
        dir = (EndPoint - StartPoint).normalized;
        height = capacitySo.height;
        ReduceSpeed = 10.1f - capacitySo.SpeedOnAir;
        if (capacitySo.speedByNbCandy) ReduceSpeed -= capacitySo.nbCandy;
        nbBounce = capacitySo.nbBounce;
        radiusRandom = capacitySo.RandomizeZoneRadius;
        isHextech = capacitySo.IsHextechFlash;
        hextechDistance = capacitySo.MinDistanceHFlash;
        if (capacitySo.ScaleBagByNbCandy)
            transform.localScale = Vector3.one * capacitySo.nbCandy;
    }

    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartPoint = Caster.position;
            
            EndPoint = StartPoint + getDirByMousePosition().normalized * hextechDistance;
            dir = (EndPoint - StartPoint).normalized;
            if (capacitySo.RandomizeZone)
                EndPoint += (UnityEngine.Random.insideUnitSphere * radiusRandom);
            EndPoint = new Vector3(EndPoint.x, 0, EndPoint.z);
            launchXerathUlt = true;
        }

        #region SimpleXerath

        if (!isHextech && launchXerathUlt)
        {
            if (finish) return;
            if (Vector3.Distance(gameObject.transform.position, EndPoint) < 0.3f)
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
                        StartPoint = gameObject.transform.position;
                        EndPoint = new Vector3(EndPoint.x + nbBounce * dir.x, EndPoint.y,
                            EndPoint.z + nbBounce * dir.z);
                        if (capacitySo.RandomizeZone)
                            EndPoint += (UnityEngine.Random.insideUnitSphere * radiusRandom);
                        EndPoint = new Vector3(EndPoint.x, 0, EndPoint.z);
                        nbBounce--;
                        ReduceSpeed *= 1.1f;
                    }
                }

                return;
            }

            Animation += Time.deltaTime;
            Animation %= ReduceSpeed;
            transform.position =
                ParabolaClass.Parabola(StartPoint, EndPoint, height, Animation / ReduceSpeed);
        }

        #endregion

         else if (isHextech)
         {
             switch (capacitySo.hextechMode)
             {
                 case HextechMode.hold:
                     if (Input.GetKeyDown(KeyCode.C))
                         hextechDistance = capacitySo.MinDistanceHFlash;
                     if (Input.GetKey(KeyCode.C))
                     {
                         if (hextechDistance < capacitySo.MaxDistanceHFlash)
                             hextechDistance += Time.deltaTime * capacitySo.HextechFlashSpeedScale;
        
                         jaugeText.text = "Jauge : " + hextechDistance.ToString("F2");
        
                         EndPoint = StartPoint + getDirByMousePosition().normalized;
                     }
        
                     if (Input.GetKeyUp(KeyCode.C))
                     {
                         EndPoint = StartPoint + getDirByMousePosition().normalized * hextechDistance;
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
        
                     EndPoint = StartPoint + getDirByMousePosition().normalized;
        
                     if (Input.GetKeyDown(KeyCode.C))
                     {
                         EndPoint = StartPoint + getDirByMousePosition().normalized * hextechDistance;
                         isHextech = false;
                     }
        
                     jaugeText.text = "Jauge : " + hextechDistance.ToString("F2");
                     return;
        
                 case HextechMode.mouseDistance:
        
                     EndPoint = StartPoint + getDirByMousePosition().normalized;
        
                     float mouseDist = Vector3.Distance(StartPoint, getDirByMousePosition());
        
                     if (Input.GetKeyDown(KeyCode.C))
                     {
                         if (capacitySo.RatioMouseDistance != 0)
                             mouseDist /= capacitySo.RatioMouseDistance;
        
                         EndPoint = StartPoint + getDirByMousePosition().normalized * mouseDist;
                         isHextech = false;
                     }
        
                     return;
             }
        }
    }
}