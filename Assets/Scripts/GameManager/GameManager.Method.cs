using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        isGameEnd = false;
        isGameStart = false;
    }

    void Start()
    {
        //// �ʿ��� �̺�Ʈ ������ ���
        //actPlayerDie += () => isGameOver = true;
        //actPlayerDie += () => audioSource.Pause();
        //actGamePause += () => audioSource.Pause();
        //actGameRestart += () => audioSource.Play();
    }

}
public partial class GameManager : MonoBehaviour
{
    //GameManager�� �̱���
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            else
            {
                return instance;
            }
        }
    }


}

