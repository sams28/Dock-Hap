using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoleculeData;
using VRPNData;



public class Select{

	public List<Atom> selectedAtoms;
	public List<Residue> selectedResidues;
	public List<Chain> selectedChains;
	private Atom minDistAtom;
	private Residue minDistResidue;
	private Chain minDistChain;



	public GameObject g;
	public ParticleSystem par;

	private bool listen;
	private float lastPressedTime;
	private bool listen2;
	private float lastPressedTime2;
	ParticleSystem.Particle[] p = new ParticleSystem.Particle[100000];


	public Select(){

		selectedAtoms = new List<Atom> ();
		selectedResidues= new List<Residue>();
		selectedChains = new List<Chain> ();
		listen = false;
		listen2 = false;



	}

	
	public Atom MinDistAtom{
		get{return minDistAtom;}
		set{minDistAtom = value;}
	}
	public Residue MinDistResidue{
		get{return minDistResidue;}
		set{minDistResidue = value;}
	}
	public Chain MinDistChain{
		get{return minDistChain;}
		set{minDistChain = value;}
	}


	public bool Listen{
		get{return listen;}
		set{listen = value;}
	}
	
	public float LastPressedTime{
		get{return lastPressedTime;}
		set{lastPressedTime = value;}
	}

	public bool Listen2{
		get{return listen2;}
		set{listen2 = value;}
	}
	
	public float LastPressedTime2{
		get{return lastPressedTime2;}
		set{lastPressedTime2 = value;}
	}

	public void SetClosestElements(Molecule m,GameObject d){
		


		
		minDistChain = m.Chains [0];
		minDistResidue = m.Chains [0].Residues [0];

		minDistAtom = m.Chains [0].Residues [0].Atoms [0];
		float minDist = Vector3.Distance(minDistAtom.Location[Main.current_frame],d.transform.position);
		float dist;
		
		//Future implementation ?
		/*
		Ray ray = new Ray (o.transform.position, o.transform.forward);
		RaycastHit rHit;
	

		if (Physics.Raycast(ray, out rHit)) {

			minDistAtom.atom =  rHit.collider.gameObject;


		}

*/


		for (int i =0; i < m.Chains.Count; i++) {

			if(m.Chains[i].Active){
		
			for (int j =0; j < m.Chains[i].Residues.Count; j++) {

					if(m.Chains[i].Residues[j].Active){

				for (int k =0; k < m.Chains[i].Residues[j].Atoms.Count; k++) {

					if(m.Chains[i].Residues[j].Atoms[k].Active){
					dist =  Vector3.Distance(m.Chains[i].Residues[j].Atoms[k].Location[Main.current_frame],d.transform.position);

					if(dist < minDist){
						minDist = dist;
						minDistAtom = m.Chains[i].Residues[j].Atoms[k];
						minDistResidue = m.Chains[i].Residues[j];
						minDistChain = m.Chains[i];
						
						
								}

							}
						}
					}	
				}
			}
		}



	}

	public void SetParticles(Molecule m,Color32 c){


		int index = 0;
		for(int i=0;i<selectedAtoms.Count;i++){
			p[index].size = 1.0f;
			
			p[index].position = m.Atoms[selectedAtoms[i].Number].Location[Main.current_frame];
			p[index].color = c;
			index++;
			
		}
		
		for (int i=0; i<selectedResidues.Count; i++) {
			for(int j=0;j<selectedResidues[i].Atoms.Count;j++){
				p[index].size = 1.0f;
				
				p[index].position = m.Atoms[selectedResidues[i].Atoms[j].Number].Location[Main.current_frame];
				p[index].color = c;
				
				index++;
				
			}
			
			
		}
		
		for (int i=0; i<selectedChains.Count; i++) {
			for(int j=0;j<selectedChains[i].Atoms.Count;j++){
				p[index].size = 1.0f;
				
				p[index].position = m.Atoms[selectedChains[i].Atoms[j].Number].Location[Main.current_frame];
				p[index].color = c;
				
				index++;
			}
			
			
		}

		
		
		
		switch (m.select) {
		case SelectDisplay.Atom :
			
			p[index].size = 1.0f;
			p[index].position = minDistAtom.Location[Main.current_frame];
			p[index].color = c;
			index++;
			break;
		case SelectDisplay.Residue :
			
			for (int i=0; i<minDistResidue.Atoms.Count; i++) {
				
				p[index].size = 1.0f;
				p[index].position = minDistResidue.Atoms[i].Location[Main.current_frame];
				p[index].color = c;
				index++;
			}
			break;
		case SelectDisplay.Chain :
			
			for (int i=0; i<minDistChain.Atoms.Count; i++) {
				
				p[index].size = 1.0f;
				p[index].position = minDistChain.Atoms[i].Location[Main.current_frame];
				p[index].color = c;
				index++;
				
			}
			
			break;
		default:break;
			
		}
		
		par.SetParticles(p,index);


	}


}





public class SelectAtoms : MonoBehaviour {


	private List<Select> selects;
	private VRPN vrpn;
	private List<Molecule> mol;
	public List<Select> Selects{
		get{return selects;}
		set{selects = value;}
	}


	void Start(){


		vrpn = GetComponent<VRPN> ();

	}



	public void Init(){

		selects = new List<Select> ();
		mol = GetComponent<Main>().molecules;
		for (int i =0; i<vrpn.Devices.Count; i++) {

			Select s= new Select();


			s.g = (GameObject)Instantiate (Resources.Load("Prefabs/halo") as GameObject, mol[0].Location[Main.current_frame], Quaternion.identity);
			//necessary for particles
			s.g.transform.SetParent (Camera.main.transform);

			s.par = s.g.GetComponent<ParticleSystem> ();
			selects.Add(s);
		}





	}



	public void ClosestAtom(List<Device> d){

		for (int i =0; i<selects.Count; i++) {

			selects[i].SetClosestElements(mol[MainUI.current_mol],d[i].obj);
			selects[i].SetParticles(mol[MainUI.current_mol],d[i].c);

		}

	}

	public void SetSelect(SelectDisplay s,int index){
		

		switch (s) {
		case SelectDisplay.Atom :
			
			
			
			
			if(!selects[index].MinDistAtom.Selected[index]){
				
				selects[index].MinDistAtom.Selected[index] = true;
		
				selects[index].MinDistAtom.ForceGameobject[index] = (GameObject)Instantiate (Resources.Load("Prefabs/Arrow") as GameObject,selects[index].MinDistAtom.Location[Main.current_frame], Quaternion.identity);
				selects[index].MinDistAtom.ForceGameobject[index].transform.SetParent(transform,true);
				selects[index].selectedAtoms.Add(selects[index].MinDistAtom);
				
				
			}
			else{
				
				selects[index].MinDistAtom.Selected[index] = false;
				GameObject.Destroy(selects[index].MinDistAtom.ForceGameobject[index]);
				selects[index].selectedAtoms.Remove(selects[index].MinDistAtom);
				
			}
			
			break;
		case SelectDisplay.Residue :
			if(!selects[index].MinDistResidue.Selected[index]){
				selects[index].MinDistResidue.Selected[index] = true;


				selects[index].MinDistResidue.ForceGameobject[index] =(GameObject)Instantiate (Resources.Load("Prefabs/Arrow") as GameObject,selects[index].MinDistResidue.Location[Main.current_frame], Quaternion.identity);
				selects[index].MinDistResidue.ForceGameobject[index].transform.SetParent(transform,true);
				selects[index].selectedResidues.Add(selects[index].MinDistResidue);
			}
			else{
				
				selects[index].MinDistResidue.Selected[index] = false;
				GameObject.Destroy(selects[index].MinDistResidue.ForceGameobject[index]);
				selects[index].selectedResidues.Remove(selects[index].MinDistResidue);
				
			}
			break;
		case SelectDisplay.Chain :
			if(!selects[index].MinDistChain.Selected[index]){
				
				selects[index].MinDistChain.Selected[index] = true;
				selects[index].MinDistChain.ForceGameobject[index] = (GameObject)Instantiate (Resources.Load("Prefabs/Arrow") as GameObject,selects[index].MinDistChain.Location[Main.current_frame], Quaternion.identity);
				selects[index].MinDistChain.ForceGameobject[index].transform.SetParent(transform,true);
				selects[index].selectedChains.Add(selects[index].MinDistChain);
				
			}
			else{
				
				selects[index].MinDistChain.Selected[index] = false;
				GameObject.Destroy(selects[index].MinDistChain.ForceGameobject[index]);
				selects[index].selectedChains.Remove(selects[index].MinDistChain);
				
			}
			break;
		default:break;
			
		}
		
		
	}







	public void FlushSelect(){

		if (selects != null) {
			for (int i =0; i<selects.Count; i++) {

				for (int j=0; j<selects[i].selectedAtoms.Count; j++) {

					Destroy (selects [i].selectedAtoms [j].ForceGameobject [i]);

				}
			
				for (int j=0; j<selects[i].selectedResidues.Count; j++) {

					Destroy (selects [i].selectedResidues [j].ForceGameobject [i]);
				
				}
			
				for (int j=0; j<selects[i].selectedChains.Count; j++) {
					Destroy (selects [i].selectedChains [j].ForceGameobject [i]);

				
				
				}
				ParticleSystem.Particle[] p = new ParticleSystem.Particle[100000];
				selects [i].par.SetParticles (p, 0);
				selects [i].selectedAtoms = new List<Atom> ();
				selects [i].selectedResidues = new List<Residue> ();
				selects [i].selectedChains = new List<Chain> ();

			}
		}

	}

	public void Delete(){

		for (int i =0; i<selects.Count; i++) {
			
			for (int j=0; j<selects[i].selectedAtoms.Count; j++) {
				
				Destroy (selects [i].selectedAtoms [j].ForceGameobject [i]);
				
			}
			
			for (int j=0; j<selects[i].selectedResidues.Count; j++) {
				
				Destroy (selects [i].selectedResidues [j].ForceGameobject [i]);
				
				
				
			}
			
			for (int j=0; j<selects[i].selectedChains.Count; j++) {
				Destroy (selects [i].selectedChains [j].ForceGameobject [i]);
				
				
				
			}
			Destroy (selects[i].g);
		}


	}

	
	// Update is called once per frame
	void Update () {


		if (vrpn.ServerStarted) {


			for(int i =0;i<vrpn.Devices.Count;i++){

				if (vrpn.Devices[i].Button0 && !selects[i].Listen){

					selects[i].Listen = true;
					selects[i].LastPressedTime = Time.realtimeSinceStartup;
				}



				if (!vrpn.Devices[i].Button0 && selects[i].Listen){


					float timeDiff = Time.realtimeSinceStartup - selects[i].LastPressedTime;

					if (timeDiff < 0.2f) {
						SetSelect (GetComponent<Main>().molecules [MainUI.current_mol].select,i);

					
					}
					selects[i].Listen =false;

				
				}


			}

		}



	
	}
}
