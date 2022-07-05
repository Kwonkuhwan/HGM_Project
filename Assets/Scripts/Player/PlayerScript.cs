using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using static NetworkManager;
using static UIManager;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public static PlayerScript PS;

    public Rigidbody2D RB;
    public SpriteRenderer SR;
    public SpriteRenderer[] CharacterSR;
    
    public Transform Character, Canvas;
    public Text NickText;

    public bool isWalk, isMove, isImposter, isKillable, isDie;
    public int actor, colorIndex;
    public float speed; //기본 40
    public PlayerScript KillTargetPlayer;
    public int targetDeadColorIndex;

    [HideInInspector] public PhotonView PV;
    [HideInInspector] public string nick;
    Vector2 input;
    bool facingRight;

    Vector2 playerDir;
    Vector3 curPos;
    
    public Animator punchAnim;

    // 2022.06.26 kkh : hp 슬라이더 추가
    private Slider hp_slider = null;
    private float hp_Max = 100.0f;
    private float hp_Cur = 100.0f;
    public float HP_Cur { get => hp_Cur; set => hp_Cur = value; }

    private void Awake()
    {
        PS = this;
    }

    void Start()
    {
        PV = photonView;
        actor = PV.Owner.ActorNumber;
        nick = PV.Owner.NickName;
        SetNick();
        NM.Players.Add(this);
        NM.SortPlayers();
        isMove = true;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
    void SetNick()
    {
        NickText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
    }

    [PunRPC]
    void FixedUpdate()
    {
        if(!PV.IsMine)
        {
            return;
        }

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        input = new Vector2(inputX, inputY);
        input *= speed;
        RB.velocity = input.normalized * speed;

        if (PV.IsMine)
        {
            if(inputX != 0)
            {
                PV.RPC("FlipXRPC", RpcTarget.AllBuffered, inputX);
            }
            NM.PointLight2D.transform.position = transform.position + new Vector3(0, 0, 10);
        }
        // IsMine이 아닌 것들은 부드럽게 위치 동기화
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);

        // 2022.06.26 kkh : 혹시 몰라서 한번더 체크
        if (UM.hp_slider != null && UM.hp_slider.enabled && hp_slider == null)
        {
            hp_slider = UM.hp_slider;
        }

        if (hp_slider != null && hp_slider.enabled)
        {

            // 2022.06.26 kkh : 피격 당했을때 이걸 넣어야하는데 그게 어디지?
            //hp_Cur -= 10;


            // 2022.06.26 kkh : hp 감소 실행
            HandleHP();
        }
    }

    [PunRPC]
    void FlipXRPC(float axis) => SR.flipX = axis == 1;

    //[PunRPC]
    //public void Filp(Vector2 input)
    //{
    //    //if (input.x < 0 && !facingRight)
    //    //{
    //    //    transform.localScale = new Vector2(1, 1); // left flip.
    //    //    playerDir = Vector2.left;
    //    //}

    //    //if (input.x > 0 && !facingRight)
    //    //{
    //    //    transform.localScale = new Vector2(-1, 1); // left flip.
    //    //    playerDir = Vector2.right;
    //    //}
    //}

    public void SetPos(Vector3 target)
    {
        transform.position = target;
    }

    [PunRPC]
    public void SetColor(int _colorIndex)
    {
        CharacterSR[0].color = UM.colors[_colorIndex];
        CharacterSR[1].color = UM.colors[_colorIndex];
        colorIndex = _colorIndex;
    }

    [PunRPC]
    void SetImpoCrew(bool _isImposter)
    {
        isImposter = _isImposter;
    }

    public void SetNickColor()
    {
        if (!isImposter) return;

        for (int i = 0; i < NM.Players.Count; i++)
        {
            if (NM.Players[i].isImposter) NM.Players[i].NickText.color = Color.red;
        }
    }

    public void SetMission()
    {
        if (!PV.IsMine) return;
        if (isImposter) return;

        List<int> GachaList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        for (int i = 0; i < 4; i++)
        {
            int rand = Random.Range(0, GachaList.Count);
            NM.Interactions[GachaList[rand]].SetActive(true);
            GachaList.RemoveAt(rand);
        }
    }

    [PunRPC]
    public void Punch()  // 펀치 함수. 
    {
        PhotonNetwork.Instantiate("Glove", transform.position + new Vector3(SR.flipX ? 9f : -9f, 0f, -1f), Quaternion.Euler(0,0,-180))
                    .GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, SR.flipX ? 1 : -1);

        StartCoroutine(UM.PunchCoolCo());


        //KillTargetPlayer.GetComponent<PhotonView>().RPC("Punch", RpcTarget.AllViaServer, true);

        //KillTargetPlayer.GetComponent<PhotonView>().RPC("SetDie", RpcTarget.AllViaServer, true, colorIndex, KillTargetPlayer.colorIndex);
        //Vector3 TargetPos = KillTargetPlayer.transform.position;
        //transform.position = TargetPos;

        //GameObject CurDeadBody = PhotonNetwork.Instantiate("DeadBody", TargetPos, Quaternion.identity);
        //CurDeadBody.GetComponent<PhotonView>().RPC("SpawnBody", RpcTarget.AllViaServer, KillTargetPlayer.colorIndex, Random.Range(0, 2));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
        }
    }

    public void SetHPBar()
    {
        hp_slider = UM.hp_slider;
        // 2022.06.26 kkh : hp 슬라이더 값 초기화
        hp_slider.value = hp_Cur / hp_Max;
    }

    private void HandleHP()
    {
        hp_slider.value = hp_Cur / hp_Max;
    }
}


//	[PunRPC] //인벤토리를 쏟아낼 함수
//void SetDie(bool b, int _killerColorIndex, int _deadBodyColorIndex) 
//{
//	isDie = b;

//	transform.GetChild(0).gameObject.SetActive(false);
//	transform.GetChild(1).gameObject.SetActive(false);

//	if (PV.IsMine) 
//	{
//		StartCoroutine(UM.DieCo(_killerColorIndex, _deadBodyColorIndex));

//		transform.GetChild(1).gameObject.SetActive(true); //유령을 스폰한다. 
//		transform.GetChild(2).gameObject.SetActive(true);
//		Physics2D.IgnoreLayerCollision(8, 9);
//		PV.RPC("SetGhostColor", RpcTarget.AllViaServer, colorIndex);
//		NM.GetComponent<PhotonView>().RPC("ShowGhostRPC", RpcTarget.AllViaServer);
//	}
//}

