using Photon.Pun;

namespace Entities.Champion
{
    public partial class Champion : ITargetable
    {
        private bool canBeTargeted;
        
        public bool CanBeTargeted()
        {
            return canBeTargeted;
        }

        public void RequestSetCanBeTargeted(bool value)
        {
            if (isMaster)
            {
                SetCanBeTargetedRPC(value);
                return;
            }
            photonView.RPC("SetCanBeTargetedRPC", RpcTarget.MasterClient, value);
        }
        
        [PunRPC]
        public void SetCanBeTargetedRPC(bool value)
        {
            canBeTargeted = value;
            OnSetCanBeTargeted?.Invoke(value);
            if (isOffline)
            {
                SyncSetCanBeTargetedRPC(value);
                return;
            }
            photonView.RPC("SyncSetCanBeTargetedRPC", RpcTarget.All, value);
        }

        [PunRPC]
        public void SyncSetCanBeTargetedRPC(bool value)
        {
            canBeTargeted = value;
            OnSetCanBeTargetedFeedback?.Invoke(value);
        }
        
        public event GlobalDelegates.BoolDelegate OnSetCanBeTargeted;
        public event GlobalDelegates.BoolDelegate OnSetCanBeTargetedFeedback;

        public void RequestOnTargeted() { }

        [PunRPC]
        public void SyncOnTargetedRPC() { }

        [PunRPC]
        public void OnTargetedRPC() { }

        public event GlobalDelegates.NoParameterDelegate OnOnTargeted;
        public event GlobalDelegates.NoParameterDelegate OnOnTargetedFeedback;

        public void RequestOnUntargeted() { }

        [PunRPC]
        public void SyncOnUntargetedRPC() { }

        [PunRPC]
        public void OnUntargetedRPC() { }

        public event GlobalDelegates.NoParameterDelegate OnOnUntargeted;
        public event GlobalDelegates.NoParameterDelegate OnOnUntargetedFeedback;
    }
}