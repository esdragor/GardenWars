using Entities.Capacities;
using GameStates;
using UnityEngine;
using Object = UnityEngine.Object;

public class XerathUltimate : ActiveCapacity
{
    private XerathUltimateSO activeCapa => (XerathUltimateSO)AssociatedActiveCapacitySO();
    private Vector3 GoalPosition;

    private GameObject candyBag = null;
    private GameObject HelperDirection = null;
    private UIJauge UIJauge = null;
    private double Animation = 0f;
    private Plane plane = new Plane(Vector3.up, 0);

    private bool IsHextech = false;
    private float time_Pressed = 0f;
    private double hextechDistance;
    private bool PositiveJaugeHextech = true;

    public void Init()
    {
        candyBag = Object.Instantiate(activeCapa.prefab, caster.transform.position + Vector3.up * 1,
            Quaternion.identity);
    }


    public Vector3 getDirByMousePosition()
    {
        float dist;
        Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray2, out dist))
        {
            Vector3 worldPosition = ray2.GetPoint(dist);
            return (new Vector3(worldPosition.x, 0, worldPosition.z) - casterPos);
        }

        return Vector3.zero;
    }

    public void Jauge()
    {
        if (PositiveJaugeHextech)
        {
            if (hextechDistance < activeCapa.MaxDistanceHFlash)
                hextechDistance += activeCapa.HextechFlashSpeedScale;
            else
                PositiveJaugeHextech = false;
        }
        else
        {
            if (hextechDistance > activeCapa.MinDistanceHFlash)
                hextechDistance -= activeCapa.HextechFlashSpeedScale;
            else
                PositiveJaugeHextech = true;
        }
    }

    protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
    {
        return true;
    }

    protected override void Press(int targetsEntityIndexes, Vector3 targetPositions)
    {
        time_Pressed = Time.time;
        hextechDistance = activeCapa.MinDistanceHFlash;
        if (activeCapa.hextechMode == HextechMode.jauge)
        {
            PositiveJaugeHextech = true;
            
            GameStateMachine.Instance.OnUpdate += Jauge;
        }
    }

    protected override void PressFeedback(int targetsEntityIndexes, Vector3 targetPositions)
    {
        if (HelperDirection) HelperDirection.SetActive(true);
        else
            HelperDirection = GameObject.CreatePrimitive(PrimitiveType.Cube);
        if (activeCapa.hextechMode == HextechMode.mouseDistance) return;
        if (UIJauge) UIJauge.gameObject.SetActive(true);
        else
            UIJauge = GameObject.Instantiate(activeCapa.prefabJauge).GetComponent<UIJauge>();
    }

    protected override void Hold(int targetsEntityIndexes, Vector3 targetPositions)
    {
    }

    protected override void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions)
    {
        if (!HelperDirection) return;
        if (activeCapa.hextechMode != HextechMode.mouseDistance)
        {
            HelperDirection.transform.position = casterPos + getDirByMousePosition().normalized + Vector3.up;
            if (!UIJauge) return;
            if (activeCapa.hextechMode == HextechMode.jauge)
                UIJauge.UpdateJaugeSlider(activeCapa.MinDistanceHFlash, activeCapa.MaxDistanceHFlash,  hextechDistance);
            else
                UIJauge.UpdateJaugeSlider(activeCapa.MinDistanceHFlash, activeCapa.MaxDistanceHFlash, hextechDistance + (Time.time - time_Pressed ) * activeCapa.HextechFlashSpeedScale );
        }
        else
            HelperDirection.transform.position = casterPos + getDirByMousePosition() + Vector3.up;
    }

    protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
    {
        Init();
        time_Pressed =  (Time.time - time_Pressed ) * activeCapa.HextechFlashSpeedScale;
        switch (activeCapa.hextechMode)
        {
            case HextechMode.hold:
                hextechDistance += time_Pressed;
                if (hextechDistance > activeCapa.MaxDistanceHFlash)
                    hextechDistance = activeCapa.MaxDistanceHFlash;
                GoalPosition =
                    GetClosestValidPoint(casterPos + getDirByMousePosition().normalized * (float)hextechDistance);
                break;

            case HextechMode.jauge:
                GameStateMachine.Instance.OnUpdate -= Jauge;
                GoalPosition =
                    GetClosestValidPoint(casterPos + getDirByMousePosition().normalized * (float)hextechDistance);
                break;

            case HextechMode.mouseDistance:
                float mouseDist = Vector3.Distance(casterPos, getDirByMousePosition());

                if (activeCapa.RatioMouseDistance != 0f)
                    mouseDist /= activeCapa.RatioMouseDistance;

                GoalPosition = GetClosestValidPoint(casterPos + getDirByMousePosition().normalized * mouseDist);
                break;
        }

        GoalPosition.y = 1;
        candyBag.GetComponent<CandyBagXerath>().Init(caster, activeCapa, GoalPosition, hextechDistance);
        if (UIJauge) UIJauge.gameObject.SetActive(false);
    }

    protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
    {
        if (HelperDirection) HelperDirection.SetActive(false);
        if (UIJauge) UIJauge.gameObject.SetActive(false);
    }
}