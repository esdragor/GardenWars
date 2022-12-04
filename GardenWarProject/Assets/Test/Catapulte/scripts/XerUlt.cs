using System;
using TMPro;
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
}