using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapObjectLabel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (TheCamera == null) {
			TheCamera = Camera.main;
		}

		rectTransform = GetComponent<RectTransform>();
		//string labelText = GetComponentInChildren<Text>().text = MyTarget.name;
		
	}
	
	public GameObject MyTarget;
	public City City;
	public Vector3 ScreenPositionOffset = new Vector3(0, 30, 0);
	public Vector3 WorldPositionOffset = new Vector3(0, 1, 0);

	public Camera TheCamera;

	RectTransform rectTransform;

	public MouseController mouseController;

	// Update is called once per frame
	void LateUpdate () {
		if (MyTarget == null) {
			Destroy(gameObject);
			return;
		}
		
		// find the position of the object and set ourselves to that
		Vector3 screenPos = TheCamera.WorldToScreenPoint( MyTarget.transform.position + WorldPositionOffset );

		rectTransform.anchoredPosition = screenPos + ScreenPositionOffset;
	}

	public void OnButtonClick() {
		// TODO: Make a script for the city screen ui panel and pass this.City reference to it
		Debug.Log("City Screen button pressed for " + City.Name);
		mouseController.SelectedCity = City;
		
	}
}
