using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoleculeData;

public class DisplayUnityObj : DisplayMolecule {


	private List<GameObject> bonds;

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
		case TypeDisplay.Points :  DisplayMolSpheres();break;
		case TypeDisplay.VDW :  DisplayMolSpheres();break;
		case TypeDisplay.Lines :  DisplayMolCylinders();break;
		case TypeDisplay.CPK : DisplayMolSpheres();DisplayMolCylinders();break;
		case TypeDisplay.Trace :DisplayMolTubes();break;
		default: DisplayMolSpheres();break;
		}
	}


	public void displayAtom(Atom a,float scale){

		a.Gameobject = (GameObject)Instantiate (Resources.Load("Prefabs/Atom") as GameObject, a.Location, Quaternion.identity);
		a.Gameobject.name = a.AtomFullName;
		a.Gameobject.transform.localScale = new Vector3(scale,scale,scale);
		
		
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
		

		a.Gameobject.GetComponent<Renderer>().material = setMaterialAtm(a,color);

		
		
		
		
		
	}
	
	
	public void DisplayMolSpheres() 
	{
		scale = 0.5f;
		
		for (int i =0; i < mol.Chains.Count; i++) {

			mol.Chains[i].Gameobject = new GameObject(mol.Chains[i].ChainID);
			mol.Chains[i].Gameobject.transform.SetParent(transform,true);

			for (int j =0; j < mol.Chains[i].Residues.Count; j++) {
				mol.Chains[i].Residues[j].Gameobject = new GameObject(mol.Chains[i].Residues[j].ResName);
				mol.Chains[i].Residues[j].Gameobject.transform.SetParent(mol.Chains[i].Gameobject.transform,true);
				for(int k =0; k < mol.Chains[i].Residues[j].Atoms.Count; k++) {
					switch(type){
					case TypeDisplay.VDW : displayAtom(mol.Chains[i].Residues[j].Atoms[k],2f*scale*mol.Chains[i].Residues[j].Atoms[k].AtomRadius);break;
					case TypeDisplay.CPK : displayAtom(mol.Chains[i].Residues[j].Atoms[k],0.5f*scale*mol.Chains[i].Residues[j].Atoms[k].AtomRadius);break;
					default : displayAtom(mol.Chains[i].Residues[j].Atoms[k],scale);break;
					}

					mol.Chains[i].Residues[j].Atoms[k].Gameobject.transform.SetParent(mol.Chains[i].Residues[j].Gameobject.transform,true);
					mol.Chains[i].Residues[j].Atoms[k].Gameobject.SetActive(mol.Chains[i].Residues[j].Atoms[k].Active);
				}
			}
		
		}
	
		
	}


	public void DisplayMolCylinders(){
		scale = 0.25f;
		bonds = new List<GameObject> ();
		for (int i =0; i < mol.Bonds.Count; i++) {


			if(mol.Atoms [mol.Bonds [i] [0]].Active && mol.Atoms [mol.Bonds [i] [1]].Active){

			GameObject b1 = (GameObject)Instantiate (Resources.Load ("Prefabs/Bond") as GameObject, mol.Atoms [mol.Bonds [i] [0]].Location, Quaternion.identity);
			GameObject b2 = (GameObject)Instantiate (Resources.Load ("Prefabs/Bond") as GameObject, mol.Atoms [mol.Bonds [i] [1]].Location, Quaternion.identity);
			b1.transform.SetParent(transform,true);
			b2.transform.SetParent(transform,true);
			b1.transform.LookAt(mol.Atoms [mol.Bonds [i] [1]].Location);
			b2.transform.LookAt(mol.Atoms [mol.Bonds [i] [0]].Location);
			float d = Vector3.Distance(mol.Atoms [mol.Bonds [i] [0]].Location,mol.Atoms [mol.Bonds [i] [1]].Location)/4;
			switch(type){
			case TypeDisplay.CPK : 
				b1.transform.localScale = new Vector3(scale/4,scale/4,d);
				b2.transform.localScale = new Vector3(scale/4,scale/4,d);
				break;
			default :
				b1.transform.localScale = new Vector3(scale,scale,d);
				b2.transform.localScale = new Vector3(scale,scale,d);
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
		scale = 0.5f;
		bonds = new List<GameObject> ();
		bool skip;


		for (int c=0; c<mol.ChainsBonds.Count; c++) {
			skip =false;
			for (int i =0; i < mol.ChainsBonds[c].Count-1 && !skip; i++) {

				if(!mol.Atoms [mol.ChainsBonds [c] [i]].Active){
					
					skip = true;
					
				}
				else{

				GameObject b1 = (GameObject)Instantiate (Resources.Load ("Prefabs/Bond2") as GameObject, mol.Atoms [mol.ChainsBonds [c] [i]].Location, Quaternion.identity);
				b1.transform.SetParent (transform, true);

				b1.transform.LookAt (mol.Atoms [mol.ChainsBonds [c] [i+1]].Location);
				float d = Vector3.Distance (mol.Atoms [mol.ChainsBonds [c] [i]].Location, mol.Atoms [mol.ChainsBonds [c] [i+1]].Location) / 2;
				b1.GetComponentInChildren<Transform>().localScale = new Vector3(1.0f,1.0f,1.0f);
				b1.transform.localScale = new Vector3 (scale, scale, d);

	
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



				b1.transform.position = mol.Atoms [mol.ChainsBonds [c] [i]].Location;
				b1.transform.LookAt (mol.Atoms [mol.ChainsBonds [c] [i+1]].Location);
				float d = Vector3.Distance (mol.Atoms [mol.ChainsBonds [c] [i]].Location, mol.Atoms [mol.ChainsBonds [c] [i+1]].Location) / 2;

				b1.transform.localScale = new Vector3 (scale, scale, d);
				index++;
			
			}
		}
	}




	public void UpdateBonds(){

		for (int i=0; i<mol.Bonds.Count; i++){

			GameObject b1 =bonds[i*2];
			GameObject b2 =bonds[i*2+1];

			b1.transform.position = mol.Atoms [mol.Bonds [i] [0]].Location;
			b2.transform.position = mol.Atoms [mol.Bonds [i] [1]].Location;

			b1.transform.LookAt(mol.Atoms [mol.Bonds [i] [1]].Location);
			b2.transform.LookAt(mol.Atoms [mol.Bonds [i] [0]].Location);
			float d = Vector3.Distance(mol.Atoms [mol.Bonds [i] [0]].Location,mol.Atoms [mol.Bonds [i] [1]].Location)/4;
			switch(type){
			case TypeDisplay.CPK : 
				b1.transform.localScale = new Vector3(scale/4,scale/4,d);
				b2.transform.localScale = new Vector3(scale/4,scale/4,d);
				break;
			default :
				b1.transform.localScale = new Vector3(scale,scale,d);
				b2.transform.localScale = new Vector3(scale,scale,d);
				break;
			}

		}

	}

	public void UpdateAtoms(){

		for (int i=0; i<mol.Atoms.Count; i++){

			mol.Atoms [i].Gameobject.transform.localPosition  = mol.Atoms[i].Location;
		}

	}





	public override void DisplayHetAtm(bool showHetAtoms){

		for (int i =0; i < mol.Chains.Count; i++) {

			if (mol.Chains [i].Type == "HETATM") {

				mol.Chains [i].Gameobject.SetActive (showHetAtoms);

			}
		}
	}
	
	
	public override void DisplayWater(bool showWater){

		for (int i =0; i < mol.Chains.Count; i++) {

			if(mol.Chains [i].Type == "HETATM"){

				for (int j =0; j < mol.Chains[i].Residues.Count; j++) {
					
					if((mol.Chains[i].Residues[j].ResName == "HOH")||(mol.Chains[i].Residues[j].ResName == "SOL")){
						mol.Chains[i].Residues[j].Gameobject.SetActive(showWater);
						
					}	
				}
			}
		}
		
		
	}

}
