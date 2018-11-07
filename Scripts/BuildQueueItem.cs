using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildQueueItem {

	// Use this for initialization
	void Start () {
		
	}
	public BuildQueueItem() {

	}
	// build job details
	public string Name;
	public string Description;
	
	// production requirement variables (public)
	public int ProductionRequired;


	// what to do when production is complete
	public delegate void ProductionCompleteDelegate( City city );
	public ProductionCompleteDelegate OnProductionComplete;

}
