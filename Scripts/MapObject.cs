using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QPath;


abstract public class MapObject {
    public MapObject() {

    }


    public string Name;
    public int HitPoints;

    public bool CanBeAttacked = true;
    public int FactionID = 0;

    public Hex Hex { get; protected set; }

    public delegate void ObjectMovedDelegate ( Hex oldHex, Hex newHex );
	public event ObjectMovedDelegate OnObjectMoved;

    public delegate void ObjectDestroyedDelegate ( MapObject mo );
    public event ObjectDestroyedDelegate OnObjectDestroyed;

   //abstract public void SetHex ( Hex newHex );
    virtual public void SetHex( Hex newHex ) {
		Hex oldHex = Hex;

		Hex = newHex;
		
		if (OnObjectMoved != null) {
			OnObjectMoved(oldHex, newHex);
		}
	}

}