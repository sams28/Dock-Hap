using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MoleculeData;
using System.Runtime.InteropServices;


public class DisplayMeshs : DisplayMolecule {



	public float resolution_surface =0.1f;
	
	private GameObject[] m_mesh;
	public List<Mesh> meshes;
	private GameObject standard_gameobject;
	//Max size of the mesh (must be a multiple of 2,3,4)
	private const int MAX_SIZE_MESH = 64500; 
	
	Vector3[] points;
	int[] indices;
	Color[] colors;


	// Use this for initialization
	void Start () {


	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void DisplayMol (ColorDisplay c, TypeDisplay t)
	{
		type = t;
		color = c;
		switch (t) {
		case TypeDisplay.Points : DisplayMolPointCloud();break;
		case TypeDisplay.Lines :  DisplayMolLines();break;
		//case TypeDisplay.CPK : DisplayMolPointCloud();DisplayMolLines();break;
		case TypeDisplay.Surface : DisplayMolSurface();break;
		case TypeDisplay.Trace : DisplayMolTrace();break;
		default: break;
		}
	}

	public override void DisplayHetAtm (bool showHetAtoms)
	{

		/*
		for (int i =0; i < mol.Chains.Count; i++) {
			
			if (mol.Chains [i].Type == "HETATM") {

				for(int j=0;j<mol.Chains[i].Atoms.Count;j++ ){

					points[mol.Chains[i].Atoms[j].Number] = Vector3.zero;


				}
				
			}

		}

		meshes [0].vertices = points_d;
		meshes[0].RecalculateBounds();*/
		throw new NotImplementedException ();


	}

	public override void DisplayWater (bool showWater)
	{
		throw new NotImplementedException ();
	}



	
	public void DisplayMolPointCloud(){
		int i = 0;
		int index;

		meshes = new List<Mesh> ();
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

				points [index] = mol.Atoms [i].Location;
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
			meshes.Add (mesh);
		
		}
		
	}


	public void DisplayMolPointCloud2(){

		int i = 0;
		int index;
		float scale = 1.0f;
		standard_gameobject = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		Mesh m = standard_gameobject.GetComponent<MeshFilter> ().mesh;



		meshes = new List<Mesh> ();
		while (i < mol.Atoms.Count) {


			Mesh mesh = new Mesh ();
			mesh.MarkDynamic ();
			GameObject g = (GameObject)Instantiate (Resources.Load("Prefabs/SubMesh") as GameObject, Vector3.zero, Quaternion.identity);
			g.transform.SetParent(this.transform,false);
			g.GetComponent<MeshFilter> ().mesh = mesh;

			CombineInstance[] combine;

			if((mol.Atoms.Count-i)*m.vertexCount < MAX_SIZE_MESH)
			{
				combine = new CombineInstance[mol.Atoms.Count-i];
				colors = new Color[(mol.Atoms.Count-i)*m.vertexCount];

			}
			else{
				combine = new CombineInstance[MAX_SIZE_MESH/m.vertexCount];
				colors = new Color[m.vertexCount*combine.Length];
			}
			index =0;

			while (i < mol.Atoms.Count && index < combine.Length) {


				standard_gameobject.transform.localPosition =mol.Atoms [i].Location;
				standard_gameobject.transform.localScale = new Vector3(scale,scale,scale);




				for(int j=0;j<m.vertexCount;j++)
					colors[index*m.vertexCount+j] = setColorAtm (mol.Atoms [i], color);


				combine[index].mesh = m;
				combine[index].transform = standard_gameobject.GetComponent<MeshFilter>().transform.localToWorldMatrix;
				i++;
				index++;
				
			}

			mesh.CombineMeshes(combine);
			//CombineMeshes does not combine colors, we have to set them manually

			mesh.colors = colors;
			mesh.RecalculateBounds();
			meshes.Add (mesh);


		}
		Destroy (standard_gameobject);

	}

	
	
	public void DisplayMolLines(){
		int i = 0;
		int index;

		meshes = new List<Mesh> ();

	

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
				points [index * 4] = mol.Atoms [mol.Bonds [i] [0]].Location;
				points [index * 4 + 1] = (mol.Atoms [mol.Bonds [i] [0]].Location + mol.Atoms [mol.Bonds [i] [1]].Location)/2 ;
				points [index * 4 + 2] = (mol.Atoms [mol.Bonds [i] [0]].Location + mol.Atoms [mol.Bonds [i] [1]].Location)/2 ;
				points [index * 4 + 3] = mol.Atoms [mol.Bonds [i] [1]].Location;


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
			meshes.Add(mesh);
		}

		
	}


	public void DisplayMolTrace(){
		int i;
		int index;
		bool skip;
		meshes = new List<Mesh> ();
		
		
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
					points [index] = mol.Atoms [mol.ChainsBonds [c] [i]].Location;
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

				meshes.Add(mesh);
				}
			}

		}

		
		
	}





	public override void UpdateMol(){

		switch(type){
		case TypeDisplay.Points : UpdateAtoms();break;
		case TypeDisplay.Lines : UpdateBonds();break;
		case TypeDisplay.Trace : UpdateChains();break;
		default:break;


		}

	}


	public void UpdateAtoms(){
		int i = 0;
		int j = 0;
		int index;
		
		while (i < mol.Atoms.Count) {
			
			points = new Vector3[MAX_SIZE_MESH];
			index=0;
			
			while (i < mol.Atoms.Count && index < MAX_SIZE_MESH) {
				

				points[index] =  mol.Atoms[i].Location;
				i++;
				index++;
			}
			meshes[j].vertices = points;
			meshes[j].RecalculateBounds();
			j++;
		}
		
		
		
		
	}


	
	public void UpdateBonds(){
		int i = 0;
		int j = 0;
		int index;
		
		while (i <  mol.Bonds.Count) {
			
			points = new Vector3[MAX_SIZE_MESH];
			index =0;
			
			
			while (i < mol.Bonds.Count && index*4+3 < MAX_SIZE_MESH) {

				points [index * 4] = mol.Atoms [mol.Bonds [i] [0]].Location;
				points [index * 4 + 1] = (mol.Atoms [mol.Bonds [i] [0]].Location + mol.Atoms [mol.Bonds [i] [1]].Location)/2 ;
				points [index * 4 + 2] = (mol.Atoms [mol.Bonds [i] [0]].Location + mol.Atoms [mol.Bonds [i] [1]].Location)/2 ;
				points [index * 4 + 3] = mol.Atoms [mol.Bonds [i] [1]].Location;


				i++;
				index++;
			}
			
			meshes[j].vertices = points;
			meshes[j].RecalculateBounds();
			j++;
		}
		
		
		
		
	}



	public void UpdateChains(){
		int j = 0;

		for (int c=0; c<mol.ChainsBonds.Count; c++) {

			int i = 0;

			int index;


			while (i <  mol.ChainsBonds[c].Count) {
			
				points = new Vector3[MAX_SIZE_MESH];
				index = 0;
			
			
				while (i < mol.ChainsBonds[c].Count && index < MAX_SIZE_MESH) {
					points [index] = mol.Atoms [mol.ChainsBonds [c] [i]].Location;
					i++;
					index++;
				}
			
				meshes [j].vertices = points;
				meshes [j].RecalculateBounds ();
				j++;
			}
		
		}
		
		
	}














	
	public void DisplayMolSurface() {
		
		
		
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

		
		float resolution;
		resolution = CapResolution (mol.Atoms.Count);

		//The size of voxel array. Be carefull not to make it to large as a mesh in unity can only be made up of 65000 verts
		int X = (int)(((mol.MaxValue.x - mol.MinValue.x ) * resolution) + 13);
		int Y = (int)(((mol.MaxValue.y - mol.MinValue.y ) * resolution) + 13);
		int Z = (int)(((mol.MaxValue.z - mol.MinValue.z ) * resolution) + 13);
		//correspond to the number vertices in a cube
		int fudgeFactor = 8;

		Vector3 offset = mol.MinValue - (new Vector3 (fudgeFactor, fudgeFactor, fudgeFactor)) / resolution;

		//Debug.Log("Density minValue :: " + resolution);
		Debug.Log("Density point X,Y,Z :: "+ X+","+Y+","+Z);
		Debug.Log("Density minValue :: " + mol.MinValue);

		
		float[,,] gridS = new float[X,Y,Z];
		Color[,,] VertColor = new Color[X, Y, Z];
		List<Mesh> mesh = new List<Mesh>();


		
		Vector3 delta = new Vector3 (resolution, resolution, resolution); //resolution
		
		// We need to refresh the molecule's origin when it's not
		// the first molecule for which we generate a surface.
		//Vector3 origin = mol.MinValue;
		Debug.Log ("Entering :: Generation of density from PDB");
		
		


		//Debug.Log ("Density point X,Y,Z :: " + X + "," + Y + "," + Z);
		//Debug.Log ("Density minValue :: " + MinValue);
		
		
		
		int i;
		int j;
		int k;
		float Dist;
		Color atomColor;
		float atomRadius;
		float density;

		for (int o=0; o<mol.Atoms.Count; o++) {

			if (mol.Atoms [o].Active) {

				i = Mathf.RoundToInt ((mol.Atoms [o].Location.x - mol.MinValue.x) * delta.x + fudgeFactor);
				j = Mathf.RoundToInt ((mol.Atoms [o].Location.y - mol.MinValue.y) * delta.y + fudgeFactor);
				k = Mathf.RoundToInt ((mol.Atoms [o].Location.z - mol.MinValue.z) * delta.z + fudgeFactor);
				Vector3 v1 = new Vector3 (i, j, k);
				
				atomRadius = mol.Atoms [o].AtomRadius;				
				atomColor = setColorAtm (mol.Atoms [o], color);

				for (int l = i-(fudgeFactor/2 -1 ); l < i+(fudgeFactor/2); l++)
					for (int m = j-(fudgeFactor/2 -1); m < j+(fudgeFactor/2); m++)
						for (int n = k-(fudgeFactor/2 -1); n < k+(fudgeFactor/2); n++) {
							Vector3 v2 = new Vector3 (l, m, n);
							Dist = Vector3.Distance (v1, v2);
							density = (float)Math.Exp (-((Dist / atomRadius) * (Dist / atomRadius)));
						
							if (density > gridS [l, m, n])
								VertColor [l, m, n] = atomColor;
							gridS [l, m, n] += density;
						}
			}
		}

		/*

		for (int o =0; o<mol.Atoms.Count; o++) {
			i = Mathf.RoundToInt ((mol.Atoms [o].Location.x - mol.MinValue.x)+fudgeFactor.x);
			j = Mathf.RoundToInt ((mol.Atoms [o].Location.y - mol.MinValue.y)+fudgeFactor.y);
			k = Mathf.RoundToInt ((mol.Atoms [o].Location.z - mol.MinValue.z)+fudgeFactor.z);
			Vector3 v1 = new Vector3 (i, j, k);


			atomRadius = mol.Atoms [o].AtomRadius;				
			atomColor = mol.Atoms [o].ObjColor;


			for (int l = i-4; l < i+9; l++)// correspond à la fenetre de fudgefactor
				for (int m = j-4; m < j+9; m++)
				for (int n = k-4; n < k+9; n++) {



					Vector3 v2 = new Vector3 (l, m, n);
					Dist = Vector3.Distance (v1, v2);
					density = (float)Math.Exp (-((Dist / atomRadius) * (Dist / atomRadius)));

					if (density > gridS [l, m, n])
						VertColor [l, m, n] = atomColor;
					gridS [l, m, n] += density;

			}



		}
		*/

		//gridS = SmoothVoxels (gridS);

		mesh = MarchingCubes.CreateMesh(gridS,VertColor);


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

		//Color



		
		//Debug.Log("Entire surface contains " + mesh.vertices.Length.ToString() + " vertices.");
		//The diffuse shader wants uvs so just fill with a empty array, there not actually used
		//mesh.uv = new Vector2[mesh.vertices.Length];
		//mesh.RecalculateNormals();
		
		//m_mesh = new GameObject("Mesh");
		
		//m_mesh.GetComponent<MeshRenderer>().material = m_material;
		m_mesh = new GameObject[mesh.Count];
		for (int s =0; s<m_mesh.Length; s++) {
			m_mesh[s] = new GameObject();
			m_mesh[s].transform.SetParent(this.transform,false);
			m_mesh[s].transform.localPosition = offset;
			m_mesh[s].transform.localScale /= resolution; 
			m_mesh[s].AddComponent<MeshFilter>();
			m_mesh[s].AddComponent<MeshRenderer>();
			m_mesh[s].GetComponent<MeshFilter>().mesh = mesh[s];
			m_mesh[s].GetComponent<MeshRenderer>().material= Resources.Load("Materials/Meshs") as Material;

			
		}


	}


	public float CapResolution(int nbAtoms) {
		
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


	int[,] m_sampler = new int[,] 
	{
		{1,-1,0}, {1,-1,1}, {0,-1,1}, {-1,-1,1}, {-1,-1,0}, {-1,-1,-1}, {0,-1,-1}, {1,-1,-1}, {0,-1,0},
		{1,0,0}, {1,0,1}, {0,0,1}, {-1,0,1}, {-1,0,0}, {-1,0,-1}, {0,0,-1}, {1,0,-1}, {0,0,0},
		{1,1,0}, {1,1,1}, {0,1,1}, {-1,1,1}, {-1,1,0}, {-1,1,-1}, {0,1,-1}, {1,1,-1}, {0,1,0}
	};



	public float[,,] SmoothVoxels(float[,,] m_voxels)
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





	

	public Vector3[,,] CalculateNormals(float[,,] m_voxels )
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

	Vector3 TriLinearInterpNormal(Vector3 pos,Vector3[,,] m_normals)
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
