using Entities.Champion;
using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    [SerializeField] private Champion champion;
    [SerializeField] private float range;
    [SerializeField] private float scaleFactor;

    private void Update()
    {
        range = champion.attackRange;

        transform.localScale = Vector3.one * (scaleFactor * range);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,champion.attackRange);
    }
}
