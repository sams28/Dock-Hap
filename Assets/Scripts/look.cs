using UnityEngine;
using System.Collections;

public class look : MonoBehaviour {


	public GameObject v;

	// Use this for initialization
	void Start () {


	
	}
	
	// Update is called once per frame
	void Update () {
		//transform.localRotation.LookRotation (Vector3.zero);
		transform.LookAt(Vector3.zero);

	
	}
}
