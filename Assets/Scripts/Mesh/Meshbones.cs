using UnityEngine;
using System.Collections;

public class Meshbones : MonoBehaviour {


	public Vector3 v=Vector3.zero;
	private Transform[] bones;
	private Matrix4x4[] bindPoses;
	private Mesh mesh;
	private SkinnedMeshRenderer rend;
	// Use this for initialization
	void Start () {
		gameObject.AddComponent<SkinnedMeshRenderer>();
		rend = GetComponent<SkinnedMeshRenderer>();
		mesh = new Mesh();
		mesh.vertices = new Vector3[] {new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(-1, 5, 0), new Vector3(1, 5, 0)};
		mesh.uv = new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)};
		mesh.triangles = new int[] {0, 1, 2, 1, 3, 2};
		mesh.RecalculateNormals();
		rend.material = new Material(Shader.Find("Diffuse"));
		BoneWeight[] weights = new BoneWeight[4];
		weights[0].boneIndex0 = 0;
		weights[0].weight0 = 1;
		weights[1].boneIndex0 = 0;
		weights[1].weight0 = 1;
		weights[2].boneIndex0 = 1;
		weights[2].weight0 = 1;
		weights[3].boneIndex0 = 1;
		weights[3].weight0 = 1;
		mesh.boneWeights = weights;
		bones = new Transform[4];
		bindPoses = new Matrix4x4[4];

		for (int i=0; i< 4; i++) {

			
			bones[i] = new GameObject().transform;
			bones[i].parent = transform;
			bones[i].localRotation = Quaternion.identity;


		}
		bones[0].localPosition = Vector3.zero;
		bindPoses[0] = bones[0].worldToLocalMatrix * transform.localToWorldMatrix;
		bones[1].localPosition = new Vector3(0,5,0);
		bindPoses[1] = bones[1].worldToLocalMatrix * transform.localToWorldMatrix;
		bones[2].localPosition = new Vector3(5,0,0);
		bindPoses[2] = bones[2].worldToLocalMatrix * transform.localToWorldMatrix;
		bones[3].localPosition = new Vector3(0,0,5);
		bindPoses[3] = bones[3].worldToLocalMatrix * transform.localToWorldMatrix;

		mesh.bindposes = bindPoses;
		rend.bones = bones;
		rend.sharedMesh = mesh;

	}
	
	// Update is called once per frame
	void Update () {

		bones [0].localPosition = v;

		rend.bones = bones;

	
	}
}
