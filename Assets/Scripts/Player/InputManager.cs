using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public Action KeyAction = null; 
    public void OnUpdate()
    {
        if (Input.anyKey == false)
        {
            return;
        }
        //키 액션이 있었다면
        if(KeyAction != null)
        {
            KeyAction.Invoke();
        }
    }
}
