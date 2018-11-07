using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QPath;
using System.Linq;

public class HexMap : MonoBehaviour, IQPathWorld {


    public GameObject HexPrefab;
    public GameObject ForestPrefab;
    public GameObject UnitSkeletonPrefab;
    public GameObject CityPrefab;

    public Mesh MeshWater;
    public Mesh MeshFlat;
    public Mesh MeshHill;
    public Mesh MeshMountain;
    public Mesh MeshForest;


    //public Material[] HexMaterials;

    public Material MatLake;
    public Material MatGrassland;
    public Material MatForest;
    public Material MatMountains;
    public Material MatShore;
    public Material MatArid;
    public Material MatMarsh;
    public Material MatOcean;

    [System.NonSerialized] public int numRows = 60;
    [System.NonSerialized] public int numColumns = 100;

    [System.NonSerialized] public float ElevationMountains = 1.249f;
    [System.NonSerialized] public float ElevationHills = 0.75f;
    [System.NonSerialized] public float ElevationFlat = 0.0f;
    [System.NonSerialized] public float ElevationShore = 0.125f;
    [System.NonSerialized] public float ElevationLake = -0.325f;

    [System.NonSerialized] public float MoistureArid = 0.05f;
    [System.NonSerialized] public float MoistureGrassland = 0.059f;
    [System.NonSerialized] public float MoistureForest = 0.35f;
    [System.NonSerialized] public float MoistureJungle = 0.55f;
    [System.NonSerialized] public float MoistureMarsh = 0.75f;

    private static Hex[,] hexes;
    private static Dictionary<Hex, GameObject> hexToGameObjectMap;
    private Dictionary<GameObject, Hex> gameObjectToHexMap;
    

    // TODO: Make Seperate Unit Lists For Each Player
    private HashSet<Unit> units;
    private Dictionary<Unit, GameObject> unitToGameObjectMap;

    private HashSet<City> cities;
    private Dictionary<City, GameObject> cityToGameObjectMap;

    public delegate void CityCreatedDelegate ( City city, GameObject cityGO );
    public event CityCreatedDelegate OnCityCreated;


    // Use this for initialization
    void Start() {
        hexes = new Hex[numColumns, numRows];
        hexToGameObjectMap = new Dictionary<Hex, GameObject>();
        gameObjectToHexMap = new Dictionary<GameObject, Hex>();
        //cityToGameObjectMap = new Dictionary<City, GameObject>();

        //OnCityCreated = null;
        GenerateMap();

        

        Unit unit = new Unit();
        City city = new City();

        SpawnUnitAt(unit, UnitSkeletonPrefab, 69, 34);

        //SpawnCityAt(city, 71, 35);
        //OnCityCreated(city, cityToGameObjectMap[city]);
        //SpawnUnitAt(unit, UnitSkeletonPrefab, 69, 35);
        //
        //

    }

    public bool AnimationIsPlaying = false;

    

    void Update() {
        // 
        if(Input.GetKeyDown(KeyCode.Space)){
            //StartCoroutine( DoAllUnitMoves() );
            EndTurn();
        }
        

    }

    IEnumerator DoAllUnitMoves() {
        if(units != null) {
                foreach(Unit u in units) {
                    yield return DoUnitMoves(u);
                    
                }
            }
        }
    public IEnumerator DoUnitMoves( Unit u ) {
         while( u.DoMove() ) {
                        Debug.Log("DoMove retruned true -- will be called again");
                        //
                        // TODO: Check to see if an animation is playing, and pause here to wait for it to finish
                        //
                        while(AnimationIsPlaying) { 
                            yield return null; 
                        }
                    }
    }

    public void EndTurn() {
        // reset the turn, check for additional unit moves needed to be had

        // whole bunch of wacky stuff

        // reset movement
        foreach (Unit u in units)
        {
            u.ResetMovement();
        }
    }


    public Hex GetHexAt(int x, int y)
    {
        if (hexes == null) {
            Debug.LogError("Hexes array not instantiated!");
            return null;
        }
        x = (x + numColumns) % numColumns;

        // return hexes[(x + numColumns) % numColumns, (y + numRows) % numRows];
        try {
            return hexes[x, y];
        } catch {
            Debug.Log("Trying to pass a null hex!! " + x + ", " + y);
            return null;
        }
    }

    public Vector3 GetHexPosition(int q, int r) {
        Hex hex = GetHexAt(q, r);
        return GetHexPosition(hex);
    }
    public Vector3 GetHexPosition(Hex hex) {
        return hex.PositionFromCamera(Camera.main.transform.position, numRows, numColumns);
    }
    

    virtual public void GenerateMap()
    {
        // Array for hex info
        

        // Map Generation
        for (int column = 0; column < numColumns; column++) {
            for (int row = 0; row < numRows; row++) {

                // Instantiate
                Hex h = new Hex(this, column, row);
                
                h.Elevation = -0.5f;

                Vector3 pos = h.PositionFromCamera(
                        Camera.main.transform.position,
                        numRows,
                        numColumns
                    );

                GameObject hexGO = (GameObject)Instantiate(
                    HexPrefab,
                    pos,
                    Quaternion.identity,
                    this.transform
                    );

                hexes[column, row] = h;

                //Debug.Log("gameObjectToHexMap generation debug: " + gameObjectToHexMap[hexGO].Q );
                gameObjectToHexMap.Add(hexGO, h);
                hexToGameObjectMap.Add(h, hexGO);

                h.TerrainType = Hex.TERRAIN_TYPE.WATER;
                h.ElevationType = Hex.ELEVATION_TYPE.OCEAN;
                h.FeatureType = Hex.FEATURE_TYPE.NONE;

                hexGO.name = string.Format("Hex {0},{1}", column, row);
                hexGO.GetComponent<HexComponent>().Hex = h;
                hexGO.GetComponent<HexComponent>().HexMap = this;

                

            }
        }

        UpdateHexVisuals();

        /* Unit unit = new Unit();
        City city = new City();

        SpawnUnitAt(unit, UnitSkeletonPrefab, 69, 34);

        SpawnCityAt(city, 71, 35);
        //SpawnUnitAt(unit, UnitSkeletonPrefab, 69, 35);
        //
        // */
    }

    //
    public void UpdateHexVisuals()
    {
        for (int column = 0; column < numColumns; column++) {
            for (int row = 0; row < numRows; row++) {
                // Update Hex Meshes
                Hex h = hexes[column, row];
                GameObject hexGO = hexToGameObjectMap[h];

                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();

                h.FeatureType = Hex.FEATURE_TYPE.NONE;
                
                
                if (h.Elevation >= ElevationFlat) {
                    h.ElevationType = Hex.ELEVATION_TYPE.FLAT;
                    
                    if (h.Moisture >= MoistureMarsh) { 
                        mr.material = MatMarsh; 
                        h.FeatureType = Hex.FEATURE_TYPE.MARSH;
                        
                    } else if (h.Moisture >= MoistureJungle) {
                        mr.material = MatForest; 
                        h.FeatureType = Hex.FEATURE_TYPE.JUNGLE;
                        //Spawn Trees
                        GameObject.Instantiate(ForestPrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                    } else if (h.Moisture >= MoistureForest) {
                        mr.material = MatForest; 
                        h.FeatureType = Hex.FEATURE_TYPE.FOREST;
                        //Spawn Trees
                        GameObject.Instantiate(ForestPrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                    } else if (h.Moisture >= MoistureGrassland) {
                        mr.material = MatGrassland; 
                        h.FeatureType = Hex.FEATURE_TYPE.GRASSLAND;
                    } else {
                        mr.material = MatArid; 
                        
                    }

                }

                if (h.Elevation >= ElevationMountains) {
                    h.ElevationType = Hex.ELEVATION_TYPE.MOUNTAIN;
                    mr.material = MatMountains;
                    mf.mesh = MeshMountain; 
                    
                }
                else if (h.Elevation < ElevationMountains && h.Elevation >= ElevationHills) {
                    // Set To hill mesh & material
                    h.ElevationType = Hex.ELEVATION_TYPE.HILLS;
                    mf.mesh = MeshHill; 
                    
                }
                else if (h.Elevation < ElevationHills && h.Elevation >= ElevationShore) {
                    h.ElevationType = Hex.ELEVATION_TYPE.FLAT;
                    mf.mesh = MeshFlat; 
                    // plain old flat ground
                }
                else if (h.Elevation < ElevationShore && h.Elevation >= ElevationFlat) {
                    // low lying shore land
                    h.ElevationType = Hex.ELEVATION_TYPE.FLAT;
                    mr.material = MatShore;
                    mf.mesh = MeshFlat; 
                    
                }

                if ( h.Elevation < 0f ) {
                    // negative elevation = body of water
                    mf.mesh = MeshWater;
                    if (h.Elevation > ElevationLake) { 
                        mr.material = MatLake;
                        h.ElevationType = Hex.ELEVATION_TYPE.LAKE;

                        
                    } else {
                        mr.material = MatOcean; 
                        h.ElevationType = Hex.ELEVATION_TYPE.OCEAN;
                    }
                    
                }



                // hex debug label

                // hexGO.GetComponentInChildren<TextMesh>().text = h.Elevation.ToString("0.00");
                // hexGO.GetComponentInChildren<TextMesh>().text = h.BaseMovementCost().ToString("0.00");
                hexGO.GetComponentInChildren<TextMesh>().text = "  ";
            }
        }

    }
    public Hex[] GetHexesWithinRangeOf(Hex centerHex, int range)
    {
        List<Hex> results = new List<Hex>();

        for (int dx = -range; dx < range-1; dx++) {
            for (int dy = Mathf.Max(-range+1, -dx-range); dy < Mathf.Min(range, -dx+range-1); dy++){
                int xx = (centerHex.Q + dx + numColumns) % numColumns;
                //int xx = centerHex.Q + dx;
                int yy = (centerHex.R + dy + numRows) % numRows;

                results.Add(hexes[xx, yy]);
            }
        }

        return results.ToArray();
    }

    public Hex GetHexFromGameObject(GameObject hexGO) {
        /* if(hexGO != null && gameObjectToHexMap != null) {
            Debug.Log("hexGameObject & gameObjectToHexMap not null");
            if( gameObjectToHexMap.ContainsKey(hexGO) ) {
                Debug.Log("Passed this test");
                return gameObjectToHexMap[hexGO]; 
            }
            return null;
        } */
        if (gameObjectToHexMap == null ) { Debug.Log("gameObjectToHexMap is NULL!"); }
        if( gameObjectToHexMap.ContainsKey(hexGO) )
        {
            return gameObjectToHexMap[hexGO];
        }
        return null;
    }

    public GameObject GetHexGO( Hex h) {
        //Debug.Log("hexToGameObjectMap: " + hexToGameObjectMap.Count);
        if (hexToGameObjectMap.ContainsKey(h)) {
            return hexToGameObjectMap[h];
        }
        return null;
    }

    public void SpawnUnitAt ( Unit unit, GameObject PreFab, int q, int r) 
    {
        if(units == null) {
            units = new HashSet<Unit>();
            unitToGameObjectMap = new Dictionary<Unit, GameObject>();
        }
        
        GameObject myHex = hexToGameObjectMap[GetHexAt(q, r)];
        unit.SetHex(GetHexAt(q, r));
        GameObject unitGO = (GameObject)Instantiate(PreFab, myHex.transform.position, Quaternion.identity, myHex.transform);
        unit.OnObjectMoved += unitGO.GetComponent<UnitView>().OnUnitMoved;

        //unit.OnUnitMoved(myHex, myHex); // TEMP
        
        units.Add(unit);
        unitToGameObjectMap[unit] = unitGO;

    }

    public void SpawnCityAt( City city, int q, int r) {
        
        if(cities == null) {
            cities = new HashSet<City>();
            cityToGameObjectMap = new Dictionary<City, GameObject>();
        }

        Hex myHex = GetHexAt(q, r);
        if (myHex.ContainsCity) {
            Debug.Log("Hex already contains city!");
            return;
        }



        GameObject myHexGO = hexToGameObjectMap[myHex];
        city.SetHex(myHex);

        GameObject cityGO = (GameObject)Instantiate(CityPrefab, myHexGO.transform.position, Quaternion.identity, myHexGO.transform);

        cities.Add(city);
        cityToGameObjectMap[city] = cityGO;

        cityGO.name = string.Format("City: {0}", city.Name);
        //cityGO.GetComponentInChildren<TextMesh>().text = city.Name;

        Debug.Log(city.Name + " founded at " + q + ", " + r);
        myHex.ContainsCity = true;

        city.TakeOwnershipOfHex();

        if( OnCityCreated != null) { OnCityCreated(city, cityGO);} 
        
    }
}
