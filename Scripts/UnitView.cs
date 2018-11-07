using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour {
	void Start () {
		newPos = this.transform.position;
		//newPos = oldPos;
		//newPos = this.transform.position + Vector3.right;
	}
	Vector3 oldPos;
	Vector3 newPos;
	Vector3 currentVelocity;
	float smoothTime = 0.5f;
	public void OnUnitMoved( Hex oldHex, Hex newHex ) {
		// Animate hex moving from old to new
		this.transform.position =  oldHex.PositionFromCamera();
		newPos =  newHex.PositionFromCamera();
		currentVelocity = Vector3.zero;
		
		if(Vector3.Distance(this.transform.position, newPos) > 2 ) 
		{
			// teleport super long distances to prevent headaches
			this.transform.position = newPos;

		} else {
			// TODO: Need to figure out animation queueing...
			GameObject.FindObjectOfType<HexMap>().AnimationIsPlaying = true;
		}
	}

	void Update() {
		this.transform.position = Vector3.SmoothDamp(this.transform.position, newPos, ref currentVelocity, smoothTime );

		// TODO: Figure out the best way to determine the end of our animation

		if( Vector3.Distance( this.transform.position, newPos) < 0.01f ) {
			GameObject.FindObjectOfType<HexMap>().AnimationIsPlaying = false;
		}
	}
}
