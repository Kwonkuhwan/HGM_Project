using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mini0Target : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	RectTransform RT;
	MinigameManager MM;
	Vector2 offset;
	bool isClear;

	void Awake() 
	{
		RT = GetComponent<RectTransform>();
		MM = transform.GetComponentInParent<MinigameManager>();
	}


	void OnEnable() 
	{
		RT.anchoredPosition = new Vector2(Random.Range(-300, 300), Random.Range(-300, 300));
		isClear = false;
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
		if (RT.anchoredPosition.x > -20 && RT.anchoredPosition.x < 20
			&& RT.anchoredPosition.y > -20 && RT.anchoredPosition.y < 20) 
		{
			if (!isClear) MissionClear();
		}
			
	}
	void MissionClear() 
	{
		isClear = true;
		RT.anchoredPosition = Vector2.zero;
		MM.CompleteMission();
	}

}
