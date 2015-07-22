using UnityEngine;
using System.Collections;

public class MeshBuilder : MonoBehaviour {

	// Use this for initialization
	void Start () {



		Mesh mesh = new Mesh ();
		mesh.MarkDynamic ();
		GetComponent<MeshFilter> ().mesh = mesh;

		Vector3[] points = new Vector3[6];
		Color[] colors = new Color[6];
		int[] indices = new int[6];

		for (int i =0; i<6; i++) {
			indices[i] = i;


		}
		points [0] = new Vector3 (0.0f, 0.0f, 0.0f);
		points [1] = new Vector3 (1.0f, 0.0f, 0.0f);
		//points [2] = new Vector3 (1.0f, 1.0f, 0.0f);
		points [2] = new Vector3 (2.0f, 0.0f, 0.0f);

		points [3] = new Vector3 (1.0f, 1.0f, 1.0f);
		points [4] = new Vector3 (1.5f, 1.5f, 1.5f);
		points [5] = new Vector3 (2.0f, 2.0f, 2.0f);


		colors [0] = Color.red;
		colors [1] = Color.blue;
		//points [2] = new Vector3 (1.0f, 1.0f, 0.0f);
		colors [2] = Color.blue;
		
		colors [3] = Color.yellow;
		colors [4] = Color.green;
		colors [5] = Color.green;


		mesh.vertices = points;
		mesh.colors = colors;

		mesh.SetIndices(indices, MeshTopology.LineStrip,0);

	
	}
	
	// Update is called once per frame
	void Update () {



	
	}
}
