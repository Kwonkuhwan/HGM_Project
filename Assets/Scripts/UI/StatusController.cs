using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    //스테미너
    [SerializeField]
    public int sp = 1000;
    private int curSp;

    //스테미나 회복
    [SerializeField]
    private int spIncreaseSpeed;

    //스테미너 재회복 딜레이
    private int spRechargeTime;
    private int curSpReChargeTime;

    //스테미너 감소 여부
    private bool spUsed;

    [SerializeField]
    private Image[] sp_image;

    private const int SP = 5;

    private void Start()
    {
        curSp = sp;

    }

    private void Update()
    {
        //DecreaseSt();
        SpRechargeTime();
        SpRecover();
        Gauge();
    }

    //얼만큼 sp를 달게할 것인가. 
    public void DecreaseSt(int _count)
    {
        spUsed = true;
        curSpReChargeTime = 0;

        //현재sp를 count만큼 깍아주면서
        if(curSp - _count > 0)
        {
            //0보다 클때는 넘어온 파라미터 값만큼 빼주고
            curSp -= _count; 
        }
        else
        {
            //현재 파라미터가 -가 된다면  마이너스되지 않도록 0으로 그냥 만들어준다. 
            curSp = 0;
        }
    }

    public void SpRechargeTime()
    {
        if(spUsed)
        {
            //현재 sp타임이 기본sp 회복 시간보다 작을경우
            if (curSpReChargeTime < spRechargeTime)
            {
                //증가
                curSpReChargeTime++;
            }
            else
            {
                spUsed = false;
            }
        }
    }

    private void SpRecover()
    {// && cursp 가 100이상 채워지면 안되기때문
        if(!spUsed && curSp < sp)
        {
            curSp += spIncreaseSpeed;
        }
    }

    private void Gauge()
    {
        sp_image[SP].fillAmount = (float)curSp / sp;
    }
}
