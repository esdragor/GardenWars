using Entities;
using Entities.Champion;
using UnityEngine;

public class CandyFollow : MonoBehaviour
{
    private Entity target;
    private int candyCount = 0;
    private float duration = 0;
    private float timer = 0;
    private bool master;
    [SerializeField] private GameObject trail;

    public void SetTarget(Entity target, float duration,int candyCount, bool master)
    {
        gameObject.SetActive(true);
        this.target = target;
        this.duration = duration;
        timer = duration;
        this.candyCount = candyCount;
        this.master = master;
        trail.SetActive(!master);
    }
    
    private void Update()
    {
        if(duration <= 0) return;

        timer -= Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, target.position, 1 - (timer / duration));
        
        if(timer > 0 && Vector3.Distance(transform.position,target.position) > 0.2f) return;
        
        if(target is Champion champion && master) champion.IncreaseCurrentCandyRPC(candyCount);
        
        gameObject.SetActive(false);
    }


    private void OnDisable()
    {
        target = null;
        duration = 0;
        timer = 0;
        candyCount = 0;
        master = false;
        trail.SetActive(false);
    }
}
