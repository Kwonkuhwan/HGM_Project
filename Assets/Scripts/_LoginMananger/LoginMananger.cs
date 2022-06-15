using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

public class LoginMananger : MonoBehaviour
{
    public GameObject ALL_Login_Panel, Login_Panel, Signup_Panel;
    public Text Status_Text;
    public Button Start_Button;

    [Header("PlayFab")]
    public PlayerLeaderboardEntry MyPlayFabInfo;
    public List<PlayerLeaderboardEntry> PlayFabUserList = new List<PlayerLeaderboardEntry>();

    [Header("Login")]
    public InputField L_EmailInput;
    public InputField L_PasswordInput;

    [Header("SignUp")]
    public InputField S_EmailInput;
    public InputField S_PasswordInput;
    public InputField S_UsernameInput;

    void ShowPanel(GameObject CurPanel)
    {
        ALL_Login_Panel.SetActive(false);
        Login_Panel.SetActive(false);
        Signup_Panel.SetActive(false);

        CurPanel.SetActive(true);
    }

    #region 로그인, 회원가입
    void Awake()
    {
        //Screen.SetResolution(960, 540, false);
        //PhotonNetwork.SendRate = 60;
        //PhotonNetwork.SerializationRate = 30;

        ShowPanel(ALL_Login_Panel);
    }

    #region 처음 로그인 화면
    public void On_HGM_LoginBtn()
    {
        ShowPanel(Login_Panel);
    }
    #endregion

    #region HGM 로그인 화면
    public void On_L_SignUpBtn()
    {
        ShowPanel(Signup_Panel);
    }

    public void On_L_LoginBtn()
    {
        Login();
    }
    public void On_L_XBtn()
    {
        ShowPanel(ALL_Login_Panel);
    }
    #endregion

    #region HGM 로그인클릭 회원가입 화면
    public void On_S_SignUpBtn()
    {
        Register();
    }

    public void On_S_XBtn()
    {
        ShowPanel(Login_Panel);
    }
    #endregion

    private void Login()
    {
        var request = new LoginWithEmailAddressRequest { Email = L_EmailInput.text, Password = L_PasswordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, 
            (result) => 
            {
                SceneManager.LoadScene("2_Ingame_Scene");
                Status_Text.text = "로그인 성공"; 
            }, 
            (error) => 
            {
                Status_Text.text = "로그인 실패"; 
            });
    }

    private void Register()
    {
        var request = new RegisterPlayFabUserRequest { Email = S_EmailInput.text, Password = S_PasswordInput.text, Username = S_UsernameInput.text, DisplayName = S_UsernameInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, 
            (result) => 
            {
                ShowPanel(Login_Panel);
                Status_Text.text = "회원가입 성공";
            }, 
            (error) => 
            {
                Status_Text.text = "회원가입 실패";
            });
    }

    private void SetStat()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "IDInfo", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => Status_Text.text = "값 저장실패");
    }

    private void GetLeaderboard(string myID)
    {
        PlayFabUserList.Clear();

        for (int i = 0; i < 10; i++)
        {
            var request = new GetLeaderboardRequest
            {
                StartPosition = i * 100,
                StatisticName = "IDInfo",
                MaxResultsCount = 100,
                ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true }
            };
            PlayFabClientAPI.GetLeaderboard(request, (result) =>
            {
                if (result.Leaderboard.Count == 0) return;
                for (int j = 0; j < result.Leaderboard.Count; j++)
                {
                    PlayFabUserList.Add(result.Leaderboard[j]);
                    if (result.Leaderboard[j].PlayFabId == myID) MyPlayFabInfo = result.Leaderboard[j];
                }
            },
            (error) => { });
        }
    }



    //void SetData(string curData)
    //{
    //    var request = new UpdateUserDataRequest()
    //    {
    //        Data = new Dictionary<string, string>() { { "Home", curData } },
    //        Permission = UserDataPermission.Public
    //    };
    //    PlayFabClientAPI.UpdateUserData(request, (result) => { }, (error) => Status_Text.text =("데이터 저장 실패"));
    //}

    //void GetData(string curID)
    //{
    //    PlayFabClientAPI.GetUserData(new GetUserDataRequest() { PlayFabId = curID }, (result) =>
    //    UserHouseDataText.text = curID + "\n" + result.Data["Home"].Value,
    //    (error) => Status_Text.text =("데이터 불러오기 실패"));
    //}
#endregion
}
