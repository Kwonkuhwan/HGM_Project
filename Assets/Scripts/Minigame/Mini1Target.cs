using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Mini1Target : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	
	RectTransform RT;
	MinigameManager MM;
	Vector2 offset, firstPos;
	bool isClear;


	void Awake()
	{
		RT = GetComponent<RectTransform>();
		MM = transform.GetComponentInParent<MinigameManager>();
	}


	void OnEnable()
	{
		isClear = false;
		firstPos = new Vector2(-139, -272);
		RT.anchoredPosition = firstPos;
	}

	Vector2 GetMousePos()
	{
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return new Vector2(mousePos.x, mousePos.y);
	}


	public void OnPointerDown(PointerEventData data)
	{
		offset = GetMousePos() - new Vector2(RT.transform.position.x, RT.transform.position.y);
	}

	public void OnDrag(PointerEventData data)
	{
		RT.transform.position = GetMousePos() - offset;
	}

	public void OnPointerUp(PointerEventData data)
	{
		if (RT.anchoredPosition.x > -90 && RT.anchoredPosition.x < 90
			&& RT.anchoredPosition.y > 170 && RT.anchoredPosition.y < 260)
		{
			if (!isClear) MissionClear();
		}
		else RT.anchoredPosition = firstPos;
	}

	void MissionClear()
	{
		isClear = true;
		RT.anchoredPosition = new Vector2(0, 215);
		MM.CompleteMission();
	}
}
