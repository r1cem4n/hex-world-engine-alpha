using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapContinent : HexMap {

    public override void GenerateMap()
    {
        base.GenerateMap();
        Random.InitState(100);

        // Make some  type of raised area
        int numContinents = 2;
        int continentSpacing = numColumns / numContinents;
        int contSpaceToForm = (numColumns - (continentSpacing/numContinents)) / numContinents;
        int contLatitude = numRows / numContinents - 1;
        float mapAspectRatio = numColumns / numRows;

        Debug.Log("Cont Space To Form: " + contSpaceToForm.ToString() + 
                " Cont Latitude Spacing: " + contLatitude.ToString() + 
                " Map Aspect Ratio: " + mapAspectRatio.ToString("0.00"));

        for (int c = 0; c < numContinents; c++) {
            int Asteroids = Random.Range(contLatitude * 3 / 4, contLatitude) + (numContinents % (c + 1));
            for (int i = 0; i < Asteroids; i++) {
                int range = Random.Range(5, 8);
                int y = Random.Range(range, contLatitude) + (c * contLatitude);
                int x = Random.Range(range, contSpaceToForm) + c * continentSpacing;
                float elevation = Random.Range(0.15f, 0.85f);

                ElevateArea(x, y, range, c, elevation);

            }
        }
        // Add some noise
        float noiseResolution = 0.25f;
        float noiseScale = 2f;
        Vector2 noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

        for (int column = 0; column < numColumns; column++) {
            for (int row = 0; row < numRows; row++) {
                Hex h = GetHexAt(column, row);
                float n = Mathf.PerlinNoise( (((float)column / numColumns / noiseResolution * mapAspectRatio)) + noiseOffset.x,
                    ((float)row / numRows / noiseResolution) + noiseOffset.y) - 0.5f;
                h.Elevation += noiseScale * n;
                if (h.Elevation > 0 && h.Elevation < 0.005f) { h.Elevation += 0.005f; }
                if (h.Elevation > 1.875f) { h.Elevation -= Random.Range(0.125f, 0.825f); }
                //if (h.Elevation > 1.125f) { h.Elevation = Mathf.Sqrt(h.Elevation); }

            }
        }
        // Smooth Map Seam
        SmoothSeam();
        // Rainfall & moisture

        float moistureNoiseRes = 1f;
        //float moistureNoiseScale = 0.1f;
        Vector2 moistureNoiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f,1f));
        int maxDistanceFromEquator = (numRows / 2);

        for (int column = 0; column < numColumns; column++)
        {
            for (int row = 0; row < numRows; row++)
            {
                Hex h = GetHexAt(column, row);
                float n = 0.5f;
                int distanceFromEquator = Mathf.Abs(row - (numRows / 2)) + 1;

                
                float moistureNoise = Mathf.PerlinNoise((((float)column / numColumns / moistureNoiseRes) + moistureNoiseOffset.x),
                    ((float)row / numRows / moistureNoiseOffset.y)) + 0.5f;

                if (h.Elevation <= 0) {
                    n = (1.25f - (float)distanceFromEquator / 100f);
                    
                } else {
                    n = (float)(maxDistanceFromEquator - distanceFromEquator) / (h.Elevation * 100f); //1f - (h.Elevation / );
                    if (n > 1f) { n = Mathf.Sqrt(n); }
                    
                }
                h.Moisture = Mathf.Abs(n * moistureNoise);
            }
        }

        
        // Set meshes
        UpdateHexVisuals();
    }

    void ElevateArea(int q, int r, int range, int c, float centerHeight = 1f)
    {
        Hex centerHex = GetHexAt(q, r);
        //centerHex.Elevation = 0.5f;

        Hex[] areaHexes = GetHexesWithinRangeOf(centerHex, range);

        foreach(Hex h in areaHexes) {
            //if (h.Elevation < 0) { h.Elevation = 0; }
            h.Elevation += centerHeight * Mathf.Lerp(0.9f, 0.25f, Hex.Distance(centerHex, h) / range);
            h.ContinentID = c;
        }

    }

    void SmoothSeam () {
        for (int i = 0; i < numRows; i++) {
            Hex a = GetHexAt(0, i);
            Hex b = GetHexAt(numColumns - 1, i);

            if (Mathf.Abs(a.Elevation - b.Elevation) > 0.075f ) {
                float elevationAdjust = Mathf.Abs(a.Elevation - b.Elevation) / 3f;
                Debug.Log("Elevation Adjust: " + elevationAdjust);
                if (a.Elevation < b.Elevation ) {
                    a.Elevation += elevationAdjust;
                    b.Elevation -= elevationAdjust;
                } else {
                    a.Elevation -= elevationAdjust;
                    b.Elevation += elevationAdjust;
                }
            }
        }
    }

    

}
