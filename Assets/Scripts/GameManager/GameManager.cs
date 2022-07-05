using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    private AudioSource audioSource;
    private static GameManager instance;

    public bool isGameEnd
    {
        get;
        private set;
    }
    public bool isGameStart
    {
        get;
        private set;
    }
}

