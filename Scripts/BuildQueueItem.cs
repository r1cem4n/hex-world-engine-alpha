using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildQueueItem {

	// Use this for initialization
	void Start () {
		
	}
	public BuildQueueItem(
		string jobName,
		string jobDesc,
		float production,
		float productionRollover,
		ProductionCompleteDelegate onProductionComplete,
		ProductionBonusDelegate productionBonusFunc
	) {
		this.Name = jobName;
		this.Description = jobDesc;
		this.ProductionRequired = production;
		// production rollover's gotta go here somewhere
		this.OnProductionComplete = onProductionComplete;
		this.ProductionBonusFunc = productionBonusFunc;
	}
	// build job details
	public string Name;
	public string Description;
	
	// production requirement variables (public)
	public float ProductionRequired;
	public float ProductionDone;


	// what to do when production is complete (production rollover)
	public delegate void ProductionCompleteDelegate( );
	public ProductionCompleteDelegate OnProductionComplete;

	public delegate float ProductionBonusDelegate( );
	public ProductionBonusDelegate ProductionBonusFunc;

	public void DoWork( float rawProduction ) {
		if ( ProductionBonusFunc != null ) {
			rawProduction *= ProductionBonusFunc();
		}

		ProductionDone += rawProduction;

		if (ProductionDone >= ProductionRequired) {
			OnProductionComplete();
			// figure out remainder production points roll over
		}
	}

}
