using UnityEngine;
using System.Collections;

public class csSceneBoolean : MonoBehaviour {

	public MeshCollider[] meshColliderA;

	// Use this for initialization
	void Start () {

		// Create new GameObject
		GameObject newObject = new GameObject();
		newObject.transform.localScale*=2f;
		MeshFilter meshFilter = newObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = newObject.AddComponent<MeshRenderer>();
		meshRenderer.materials = new Material[2]{meshColliderA[0].transform.GetComponent<Renderer>().materials[0],meshColliderA[1].transform.GetComponent<Renderer>().materials[0]};
	
		// Assign booleanMesh
		BooleanMesh booleanMesh = new BooleanMesh(meshColliderA[0],meshColliderA[1]);
		//meshFilter.mesh = booleanMesh.Difference();
		//meshFilter.mesh = booleanMesh.Union();
		meshFilter.mesh = booleanMesh.Union();

		for (int i=2; i < meshColliderA.Length; i++) {
			//BooleanMesh booleanMesh2 = new BooleanMesh(meshFilter,meshColliderA[1]);
			//meshFilter.mesh = booleanMesh2.Union();

		}
	
	}	

}
