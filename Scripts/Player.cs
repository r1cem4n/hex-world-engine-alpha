using System;
using System.Collections;
using System.Collections.Generic;
using Unity;
using System.Linq;

public class Player {
    public Player ( string name ) {
        PlayerName = name;
        FactionID = 1;
    }

    public string PlayerName;
    public int FactionID;

    public enum PlayerType { LOCAL, AI, REMOTE };
    public PlayerType Type = PlayerType.LOCAL;

    private HashSet<Unit> units;
    private HashSet<City> cities;

    public Unit[] Units {
        get { return units.ToArray(); }
    }
}