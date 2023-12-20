using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//using System.Runtime.InteropServices;
//using System.Xml.Linq;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler

{
	Image joyStickBgImage;
	Image joyStickImage;


	static Vector3 inputVector;

	public static bool canRotate;

	//this is required in sunstrike
	public static bool isPressed = false;


	void OnEnable()
	{

		//GameManager.OnRoundOverEvent += RoundOver;
	}

	void Start()
	{


		canRotate = false;
		inputVector = new Vector3(0, 0, 0);
		joyStickBgImage = GetComponent<Image>();
		joyStickImage = transform.GetChild(0).GetComponent<Image>();

	}



	public virtual void OnDrag(PointerEventData ped)
	{

		Vector2 pos;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joyStickBgImage.rectTransform, ped.position, ped.pressEventCamera, out pos))
		{

			pos.x = (pos.x / joyStickBgImage.rectTransform.sizeDelta.x);
			pos.y = (pos.y / joyStickBgImage.rectTransform.sizeDelta.y);

			inputVector = new Vector3(pos.x * 2 - 1, 0, pos.y * 2 - 1);
			inputVector = inputVector.magnitude > 1.0f ? inputVector.normalized : inputVector;



			joyStickImage.rectTransform.anchoredPosition = new Vector3(inputVector.x * (joyStickBgImage.rectTransform.sizeDelta.x / 2),
				inputVector.z * (joyStickBgImage.rectTransform.sizeDelta.y / 2));

		}

	}


	public virtual void OnPointerDown(PointerEventData ped)
	{

		OnDrag(ped);

		canRotate = true;

		//this is required in sunstrikeABA Script
		isPressed = true;


		//Debug.Log("button pressed");


	}
	public virtual void OnPointerUp(PointerEventData ped)
	{

		canRotate = false;
		inputVector = new Vector3(0, 0, 0);
		joyStickImage.rectTransform.anchoredPosition = new Vector3(0, 0, 0);

		//this is required in sunstrikeABA Script
		isPressed = false;

		// Debug.Log("button not pressed");

	}


	public static float Horizontal()
	{


		if (inputVector.x != 0)
		{
			return inputVector.x;
		}
		else
			return Input.GetAxis("Horizontal");
	}



	public static float Vertical()
	{

		if (inputVector.z != 0)

			return inputVector.z;

		else
			return Input.GetAxis("Vertical");

	}



	public static float VerticalMagnitude()
	{


		if (inputVector.magnitude != 0)
		{

			return inputVector.magnitude;
		}

		else
			return Input.GetAxis("Vertical");



	}

	void RoundOver()
	{


	}

	void OnDisable()
	{


		//GameManager.OnRoundOverEvent -= RoundOver;
	}


}
