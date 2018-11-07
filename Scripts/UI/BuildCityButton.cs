using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCityButton : MonoBehaviour {
	
	public void BuildCity() {


		City city = new City();
		HexMap map = GameObject.FindObjectOfType<HexMap>();
		MouseController mouseController = GameObject.FindObjectOfType<MouseController>();
		Hex newCityHex = mouseController.SelectedUnit.Hex;
		int q = newCityHex.Q;
		int r = newCityHex.R;

		map.SpawnCityAt( city, q, r);
	}
}
