using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapObjectUIController : MonoBehaviour {

	// Use this for initialization
	public void Start () {
		GameObject.FindObjectOfType<HexMap>().OnCityCreated += CreateCityLabel;
		Debug.Log("City Label event successful!");

		//mouseController = FindObjectOfType<MouseController>();

	}


	public MouseController mouseController;
	/* void OnDestroy() {
	
		GameObject.FindObjectOfType<HexMap>().OnCityCreated -= CreateCityLabel;
	} */

	
	// Update is called once per frame
	void Update () {
		
	}

	public GameObject CityLabelPrefab;

	public void CreateCityLabel( City city, GameObject cityGO ) {
		GameObject nameGO = (GameObject)Instantiate(CityLabelPrefab, this.transform);
		nameGO.GetComponent<MapObjectLabel>().MyTarget = cityGO;
		nameGO.GetComponentInChildren<Text>().text = city.Name;
		nameGO.GetComponent<MapObjectLabel>().City = city;
		nameGO.GetComponent<MapObjectLabel>().mouseController = mouseController;
	}
}
