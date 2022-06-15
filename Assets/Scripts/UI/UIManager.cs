using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using static NetworkManager;

public class UIManager : MonoBehaviourPun
{
    public static UIManager UM = null;
    private void Awake()
    {
        if (UM == null)
        {
            UM = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    // 0 : use, 1 customize, 2 cancel, 3 start, 4 report, 5 kill, 6 sabotage, 7 null, 8 emergency
    public Sprite[] sprites;
    int curBtn0, curBtn1, curBtn2;
    bool active0, active1, active2;
    public Image WaitingInteractionBtn0, InteractionBtn1, InteractionBtn2;
    public Text Interaction2Text;

    public Image PreviewImage;
    public Color[] colors;
    public GameObject CustomizePanel, DiePanel;
    public GameObject[] ColorCancel;
    public Button[] ColorBtn;
    public Button StartBtn;
    public Transform LeftBottom, RightTop, LeftBottomMap, RightTopMap, PlayerMap;
    public GameObject[] MissionMaps;
    public Image KillerImage, DeadbodyImage;
    public Text LogText;
    public Text broadCastText; 


    public GameObject[] Minigames;
    public GameObject MissionClearText;


    public int curInteractionNum;
    public Slider MissionGageSlider;
    public GameObject SabotagePanel;

    public GameObject Rock;

    PhotonView PV;
    public InputField ChatInput;
    public Text ChatText;
    public Scrollbar ChatScroll;
    public RectTransform ChatContent;
    public GameObject[] ChatPanels;
    public int killCooltime, emergencyCooltime;

    void Start()
    {
        PV = photonView;
    }

    // 대기실
    public void SetInteractionBtn0(int index, bool _active)
    {
        curBtn0 = index;
        active0 = _active;

        if (!NM.isGameStart)
        {
            WaitingInteractionBtn0.sprite = sprites[index];
            WaitingInteractionBtn0.GetComponent<Button>().interactable = active0;
        }
        else
        {
            InteractionBtn1.sprite = sprites[index];
            InteractionBtn1.GetComponent<Button>().interactable = active0;
        }
    }

    //공통에서 가질 use버튼 활성화 
    //공통의 use btn_1
    public void SetInteractionBtn1(int index, bool _active)
    {
        curBtn0 = index;
        active0 = _active;
        InteractionBtn1.sprite = sprites[index];
        InteractionBtn1.GetComponent<Button>().interactable = active0;
    }

    //공통의 kill btn_2
    public void SetInteractionBtn2(int index, bool _active)
    {
        curBtn1 = index;
        active1 = _active;
        InteractionBtn2.sprite = sprites[index];
        InteractionBtn2.GetComponent<Button>().interactable = active1;
    }

    public void ColorChange(int _colorIndex)
    {
        PreviewImage.color = colors[_colorIndex];
        NM.MyPlayer.GetComponent<PhotonView>().RPC("SetColor", RpcTarget.AllBuffered, _colorIndex);
    }

    public void ClickInteractionBtn0()
    {
        // 커스터마이즈
        if (curBtn0 == 1)
        {
            CustomizePanel.SetActive(true);
            SetIsCustomize(false);
            PreviewImage.color = colors[NM.MyPlayer.colorIndex];
        }
    }
    public void ClickInteractionBtn1()
    {
        if(curBtn0 == 0)
        {
            // 크루원 작업
            GameObject CurMinigame = Minigames[Random.Range(0, Minigames.Length)];
            CurMinigame.GetComponent<MinigameManager>().StartMission();
        }
    }

    public void ClickInteractionBtn2()
    {
        // 킬
        if (curBtn1 == 5)
        {
            if (NM.MyPlayer.isDie) return;
            NM.MyPlayer.Kill();
        }
    }

    public void SetIsCustomize(bool b)
    {
        NM.MyPlayer.isMove = b;
    }

    void Update()
    {
        if (!PhotonNetwork.InRoom) return;
        SetActiveColors();
        SetMap();
        if (!PhotonNetwork.IsMasterClient) return;
        ShowStartBtn();
    }

    void ShowStartBtn()
    {
        StartBtn.gameObject.SetActive(true);
        StartBtn.interactable = PhotonNetwork.CurrentRoom.PlayerCount >= 1; // 2
    }

    public void SetActiveColors()
    {
        List<int> colorList = new List<int>();
        for (int i = 0; i < NM.Players.Count; i++)
            colorList.Add(NM.Players[i].colorIndex);

        for (int i = 0; i < ColorCancel.Length; i++)
        {
            bool contain = colorList.Contains(i + 1);
            ColorCancel[i].SetActive(contain);
            ColorBtn[i].interactable = !contain;
        }
    }

    public void SetMap()
    {
        // 실제 맵
        float width = RightTop.position.x - LeftBottom.position.x;
        float height = RightTop.position.y - LeftBottom.position.y;

        Vector3 MyPlayerPos = NM.MyPlayer.transform.position;
        float playerWidth = MyPlayerPos.x - LeftBottom.position.x;
        float playerHeight = MyPlayerPos.y - LeftBottom.position.y;

        // 지도
        float widthMap = RightTopMap.position.x - LeftBottomMap.position.x;
        float heightMap = RightTopMap.position.y - LeftBottomMap.position.y;

        float playerMapX = LeftBottomMap.position.x + (playerWidth / width) * widthMap;
        float playerMapY = LeftBottomMap.position.y + (playerHeight / height) * heightMap;

        PlayerMap.position = new Vector3(playerMapX, playerMapY, 0);
    }


    public IEnumerator KillCo()
    {
        SetInteractionBtn2(5, false);
        NM.MyPlayer.isKillable = false;

        for (int i = 10; i > 0; i--) // 기본 15초 킬대기
        //for (int i = 3; i > 0; i--)
        {
            killCooltime = i;

            if (UM.curBtn1 == 5) 
                Interaction2Text.text = killCooltime.ToString();
            else
                Interaction2Text.text = "";

            yield return new WaitForSeconds(1);
        }
        killCooltime = 0;
        Interaction2Text.text = "";

        NM.MyPlayer.isKillable = true;
    }


    //킬 당한 인원에게 킬 연출 true
    public IEnumerator DieCo(int killerColorIndex, int deadBodyColorIndex)
    {
        DiePanel.SetActive(true);
        KillerImage.color = UM.colors[killerColorIndex];
        DeadbodyImage.color = UM.colors[deadBodyColorIndex];

        yield return new WaitForSeconds(4);
        DiePanel.SetActive(false);
    }

    public void ShowLog(string log)
    {
        LogText.text = log;
    }






    [PunRPC]
    public void SetMaxMissionGage()
    {
        MissionGageSlider.maxValue = NM.GetCrewCount();
    }

    [PunRPC]
    public void AddMissionGage()
    {
        //미션 게이지  수정할것. 
        MissionGageSlider.value += 5.0f;

        if (MissionGageSlider.value == MissionGageSlider.maxValue) 
        {
            Rock.SetActive(false);
            // 미션게이지가 다 찰경우 문을 오픈.
            // 크루원 승리
            //NM.Winner(true);
        }
    }

    public IEnumerator MissionClearCo(GameObject MissionPanel) 
    {
        MissionPanel.SetActive(false);
        MissionClearText.SetActive(true);
        yield return new WaitForSeconds(2);
        MissionClearText.SetActive(false);
    }

    //미션클리어 함수
    public void MissionClear(GameObject MissionPanel)
    {
        StartCoroutine(MissionClearCo(MissionPanel));
        PV.RPC("AddMissionGage", RpcTarget.AllViaServer);
    }
}

