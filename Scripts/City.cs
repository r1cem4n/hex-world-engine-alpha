using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class City : MapObject {
    public City() {
        /// Cities!
        /// Map object type
        Name = "Spook City";
        Population = 1;
        BuildingsAvailable = BuildingType.Monument & BuildingType.Granary;
        Buildings = BuildingType.None;
        //this.hexMap = GameObject.FindObjectOfType<HexMap>();
        

    }

    //HexMap hexMap;
    public int Population;
    public int GoldPerTurn;

    /* void Awake() {
        Debug.Log("City::Awake");    
        TakeOwnershipOfHex();
    } */
   

    public List<Hex> OwnedHexes { get; protected set; }


    public enum BuildingType {
        None        = 0x0,      // 0000 0000
        Monument    = 1,        // 0000 0001
        Granary     = 1 << 1,   // 0000 0010
        Barracks    = 1 << 2,
        Workshop    = 1 << 3

    }
    
    public BuildingType Buildings;
    public BuildingType BuildingsAvailable;

    BuildQueueItem buildJob;

    public void TakeOwnershipOfHex ( Hex hex ) {

        // check if hex is already under ownership
        if (hex.IsOwned == false) {
            OwnedHexes.Add( hex );
            hex.OwnedByFactionID = FactionID;
            Debug.Log("Hex added to City ownership!");

        }
    }
    public void TakeOwnershipOfHex ( Hex[] hexes ) {
        foreach (Hex hex in hexes) {
            TakeOwnershipOfHex( hex );
        }
    } 
    public void TakeOwnershipOfHex () {
        if (OwnedHexes == null) {
            OwnedHexes = new List<Hex>();
            TakeOwnershipOfHex( Hex.GetNeighboringHexes() );
        } else {
            Debug.Log("Improper use of City::TakeOwnershipOfHex() !!!");
        }
    }

    public void RemoveOwnershipOfHex ( Hex h ) {
        if (OwnedHexes.Contains(h)) {
            OwnedHexes.Remove( h );
            Debug.Log("City lost ownership of hex!");
        }
    }

    public void DoTurn() {
        // Check build queue, go back to build selection if not ready

        // Do Production queue
        // Calculate income/bonuses (or debt)
    }

    

    override public void SetHex( Hex hex ) {
        base.SetHex( hex ); 

        Hex.AddCity(this);
    } 
}