using System;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities
{
    using FogOfWar;
    using Inventory;
    
    public class Pinata : Entity, IDeadable
{
    [Header("Visual")]
    [SerializeField] private Transform FalseFX;
    [SerializeField] private GameObject Mesh;
    [SerializeField] private GameObject PinataDieFX;
    [SerializeField] private ParticleSystem HitFX;
    [SerializeField] private Animator[] Myanimators;

    [Header("Fill")]
    [SerializeField] private int maxBonbon = 50;
    private int currentBonbon;
    
    private bool isAlive = true;
    private bool canDie = true;


    protected override void OnStart()
    {
        Item item = AssignRandomItem();
        Mesh.GetComponent<Renderer>().material.color = item.AssociatedItemSO().itemColor;
        RequestAddItem(item.indexOfSOInCollection);
        OnInstantiated();
        animators = Myanimators;
    }

    public override void OnInstantiated()
    {
        isAlive = true;
        
        UIManager.Instance.InstantiateHealthBarForEntity(this);
    }
    
    protected override void OnUpdate()
    {
        
    }

    private Item AssignRandomItem()
    {
        ItemCollectionManager im = ItemCollectionManager.Instance;
        int randomItem = Random.Range(0, im.allItemSOs.Count);

        return im.CreateItem((byte)randomItem, this);
    }

    private void DropItem()
    {
        
    }
    
    public bool IsAlive()
    {
        return isAlive;
    }

    public bool CanDie()
    {
        return canDie;
    }

    public void RequestSetCanDie(bool value)
    {
        photonView.RPC("SetCanDieRPC",RpcTarget.MasterClient,value);
    }

    public void SyncSetCanDieRPC(bool value)
    {
        canDie = value;
    }

    public void SetCanDieRPC(bool value)
    {
        canDie = value;
        photonView.RPC("SyncSetCanDieRPC",RpcTarget.All,value);
    }

    public event GlobalDelegates.BoolDelegate OnSetCanDie;
    public event GlobalDelegates.BoolDelegate OnSetCanDieFeedback;

    public void RequestDie(int KillerID)
    {
        photonView.RPC("DieRPC", RpcTarget.MasterClient, KillerID);
    }
    
    [PunRPC]
    public void DieRPC(int killerId)
    {
        var entity = EntityCollectionManager.GetEntityByIndex(killerId);
        if (entity && (entity is Champion.Champion))
        {
            // GameObject item = PhotonNetwork.Instantiate(activePinataAutoSO.ItemBagPrefab.name, transform.position, Quaternion.identity);
            // ItemBag bag = item.GetComponent<ItemBag>();
            // bag.SetItemBag(items[0].indexOfSOInCollection, EntityCollectionManager.GetEntityByIndex(killerId).team);
            // bag.ChangeVisuals(true);
            // bag.IsCollectible();
            entity.AddItemRPC(items[0].indexOfSOInCollection);

            var rp = new RespawnPinata
            {
                delay = 0,
            };

            SpawnAIs.Instance.PinatasRespawned.Add(rp);
            
        }
        
        OnDie?.Invoke(killerId);
        
        if (isOffline)
        {
            SyncDieRPC(killerId);
            
            return;
        }
        photonView.RPC("SyncDieRPC", RpcTarget.All, killerId);
    }
    
    [PunRPC]
    public void SyncDieRPC(int killerId)
    {
        isAlive = false;
        FogOfWarManager.Instance.RemoveFOWViewable(this);

        Destroy(Instantiate(PinataDieFX, transform.position, Quaternion.identity), 2f);
        SetAnimatorTrigger("Death");

        gameObject.SetActive(false);
        
        OnDieFeedback?.Invoke(killerId);
    }

    public event Action<int> OnDie;
    public event Action<int> OnDieFeedback;

    public void RequestRevive()
    {
        
    }

    public void SyncReviveRPC()
    {
       
    }

    public void ReviveRPC()
    {
        
    }

    public event GlobalDelegates.NoParameterDelegate OnRevive;
    public event GlobalDelegates.NoParameterDelegate OnReviveFeedback;
}
}

