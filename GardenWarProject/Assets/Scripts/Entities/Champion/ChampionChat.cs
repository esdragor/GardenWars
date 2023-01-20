using System;
using Photon.Pun;
using UnityEngine;

namespace Entities.Champion
{
    public partial class Champion
    {
        public void OnInitializedChat()
        {
            OnAddMessage("", entityIndex);
        }

        public void OnAddMessage(string message, int index)
        {
            Debug.Log("Sending Message");
            photonView.RPC("AddMessageRPC", RpcTarget.All, message, index);
        }
    
        [PunRPC]
        public void AddMessageRPC(string message, int entityIndex)
        {
            Debug.Log("Received Message");
            ChatManager.Instance.UpdateMessageBox(message, entityIndex);
        }
    }
}