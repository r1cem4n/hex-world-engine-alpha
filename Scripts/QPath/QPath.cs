using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

namespace QPath {

	//
	//	QPath.FindPath( ourWorld, theUnit, startTile, endTile );
	//
	//	theUnit:	game object that is doing the pathfinding, may have it's own logic for movement cost 
	//	
	//	Our tiles need to be able to return the following:
	//		1)	List of neighbors
	//		2)	The aggregate (or total) cost to enter each tile
	//
	//
	//
	//

	public static class QPath {

		public static T[] FindPath<T>( 
			IQPathWorld theWorld, 
			IQPathUnit theUnit, 
			T startTile, 
			T endTile,
			CostEstimateDelegate costEstimateFunc 
		) where T : IQPathTile {
			// Debug.Log("QPath started!");
			if(theWorld == null || theUnit == null || startTile == null || endTile == null) {
				Debug.LogError("Null in pathfinding QPath:IQPathTile:FindPath");
				return null;
			}

			QPath_AStar<T> resolver = new QPath_AStar<T>(theWorld, theUnit, startTile, endTile, costEstimateFunc);

			resolver.DoWork();
			return resolver.GetList(); 
		}
	} 

	public delegate float CostEstimateDelegate(IQPathTile a, IQPathTile b);
}
