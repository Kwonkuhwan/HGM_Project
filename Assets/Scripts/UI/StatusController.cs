using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    //���׹̳�
    [SerializeField]
    public int sp = 1000;
    private int curSp;

    //���׹̳� ȸ��
    [SerializeField]
    private int spIncreaseSpeed;

    //���׹̳� ��ȸ�� ������
    private int spRechargeTime;
    private int curSpReChargeTime;

    //���׹̳� ���� ����
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

    //��ŭ sp�� �ް��� ���ΰ�. 
    public void DecreaseSt(int _count)
    {
        spUsed = true;
        curSpReChargeTime = 0;

        //����sp�� count��ŭ ����ָ鼭
        if(curSp - _count > 0)
        {
            //0���� Ŭ���� �Ѿ�� �Ķ���� ����ŭ ���ְ�
            curSp -= _count; 
        }
        else
        {
            //���� �Ķ���Ͱ� -�� �ȴٸ�  ���̳ʽ����� �ʵ��� 0���� �׳� ������ش�. 
            curSp = 0;
        }
    }

    public void SpRechargeTime()
    {
        if(spUsed)
        {
            //���� spŸ���� �⺻sp ȸ�� �ð����� �������
            if (curSpReChargeTime < spRechargeTime)
            {
                //����
                curSpReChargeTime++;
            }
            else
            {
                spUsed = false;
            }
        }
    }

    private void SpRecover()
    {// && cursp �� 100�̻� ä������ �ȵǱ⶧��
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
