using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;

public class ItemBlueprint {
	public ItemBlueprint (
		int id,
		string name,
		string desc,
		float buildCost,
		OnCompleteDelegate completionFunc
	) {
		this.ID = id;
		this.Name = name;
		this.Description = desc;
		this.BuildCost = buildCost;
		this.OnCompleteFunc = completionFunc;
		this.TechRequirement = 0;
	}
	public readonly int ID;
	public string Name;
	public string Description;
	public float BuildCost;
	public int TechRequirement;

	public enum BUILD_TYPE { BUILDING, UNIT, DISTRICT }

	public delegate void OnCompleteDelegate( City c );
	public OnCompleteDelegate OnCompleteFunc;
	

}


static public class BuildQueueItemDB {

	static BuildQueueItemDB() {
		items = new Dictionary<int, ItemBlueprint>();
	}

	static Dictionary<int, ItemBlueprint> items;

	static public ItemBlueprint[] GetListOfJobs( City c ) {
		// City reference is being passed so we can check dependencies
		List<ItemBlueprint> iBarry = new List<ItemBlueprint>(items.Values.ToArray());
		for (int i = 0; i > iBarry.Count; i++ ) {
			// if (iBarry)
			/* if(c.Player.TechTree.Contains(iBarry[i].TechRequirement) == false ) {

			} */
		}
		return items.Values.ToArray();
	}

	static void LoadBuildQueueDB() {
		// put this in an XML or JSON file or something

		ItemBlueprint item;
		int id = 0;

		item = new ItemBlueprint (
			id,								// id, leave this one alone
			"Monument",						// Item Name
			"Increases some stat",			// Description
			100f,							// Production Cost
			(c) => {						// OnComplete Funtion delegate - 'c' refers to the City we're building in
				c.AddBuilding(City.BuildingType.Monument);
			}
		);
		items[id++] = item;

		item = new ItemBlueprint (
			id,
			"Granary",
			"No Description",
			150f,
			(c) => {
				c.AddBuilding(City.BuildingType.Granary);
			}
		);
		items[id++] = item;

		item = new ItemBlueprint (
			id,
			"Skeletal Warrior",
			"No Description",
			50f,
			(c) => {
				Unit u = new Unit();
				GameObject pf = c.Hex.HexMap.UnitSkeletonPrefab;
				c.Hex.HexMap.SpawnUnitAt(u, pf, c.Hex.Q, c.Hex.R);
			}
		);
		items[id++] = item;
	}

}
