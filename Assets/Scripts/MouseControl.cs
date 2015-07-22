using UnityEngine;
using System.Collections;

public class MouseControl : MonoBehaviour {





	private float xDeg;
	private float yDeg;
	private float xTrans;
	private float yTrans;
	private float zTrans;
	private float xPos;
	private float yPos;
	public Vector3 center= new Vector3 (0, 0, 0);
	public float sensitivityX = 0.5f;
	public float sensitivityY = 0.5f;
	private Quaternion rot;
	// Use this for initialization
	void Start () {


	
	}
	
	// Update is called once per frame
	void Update () {

		MoveMolecule ();

	}


	public void MoveMolecule(){

		
		if (Input.GetMouseButton (0)) {
			if (Input.mousePosition.x < Screen.width * 0.85f && Input.mousePosition.y < Screen.height * 0.85f && Input.mousePosition.y > Screen.height * 0.15f) {	
				xDeg += Input.GetAxis ("Mouse X") * sensitivityX;
				yDeg += -Input.GetAxis ("Mouse Y") * sensitivityY;
			}
			else{
				xDeg += Input.GetAxis ("Mouse X") * sensitivityX;
				yDeg += Input.GetAxis ("Mouse Y") * sensitivityY;
			}
			
		} 
		else if (Input.GetMouseButton (2)) {
			xTrans += Input.GetAxis ("Mouse X") * sensitivityX;
			yTrans += Input.GetAxis ("Mouse Y") * sensitivityY;
			xPos = xTrans;
			yPos = yTrans;
		}
		
		else {
			
			xDeg=0;
			yDeg=0;
			xTrans=0;
			yTrans=0;
			
		}
		
		

		
		if (!Camera.main.orthographic){
			zTrans = Input.GetAxis ("Mouse ScrollWheel")*5;//
			
		}
		
		Camera.main.transform.RotateAround (new Vector3 (xPos+center.x, yPos+center.y,center.z), Camera.main.transform.up, xDeg);
		Camera.main.transform.RotateAround (new Vector3 (xPos+center.x, yPos+center.y,center.z), Camera.main.transform.right, yDeg);
		Camera.main.transform.Translate (new Vector3 (xTrans, yTrans, zTrans*10), Space.Self);



	}






}
