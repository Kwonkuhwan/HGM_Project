using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIManager;
using static NetworkManager;

public class MinigameManager : MonoBehaviour, IMinigame
{
    public int curRemainMission;
	int remainMission;

	public void StartMission()
    {
		remainMission = curRemainMission;

		gameObject.SetActive(true);
    }

    public void CancelMission()
	{
        gameObject.SetActive(false);
	}

	public void CompleteMission()
	{
		if (--remainMission <= 0)
		{
			UM.MissionClear(gameObject);
			NM.Interactions[UM.curInteractionNum].SetActive(false);
			UM.MissionMaps[UM.curInteractionNum].SetActive(false);
		}
	}

}
