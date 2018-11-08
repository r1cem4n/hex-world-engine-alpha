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
    public List<Hex> HexesWorked { get; protected set; }


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


    //
    // Hex Ownership
    //

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
            // It is likely that this function is being ran for the first time, so let's take ownership of the tile we're on and it's neighbors
            OwnedHexes = new List<Hex>();
            TakeOwnershipOfHex( this.Hex ); 
            TakeOwnershipOfHex( Hex.GetNeighboringHexes() );
        } else {
            Debug.Log("Improper use of City::TakeOwnershipOfHex() !!!");
            // want to change this eventually
        }
    }

    public void RemoveOwnershipOfHex ( Hex h ) {
        if (OwnedHexes.Contains(h)) {
            OwnedHexes.Remove( h );
            h.IsOwned = false;
            Debug.Log("City lost ownership of hex!");
        }
    }

    //
    // Buildings
    //
    // Maybe implement buildings as a list of function delegates that contain very simple equations and algorithms ??

    public void AddBuilding( BuildingType b ) {
        // check if building is already built, then add it to built items list (??)

        // this will be ran when build jobs complete 
    }
    public void RemoveBuilding( BuildingType b ) {
        // check if building is already built, then remove it from built items list (??)
    }

    // Turn Stuff
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