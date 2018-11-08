using System.Collections;
using System.Collections.Generic;

using System.Linq;
using UnityEngine;
using QPath;


// defines grid position, world space position, size, neighbors, etc
// does not interact with unity, purely logic
// defines individual hexes 
// 

public class Hex : IQPathTile {

    public Hex(HexMap hMap, int q, int r)
    {
        this.HexMap = hMap;

        this.Q = q;
        this.R = r;
        this.S = -(q + r);

        this.units = new HashSet<Unit>();
        this.ContainsCity = false;
        this.HexResources = new Dictionary<RESOURCES, int>();
        this.HexResources[RESOURCES.GOLD] = 0;
        this.HexResources[RESOURCES.PROD] = 0;
        this.HexResources[RESOURCES.FOOD] = 0;
        this.HexResources[RESOURCES.MANA] = 0;
    }

    public HexMap HexMap;
    public readonly int Q; // Column
    public readonly int R; // Row
    public readonly int S; // Lambda, Sum

    public bool ContainsCity;
    public City City { get; protected set;}

    HashSet<Unit> units;
    public Unit[] Units {
        get {
            return units.ToArray();
        }
    }
  
    // for axial coordinates
    public enum DIRECTION {
        NE, E, SE, SW, W, NW
    }

    public int ContinentID;

    public bool IsOwned = false;
    private int __ownedByFactionID;
    public int OwnedByFactionID {
        get {
            return __ownedByFactionID;
        }
        set {
            __ownedByFactionID = value;
            this.IsOwned = true;
        }
    }

    public float Elevation;
    public float Moisture;

    public enum TERRAIN_TYPE { PLAINS, FLOODPLAINS, DESERT, WATER }
    public enum ELEVATION_TYPE { FLAT, HILLS, MOUNTAIN, LAKE, OCEAN }
    public enum FEATURE_TYPE { NONE, GRASSLAND, FOREST, JUNGLE, MARSH, ICECAP, RIVER }

    public TERRAIN_TYPE TerrainType { get; set; }
    public ELEVATION_TYPE ElevationType { get; set; }
    public FEATURE_TYPE FeatureType { get; set; }

    // Hex Resources

    public enum RESOURCES { GOLD, FOOD, PROD, MANA, CULTURE, SCIENCE, FAITH }
    public Dictionary<RESOURCES, int> HexResources { get; set; }
    
    
    // Need to add more variables to store other hex data
    // Terrain type, resources, road, city, features, etc...

    //
    // Q + R + S = 0
    // S = -(Q + R)
    //

    static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;
    float radius = 1f;
    

    // returns world-space position
    public Vector3 Position()
    {
        
        return new Vector3(
            HexHorizontalSpacing() * (this.Q + this.R / 2f),
            this.Elevation / 2f,
            HexVerticalSpacing() * this.R
        );
         
    }

    // hex dimension functions
    public float HexHeight()
    {
        return radius * 2;
    }
    public float HexWidth()
    {
        return WIDTH_MULTIPLIER * HexHeight();
    }
    public float HexVerticalSpacing()
    {
        return HexHeight() * 0.75f;
    }
    public float HexHorizontalSpacing ()
    {
        return HexWidth();
    }

    //public static float Distance(Hex a, Hex b)
    //{
    //    return
    //        Mathf.Max(
    //            Mathf.Abs(a.Q - b.Q),
    //            Mathf.Abs(a.R - b.R),
    //            Mathf.Abs(a.S - b.S)
    //        );
    //}

    public static float CostEstimate ( IQPathTile aa, IQPathTile bb) {
        // heuristic function for a-star. Maybe pad distance based on tile type
        // to smooth out pathfinding
        return Distance((Hex)aa, (Hex)bb);
    }

    // returns exact distance between 2 hexes
    public static float Distance(Hex a, Hex b)
    {
    

        float dQ = Mathf.Abs(a.Q - b.Q);

        // fix for wrapping around x axis
        if (dQ >= (a.HexMap.numColumns / 2)) {
            dQ = a.HexMap.numColumns - dQ;
            // Debug.Log("Distance Wrap Occured! dQ:" + dQ.ToString("00"));
        }

        return
            Mathf.Max(
                dQ,
                //Mathf.Abs(a.Q - b.Q),
                Mathf.Abs(a.R - b.R),
                Mathf.Abs(a.S - b.S)
            );
    }

    



    // wrap camera positioning
    //

    public Vector3 PositionFromCamera() {
        return HexMap.GetHexPosition(this);
    }
    public Vector3 PositionFromCamera( Vector3 cameraPosition, float numRows, float numColumns)
    {
        //float mapHeight = numRows * HexVerticalSpacing();
        float mapWidth = numColumns * HexHorizontalSpacing();

        Vector3 position = Position();

        float howManyWidthsFromCamera = (position.x - cameraPosition.x) / mapWidth;

        if (howManyWidthsFromCamera > 0) {
            howManyWidthsFromCamera += 0.5f;
        }
        else {
            howManyWidthsFromCamera -= 0.5f;
        }
        int howManyWidthsToFix = (int)howManyWidthsFromCamera;

        position.x -= howManyWidthsToFix * mapWidth;
        return position;
    }


    public void AddUnit( Unit unit )
    {
        if(units == null) {
            units = new HashSet<Unit>();
        }

        units.Add(unit);
    }

    public void RemoveUnit( Unit unit ) {
        if (units != null) { units.Remove(unit); }
    }

  

    public void AddCity( City city )
    {
        if(this.City != null || this.ContainsCity) {
            Debug.Log("AddCity : Contains city!");
            return;
        }

        this.City = city;
    }

    public void RemoveCity( City city) {
        if(this.City == null) {
            Debug.Log("Trying to remove a non-existant city!");
            return;
            // there should be no reason why this is ever ran
        }
        if (this.City != city ) {
            Debug.Log("Trying to remove the wrong city!");
            return;
            // not sure why this should ever be ran either. Sanity checks...
        }
        this.City = null;
    }
    

    Hex[] neighbors;
    Dictionary<DIRECTION, Hex> neighborsByDirection;
    public IQPathTile[] GetNeighbors() {
        if(this.neighbors != null)
            return this.neighbors;
        
        List<Hex> neighbors = new List<Hex>();

        neighbors.Add( HexMap.GetHexAt( Q +  1,  R +  0 ) );
        neighbors.Add( HexMap.GetHexAt( Q + -1,  R +  0 ) );
        neighbors.Add( HexMap.GetHexAt( Q +  0,  R + +1 ) );
        neighbors.Add( HexMap.GetHexAt( Q +  0,  R + -1 ) );
        neighbors.Add( HexMap.GetHexAt( Q + +1,  R + -1 ) );
        neighbors.Add( HexMap.GetHexAt( Q + -1,  R + +1 ) );

        List<Hex> neighbors2 = new List<Hex>();

        foreach(Hex h in neighbors)
        {
            if(h != null)
            {
                neighbors2.Add(h);
            }
        }

        this.neighbors = neighbors2.ToArray();

        return this.neighbors; 
        //return (IQPathTile[])HexMap.GetHexesWithinRangeOf((Hex)this, 1);
    }
    public Hex[] GetNeighboringHexes() {
        if(this.neighbors != null)
            return this.neighbors;
        
        List<Hex> neighbors = new List<Hex>();

        neighbors.Add( HexMap.GetHexAt( Q +  1,  R +  0 ) );
        neighbors.Add( HexMap.GetHexAt( Q + -1,  R +  0 ) );
        neighbors.Add( HexMap.GetHexAt( Q +  0,  R + +1 ) );
        neighbors.Add( HexMap.GetHexAt( Q +  0,  R + -1 ) );
        neighbors.Add( HexMap.GetHexAt( Q + +1,  R + -1 ) );
        neighbors.Add( HexMap.GetHexAt( Q + -1,  R + +1 ) );

        List<Hex> neighbors2 = new List<Hex>();

        foreach(Hex h in neighbors)
        {
            if(h != null)
            {
                neighbors2.Add(h);
            }
        }

        this.neighbors = neighbors2.ToArray();

        return this.neighbors; 
        //return (IQPathTile[])HexMap.GetHexesWithinRangeOf((Hex)this, 1);
    }

	public float AggregateTileCostToEnter( float costSoFar, IQPathTile sourcetile, IQPathUnit theUnit ) {
        // TODO: We are ignoring source tile for now, will change soon
        // return ((Unit)theUnit).TotalTurnsToEnterHex(this, costSoFar);
        //return BaseMovementCost();
        return ((Unit)theUnit).TotalTurnsToEnterHex((Hex)sourcetile, costSoFar );
        //throw new System.NotImplementedException();
    }

}
