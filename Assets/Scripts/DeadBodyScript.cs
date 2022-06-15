using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static UIManager;
using static NetworkManager;

public class DeadBodyScript : MonoBehaviourPun
{
    public Transform[] Bodys;
    Transform CurBody;
    public int colorIndex;

    [PunRPC]
    public void SpawnBody(int _colorIndex, int randBody)
    {
        CurBody = Bodys[randBody];
        CurBody.gameObject.SetActive(true);
        CurBody.GetChild(1).GetComponent<SpriteRenderer>().color = UM.colors[_colorIndex];
        gameObject.tag = "DeadBody";
        colorIndex = _colorIndex;
        //NM.WinCheck();
    }

}
