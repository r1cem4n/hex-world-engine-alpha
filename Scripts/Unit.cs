using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QPath;

public class Unit : MapObject, IQPathUnit {

	public Unit() {
		Name = "Skeleton";
		HitPoints = 100;

		
	}

 
	public int Strength = 8;
	public int Movement = 2;
	public int MovementRemaining = 2;

	public bool isLandBound = false;	// if a unit is landbound, they cannot enter water tiles at all (i.e. a seige unit)
	public bool isWaterBound = false;	// if a unit is waterbound, they cannot enter land tile at all (i.e. a boat)

	public bool isHillWalker = false;
	public bool isForestWalker = false;
	public bool isSwampWalker = true;
	public bool isMountainClimber = false;
	public bool isLakeSwimmer = true;
	public bool isOceanSwimmer = false;

	public bool CanBuildCities = true;

	
	

	//
	// hexPath Queue - list of hexes to walk through.
	// 		FIRST ITEM IS CURRENT HEX UNIT IS OCCUPYING
	//
	List<Hex> hexPath; 

	const bool MOVEMENT_RULES_CIV6 = true;
	
	override public void SetHex( Hex newHex ) {
		Hex oldHex = Hex;
		if(Hex != null) {
			Hex.RemoveUnit(this);
		}

		base.SetHex(newHex);

		Hex.AddUnit(this);
		
	}

	public void DUMMY_PATHFINDING_FUNCTION() {
		
		Hex[] p = QPath.QPath.FindPath<Hex>(
			Hex.HexMap,	// theWorld
			this,				// theUnit
			Hex,					// startTile
			Hex.HexMap.GetHexAt( Hex.Q - 3, Hex.R - 11),					// endTile
			Hex.CostEstimate	// costEstimateFunc
		);

		// Hex[] hs = System.Array.ConvertAll( p, a => (Hex)a );
		Debug.Log("Path Length Found: " + p.Length);
		SetHexPath(p);
	}

	public void ClearHexPath() {
		// Clears the movement/hex path, resetting it to a new empty queue
		this.hexPath = new List<Hex>();
	}
	//	

	public void SetHexPath( Hex[] hexPath ) {
		this.hexPath = new List<Hex>( hexPath );

		//if (hexPath.Count() > 0 ) {
			// Movement path begins on the current tile, wasting the first turn
			// Shuck it off to start movement right away (if there is a path to begin with)
			//this.hexPath.Dequeue();

			// moved this logic to doturn
		//}
	}
	public Hex[] GetHexPath() {
		return (this.hexPath == null) ? null : this.hexPath.ToArray();
	}
	
	public bool IsWaitingForOrders() {
		// returns true if we have movement left but nothing queued

		if ( MovementRemaining > 0 && (hexPath == null || hexPath.Count == 0)) {
			return true;
		} 

		return false;
	}
	//	Processes one tile worth of movement for the unit.
	//	Returns true if this should be called again immediately to finish 
	//	movement for the turn

	public bool DoMove() {
		// Do queued moves

		Hex oldHex = hexPath[0];
		Hex newHex = hexPath[1];

		if(hexPath == null || hexPath.Count == 1) {
			return false;
		}
		
		int costToEnter = MovementCostToEnterHex ( newHex ); 

		if ( costToEnter > MovementRemaining && 
				MovementRemaining < Movement && 
				MOVEMENT_RULES_CIV6) {
			// we cant enter hex this turn
			return false;
		}
		hexPath.RemoveAt(0);
		// Perform stuff
		// Hex oldHex = Hex;
		

		if (hexPath.Count == 1) {
			// the only hex left in the list, is the one we are moving to. 
			// setting to null to avoid confusion (?)
			hexPath = null;
		}
		SetHex( newHex );
		MovementRemaining = Mathf.Max(MovementRemaining - costToEnter, 0);

		return hexPath!= null && MovementRemaining > 0;
	}

	public int MovementCostToEnterHex( Hex hexMovingTo ) {
		// TODO: Factor in unit abilities 
		int MoveCost = 0;
		/* 	/// summary ///
			//////////////

			Checks the hex elevation type (mountain, hill, water) and determines how much it'll
			cost the unit depending on it's abilities. 
			
			Returning a negative number will render the tile impassable



		*/

		switch (hexMovingTo.ElevationType) {
			case Hex.ELEVATION_TYPE.MOUNTAIN:
				if ( this.isMountainClimber ) {
					MoveCost += 1;
				} else {
					return -1;
				}
				if ( isWaterBound ) {
					return -1;
				}
				break;

			case Hex.ELEVATION_TYPE.HILLS:
				if ( this.isHillWalker ) {
					//
					MoveCost += 1;
				} else {
					MoveCost += 2;
				}
				if ( isWaterBound ) {
					return -1;
				}
				break;
			
			case Hex.ELEVATION_TYPE.FLAT:
				MoveCost += 1;
				if ( this.isWaterBound ) {
					return -1;
				}
				break;

			case Hex.ELEVATION_TYPE.LAKE:
				//
				if ( this.isLakeSwimmer ) {
					MoveCost += 1;
				} else {
					return -100;
				}
				if ( this.isLandBound ) {
					return -100;
				}
				break;

			case Hex.ELEVATION_TYPE.OCEAN:
				//
				if ( this.isOceanSwimmer ) {
					MoveCost += 1;
				} else {
					return -100;
				}
				if ( this.isLandBound ) {
					return -100;
				}
				break;
		}

		if ( !this.isWaterBound ) {
			//
			// logic for land based features
			if ( hexMovingTo.FeatureType == Hex.FEATURE_TYPE.FOREST ) {
				if ( this.isForestWalker ) {
					MoveCost += 1;
				} else {
					MoveCost += 2;
				}
			} else if ( hexMovingTo.FeatureType == Hex.FEATURE_TYPE.MARSH ) {
				if ( this.isSwampWalker ) {
					MoveCost += 1;
				} else {
					MoveCost += 2;
				}
			}
			// is a river a land based feature?

		}

		if ( !this.isLandBound ) {
			//
			// logic for water based features
		}

		return MoveCost;
	}

	public float TotalTurnsToEnterHex( Hex hex, float turnsToDate ) {
		// Trying to enter a tile with movement cost greater than your 
		// current remaining movement points will result in either a cheaper
		// than expected turn cost or a more expensive than expected

		float baseTurnsToEnterHex = MovementCostToEnterHex(hex) / Movement;	// ex: entering a forest is '1' turn
		Debug.Log("MovementCostToEnterHex: " + MovementCostToEnterHex(hex));
		if (baseTurnsToEnterHex < 0 || MovementCostToEnterHex(hex) < 0) {
			// Impassable Terrain
			return -9999;
		}
		float turnsRemaining = MovementRemaining / Movement;				// ex: if we have 1/2 moves, then we have '0.5' turn
																			// forces pathfinding to emphasize turns over movement
		if (baseTurnsToEnterHex > 1) { baseTurnsToEnterHex = 1; }

		float turnsToDateWhole = Mathf.Floor(turnsToDate);
		float turnsToDateFraction = turnsToDate - turnsToDateWhole;

		if( (turnsToDateFraction > 0 && turnsToDateFraction < 0.01f) || turnsToDateFraction > 0.99f )
        {
            Debug.LogError("Looks like we've got floating-point drift: " + turnsToDate);

            if( turnsToDateFraction < 0.01f )
                turnsToDateFraction = 0;

            if( turnsToDateFraction > 0.99f )
            {
                turnsToDateWhole   += 1;
                turnsToDateFraction = 0;
            }
        }
		/* if( turnsToDateFraction < 0.01f ) { turnsToDateFraction = 0; } 
		if( turnsToDateFraction > 0.99f ) { 
			turnsToDateFraction  = 0;
			turnsToDateWhole	+= 1;
			Debug.Log("Flaoting point drift in turns to enter hex unit class!"); 

		} */

		float turnsUsedAfterThisMove = turnsToDateFraction + baseTurnsToEnterHex; 
		if (turnsUsedAfterThisMove > 1) {
			//
			//

			if(MOVEMENT_RULES_CIV6) {
				// do not enter hex
				if (turnsToDateFraction == 0) {
					// We have a full movement available, but not enough to enter
					
				} else {
					turnsToDateWhole 	+= 1;
					turnsToDateFraction  = 0;
				}

				// We know here we are looking at difficult terrain on a fresh turn
				turnsUsedAfterThisMove = baseTurnsToEnterHex;

			} else {
				// Civ 5 style movement rules
				turnsUsedAfterThisMove = 1;
			}

			// at this point, turnsUsedAfterThisMove should be some value between 0 and 1

			

		}
		return turnsToDateWhole + turnsUsedAfterThisMove;
		// Turns used after this move 
	}

	// cost in turns
	public float CostToEnterHex( IQPathTile sourceTile, IQPathTile destinationTile ) {
		// return 1;

		// there doesn't appear to be anything that uses this
		

		return MovementCostToEnterHex((Hex)destinationTile);
	}

	public void ResetMovement () {
		MovementRemaining = Movement;
	}
}
