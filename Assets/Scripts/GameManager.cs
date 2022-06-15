using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    private static GameManager GM = null; // 유일성이 보장된다.
    private static GameManager Instance { get { Init(); return GM; } }

    InputManager _input = new InputManager();
    public static InputManager Input { get { return Instance._input; } }

    void Start()
    {
        Init();
    }

    private void Update()
    {
        _input.OnUpdate(); //입력장치에 접근
    }

    static void Init()
    {
        if (GM == null)
        {
            GameObject go = GameObject.Find("@GameManager");
            if (go == null)
            {
                go = new GameObject { name = "GameManager" };
                DontDestroyOnLoad(go);
                GM = go.AddComponent<GameManager>();
                GM = go.GetComponent<GameManager>();
            }
        }
        else
        {
            Destroy(GM);
        }
    }
}
