using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using static NetworkManager;

public class UIManager : MonoBehaviourPun
{
    public static UIManager UM;
    //public ActionController actionController;

    void Awake() => UM = this;

    // 0 : use, 1 customize, 2 cancel, 3 start, 4 report, 5 kill, 6 sabotage, 7 null, 8 emergency
    public Sprite[] sprites;
    int curBtn0, curBtn1, curBtn2;
    bool active0, active1, active2;
    //그냥 0 use , 1 attack   2 pickup
    public Image WaitingInteractionBtn0, InteractionBtn0, InteractionBtn1, InteractionBtn2;
    public Text Interaction1Text;

    public Image PreviewImage;
    public Color[] colors;
    public GameObject CustomizePanel, DiePanel;
    public GameObject[] ColorCancel;
    public Button[] ColorBtn;
    public Button StartBtn;
    public Text LogText;
    public GameObject[] Minigames;
    public GameObject MissionClearText;
    public int curInteractionNum;
    public Slider MissionGageSlider;
    PhotonView PV;

    public InputField ChatInput;
    public Text ChatText;
    public Scrollbar ChatScroll;
    public RectTransform ChatContent;
    public GameObject[] ChatPanels;

    public Animator anim;

    public int killCooltime;

    // 2022.06.27 kkh : hp 슬라이더 값 추가
    public Slider hp_slider = null;

    void Start()
    {
        PV = photonView;
    }

    public void SetInteractionBtn0(int index, bool _active)
    {
        curBtn0 = index;
        active0 = _active;

        // 대기실
        if (!NM.isGameStart)
        {
            WaitingInteractionBtn0.sprite = sprites[index];
            WaitingInteractionBtn0.GetComponent<Button>().interactable = active0;
        }
        else // 
        {
            InteractionBtn0.sprite = sprites[index];
            InteractionBtn0.GetComponent<Button>().interactable = active0;
        }
    }

    public void SetInteractionBtn1(int index, bool _active)
    {
        curBtn1 = index;
        active1 = _active;
        InteractionBtn1.sprite = sprites[index];
        InteractionBtn1.GetComponent<Button>().interactable = active1;
    }
    public void SetInteractionBtn2(int index, bool _active)
    {
        curBtn2 = index;
        active2 = _active;
        InteractionBtn2.sprite = sprites[index];
        InteractionBtn2.GetComponent<Button>().interactable = active2;
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
        // 사용
        else if (curBtn0 == 0)
        {
            // 크루원 작업
            GameObject CurMinigame = Minigames[Random.Range(0, Minigames.Length)];
            CurMinigame.GetComponent<MinigameManager>().StartMission();
        }
    }

    public void ClickInteractionBtn1()
    {
        // 킬
        if (curBtn1 == 5)
        {
            if (NM.MyPlayer.isDie) return;
            NM.MyPlayer.Punch();
        }
    }

    public void ClickInteractionBtn2()
    {
        // 획득
        if (curBtn2 == 6)
        {
            Debug.Log("PickUp");
            //pickUp.SetPickUp();
        }

                // 이것의 결과를 가져와야할 것이다~ 이말이야. 

                //if (NM.MyPlayer.isDie) return;
                //NM.MyPlayer.Kill();

                //inventory.items[i] = 1; // makes sure that the slot is now considered FULL
                //Instantiate(itemButton, inventory.slots[i].transform, false); // spawn the button so that the player can interact with it
                //Destroy(gameObject);
                //break;

    }
    public void SetIsCustomize(bool b)
    {
        NM.MyPlayer.isMove = b;
    }

    void Update()
    {
        if (!PhotonNetwork.InRoom) return;
        SetActiveColors();
        if (!PhotonNetwork.IsMasterClient) return;
        ShowStartBtn();
    }

    void ShowStartBtn()
    {
        StartBtn.gameObject.SetActive(true);
        //StartBtn.interactable = PhotonNetwork.CurrentRoom.PlayerCount >= 7; // 기본값
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


    public IEnumerator PunchCoolCo()
    {
        SetInteractionBtn1(5, false);
        NM.MyPlayer.isKillable = false;

        for (int i = 1; i > 0; i--) // 기본 15초 킬대기
        {
            killCooltime = i;

            if (UM.curBtn1 == 5) 
                Interaction1Text.text = killCooltime.ToString();
            else
                Interaction1Text.text = "";

            yield return new WaitForSeconds(1);
        }
        killCooltime = 0;
        Interaction1Text.text = "";
        //Enum상태 변경
        NM.MyPlayer.isKillable = true;
        SetInteractionBtn1(5, true);
    }


    public IEnumerator DieCo(int killerColorIndex, int deadBodyColorIndex)
    {
        DiePanel.SetActive(true);
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
        MissionGageSlider.value += 0.25f;

        if (MissionGageSlider.value == MissionGageSlider.maxValue) 
        {
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

    public void MissionClear(GameObject MissionPanel) 
    {
        StartCoroutine(MissionClearCo(MissionPanel));
        PV.RPC("AddMissionGage", RpcTarget.AllViaServer);
    }

    // 2022.06.26 kkh : HPBar UI Set
    public void SetHPBar()
    {
        if (!NM.isGameStart)
        {
            hp_slider.enabled = false;
        }
        else
        {
            hp_slider.enabled = true;
        }
    }
}
