using Photon.Pun;
using UnityEngine;

namespace Entities.Champion
{
    public partial class Champion : ICandyable
    {
        public void OnAddMessage(string message)
        {
            photonView.RPC("AddMessage", RpcTarget.All, message);
        }
    
        [PunRPC]
        public void AddMessage(string message)
        {
            ChatManager.Instance.UpdateMessageBox(message);
        }
    }
}