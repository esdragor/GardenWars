using Entities;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BushCollider : MonoBehaviour
{
    private Bush bush;
    
    public void LinkWithBush(Bush linkedBush)
    {
        bush = linkedBush;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var hitEntity = other.gameObject.GetComponent<Entity>();
        if (hitEntity == null) return;
        bush.EntityEnter(hitEntity);
    }
    
    private void OnTriggerExit(Collider other)
    {
        var hitEntity = other.gameObject.GetComponent<Entity>();
        if (hitEntity == null) return;
        bush.EntityExit(hitEntity);
    }
}
