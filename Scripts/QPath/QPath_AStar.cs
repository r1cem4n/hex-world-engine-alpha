using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Priority_Queue;

namespace QPath {
	public class QPath_AStar<T> where T : IQPathTile {
		public QPath_AStar(
			IQPathWorld theWorld, 
			IQPathUnit theUnit, 
			T startTile, 
			T endTile,
			CostEstimateDelegate costEstimateFunc
		) {
			// Do Setup
			this.theWorld = theWorld;
			this.theUnit = theUnit;
			this.startTile = startTile;
			this.endTile = endTile;
			this.costEstimateFunc = costEstimateFunc;
		}

		IQPathWorld theWorld;
		IQPathUnit theUnit;
		T startTile;
		T endTile;
		CostEstimateDelegate costEstimateFunc;

		Queue<T> path;

		public void DoWork() {
			path = new Queue<T>();

			HashSet< T > closedSet = new HashSet<T>();

			PathfindingPriorityQueue< T > openSet = new PathfindingPriorityQueue<T>();
			openSet.Enqueue(startTile, 0);

			Dictionary<T, T> cameFrom = new Dictionary<T, T>();

			Dictionary<T, float> g_score = new Dictionary<T, float>();
			g_score[startTile] = 0;
			
			Dictionary<T, float> f_score = new Dictionary<T, float>();
			f_score[startTile] = costEstimateFunc(startTile, endTile);

			// Debug.Log("AStar Doing Something!");

			//List<T> currentNeighbors;

			while (openSet.Count > 0) {
				T current = openSet.Dequeue();

				if ( System.Object.ReferenceEquals( current, endTile) ) {
					Reconstuct_path(cameFrom, current);
					return;
				}
				

				closedSet.Add(current);

				// Debug.Log("Neighbor Count: " + current.GetNeighbors().Length);
				foreach (T neighbor in current.GetNeighbors() ) {

					if (closedSet.Contains(neighbor)) {
						continue;
					}

					// doing something weird
					float total_pathfinding_cost_to_neighbor = 
						current.AggregateTileCostToEnter(g_score[current], neighbor, theUnit);
						//

					if (total_pathfinding_cost_to_neighbor < 0) {
						// Less than zero value; Impassable by this unit!
						// Debug.Log("Imapssable Terrain!");
						continue;
					}
					// Debug.Log("total_pathfinding_cost_to_neighbor : " + total_pathfinding_cost_to_neighbor);
					float tentative_g_score = total_pathfinding_cost_to_neighbor;

					////
					//// something strange going on here
					if (openSet.Contains(neighbor) && tentative_g_score >= g_score[neighbor]) {
						continue; // not a better path, move along
					} 

					// this is either a new tile or just found a cheaper route to it
					cameFrom[neighbor] = current;
					g_score[neighbor] = tentative_g_score;

					// figure cost from neighbor to destination
					f_score[neighbor] = g_score[neighbor] + costEstimateFunc(neighbor, endTile);

					openSet.EnqueueOrUpdate(neighbor, f_score[neighbor]);

				} // foreach neighbor
			} // while
		
		}

		// debigging infinite loop/memory overflow
		int loopCount;

		private void Reconstuct_path(
			Dictionary<T, T> came_From,
			T current
		) {
			Queue<T> total_path = new Queue<T>();
			total_path.Enqueue(current); // This "final" step is the path to the goal

			int loopCount = 0;
			while (came_From.ContainsKey (current) && loopCount < 100) {
				// came_From is a map, where the key to value relation is real saying some_tile => we_got_there_from_this_tile
				loopCount++;
				current = came_From[current];
				total_path.Enqueue(current);

			}

			// At this point, total_path is a queue that is running backwards from the end tile to the start tile.
			// Time to reverse it
			path = new Queue<T>(total_path.Reverse());
			
		}
		//float costEstimate(T src, T dst) {
		//	return 1;
		//}

		public T[] GetList() {
			return path.ToArray();
		}

	}
}