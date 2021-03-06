using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MoleculeData;
using System.Runtime.InteropServices;
using System.Threading;

public class DisplayMeshs : DisplayMolecule {




	
	private GameObject[] m_mesh;
	private List<Mesh> meshes_atm;
	private List<Mesh> meshes_bond;
	private List<Mesh> meshes_trace;
	private GameObject standard_gameobject;
	//Max size of the mesh (must be a multiple of 2,3,4)
	private const int MAX_SIZE_MESH = 64500; 

	private SkinnedMeshRenderer[] rend;
	private List<Transform> bones;


	Vector3[] points;
	int[] indices;
	Color[] colors;


	// Use this for initialization
	void Start () {


	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void DisplayMol (ColorDisplay c, TypeDisplay t,int f)
	{
		type = t;
		color = c;
		frame = f;
		switch (t) {
		case TypeDisplay.Points : DisplayMolPointCloud();break;
		case TypeDisplay.Lines :  DisplayMolLines();break;
		//case TypeDisplay.CPK : DisplayMolPointCloud();DisplayMolLines();break;
		case TypeDisplay.Surface : DisplayMolSurface();break;
		case TypeDisplay.Trace : DisplayMolTrace();break;
		default: break;
		}
	}




	
	public void DisplayMolPointCloud(){
		int i = 0;
		int index;

		meshes_atm = new List<Mesh> ();
		while (i < mol.Atoms.Count) {

			Mesh mesh = new Mesh ();
			mesh.MarkDynamic ();
			GameObject g = (GameObject)Instantiate (Resources.Load("Prefabs/SubMesh") as GameObject, Vector3.zero, Quaternion.identity);
			g.transform.SetParent(this.transform,false);
			g.GetComponent<MeshFilter> ().mesh = mesh;


			
			points = new Vector3[MAX_SIZE_MESH];
			indices = new int[MAX_SIZE_MESH];
			colors = new Color[MAX_SIZE_MESH];
			index =0;



			while (i < mol.Atoms.Count && index < MAX_SIZE_MESH) {

				if(mol.Atoms[i].Active){

				points [index] = mol.Atoms [i].Location[frame];
				colors[index] = setColorAtm (mol.Atoms [i], color);
				indices[index] =  index;
				
				index++;
				}
				i++;
			}

			mesh.vertices = points;
			mesh.colors = colors;
			mesh.SetIndices (indices, MeshTopology.Points, 0);
			mesh.RecalculateBounds();
			meshes_atm.Add (mesh);
		
		}
		
	}




	
	
	public void DisplayMolLines(){
		int i = 0;
		int index;

		meshes_bond = new List<Mesh> ();

	

		while (i <  mol.Bonds.Count) {

			Mesh mesh = new Mesh ();
			mesh.MarkDynamic ();
			GameObject g = (GameObject)Instantiate (Resources.Load ("Prefabs/SubMesh") as GameObject, Vector3.zero, Quaternion.identity);
			g.transform.SetParent (this.transform, false);
			g.GetComponent<MeshFilter> ().mesh = mesh;

			points = new Vector3[MAX_SIZE_MESH];
			indices = new int[MAX_SIZE_MESH];
			colors = new Color[MAX_SIZE_MESH];
			index =0;

			while (i < mol.Bonds.Count && index*4+3 < MAX_SIZE_MESH) {

				if(mol.Atoms [mol.Bonds [i] [0]].Active && mol.Atoms [mol.Bonds [i] [1]].Active){
					points [index * 4] = mol.Atoms [mol.Bonds [i] [0]].Location[frame];
					points [index * 4 + 1] = (mol.Atoms [mol.Bonds [i] [0]].Location[frame] + mol.Atoms [mol.Bonds [i] [1]].Location[frame])/2 ;
					points [index * 4 + 2] = (mol.Atoms [mol.Bonds [i] [0]].Location[frame] + mol.Atoms [mol.Bonds [i] [1]].Location[frame])/2 ;
					points [index * 4 + 3] = mol.Atoms [mol.Bonds [i] [1]].Location[frame];


					colors[index * 4] = setColorAtm (mol.Atoms [mol.Bonds [i] [0]], color);
					colors[index * 4 + 1] =setColorAtm (mol.Atoms [mol.Bonds [i] [0]], color);
					colors[index * 4 + 2] = setColorAtm (mol.Atoms [mol.Bonds [i] [1]], color);
					colors[index * 4 + 3] = setColorAtm (mol.Atoms [mol.Bonds [i] [1]], color);
				

					indices[index * 4] = index * 4;
					indices[index * 4 + 1] = index * 4 + 1;
					indices[index * 4 + 2] = index * 4 + 2;
					indices[index * 4 + 3] = index * 4 + 3;
					index++;
				}
				i++;
			}
			mesh.vertices = points;
			mesh.colors = colors;
			mesh.SetIndices(indices, MeshTopology.Lines,0);
			mesh.RecalculateBounds();
			meshes_bond.Add(mesh);
		}

		
	}


	public void DisplayMolTrace(){
		int i;
		int index;
		bool skip;
		meshes_trace = new List<Mesh> ();
		
		
		for (int c=0; c<mol.ChainsBonds.Count; c++) {

			i=0;
			skip = false;


			while (i <  mol.ChainsBonds[c].Count) {



				Mesh mesh = new Mesh ();
				mesh.MarkDynamic ();
				GameObject g = (GameObject)Instantiate (Resources.Load ("Prefabs/SubMesh") as GameObject, Vector3.zero, Quaternion.identity);
				g.name = mol.Atoms [mol.ChainsBonds [c] [i]].AtomChain.ChainID;
				g.transform.SetParent (this.transform, false);
				g.GetComponent<MeshFilter> ().mesh = mesh;

				if(mol.ChainsBonds[c].Count < MAX_SIZE_MESH)
				{

					points = new Vector3[mol.ChainsBonds[c].Count];
					indices = new int[mol.ChainsBonds[c].Count];
					colors = new Color[mol.ChainsBonds[c].Count];

				}
				else{
					points = new Vector3[MAX_SIZE_MESH];
					indices = new int[MAX_SIZE_MESH];
					colors = new Color[MAX_SIZE_MESH];


				}


				index =0;
				
				
				
				while (i < mol.ChainsBonds[c].Count && index < MAX_SIZE_MESH) {

					if(!mol.Atoms [mol.ChainsBonds [c] [i]].Active){

						skip = true;
						i = mol.ChainsBonds[c].Count;

					}
					else{
						points [index] = mol.Atoms [mol.ChainsBonds [c] [i]].Location[frame];
						colors[index] = setColorAtm (mol.Atoms [mol.ChainsBonds [c] [i]], color);
						indices[index] = index;
						index++;
					i++;
					}
					
					
				}

				if(!skip){
				mesh.vertices = points;
				mesh.colors = colors;

				mesh.SetIndices(indices, MeshTopology.LineStrip,0);

					meshes_trace.Add(mesh);
				}
			}

		}

		
		
	}





	public override void UpdateMol(){

		switch(type){
		case TypeDisplay.Points : UpdateAtoms();break;
		case TypeDisplay.Lines : UpdateBonds();break;
		case TypeDisplay.Trace : UpdateChains();break;
		case TypeDisplay.Surface:UpdateSurface();break;
		default:break;


		}

	}


	public void UpdateAtoms(){
		int i = 0;
		int j = 0;
		int index;
		
		while (i < mol.Atoms.Count) {
			
			points = meshes_atm[j].vertices;
			index=0;
			
			while (i < mol.Atoms.Count && index < meshes_atm[j].vertexCount) {
				
				if(mol.Atoms[i].Active){
				points[index] =  mol.Atoms[i].Location[Main.current_frame];
				
				index++;
				}
				i++;
			}
			meshes_atm[j].vertices = points;
			meshes_atm[j].RecalculateBounds();
			j++;
		}
		
		
		
		
	}


	
	public void UpdateBonds(){
		int i = 0;
		int j = 0;
		int index;
		
		while (i <  mol.Bonds.Count) {
			
			points = meshes_bond[j].vertices;
			index =0;
			
			
			while (i < mol.Bonds.Count && index*4+3 < meshes_bond[j].vertexCount) {
				if(mol.Atoms [mol.Bonds [i] [0]].Active && mol.Atoms [mol.Bonds [i] [1]].Active){
				points [index * 4] = mol.Atoms [mol.Bonds [i] [0]].Location[Main.current_frame];
				points [index * 4 + 1] = (mol.Atoms [mol.Bonds [i] [0]].Location[Main.current_frame] + mol.Atoms [mol.Bonds [i] [1]].Location[Main.current_frame])/2 ;
				points [index * 4 + 2] = (mol.Atoms [mol.Bonds [i] [0]].Location[Main.current_frame] + mol.Atoms [mol.Bonds [i] [1]].Location[Main.current_frame])/2 ;
				points [index * 4 + 3] = mol.Atoms [mol.Bonds [i] [1]].Location[Main.current_frame];
				
				index++;
			}
				i++;
			}
			meshes_bond[j].vertices = points;
			meshes_bond[j].RecalculateBounds();
			j++;
		}
		
		
		
		
	}



	public void UpdateChains(){
		int j = 0;

		for (int c=0; c<mol.ChainsBonds.Count; c++) {

			int i = 0;

			int index;

			while (i <  mol.ChainsBonds[c].Count) {
			
				points = meshes_trace [j].vertices;
				index = 0;
			
			
				while (i < mol.ChainsBonds[c].Count && index < meshes_trace [j].vertexCount) {
					points [index] = mol.Atoms [mol.ChainsBonds [c] [i]].Location[Main.current_frame];
					i++;
					index++;
				}
			
				meshes_trace [j].vertices = points;
				meshes_trace [j].RecalculateBounds ();
				j++;
			}
		
		}
		
		
	}





	
	public void DisplayMolSurface() {



		
		float resolution_surface =0.1f/scale;
		//Target is the value that represents the surface of mesh
		//For example the perlin noise has a range of -1 to 1 so the mid point is were we want the surface to cut through
		//The target value does not have to be the mid point it can be any value with in the range
		MarchingCubes.SetTarget(resolution_surface);
		
		//Winding order of triangles use 2,1,0 or 0,1,2
		//MarchingCubes.SetWindingOrder(2, 1, 0);
		
		//Set the mode used to create the mesh
		//Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface
		//MarchingCubes.SetModeToCubes();
		MarchingCubes.SetModeToCubes();


		int nbatoms=0;
		for (int v=0; v<mol.Atoms.Count; v++) {
			if (mol.Atoms [v].Active) {
				nbatoms++;
			}
		}
		float resolution = CapResolution (nbatoms);
		//Debug.Log (frame);
		//The size of voxel array. Be carefull not to make it to large as a mesh in unity can only be made up of 65000 verts
		int X = (int)(((mol.MaxValue[frame].x - mol.MinValue[frame].x ) * resolution) + 13);
		int Y = (int)(((mol.MaxValue[frame].y - mol.MinValue[frame].y ) * resolution) + 13);
		int Z = (int)(((mol.MaxValue[frame].z - mol.MinValue[frame].z ) * resolution) + 13);
		//correspond to the number vertices in a cube
		int fudgeFactor = 8;

		Vector3 offset = mol.MinValue[frame] - (new Vector3 (fudgeFactor, fudgeFactor, fudgeFactor)) / resolution;

		//Debug.Log("Density minValue :: " + resolution);
		//Debug.Log("Density point X,Y,Z :: "+ X+","+Y+","+Z);
		//Debug.Log("Density minValue :: " + mol.MinValue[frame]);

		
		float[,,] gridS = new float[X,Y,Z];
		Color[,,] VertColor = new Color[X, Y, Z];
		//int[,,][] ids = new int[X,Y,Z][10];
		List<Mesh> mesh = new List<Mesh>();


		bones = new List<Transform> ();
		List<Vector3> pos_bones = new List<Vector3> ();
		List<Matrix4x4> bindPoses = new List<Matrix4x4> ();

		GameObject  bones_g = new GameObject ("bones");;
		bones_g.transform.SetParent (this.transform, false);

		Vector3 delta = new Vector3 (resolution, resolution, resolution); //resolution
		
		// We need to refresh the molecule's origin when it's not
		// the first molecule for which we generate a surface.
		//Vector3 origin = mol.MinValue;
		Debug.Log ("Entering :: Generation of density from PDB");

		//Debug.Log ("Density point X,Y,Z :: " + X + "," + Y + "," + Z);
		//Debug.Log ("Density minValue :: " + MinValue);

		//count for each atom the grid ?
		int i;
		int j;
		int k;
		float Dist;
		Color atomColor;
		float atomRadius;
		float density;

		for (int o=0; o<mol.Atoms.Count; o++) {


			if (mol.Atoms [o].Active) {


				if (Main.options.activateBones) {
				Transform tr = new GameObject(mol.Atoms [o].AtomName).transform;
				tr.parent = bones_g.transform;
				tr.localRotation = Quaternion.identity;
				tr.localPosition = mol.Atoms [o].Location[frame];
				bones.Add(tr);
				pos_bones.Add(tr.localPosition);
				bindPoses.Add(tr.worldToLocalMatrix * mol.Gameobject[frame].transform.localToWorldMatrix);
				}

				i = Mathf.RoundToInt ((mol.Atoms [o].Location[frame].x - mol.MinValue[frame].x) * delta.x + fudgeFactor);
				j = Mathf.RoundToInt ((mol.Atoms [o].Location[frame].y - mol.MinValue[frame].y) * delta.y + fudgeFactor);
				k = Mathf.RoundToInt ((mol.Atoms [o].Location[frame].z - mol.MinValue[frame].z) * delta.z + fudgeFactor);
				Vector3 v1 = new Vector3 (i, j, k);
				
				atomRadius = mol.Atoms [o].AtomRadius;
			
				atomColor = setColorAtm (mol.Atoms [o], color);

				for (int l = i-(fudgeFactor/2 -1 ); l < i+(fudgeFactor/2); l++)
					for (int m = j-(fudgeFactor/2 -1); m < j+(fudgeFactor/2); m++)
						for (int n = k-(fudgeFactor/2 -1); n < k+(fudgeFactor/2); n++) {
						Vector3 v2 = new Vector3 (l, m, n);
						Dist = Vector3.Distance (v1, v2);
						density = (float)Math.Exp (-((Dist / atomRadius) * (Dist / atomRadius)));
							

						if (density > gridS [l, m, n]){

							VertColor [l, m, n] = atomColor;
						}
							gridS [l, m, n] += density;
						}

			}

		}

	
		//gridS = SmoothVoxels (gridS);

		mesh = MarchingCubes.CreateMesh(gridS,VertColor);


		//Bones

		if (Main.options.activateBones) {

			for (int l=0; l<mesh.Count; l++) {


				Vector3[] v = mesh [l].vertices;
				int threshold = mesh [l].vertexCount / 16;
				ManualResetEvent[] doneEvents = new ManualResetEvent[16];
				WorkChunk[] workChunks = new WorkChunk[16];
				BoneWeight[] bonesWeight = new BoneWeight[mesh [l].vertexCount];

				for (int w = 0; w < doneEvents.Length; w++) {
					workChunks [w] = new WorkChunk (v, bones.Count, bonesWeight, resolution, offset, pos_bones, w * threshold, (w + 1) * threshold);
					doneEvents [w] = workChunks [w].doneEvent;
				}
	

			
				for (int w = 0; w < doneEvents.Length; w++) {
					doneEvents [w].Reset ();
					ThreadPool.QueueUserWorkItem (workChunks [w].CalculateBones);
				
				}

				WaitHandle.WaitAll (doneEvents);
				mesh [l].boneWeights = bonesWeight;
				mesh [l].bindposes = bindPoses.ToArray ();

			}
		}

		//Normals


		Vector3[,,] normals_vox = CalculateNormals (gridS);
		for(int l=0;l<mesh.Count;l++){

			int size = mesh[l].vertices.Length;
			Vector3[] normals = new Vector3[size];
			Vector3[] verts = mesh[l].vertices;

			for(int m = 0; m < size; m++){


				normals[m] = TriLinearInterpNormal(verts[m],normals_vox);
			}
			
			mesh[l].normals = normals;

		}




		
		//Debug.Log("Entire surface contains " + mesh.vertices.Length.ToString() + " vertices.");
		//The diffuse shader wants uvs so just fill with a empty array, there not actually used
		//mesh.uv = new Vector2[mesh.vertices.Length];
		//mesh.RecalculateNormals();
		
		//m_mesh = new GameObject("Mesh");
		
		//m_mesh.GetComponent<MeshRenderer>().material = m_material;
		m_mesh = new GameObject[mesh.Count];
		rend = new SkinnedMeshRenderer[mesh.Count];
		bones_g.transform.localPosition = offset;
		bones_g.transform.localScale /= resolution;
		for (int s =0; s<m_mesh.Length; s++) {
			m_mesh[s] = new GameObject();
			m_mesh[s].transform.SetParent(this.transform,false);

			if (!Main.options.activateBones) {
			m_mesh[s].transform.localPosition = offset;
			m_mesh[s].transform.localScale /= resolution;
			m_mesh[s].AddComponent<MeshFilter>();
			m_mesh[s].AddComponent<MeshRenderer>();
			m_mesh[s].GetComponent<MeshFilter>().mesh = mesh[s];
			m_mesh[s].GetComponent<MeshRenderer>().material= Resources.Load("Materials/Surface") as Material;

			}
			else{
			m_mesh[s].AddComponent<SkinnedMeshRenderer>();
			rend[s] = m_mesh[s].GetComponent<SkinnedMeshRenderer>();
			rend[s].bones = bones.ToArray();
			rend[s].sharedMesh = mesh[s];
			rend[s].material = Resources.Load("Materials/Surface") as Material;
			}
			
		}


	}


	struct WorkChunk
	{
		// A work chunk contains an array of work items

		public Vector3[] vertices;
		public ManualResetEvent doneEvent; // a flag to signal when the work is complete
		public int start;
		public int end;
		public BoneWeight[] boneWeights;
		public List<Vector3> pos_bones;
		public float resolution;
		public Vector3 offset;
		public int bones_num;
		public WorkChunk (Vector3[] vert,int bones_num,BoneWeight[] boneWeights,float resolution,Vector3 offset,List<Vector3> pos_bones,int start,int end)
		{
			this.vertices = vert;
			this.boneWeights = boneWeights;
			this.resolution =resolution;
			this.offset = offset;
			this.pos_bones = pos_bones;
			this.start = start;
			this.end = end;
			this.bones_num =bones_num;
			this.doneEvent = new ManualResetEvent(false);
		}
		
		public void CalculateBones (System.Object o)
		{
			doneEvent.Reset();
			

			float dist;
			float minDist;
			
			for (int a = start; a< end && a < vertices.Length;a++){
				
				
				minDist = Vector3.Distance(vertices[start]/resolution +offset,pos_bones[0]);
				boneWeights[a].boneIndex0 = 0;
				
				for(int  b=0;b<bones_num;b++){
					dist =  Vector3.Distance(vertices[a]/resolution +offset,pos_bones[b]);
					
					if (dist < minDist) {
						minDist = dist;
						boneWeights[a].boneIndex0 =b;
					}
					
				}
				
				boneWeights[a].weight0 = 1;
				
				
				
			}
			
			doneEvent.Set ();
		}
	}




	private void UpdateSurface(){
		
		int j =0;
	
		for(int i=0;i<mol.Atoms.Count;i++){
			if (mol.Atoms [i].Active) {
			bones[j].localPosition= mol.Atoms[i].Location[Main.current_frame];
			
				j++;
			}
		}


		for (int i=0; i<rend.Length; i++) {

			rend[i].bones = bones.ToArray();


		}

	}




	private float CapResolution(int nbAtoms) {
		
		float resolution = 3f;
		
		if(nbAtoms > 500)
			resolution = 2.5f;
		if(nbAtoms > 1000)
			resolution = 2.2f;
		if(nbAtoms > 2000)
			resolution = 2.0f;
		if(nbAtoms > 4000)
			resolution = 1.8f;
		if(nbAtoms > 5000)
			resolution = 1.7f;
		if(nbAtoms > 6000)
			resolution = 1.6f;
		if(nbAtoms > 8000)
			resolution = 1.5f;
		if(nbAtoms > 10000)
			resolution = 1.4f;
		if(nbAtoms > 14000)
			resolution = 1.2f;
		if(nbAtoms > 20000)
			resolution = 1.0f;
		
		return resolution;
	}


	private int[,] m_sampler = new int[,] 
	{
		{1,-1,0}, {1,-1,1}, {0,-1,1}, {-1,-1,1}, {-1,-1,0}, {-1,-1,-1}, {0,-1,-1}, {1,-1,-1}, {0,-1,0},
		{1,0,0}, {1,0,1}, {0,0,1}, {-1,0,1}, {-1,0,0}, {-1,0,-1}, {0,0,-1}, {1,0,-1}, {0,0,0},
		{1,1,0}, {1,1,1}, {0,1,1}, {-1,1,1}, {-1,1,0}, {-1,1,-1}, {0,1,-1}, {1,1,-1}, {0,1,0}
	};



	private float[,,] SmoothVoxels(float[,,] m_voxels)
	{
		//float startTime = Time.realtimeSinceStartup;
		
		//This averages a voxel with all its neighbours. Its is a optional step
		//but I think it looks nicer. You might what to do a fancier smoothing step
		//like a gaussian blur
		
		int w = m_voxels.GetLength(0);
		int h = m_voxels.GetLength(1);
		int l = m_voxels.GetLength(2);
		
		float[,,] smothedVoxels = new float[w,h,l];
		
		for(int x = 1; x < w-1; x++)
		{
			for(int y = 1; y < h-1; y++)
			{
				for(int z = 1; z < l-1; z++)
				{
					float ht = 0.0f;
					
					for(int i = 0; i < 27; i++)
						ht += m_voxels[x + m_sampler[i,0], y + m_sampler[i,1], z + m_sampler[i,2]];
					
					smothedVoxels[x,y,z] = ht/27.0f;
				}
			}
		}
		
		return smothedVoxels;
		
		//Debug.Log("Smooth voxels time = " + (Time.realtimeSinceStartup-startTime).ToString() );
	}







	

	private Vector3[,,] CalculateNormals(float[,,] m_voxels )
	{
		//float startTime = Time.realtimeSinceStartup;
		
		//This calculates the normal of each voxel. If you have a 3d array of data
		//the normal is the derivitive of the x, y and z axis.
		//Normally you need to flip the normal (*-1) but it is not needed in this case.
		//If you dont call this function the normals that Unity generates for a mesh are used.
		Vector3[,,] m_normals;
		int w = m_voxels.GetLength(0);
		int h = m_voxels.GetLength(1);
		int l = m_voxels.GetLength(2);
		
		m_normals = new Vector3[w,h,l];
		
		for(int x = 2; x < w-2; x++)
		{
			for(int y = 2; y < h-2; y++)
			{
				for(int z = 2; z < l-2; z++)
				{
					float dx = m_voxels[x+1,y,z] - m_voxels[x-1,y,z];
					float dy = m_voxels[x,y+1,z] - m_voxels[x,y-1,z];
					float dz = m_voxels[x,y,z+1] - m_voxels[x,y,z-1];
					
					m_normals[x,y,z] = Vector3.Normalize(new Vector3(dx,dy,dz));
				}
			}
		}
		
		//Debug.Log("Calculate normals time = " + (Time.realtimeSinceStartup-startTime).ToString() );
		return m_normals;
	}

	private Vector3 TriLinearInterpNormal(Vector3 pos,Vector3[,,] m_normals)
	{	
		int x = (int)pos.x;
		int y = (int)pos.y;
		int z = (int)pos.z;
		
		float fx = pos.x-x;
		float fy = pos.y-y;
		float fz = pos.z-z;
		
		Vector3 x0 = m_normals[x,y,z] * (1.0f-fx) + m_normals[x+1,y,z] * fx;
		Vector3 x1 = m_normals[x,y,z+1] * (1.0f-fx) + m_normals[x+1,y,z+1] * fx;
		
		Vector3 x2 = m_normals[x,y+1,z] * (1.0f-fx) + m_normals[x+1,y+1,z] * fx;
		Vector3 x3 = m_normals[x,y+1,z+1] * (1.0f-fx) + m_normals[x+1,y+1,z+1] * fx;
		
		Vector3 z0 = x0 * (1.0f-fz) + x1 * fz;
		Vector3 z1 = x2 * (1.0f-fz) + x3 * fz;
		
		return z0 * (1.0f-fy) + z1 * fy;
	}











}
