using Entities.Capacities;
using UnityEngine;

public class Flipper : ActiveCapacity
{

    private Transform HelperDirection;
    
    private FlipperSO flipperSO => (FlipperSO)AssociatedActiveCapacitySO();

    private bool LaunchFlipper = false;
    private GameObject candyBag = null;
    private Ray ray;
    private Vector3 worldPosition = Vector3.zero;
    private Plane plane = new Plane(Vector3.up, 0);
    private float distance = 0.0f;
    private Vector3 dir;
    private Rigidbody rb;

    protected override void Press(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        
    }

    protected override void PressFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        
    }

    protected override void Hold(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        
    }

    protected override void HoldFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (!HelperDirection) return;
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(mouseRay, out var hit)) return;
        
        hit.point = (new Vector3(hit.point.x, 0, hit.point.z));

        dir = (hit.point - caster.transform.position).normalized;
        HelperDirection.position = caster.transform.position + dir;
    }

    protected override void Release(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(mouseRay, out var hit)) return;
        
        hit.point = (new Vector3(hit.point.x, 0, hit.point.z));

        dir = (hit.point - caster.transform.position).normalized;
        
        candyBag = GameObject.Instantiate(flipperSO.CandyBagPrefab, caster.transform.position + dir + Vector3.up, Quaternion.identity);
        candyBag.GetComponent<CandyScript>().Init(flipperSO, dir);
    }

    protected override void ReleaseFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        
    }
}
