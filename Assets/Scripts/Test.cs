using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {

		Vector3 norm;

		Vector3[] vertices;
		List<int> indices;

		vertices = GetComponent<MeshFilter>().mesh.vertices;
		indices = new List<int>(GetComponent<MeshFilter>().mesh.triangles);
		int count = indices.Count / 3;
		norm = transform.localPosition.normalized;

		Debug.Log (norm);
		for (int j = count-1; j >= 0; j--)
		{
			Vector3 V1 = vertices[indices[j*3 + 0]];
			Vector3 V2 = vertices[indices[j*3 + 1]];
			Vector3 V3 = vertices[indices[j*3 + 2]];
			float t1 = V1.x*norm.x+V1.y*norm.y+V1.z*norm.z;
			float t2 = V2.x*norm.x+V2.y*norm.y+V2.z*norm.z;
			float t3 = V3.x*norm.x+V3.y*norm.y+V3.z*norm.z;
			if(norm != Vector3.zero){
				if (t1 < 0.01f && t2 < 0.01f && t3 < 0.01f)
					indices.RemoveRange(j*3, 3);

			}

				
		}

		GetComponent<MeshFilter>().mesh.triangles = indices.ToArray();





	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
