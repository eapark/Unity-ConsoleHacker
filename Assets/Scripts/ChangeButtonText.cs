using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeButtonText : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler {
	private Text buttonText;
	private Button button;

	// Use this for initialization
	void Start () {
		button = GetComponent<Button> ();
		buttonText = GetComponentInChildren<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (button.interactable) {
			buttonText.color = Color.black;
		}
		else {
			buttonText.color = Color.green;
		}
	}

	public void OnPointerExit(PointerEventData eventData) {
		buttonText.color = Color.green;
	}

	public void OnPointerClick(PointerEventData eventData){
		buttonText.color = Color.green;
	}

}
