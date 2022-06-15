using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static UIManager;
using static NetworkManager;
using UnityEngine.UI;

public class ChatManager : MonoBehaviourPun
{
    public static ChatManager CM;
	void Awake() => CM = this;


	public void ChatClear() 
    {
        photonView.RPC("ChatClearRPC", RpcTarget.AllViaServer, true);
    }


    public void OnEndEdit() 
    {
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrWhiteSpace(UM.ChatInput.text)) 
        {
            string chat = $"{NM.MyPlayer.nick} : {UM.ChatInput.text}";
            UM.ChatInput.text = "";

            photonView.RPC("ChatRPC", RpcTarget.All, chat);
        }
    }

    [PunRPC]
    void ChatRPC(string chat) 
    {
        UM.ChatText.text += (UM.ChatText.text == "" ? "" : "\n") + chat;
        

        Fit(UM.ChatText.GetComponent<RectTransform>());
        Fit(UM.ChatContent);

        UM.ChatScroll.value = 0;
    }

    void Update()
    {
        if (!PhotonNetwork.InRoom) return;

        ChatEnable();
    }

    void ChatEnable()
    {
        // 엔터시 채팅 활성화
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UM.ChatInput.ActivateInputField();
            UM.ChatInput.Select();
        }
    }

    [PunRPC]
    void ChatClearRPC(bool active)
    {
        UM.ChatText.text = "";
        UM.ChatPanels[0].SetActive(active);
        UM.ChatPanels[1].SetActive(false);
    }

    void Fit(RectTransform rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
}
