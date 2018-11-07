using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexComponent : MonoBehaviour {

    public Hex Hex;
    public HexMap HexMap;
    //
    //keeps hexes/tiles postioned correctly around camera
    public void UpdatePosition()
    {
        this.transform.position = Hex.PositionFromCamera(
            Camera.main.transform.position,
            HexMap.numRows,
            HexMap.numColumns
         );
    }
}
