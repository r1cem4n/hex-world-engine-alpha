using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QPath {
	public interface IQPathTile {

		IQPathTile[] GetNeighbors();
		
		float AggregateTileCostToEnter( float costSoFar, IQPathTile sourcetile, IQPathUnit unit );

		// float CostEstimate(IQPathTile aa, IQPathTile bb); 

	}
}