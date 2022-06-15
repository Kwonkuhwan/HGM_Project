using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Mini2Target : MonoBehaviour, IPointerUpHandler
{
	MinigameManager MM;
	bool isClear;
	Slider slider;


	void Awake()
	{
		MM = transform.GetComponentInParent<MinigameManager>();
		slider = GetComponent<Slider>();

	}


	void OnEnable()
	{
		isClear = false;
		slider.value = 0;
		slider.interactable = true;
	}


	public void MissionClear(float f)
	{
		if (isClear) return;
		if (f > 0.9f)
		{
			isClear = true;
			slider.interactable = false;
			slider.value = 1;
			MM.CompleteMission();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (isClear) return;
		slider.value = 0;
	}
}
