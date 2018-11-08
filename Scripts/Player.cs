using System;
using System.Collections;
using System.Collections.Generic;
using Unity;
using System.Linq;

public class Player {
    public Player ( string name, int factionId ) {
        PlayerName = name;
        FactionID = factionId;

        units = new HashSet<Unit>();
        cities = new HashSet<City>();
    }

    public string PlayerName;
    public int FactionID;
    public TechTree TechTree;

    public float GoldEarnedToDate;
    public float ManaEarnedToDate;
    public float CultureEarnedToDate;
    public float ScienceEarnedToDate;
    public float FaithEarnedToDate;
    
    //  public enum RESOURCES { GOLD, FOOD, PROD, MANA, CULTURE, SCIENCE, FAITH }

    public enum PlayerType { LOCAL, AI, REMOTE };
    public PlayerType Type = PlayerType.LOCAL;

    private HashSet<Unit> units;
    private HashSet<City> cities;

    public Unit[] Units {
        get { return units.ToArray(); }
    }
    public City[] Cities {
        get { return cities.ToArray(); }
    }

    float ProductionBonus;
    float GoldBonus;
    float ScienceBonus;
    float ManaBonus;
    float FoodBouns;
    float FaithBonus;

    public void DoWork() {
        // ensure we are researching something

        // apply research points

    }
}