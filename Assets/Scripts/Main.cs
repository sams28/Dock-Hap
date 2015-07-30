using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MoleculeData;
using LoadData;




public class Main : MonoBehaviour {



	private StreamReader sr;


	//File path
	public MainUI ui;
	public string molecule_file;
	public string trajectory_file;
	public List<Molecule> molecules;
	public Material mainMaterial;
	static public float globalScale = 1.0f;
	public const int MAX_NUM_DEVICES =10;
	public const int MAX_FRAMES =100;
	static public int current_frame =0;
	static public int total_frames =1;
	public int start_frame =0;
	public int end_frame =0;
	#if UNITY_EDITOR
	void Awake () {
		QualitySettings.vSyncCount = 0;
		//Application.targetFrameRate = -1;
			
	}
	#endif
	
	
	//Initialize the system for a file
	public void Init() {
	
		molecules = new List<Molecule> ();


		LoadFile (molecule_file);
		LoadFile (trajectory_file);

	

		molecules[0].CalculateCenters();
		molecules[0].CalculateChains ();
		molecules[0].CalculateBonds();
		SetMolecule ("all",true);

		ui.center = molecules[0].Location[0];
		Camera.main.transform.localPosition = new Vector3 (molecules[0].Location[0].x, molecules[0].Location[0].y, /*target.z*/ - (Vector3.Distance (molecules[0].MaxValue, molecules[0].MinValue)));
		SetColors (1);

		mainMaterial = new Material(Resources.Load("Materials/UnityObj") as Material);
		SetMaterials (1);
		DisplayMolecules (1);


	}

	// Update is called once per frame
	void Update () {

		if (GetComponent<VRPN> ().ServerStarted) {

			GetComponent<SelectAtoms> ().ClosestAtom(molecules[1],GetComponent<VRPN> ().Devices);
			GetComponent<ApplyForces> ().setForceForAtomPosition ();

				if(GetComponent<IMD>().IsIMDRunning())
				{

					GetComponent<ApplyForces> ().applyForcesVector();


				}
			
		}
	
	}

	public bool SetMolecule(string text,bool addMol,int currentMol =0){


		string[] sl;
		bool not;
		sl = text.Split (new Char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
		Molecule mol;

		if (addMol) {
			mol = new Molecule (molecules [0]);
			molecules.Add (mol);
		} else {

			mol = molecules[currentMol];


		}
		mol.SetActive(false);
		for (int ii =0; ii<sl.Length; ii++) {



			if (sl [0] == "all") {

				mol.SetActive(true);

				return true;

			}

			not=false;

			if(ii-1 >-1){

				if(sl [ii-1] == "not"){

					not=true;

				}

			}
			
			switch (sl [ii]) {
			case "chain":

				if (ii + 1 >= sl.Length) {
					return false;
				}

				for (int i=0; i< mol.Chains.Count; i++) {

					if ((mol.Chains [i].ChainID == sl [ii + 1]) ^not   ) {

						mol.Chains [i].SetActive (true);

					}

				}

				break;
			case "residue":
				
				if (ii + 1 >= sl.Length) {
					return false;
				}
				
				for (int i=0; i< mol.Residues.Count; i++) {
					
					if ((mol.Residues [i].ResName == sl [ii + 1]) ^not) {
						
						mol.Residues [i].SetActive (true);
						
					}
					
				}
				
				break;
			case "atom":
				
				if (ii + 1 >= sl.Length) {
					return false;
				}
				
				for (int i=0; i< mol.Atoms.Count; i++) {
					
					if ((mol.Atoms [i].AtomName == sl [ii + 1])^not) {

						mol.Atoms [i].Active = true;
						
					}
					
				}
				
				break;

			case "hetero":
				

				
				for (int i=0; i< mol.Chains.Count; i++) {
					
					if ((mol.Chains [i].Type == "HETATM" )^not) {
						
						mol.Chains [i].SetActive(true);
						
					}
					
				}
				
				break;



			case "water":



				for (int i=0; i< mol.Chains.Count; i++) {

					for (int j=0; j< mol.Chains[i].Residues.Count; j++) {

						if ((mol.Chains [i].Type == "HETATM" && ((mol.Chains[i].Residues[j].ResName =="WAT") || (mol.Chains[i].Residues[j].ResName =="SOL") || (mol.Chains[i].Residues[j].ResName == "HOH"))) ^not) {

							mol.Chains[i].SetActive (true);
							mol.Chains[i].Residues[j].SetActive (true);

							}

						}
					
				}
				
				break;
			default:
				break;
	
			}

		}


		return true;

	}


	public void SetFrames(string s){
		string[] sl;
		
		sl = s.Split (new Char[] {':'}, StringSplitOptions.RemoveEmptyEntries);
		if (sl.Length == 1) {
			start_frame = int.Parse (sl [0]);
			end_frame = int.Parse (sl [0]);
		} else if (sl.Length == 2) {
			start_frame = int.Parse (sl [0]);
			end_frame = int.Parse (sl [1]);
		} else {
			start_frame = 0;
			end_frame = 0;
		}


		Debug.Log (start_frame + " " +end_frame+ " "+ total_frames);

	}



	public void DisplayMolecules(int current_mol){


		
		for (int i =0; i<total_frames; i++) {
		
			if (molecules [current_mol].Gameobject [i] != null) {
				Destroy (molecules [current_mol].Gameobject [i]);
				Resources.UnloadUnusedAssets ();
			
			}
		}

		for (int i =start_frame; i<end_frame+1; i++) {




			molecules [current_mol].Gameobject[i] = (GameObject)Instantiate (Resources.Load ("Prefabs/Molecule") as GameObject, Vector3.zero, Quaternion.identity);

			molecules [current_mol].Gameobject[i].transform.SetParent (transform, true);


			switch (molecules [current_mol].render) {
			case RenderDisplay.UnityObjects:
				molecules [current_mol].Gameobject[i].AddComponent<DisplayUnityObj> ();
				break;
			case RenderDisplay.Particles:
				molecules [current_mol].Gameobject[i].AddComponent<DisplayParticles> ();
				break;
			case RenderDisplay.Meshs:
				molecules [current_mol].Gameobject[i].AddComponent<DisplayMeshs> ();
				break;
			default:
				break;
			}

			molecules [current_mol].Gameobject[i].GetComponent<DisplayMolecule> ().Init (molecules [current_mol], mainMaterial,globalScale);
			molecules [current_mol].Gameobject[i].GetComponent<DisplayMolecule> ().DisplayMol (molecules [current_mol].color, molecules [current_mol].type, i);

			//molecules [i].molecule.GetComponent<DisplayMolecule>().SetColors(ColorDisplay.Name);
		

		}


	}

	
	public void SetColors(int current_mol){
		

		switch (molecules [current_mol].color) {
		case ColorDisplay.Name:
		
		
			for (int i=0; i<molecules [current_mol].Atoms.Count; i++) {
				switch (molecules [current_mol].Atoms[i].AtomName) {
				case "H":molecules [current_mol].Atoms[i].ObjColor = Color.white;break;
				case "O":molecules [current_mol].Atoms[i].ObjColor = Color.red;break;
				case "C":molecules [current_mol].Atoms[i].ObjColor = Color.green;break;
				case "N":molecules [current_mol].Atoms[i].ObjColor = Color.blue;break;
				case "S":molecules [current_mol].Atoms[i].ObjColor = Color.yellow;break;
				case "P":molecules [current_mol].Atoms[i].ObjColor = (new Color(0.5f,0.5f,0.2f));break;
				default:molecules [current_mol].Atoms[i].ObjColor = Color.magenta;break;
				}
			
			
			}
		
			break;
		
		case ColorDisplay.ResName:
			List<Residue> res = new List<Residue>();
			for (int i=0; i<molecules [current_mol].Residues.Count; i++) {
				
				Residue r = res.Find(x => x.ResName == molecules [current_mol].Residues[i].ResName);
				if(r != null){
					
					molecules [current_mol].Residues[i].ObjColor = r.ObjColor;
					
				}
				else {
					
					switch (molecules [current_mol].Residues[i].ResName) {
					case "SOL" : molecules [current_mol].Residues[i].ObjColor = Color.cyan;break;
					case "H20" : molecules [current_mol].Residues[i].ObjColor = Color.cyan;break;
					case "WAT" : molecules [current_mol].Residues[i].ObjColor = Color.cyan;break;
					case "CL" : molecules [current_mol].Residues[i].ObjColor = Color.green;break;
					default :  molecules [current_mol].Residues[i].ObjColor = new Color(UnityEngine.Random.Range(0.1f,1f),UnityEngine.Random.Range(0.1f,1f),UnityEngine.Random.Range(0.1f,1f));break;
					}
					res.Add(molecules [current_mol].Residues[i]);
				}
				
			}
			break;
		case ColorDisplay.ResID:
			for (int i=0; i<molecules [current_mol].Residues.Count; i++) {
			
				molecules [current_mol].Residues[i].ObjColor = new Color(UnityEngine.Random.Range(0.1f,1f),UnityEngine.Random.Range(0.1f,1f),UnityEngine.Random.Range(0.1f,1f));
				
			}
			break;
		case ColorDisplay.ChainID:

			List<Chain> ch = new List<Chain>();
			for (int i=0; i<molecules [current_mol].Chains.Count; i++) {
				Chain c = ch.Find(x => x.ChainID == molecules [current_mol].Chains[i].ChainID);
				if(c != null){
					
					molecules [current_mol].Chains[i].ObjColor = c.ObjColor;
					
				}
				else{

					molecules [current_mol].Chains[i].ObjColor = new Color(UnityEngine.Random.Range(0.1f,1f),UnityEngine.Random.Range(0.1f,1f),UnityEngine.Random.Range(0.1f,1f));
					ch.Add(molecules [current_mol].Chains[i]);
				}

				
			}
			break;
		default:
			break;
			
			
			
		}

		
	}

	public void SetMaterials(int current_mol){



		List<Material> mat = new List<Material> ();
		switch (molecules [current_mol].color) {
		case ColorDisplay.Name:
		

			for (int i=0; i<molecules [current_mol].Atoms.Count; i++) {


				Material m = mat.Find (x => x.color == molecules [current_mol].Atoms [i].ObjColor);
				if (m != null) {
					molecules [current_mol].Atoms [i].ObjMaterial = m;
				} else {
					molecules [current_mol].Atoms [i].ObjMaterial = new Material (mainMaterial);
					molecules [current_mol].Atoms [i].ObjMaterial.SetColor ("_Color", molecules [current_mol].Atoms [i].ObjColor);
					mat.Add (molecules [current_mol].Atoms [i].ObjMaterial);

				}

			}


		
			break;
		case ColorDisplay.ResName:
			for (int i=0; i<molecules [current_mol].Residues.Count; i++) {
			
			
				Material m = mat.Find (x => x.color == molecules [current_mol].Residues [i].ObjColor);
				if (m != null) {
					molecules [current_mol].Residues [i].ObjMaterial = m;
				} else {
					molecules [current_mol].Residues [i].ObjMaterial = new Material (mainMaterial);
					molecules [current_mol].Residues [i].ObjMaterial.SetColor ("_Color", molecules [current_mol].Residues [i].ObjColor);
					mat.Add (molecules [current_mol].Residues [i].ObjMaterial);
				
				}
			
			}


			break;
		case ColorDisplay.ResID:
			for (int i=0; i<molecules [current_mol].Residues.Count; i++) {
				molecules [current_mol].Residues [i].ObjMaterial = new Material (mainMaterial);
				molecules [current_mol].Residues [i].ObjMaterial.SetColor ("_Color", molecules [current_mol].Residues [i].ObjColor);
			
			}
			break;
		case ColorDisplay.ChainID:
			for (int i=0; i<molecules [current_mol].Chains.Count; i++) {
			
				molecules [current_mol].Chains [i].ObjMaterial = new Material (mainMaterial);
				molecules [current_mol].Chains [i].ObjMaterial.SetColor ("_Color", molecules [current_mol].Chains [i].ObjColor);
			}
		
			break;
		default:
			break;
		
		
		
		}
		

	}











	public void LoadFile(string s){

		if (s != "") {

			string[] sl = s.Split (new Char[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
			StreamReader sr;
			switch (sl [sl.Length - 1]) {
			case "pdb": 
				sr = new StreamReader (s);
				molecules.Add (ReadFiles.ReadPDB (sr));
				sr.Close ();
				break;
			case "gro": 
				sr = new StreamReader (s);
				molecules.Add (ReadFiles.ReadGRO (sr));
				sr.Close ();
				break;
			case "mol2": 
				sr = new StreamReader (s);
			//molecules.Add(ReadFiles.ReadMOL2 (sr));
				sr.Close ();
				break;
			case "xtc": 
				ReadFiles.ReadXTC (s, molecules [0]);
				break;
			default:
				Debug.LogError ("File not supported");
				break;



			}
	
		}

	}





}
