using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace MoleculeData{


	 public  class Element{
		public string name;
		public float mass;
		public float radius;


		public Element(string n,float m,float r){
			name = n;
			mass = m;
			radius = r;
		}

	}

	//static const List<Element> table = {new Element("H",1.00794f,1.2f),new Element("He",4.00260,1.40f),new Element("Li",6.941,1.82f)};







	public abstract class Mol_Object {

		protected Vector3[] location;
		protected GameObject[] gameobject;
		protected GameObject[] forceGameobject;
		protected Color objColor;
		protected Material objMaterial;
		protected bool active;
		protected bool[] selected;

		protected Mol_Object(){

			location = new Vector3[Main.MAX_FRAMES];
			gameobject = new GameObject[Main.MAX_FRAMES];
			active =false;
			selected =new bool[Main.MAX_NUM_DEVICES];
			for (int i =0; i<Main.MAX_NUM_DEVICES; i++)
				selected [i] = false;
			forceGameobject = new GameObject[Main.MAX_NUM_DEVICES];
		}

		protected Mol_Object(Mol_Object m){
			location = new Vector3[Main.MAX_FRAMES];
			m.location.CopyTo(location,0);

			gameobject = new GameObject[Main.MAX_FRAMES];
			m.gameobject.CopyTo(gameobject,0);

			selected =new bool[Main.MAX_NUM_DEVICES];
			m.selected.CopyTo(selected,0);
			forceGameobject = new GameObject[Main.MAX_NUM_DEVICES];
			m.forceGameobject.CopyTo(forceGameobject,0);
			objColor = m.objColor;
			objMaterial = m.objMaterial;

		}

		public Vector3[] Location{ 
			get{return location;} 
			set{ location = value; }
		}

		public Color ObjColor{
			get{return objColor;}
			set{objColor = value;}
		}


		public Material ObjMaterial{
			get{return objMaterial;}
			set{objMaterial = value;}
		}


		public bool[] Selected{
			get{return selected;}
			set{selected = value;}
		}

		public bool Active{
			get{return active;}
			set{active = value;}
		}


		public GameObject[] Gameobject{
			get{return gameobject;}
			set{gameobject = value;}
		}

		public GameObject[] ForceGameobject{
			get{return forceGameobject;}
			set{forceGameobject = value;}
		}




	}





	public class Atom : Mol_Object
	{



		//from periodicTable.C in VMD

		static string[] tableName = {
			"X",  "H",  "He",
			"Li", "Be", "B",  "C",  "N",  "O",  "F",  "Ne",
			"Na", "Mg", "Al", "Si", "P" , "S",  "Cl", "Ar", 
			"K",  "Ca"
		};
		
		static float[] tableMass = {
			/* X  */ 0.00000f, 1.00794f, 4.00260f, 
			/* Li */ 6.941f, 9.012182f, 10.811f,12.0107f, 14.0067f, 15.9994f, 18.9984032f, 20.1797f, 
			/* Na */ 22.989770f, 24.3050f, 26.981538f, 28.0855f, 30.973761f,32.065f, 35.453f, 39.948f,
			/* K  */ 39.0983f, 40.078f

		};
		
		static float[] tableRadius = {
			/* X  */ 1.50f, 1.20f, 1.40f, 
			/* Li */ 1.82f, 2.00f, 2.00f,1.70f, 1.55f, 1.52f, 1.47f, 1.54f,
			/* Na */ 1.36f, 1.18f, 2.00f, 2.10f, 1.80f,1.90f, 2.27f, 1.88f, 
			/* K  */ 1.76f, 1.37f

		};


		//name of the element
		private string atomName;
		private string atomFullName;
		private float atomCharge;
		private float atomRadius;
		private float atomMass;
		private Residue atomResidue;
		private Chain atomChain;
		private List<int> bonds;
		private int number;


		public Atom(string name,float charge,int num,Residue r,Chain c) :base(){



			number = num;
			atomFullName = name;
			atomName = name [0].ToString();
			atomCharge = charge;
			bonds = new List<int> ();

			int i =0;
			
			while((atomName != tableName[i]) && (i < tableName.Length)){

				i++;

			}

			atomMass = tableMass [i];
			atomRadius = tableRadius [i];

			atomResidue = r;
			atomChain = c;



		}

		public Atom(Atom a,Residue r,Chain c) : base(a){

			number = a.number;
			atomFullName = a.atomFullName;
			atomName = a.atomName;
			atomCharge = a.atomCharge;
			bonds = new List<int>(a.bonds);
			atomMass = a.atomMass;
			atomRadius = a.atomRadius;
			
			atomResidue = r;
			atomChain = c;


		}


		public Residue AtomResidue{
			get{return atomResidue;}
			set{atomResidue = value;}
		}

		public Chain AtomChain{
			get{return atomChain;}
			set{atomChain = value;}
		}

		public string AtomFullName{
			get{return atomFullName;}
			set{atomFullName = value;}
		}  


		public string AtomName{
			get{return atomName;}
			set{atomName = value;}
		} 
		public float AtomRadius{
			get{return atomRadius;}
			set{atomRadius = value;}
		}
		public float AtomMass{
			get{return atomMass;}
			set{atomMass = value;}
		}


		public float AtomCharge{
			get{return atomCharge;}
			set{atomCharge = value;}
		} 

		public List<int> Bonds{
			get{return bonds;}
			set{bonds = value;}
		}

		public int Number{
			get{return number;}
			set{number = value;}
		}

		
	}

	public class Residue : Mol_Object
	{
		private List<Atom> atoms;
		private string resName;
		private int resID;
		private Chain resChain;

		public Residue():base(){
		}


		public Residue(string resname,int resid,Chain c):base(){

			resName = resname;
			resID = resid;
			atoms = new List<Atom> ();
			resChain = c;
			
		}

		public Residue(Residue r,Chain c): base(r){


			atoms = new List<Atom>();
			resName =r.resName;
			resID = r.resID;
			resChain = c;




		}



		public void CalculateCenter(){
			

			Vector3 bary = Vector3.zero;
			
			for (int i=0; i<Atoms.Count; i++) {	
				Vector3 position = Atoms [i].Location[Main.current_frame];
				bary = bary + (new Vector3 (position [0], position [1], position [2]));

			}

			location[Main.current_frame] = bary/Atoms.Count;
			
			
		}

		public void SetActive(bool b){
			active = b;
			for (int i=0; i<atoms.Count; i++) {
				atoms[i].Active = b;
			}


		}


		public Chain ResChain{
			get{return resChain;}
			set{resChain = value;}
		}




		public string ResName{
			get{return resName;}
			set{resName = value;}
		}

		public int ResID{
			get{return resID;}
			set{resID = value;}
		} 

		public List<Atom> Atoms{
			get{return atoms;}
			set{atoms = value;}

		}

	}


	public class Chain : Mol_Object
	{
		private string chainID;
		//type molecule (atom,hetatom ...)
		private string type;
		private List<Residue> residues;
		private List<Atom> atoms;

		public Chain():base(){
		}


		public Chain(string t,string chainid):base(){
	
			type = t;
			chainID = chainid;
			residues = new List<Residue> ();
			atoms = new List<Atom> ();
			
		}

		public Chain(Chain c) :base(c){
			chainID = c.chainID;
			type = c.type;
			residues = new List<Residue> ();
			atoms = new List<Atom> ();


		}


		public void CalculateCenter(){

			Vector3 bary = Vector3.zero;
			
			for (int i=0; i<Residues.Count; i++) {
				for (int j=0; j<Residues[i].Atoms.Count; j++) {

					Vector3 position = Residues [i].Atoms [j].Location[Main.current_frame];

					bary = bary + (new Vector3 (position [0], position [1], position [2]));

				}
			}

			location[Main.current_frame] = bary/atoms.Count;

			
		}


		public void SetActive(bool b){
			active = b;
			for (int i=0; i<residues.Count; i++) {

				residues[i].SetActive(b);


			}

		}





		public List<Atom> Atoms{
			get{return atoms;}
			set{atoms = value;}
			
		}
	
	
		public string ChainID{
			get{return chainID;}
			set{chainID = value;}
		} 
		
		public List<Residue> Residues{
			get{return residues;}
			set{residues = value;}
			
		}

		public string Type{
			get{return type;}
			set{type = value;}
		} 




	}








	public struct IMDEnergies
	{
		public int tstep;  //!< integer timestep index
		public float T;          //!< Temperature in degrees Kelvin
		public float Etot;       //!< Total energy, in Kcal/mol
		public float Epot;       //!< Potential energy, in Kcal/mol
		public float Evdw;       //!< Van der Waals energy, in Kcal/mol
		public float Eelec;      //!< Electrostatic energy, in Kcal/mol
		public float Ebond;      //!< Bond energy, Kcal/mol
		public float Eangle;     //!< Angle energy, Kcal/mol
		public float Edihe;      //!< Dihedral energy, Kcal/mol
		public float Eimpr;
	};


	public enum ColorDisplay{
		Name,
		ResName,
		ChainID,
		ResID,
		Charges
		
		
	}
	public enum TypeDisplay{
		Points,
		Lines,
		VDW,
		CPK,
		ChainID,
		Trace,
		Surface
	}

	public enum RenderDisplay{
		UnityObjects,
		Particles,
		Meshs
	}


	public enum SelectDisplay{
		Atom,
		Residue,
		Chain,
		Molecule
		
	}




	public class Molecule : Mol_Object {

	
		private IMDEnergies energies;
		private List<Chain> chains;
		private List<Residue> residues;
		private List<Atom> atoms;

		private Vector3 minValue; 
		private Vector3 maxValue;


		private List<int[]> bonds;
		private List<List<int>> chainsBonds;

		//Parameters for the display
		public bool showHetAtoms = true;
		public bool showWater= true;
		public ColorDisplay color= ColorDisplay.Name;
		public TypeDisplay type= TypeDisplay.Points;
		public RenderDisplay render = RenderDisplay.Meshs;
		public SelectDisplay select= SelectDisplay.Atom;


		public Molecule() : base(){

			chains = new List<Chain> ();
			residues = new List<Residue> ();
			atoms = new List<Atom> ();
			minValue =new Vector3 (0, 0, 0);
			maxValue = new Vector3 (0, 0, 0);
			bonds = new List<int[]> ();
			chainsBonds = new List<List<int>> ();


		}

		public Molecule(Molecule m){


			chains = new List<Chain> ();
			residues = new List<Residue> ();
			atoms = new List<Atom> ();


			for (int i = 0; i<m.Chains.Count; i++) {
				Chain c= new Chain(m.Chains[i]);
				this.Chains.Add(c);

				for (int j = 0; j<m.Chains[i].Residues.Count; j++) {
					Residue r = new Residue(m.Chains[i].Residues[j],c);
					
					this.Chains[i].Residues.Add(r);
					this.Residues.Add(r);
					
					for (int k =0; k<m.Chains[i].Residues[j].Atoms.Count; k++) {

						
						Atom a = new Atom(m.Chains[i].Residues[j].Atoms[k],r,c);
						this.Chains[i].Residues[j].Atoms.Add(a);
						this.Chains[i].Atoms.Add(a);
						this.Atoms.Add(a);

						
						
						
					}


					
				}




			}


			bonds = new List<int[]> (m.Bonds);
			chainsBonds = new List<List<int>> (m.chainsBonds);
			minValue =m.MinValue;
			maxValue = m.MaxValue;

			color = m.color;
			type = m.type;
			render = m.render;
			select = m.select;
			energies = m.energies;





		}

		public void SetActive(bool b){
			active = b;
			for (int i=0; i<chains.Count; i++) {
				
				chains[i].SetActive(b);

			}



			
		}



		public List<Chain> Chains{
			get{return chains;}
			set{chains = value;}
			
		}


		public List<Atom> Atoms{
			get{return atoms;}
			set{atoms = value;}
			
		}

		public List<int[]> Bonds{
			get{return bonds;}
			set{bonds = value;}
			
		}

		public List<List<int>> ChainsBonds{
			get{return chainsBonds;}
			set{chainsBonds = value;}
			
		}


		
		public List<Residue> Residues{
			get{return residues;}
			set{residues = value;}
			
		}


		public IMDEnergies Energies{
			get{return energies;}
			set{energies = value;}
		}




		public Vector3 MinValue{
			get{return minValue;}
			set{minValue = value;}
		} 

		public Vector3 MaxValue{
			get{return maxValue;}
			set{maxValue = value;}
		} 



		 




	
		public void CalculateCenters(){

			Vector3 minPoint= new Vector3(float.MaxValue,float.MaxValue,float.MaxValue);
			Vector3 maxPoint= new Vector3(float.MinValue,float.MinValue,float.MinValue);
			Vector3 bary = Vector3.zero;
			
			for (int i=0; i<Chains.Count; i++) {
				Chains[i].CalculateCenter();

				for (int j=0; j<Chains[i].Residues.Count; j++) {

					Chains[i].Residues[j].CalculateCenter();
					for (int k=0; k<Chains[i].Residues[j].Atoms.Count; k++) {
					
						Vector3 position = Chains [i].Residues [j].Atoms [k].Location[Main.current_frame];

						minPoint = Vector3.Min (minPoint, position);
						maxPoint = Vector3.Max (maxPoint, position);
						bary = bary + position;
					}
				}
			}

			location[Main.current_frame] = bary/Atoms.Count;

			//Debug.Log("centerPoint:"+location + " min/max " + minPoint + "/" + maxPoint);

			MinValue = minPoint;
			MaxValue = maxPoint;


		}





		public void CalculateChains(){
			for (int i=0; i<Chains.Count; i++) {

				List<int> chain = new List<int>();
				//int j=0;
				for(int j=0;j<Chains[i].Atoms.Count;j++){

					if(Chains[i].Atoms[j].AtomFullName == "CA"){
						chain.Add(Chains[i].Atoms[j].Number);
					}
				}


				if(chain !=null){
					chainsBonds.Add(chain);
				}


			}


		}







		public void CalculateBonds(){
			string type1, type2;
			Atom atom_a, atom_b;
			float cutoff =0.0f;
		
			for (int i=0; i<Atoms.Count; i++) {  
				float rad = Atoms[i].AtomRadius;
				if (rad > cutoff) cutoff = rad;
			}

			for (int i=0; i<Atoms.Count; i++) {

				atom_a = Atoms [i];
				type1 = atom_a.AtomName;

				//we need to find a better way to do this
				for (int j=i+1; j<i+50 && j<Atoms.Count; j++) {


					atom_b = Atoms [j];
					type2 = atom_b.AtomName;

					if ((type1 == "H") && (type2 == "H"))
						continue;

					if (Vector3.Distance (atom_a.Location[Main.current_frame], atom_b.Location[Main.current_frame]) <= cutoff) {

						if (type1 == "H") {

							if (atom_a.Bonds.Count == 0) {

								atom_a.Bonds.Add (atom_b.Number);
								atom_b.Bonds.Add (atom_a.Number);
								bonds.Add (new int[2]{atom_a.Number,atom_b.Number});
							}



						} else if (type2 == "H") {
							
							if (atom_b.Bonds.Count == 0) {


								atom_a.Bonds.Add (atom_b.Number);
								atom_b.Bonds.Add (atom_a.Number);
								bonds.Add (new int[2]{atom_a.Number,atom_b.Number});
							}
							
							
							
						} else {

							atom_a.Bonds.Add (atom_b.Number);
							atom_b.Bonds.Add (atom_a.Number);
							bonds.Add (new int[2]{atom_a.Number,atom_b.Number});

						}


						


					}


				}
			}


			Debug.Log("Bonds:"+bonds.Count);


		}

		public void Update(float[] temp){
			
			for (int i=0; i<Atoms.Count; i++) {

				Vector3 tem = new Vector3 (-temp [i*3], temp [i*3+ 1], temp [i*3+2]);

				Atoms[i].Location[Main.current_frame] = Vector3.Lerp (Atoms[i].Location[Main.current_frame], tem, 0.2f);;

			}
				
			CalculateCenters ();
				
		}






	}



}
