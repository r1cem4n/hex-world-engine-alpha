using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraMotion : MonoBehaviour {

    Vector3 oldPosition;

	// Use this for initialization
	void Start () {
        oldPosition = this.transform.position;
	}
    public HexMap hexMap;
    public MouseController mouseController;
    //Vector3 p;
    Vector3 newPosition;
    Vector3 dir;
    Vector3 currentVelocity = Vector3.zero;

    bool panningCameraToHex = false;

	// Update is called once per frame
	void Update () {
        CheckIfCameraMoved();

        if (panningCameraToHex == true) {
            //this.transform.position = Vector3.SmoothDamp(p, newPosition, ref currentVelocity, 0.1f );
           this.transform.position = Vector3.SmoothDamp(this.transform.position, newPosition, ref currentVelocity, 0.25f );
           this.transform.rotation = Quaternion.Slerp(this.transform.rotation, cameraPanAngle, Time.deltaTime);
            if (this.transform.position == newPosition) {
                panningCameraToHex = false;
            }
        }

	}
    Quaternion cameraPanAngle = Quaternion.Euler(60, 0, 0);
    public void PanToHex(Hex hex)
    {
        //
        newPosition = hexMap.GetHexPosition( hex );
        newPosition.y = 8.25f;
        newPosition.z += -4.5f;
        newPosition.x += -0.7f; 
        
       // newPosition = cameraPanAngle;

        //dir = newPosition - Camera.main.transform.position;
        panningCameraToHex = true;
		//p = Camera.main.transform.position;
        

        //this.transform.Translate(dir * Time.deltaTime, Space.World);
        //SetPositionAndRotation(newPosition, Quaternion.Euler(60,0,0));
        
    }

    void CheckIfCameraMoved()
    {
        if(oldPosition != this.transform.position) {
            //something moved camera
            oldPosition = this.transform.position;

            HexComponent[] hexes = GameObject.FindObjectsOfType<HexComponent>();

            foreach(HexComponent hex in hexes) {
                hex.UpdatePosition();
            }
        }
    }
}
