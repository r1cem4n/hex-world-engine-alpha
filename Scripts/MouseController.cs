using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class MouseController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		hexMap = GameObject.FindObjectOfType<HexMap>();
		Update_CurrentFunc  = Update_DetectModeStart;
		// hexUnderMouse = hexMap.GetHexAt(20,20);

		theLineRenderer = this.transform.GetComponentInChildren<LineRenderer>();
		theLineRenderer.enabled = false;
	}

	// Generic Bookkeeping
	Vector3 lastMousePosition; //from Input.Mouse.position
	HexMap hexMap;
	Hex hexUnderMouse;
	Hex lastHexUnderMouse;
	

	public LayerMask HexTileLayerID;

	// UI Stuff

	public GameObject InfoPanel; 
	public GameObject CityScreen;

	// Camera Dragging Bookkeeping 
	int mouseDragThreshold = 2;
	Vector3 LastMouseGroundPlanePosition;

	// Unit Movement
	Unit __selectedUnit = null;
	public Unit SelectedUnit {
		get { return __selectedUnit; }
		set {
			__selectedUnit = value;
			InfoPanel.SetActive( __selectedUnit != null );
		}
	}

	City __selectedCity;
	public City SelectedCity {
		get { return __selectedCity; }
		set {
			__selectedCity = value;
			if ( __selectedCity != null ) { CitySelector(__selectedCity); }

		}
	}

	Hex[] hexPath;
	LineRenderer theLineRenderer;

	// Mouse mode switching stuff
	delegate void UpdateFunc();
	UpdateFunc Update_CurrentFunc;

	void Update() {
		
		if(Input.GetKeyDown(KeyCode.Escape)) {
			// Escape key pressed, cancel any current mouse mode
			SelectedUnit = null;
			CancelUpdateFunc();
		}
		hexUnderMouse = MouseToHex();

		Update_CurrentFunc();

		Update_ScrollZoom();

		lastMousePosition = Input.mousePosition;
		lastHexUnderMouse = hexUnderMouse;


		// draw unit path
		if (SelectedUnit != null) {
			DrawPath( (hexPath != null) ? hexPath : SelectedUnit.GetHexPath() );
		} else {
			DrawPath(null);
		}

	}

	void DrawPath(Hex[] hexPath) {
		if ( hexPath == null ) { 
			theLineRenderer.enabled = false;
			return; 
		}
		if (hexPath.Length == 0) {
			theLineRenderer.enabled = false;
			return;
		}
		theLineRenderer.enabled = true;

		Vector3[] ps = new Vector3[ hexPath.Length ];
		for (int i = 0; i < hexPath.Length; i++ )	{
			GameObject hexGO = hexMap.GetHexGO(hexPath[i]);
			ps[i] = hexGO.transform.position + Vector3.up * 0.15f;
		}
		theLineRenderer.positionCount = ps.Length;
		theLineRenderer.SetPositions( ps );
	}
	void CancelUpdateFunc() {
		Update_CurrentFunc = Update_DetectModeStart;
		// also clean up any UI stuff, or anything related to current mouse mode

		// TODO: turn off city view
		CityScreen.SetActive( false );
		
		hexPath = null;
	}

	void Update_DetectModeStart () {
		// check here if mouse is above a UI element. If so, ignore all of the following....
		if (EventSystem.current.IsPointerOverGameObject() ) {
			return;
			//do we want to ignore all gui objects?
		}
		

		if (Input.GetMouseButtonDown (0)) {
			// Left mouse button went down
			//
		} else if (Input.GetMouseButtonUp(0)) {
			// Left Click
			Debug.Log("Mouse Up - CLICK");
			
			// if clicking a second time on a tile with a unit, deselects the unit
			if (SelectedUnit != null && SelectedUnit.Hex == hexUnderMouse) {
				SelectedUnit = null;
				Update_CurrentFunc = CancelUpdateFunc;
				
			} else {

				// Check to see if unit is on tile
				//		If yes, select the unit
				Unit[] us = hexUnderMouse.Units;

				// TODO - add support/cycling for multiple units in same tile

				if (us.Length > 0 ) {
					SelectedUnit = us[0];
					
				}
			}
		} else if (Input.GetMouseButton(0) && 
			Vector3.Distance(Input.mousePosition, lastMousePosition) > mouseDragThreshold) {
			// Left mouse button is being held down, and mouse is moving
			// Drag camera around map
			Update_CurrentFunc = Update_CameraDrag;
			LastMouseGroundPlanePosition = MouseToGroundPlane(Input.mousePosition);
			Debug.Log("Mouse Down - DRAG");
			//Update_CurrentFunc();
		} else if (Input.GetMouseButton(1)) {
			//
			if (SelectedUnit != null) {
				// We havea selected unit and right mouse is down
				// Movement Mode - do pathfinding to mouse position
				Update_CurrentFunc = Update_UnitMovement;

				//Update_CurrentFunc();
			}
			//
		}
	}

	// sets the unit's movement path
	void Update_UnitMovement () {
		if( Input.GetMouseButtonUp(1) || SelectedUnit == null) {
			Debug.Log("End Unit Movement");
				//copy over pathfinding before exiting
				if(SelectedUnit != null ) {
					SelectedUnit.SetHexPath(hexPath); 
					StartCoroutine( hexMap.DoUnitMoves(SelectedUnit) );
				}

			Update_CurrentFunc = CancelUpdateFunc;
			return;
		}
		//
		// we have a selected unit
		//
		// pass pathfinding data for potential unit path

		if( hexUnderMouse != null && (hexPath == null || hexUnderMouse != lastHexUnderMouse) ) {
			hexPath = QPath.QPath.FindPath<Hex>(
				hexMap, 
				SelectedUnit, 
				SelectedUnit.Hex, 
				hexUnderMouse, 
				Hex.CostEstimate);

		}

		
	}

	
	// Update is called once per frame
	void Update_CameraDrag () {
		 if( Input.GetMouseButtonUp(0) ) {
			CancelUpdateFunc();
			return;
		 }
		// camera controls
		
		Vector3 hitPos = MouseToGroundPlane(Input.mousePosition);
		//
		
		Vector3 diff = LastMouseGroundPlanePosition - hitPos;
		Camera.main.transform.Translate(diff, Space.World);

		LastMouseGroundPlanePosition = hitPos = MouseToGroundPlane(Input.mousePosition);

		// zoom to mouse point
	
		
	}

	void Update_ScrollZoom() {
		float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
		float minHeight = 2f;
		float maxHeight = 25f;
		float lowZoom = 8f;
		float highZoom = 15f;
		Vector3 hitPos = MouseToGroundPlane(Input.mousePosition);

		// Debug.Log("Camera");
		
			if(Mathf.Abs(scrollAmount) > 0.01f) {
				Vector3 dir = hitPos - Camera.main.transform.position;
				Vector3 p = Camera.main.transform.position;

				if(scrollAmount > 0 || p.y < maxHeight - 0.1f ) {
					Camera.main.transform.Translate(dir * scrollAmount, Space.World);
				}

				p = Camera.main.transform.position;
				if (p.y <= minHeight) { 
					p.y = minHeight; 
				} else if (p.y > maxHeight) { 
					p.y = maxHeight; 
				}
				Camera.main.transform.position = p;

				if (p.y < lowZoom) {
					Camera.main.transform.rotation = Quaternion.Euler(
						Mathf.Lerp(25, 60, (p.y - minHeight) / (lowZoom - minHeight)),
						Camera.main.transform.rotation.eulerAngles.y,
						Camera.main.transform.rotation.eulerAngles.z
					);
				}
				else if (p.y > highZoom) {
					Camera.main.transform.rotation = Quaternion.Euler(
						Mathf.Lerp(60, 90, (p.y - highZoom) / (maxHeight - highZoom)),
						Camera.main.transform.rotation.eulerAngles.y,
						Camera.main.transform.rotation.eulerAngles.z
					);
				}
				else {
					Camera.main.transform.rotation = Quaternion.Euler(
						60,
						Camera.main.transform.rotation.eulerAngles.y,
						Camera.main.transform.rotation.eulerAngles.z
					);
				}
				
			}
	} 
	
	void Update_CityView() {
		//
		// flip on city screen
		if (SelectedCity == null) {
			CancelUpdateFunc();
		}
		
		// EnableCityScreenView( SelectedCity );
	}
	Hex MouseToHex() {
		Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitInfo;

		int layerMask = HexTileLayerID.value;
		// Debug.Log("Layermask " + layerMask);
		if( Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, layerMask)) {
			//
			//Debug.Log("Raycast HitInfo: " + hitInfo.collider.transform.parent.name);
			// the collider is the child of the hex we need to return
			
			GameObject hexGO = hitInfo.collider.transform.parent.gameObject;

			if(hexGO == null) { Debug.Log("hexGO in MouseToHex() is NULL!!!"); }
			
			// Hex h = hexMap.GetHexFromGameObject(hexGO);
			Hex h = hexGO.GetComponent<HexComponent>().Hex; //hexMap.GetHexFromGameObject( hexGO );
			//hexGO.GetComponent<HexComponent>().Hex 
			//Debug.Log("Mouse To Hex Findings: " + h.Q);
			return h;
			
		}
		return null;
	}

	Vector3 MouseToGroundPlane (Vector3 mousePos) {
		Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
		// find where mouse ray intersects y=0
		float rayLength = (mouseRay.origin.y / mouseRay.direction.y);
		return mouseRay.origin - (mouseRay.direction * rayLength);
	}
	
	
	public void CitySelector ( City city) {
		CancelUpdateFunc();

		Update_CurrentFunc = Update_CityView;
		SelectedUnit = null;
		Camera.main.GetComponent<CameraMotion>().PanToHex(city.Hex);
		CityScreen.SetActive( true );
		
		//SelectedCity = city;

	}
	// ui controls

		// hex select

		// unit select
}
