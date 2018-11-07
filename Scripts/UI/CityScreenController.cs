using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityScreenController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	void OnEnable() {
		if (mouseController.SelectedCity != null) {
			city = mouseController.SelectedCity;
			Header.GetComponentInChildren<Text>().text = city.Name;
			if (city.OwnedHexes != null) {
				foreach (Hex h in city.OwnedHexes ) {
					Debug.Log("City owns tile at " + h.Q + ", " + h.R);
				}
			} else {
				Debug.Log("OwnedHexes is null!!!");
			}
		}
		
	}
	public MouseController mouseController;
	public GameObject Header;

	private City city;
	// Update is called once per frame
	void Update () {
		
	}
}
