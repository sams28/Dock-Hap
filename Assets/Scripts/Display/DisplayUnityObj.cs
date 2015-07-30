using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoleculeData;

public class DisplayUnityObj : DisplayMolecule {

	private List<GameObject> bonds;
	private GameObject[] m_mesh;
	public List<Mesh> meshes;
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

	public override void DisplayMol (ColorDisplay c, TypeDisplay t,int f)
	{
		type = t;
		color = c;
		frame = f;
		switch (t) {
		case TypeDisplay.Points :  DisplayMolSpheresBatch();break;
		case TypeDisplay.VDW :  DisplayMolSpheres();break;
		case TypeDisplay.Lines :  DisplayMolCylindersBatch();break;
		case TypeDisplay.CPK : DisplayMolSpheres();DisplayMolCylinders();break;
		case TypeDisplay.Trace :DisplayMolTubes();break;
		default: DisplayMolSpheres();break;
		}
	}


	public void displayAtom(Atom a,float scale){

		a.Gameobject[frame] = (GameObject)Instantiate (Resources.Load("Prefabs/Atom") as GameObject, a.Location[frame], Quaternion.identity);
		a.Gameobject[frame].name = a.AtomFullName;
		a.Gameobject[frame].transform.localScale = new Vector3(scale,scale,scale);
		
		
		/* cut sphere
		Vector3[] vertices =a.Gameobject.GetComponent<MeshFilter>().mesh.vertices;
		List<int> indices = new List<int>(a.Gameobject.GetComponent<MeshFilter>().mesh.triangles);
		int count = indices.Count / 3;
		Vector3 norm = a.Gameobject.transform.position.normalized;
		float d = 0.1f; 
			for (int j = count-1; j >= 0; j--)
			{
				Vector3 V1 = vertices[indices[j*3 + 0]];
				Vector3 V2 = vertices[indices[j*3 + 1]];
				Vector3 V3 = vertices[indices[j*3 + 2]];
				float t1 = V1.x*norm.x+V1.y*norm.y+V1.z*norm.z;
				float t2 = V2.x*norm.x+V2.y*norm.y+V2.z*norm.z;
				float t3 = V3.x*norm.x+V3.y*norm.y+V3.z*norm.z;
				if(norm != Vector3.zero){
					if (t1 < d && t2 < d && t3 < d)
						indices.RemoveRange(j*3, 3);
					
				}
			}

		a.Gameobject.GetComponent<MeshFilter>().mesh.triangles = indices.ToArray();
		*/
		

		a.Gameobject[frame].GetComponent<Renderer>().material = setMaterialAtm(a,color);

		
		
		
		
		
	}
	
	
	public void DisplayMolSpheres() 
	{

	
		
		for (int i =0; i < mol.Chains.Count; i++) {

			mol.Chains[i].Gameobject[frame] = new GameObject(mol.Chains[i].ChainID);
			mol.Chains[i].Gameobject[frame].transform.SetParent(transform,true);

			for (int j =0; j < mol.Chains[i].Residues.Count; j++) {
				mol.Chains[i].Residues[j].Gameobject[frame] = new GameObject(mol.Chains[i].Residues[j].ResName);
				mol.Chains[i].Residues[j].Gameobject[frame].transform.SetParent(mol.Chains[i].Gameobject[frame].transform,true);
				for(int k =0; k < mol.Chains[i].Residues[j].Atoms.Count; k++) {
					switch(type){
					case TypeDisplay.VDW : displayAtom(mol.Chains[i].Residues[j].Atoms[k],scale*mol.Chains[i].Residues[j].Atoms[k].AtomRadius);break;
					case TypeDisplay.CPK : displayAtom(mol.Chains[i].Residues[j].Atoms[k],0.25f*scale*mol.Chains[i].Residues[j].Atoms[k].AtomRadius);break;
					default : displayAtom(mol.Chains[i].Residues[j].Atoms[k],scale * 0.5f);break;
					}

					mol.Chains[i].Residues[j].Atoms[k].Gameobject[frame].transform.SetParent(mol.Chains[i].Residues[j].Gameobject[frame].transform,true);
					mol.Chains[i].Residues[j].Atoms[k].Gameobject[frame].SetActive(mol.Chains[i].Residues[j].Atoms[k].Active);
				}
			}
		
		}
	
		
	}


	public void DisplayMolSpheresBatch(){
		
		int i = 0;
		int index;
		GameObject standard_gameobject = (GameObject)Instantiate (Resources.Load("Prefabs/Atom") as GameObject,Vector3.zero, Quaternion.identity);;
		Mesh m = standard_gameobject.GetComponent<MeshFilter> ().mesh;
		
		
		
		meshes = new List<Mesh> ();
		while (i < mol.Atoms.Count) {
			
			
			Mesh mesh = new Mesh ();
			mesh.MarkDynamic ();
			GameObject g = (GameObject)Instantiate (Resources.Load("Prefabs/SubBatch") as GameObject, Vector3.zero, Quaternion.identity);
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
				
				if(mol.Atoms[i].Active){
				standard_gameobject.transform.localPosition =mol.Atoms [i].Location[frame];
				standard_gameobject.transform.localScale = new Vector3(scale,scale,scale);
				
				
				
				
				for(int j=0;j<m.vertexCount;j++)
					colors[index*m.vertexCount+j] = setColorAtm (mol.Atoms [i], color);
				
				
				combine[index].mesh = m;
				combine[index].transform = standard_gameobject.GetComponent<MeshFilter>().transform.localToWorldMatrix;
					index++;

				}
				i++;
				
			}
			
			mesh.CombineMeshes(combine);
			//CombineMeshes does not combine colors, we have to set them manually
			
			mesh.colors = colors;
			mesh.RecalculateBounds();
			meshes.Add (mesh);
			
			
		}
		Destroy (standard_gameobject);
		
	}


	public void DisplayMolCylindersBatch(){
		
		int i = 0;
		int index;
		GameObject b1 = (GameObject)Instantiate (Resources.Load("Prefabs/Bond") as GameObject,Vector3.zero, Quaternion.identity);
		GameObject b2 = (GameObject)Instantiate (Resources.Load("Prefabs/Bond") as GameObject,Vector3.zero, Quaternion.identity);
		Mesh m1 = b1.GetComponentInChildren<MeshFilter> ().mesh;
		Mesh m2 = b1.GetComponentInChildren<MeshFilter> ().mesh;
		
		
		meshes = new List<Mesh> ();
		while (i < mol.Bonds.Count) {
			
			
			Mesh mesh = new Mesh ();
			mesh.MarkDynamic ();
			GameObject g = (GameObject)Instantiate (Resources.Load("Prefabs/SubBatch") as GameObject, Vector3.zero, Quaternion.identity);
			g.transform.SetParent(this.transform,false);
			g.GetComponent<MeshFilter> ().mesh = mesh;
			
			CombineInstance[] combine;
			
			if((mol.Bonds.Count-i)*m1.vertexCount < MAX_SIZE_MESH)
			{
				
				combine = new CombineInstance[mol.Bonds.Count-i];
				colors = new Color[(mol.Bonds.Count-i)*m1.vertexCount];
				
			}
			else{
				combine = new CombineInstance[MAX_SIZE_MESH/m1.vertexCount];
				colors = new Color[m1.vertexCount*combine.Length];
			}
			index =0;
			
			while (i < mol.Bonds.Count && index*2+1 < combine.Length) {
				
				if(mol.Atoms [mol.Bonds [i] [0]].Active && mol.Atoms [mol.Bonds [i] [1]].Active){

					b1.transform.SetParent(transform,true);
					b2.transform.SetParent(transform,true);
					b1.transform.LookAt(mol.Atoms [mol.Bonds [i] [1]].Location[frame]);
					b2.transform.LookAt(mol.Atoms [mol.Bonds [i] [0]].Location[frame]);
					float d = Vector3.Distance(mol.Atoms [mol.Bonds [i] [0]].Location[frame],mol.Atoms [mol.Bonds [i] [1]].Location[frame])/4;
					switch(type){
					case TypeDisplay.CPK : 
						b1.transform.localScale = new Vector3(scale/16,scale/16,d);
						b2.transform.localScale = new Vector3(scale/16,scale/16,d);
						break;
					default :
						b1.transform.localScale = new Vector3(scale/4,scale/4,d);
						b2.transform.localScale = new Vector3(scale/4,scale/4,d);
						break;
					}


					for(int j=0;j<m1.vertexCount;j++){
						colors[index*2*m1.vertexCount+j] = setColorAtm (mol.Atoms [mol.Bonds [i] [0]], color);
						colors[(index*2+1)*m1.vertexCount+j] = setColorAtm (mol.Atoms [mol.Bonds [i] [1]], color);
					}



					combine[index*2].mesh = m1;
					combine[index*2].transform = b1.GetComponentInChildren<MeshFilter>().transform.localToWorldMatrix;
					combine[index*2+1].mesh = m2;
					combine[index*2+1].transform = b2.GetComponentInChildren<MeshFilter>().transform.localToWorldMatrix;
					index++;
				}
				i++;
				
			}
			
			mesh.CombineMeshes(combine);
			//CombineMeshes does not combine colors, we have to set them manually
			
			mesh.colors = colors;
			mesh.RecalculateBounds();
			meshes.Add (mesh);
			
			
		}
		Destroy (b1);
		Destroy (b2);
		
	}











	public void DisplayMolCylinders(){
		bonds = new List<GameObject> ();
		for (int i =0; i < mol.Bonds.Count; i++) {


			if(mol.Atoms [mol.Bonds [i] [0]].Active && mol.Atoms [mol.Bonds [i] [1]].Active){

				GameObject b1 = (GameObject)Instantiate (Resources.Load ("Prefabs/Bond") as GameObject, mol.Atoms [mol.Bonds [i] [0]].Location[frame], Quaternion.identity);
				GameObject b2 = (GameObject)Instantiate (Resources.Load ("Prefabs/Bond") as GameObject, mol.Atoms [mol.Bonds [i] [1]].Location[frame], Quaternion.identity);
				b1.transform.SetParent(transform,true);
				b2.transform.SetParent(transform,true);
				b1.transform.LookAt(mol.Atoms [mol.Bonds [i] [1]].Location[frame]);
				b2.transform.LookAt(mol.Atoms [mol.Bonds [i] [0]].Location[frame]);
				float d = Vector3.Distance(mol.Atoms [mol.Bonds [i] [0]].Location[frame],mol.Atoms [mol.Bonds [i] [1]].Location[frame])/4;
				switch(type){
				case TypeDisplay.CPK : 
					b1.transform.localScale = new Vector3(scale/16,scale/16,d);
					b2.transform.localScale = new Vector3(scale/16,scale/16,d);
					break;
				default :
					b1.transform.localScale = new Vector3(scale/4,scale/4,d);
					b2.transform.localScale = new Vector3(scale/4,scale/4,d);
					break;
				}
				b1.GetComponentInChildren<Renderer>().material = setMaterialAtm(mol.Atoms [mol.Bonds [i] [0]],color);
				b2.GetComponentInChildren<Renderer>().material = setMaterialAtm(mol.Atoms [mol.Bonds [i] [1]],color);
				bonds.Add(b1);
				bonds.Add(b2);
			}

		}

	}

	public void DisplayMolTubes(){

		bonds = new List<GameObject> ();
		bool skip;


		for (int c=0; c<mol.ChainsBonds.Count; c++) {
			skip =false;
			for (int i =0; i < mol.ChainsBonds[c].Count-1 && !skip; i++) {

				if(!mol.Atoms [mol.ChainsBonds [c] [i]].Active){
					
					skip = true;
					
				}
				else{

					GameObject b1 = (GameObject)Instantiate (Resources.Load ("Prefabs/Bond2") as GameObject, mol.Atoms [mol.ChainsBonds [c] [i]].Location[frame], Quaternion.identity);
					b1.transform.SetParent (transform, true);

					b1.transform.LookAt (mol.Atoms [mol.ChainsBonds [c] [i+1]].Location[frame]);
					float d = Vector3.Distance (mol.Atoms [mol.ChainsBonds [c] [i]].Location[frame], mol.Atoms [mol.ChainsBonds [c] [i+1]].Location[frame]) / 2;
					b1.GetComponentInChildren<Transform>().localScale = new Vector3(1.0f,1.0f,1.0f);
					b1.transform.localScale = new Vector3 (scale * 0.5f, scale * 0.5f, d);

		
					b1.GetComponentInChildren<Renderer> ().material = setMaterialAtm (mol.Atoms [mol.ChainsBonds [c] [i]], color);
					bonds.Add (b1);
				}
			
			
			}
		}
	}
	
	public override void UpdateMol(){
		
		switch(type){
		case TypeDisplay.Points : UpdateAtoms();break;
		case TypeDisplay.VDW : UpdateAtoms();break;
		case TypeDisplay.Lines : UpdateBonds();break;
		case TypeDisplay.CPK : UpdateAtoms();UpdateBonds();break;
		case TypeDisplay.Trace : UpdateChains();break;
		default:break;
			
			
		}
		
	}
	
	



	public void UpdateChains(){
		int index = 0;
		for (int c=0; c<mol.ChainsBonds.Count; c++) {
			for (int i=0; i<mol.ChainsBonds[c].Count-1; i++) {
				GameObject b1 = bonds [index];



				b1.transform.position = mol.Atoms [mol.ChainsBonds [c] [i]].Location[Main.current_frame];
				b1.transform.LookAt (mol.Atoms [mol.ChainsBonds [c] [i+1]].Location[Main.current_frame]);
				float d = Vector3.Distance (mol.Atoms [mol.ChainsBonds [c] [i]].Location[Main.current_frame], mol.Atoms [mol.ChainsBonds [c] [i+1]].Location[Main.current_frame]) / 2;

				b1.transform.localScale = new Vector3 (scale * 0.5f, scale * 0.5f, d);
				index++;
			
			}
		}
	}




	public void UpdateBonds(){

		for (int i=0; i<mol.Bonds.Count; i++){

			GameObject b1 =bonds[i*2];
			GameObject b2 =bonds[i*2+1];

			b1.transform.position = mol.Atoms [mol.Bonds [i] [0]].Location[Main.current_frame];
			b2.transform.position = mol.Atoms [mol.Bonds [i] [1]].Location[Main.current_frame];

			b1.transform.LookAt(mol.Atoms [mol.Bonds [i] [1]].Location[Main.current_frame]);
			b2.transform.LookAt(mol.Atoms [mol.Bonds [i] [0]].Location[Main.current_frame]);
			float d = Vector3.Distance(mol.Atoms [mol.Bonds [i] [0]].Location[Main.current_frame],mol.Atoms [mol.Bonds [i] [1]].Location[Main.current_frame])/4;
			switch(type){
			case TypeDisplay.CPK : 
				b1.transform.localScale = new Vector3(scale/16,scale/16,d);
				b2.transform.localScale = new Vector3(scale/16,scale/16,d);
				break;
			default :
				b1.transform.localScale = new Vector3(scale/4,scale/4,d);
				b2.transform.localScale = new Vector3(scale/4,scale/4,d);
				break;
			}

		}

	}

	public void UpdateAtoms(){

		for (int i=0; i<mol.Atoms.Count; i++){

			mol.Atoms [i].Gameobject[Main.current_frame].transform.localPosition  = mol.Atoms[i].Location[Main.current_frame];
		}

	}



}
