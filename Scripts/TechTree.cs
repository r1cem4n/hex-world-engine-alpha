using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Technology {

	public Technology(
		int id,
		string name,

		int[] requiredTechs,
		string desc,
		float cost,
		OnCompleteDelegate completeFunc
	) {
		this.ID = id;
		this.Name = name;

		this.Description = desc;
		this.RequiredTechs = new int[] { 0, 1, 3 };	// = requiredTechs; // example
		this.IsResearched = false;
	}

	public int ID;
	public string Name;
	public int[] RequiredTechs;
	public string Description;

	public bool IsResearched;

	public delegate void OnCompleteDelegate( Player p );
	public OnCompleteDelegate OnCompleteFunc;

}

public class TechTree {

	public TechTree() {


	}


}

