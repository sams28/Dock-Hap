using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MoleculeData;

public class DisplayParticles : DisplayMolecule {



	private ParticleSystem.Particle[] particles_atoms;
	private ParticleSystem.Particle[] particles_bonds;


	public override void DisplayHetAtm (bool showHetAtoms)
	{
		throw new NotImplementedException ();
	}

	public override void DisplayWater (bool showWater)
	{
		throw new NotImplementedException ();
	}






	public override void DisplayMol (ColorDisplay c, TypeDisplay t)
	{
		type = t;
		color = c;
		switch (t) {
		case TypeDisplay.Points :  DisplayMolPoints();break;
		//case TypeDisplay.Lines :  DisplayMolLines();break;
		case TypeDisplay.VDW : DisplayMolPoints();break;
		default:DisplayMolPoints(); break;
		}
	}






	public void DisplayMolPoints()
	{
		scale = 1.0f;

		particles_atoms = new ParticleSystem.Particle[mol.Atoms.Count];
		//the Shuriken object must be visible for the camera, so we set it to the camera
		mol.Gameobject.transform.localPosition = mol.Location;
		mol.Gameobject.transform.SetParent (Camera.main.transform);

		for (int i =0; i < mol.Atoms.Count; i++) {
			if(mol.Atoms[i].Active){
			switch(type){
			case TypeDisplay.VDW : particles_atoms[i].size = 1.5f*scale*mol.Atoms[i].AtomRadius;break;
			default : particles_atoms[i].size = scale;break;
			}
			particles_atoms[i].position = mol.Atoms[i].Location;
			particles_atoms[i].color = setColorAtm(mol.Atoms[i],color);
				
			}	
		}
		GetComponent<ParticleSystem>().SetParticles(particles_atoms,mol.Atoms.Count);
		GetComponent<ParticleSystem>().GetComponent<Renderer>().enabled = true;
		
		
		
	}
	public void DisplayMolLines(){

		scale = 1.0f;
		
		particles_bonds = new ParticleSystem.Particle[mol.Bonds.Count*2];
		//the Shuriken object must be visible for the camera, so we set it to the camera
		mol.Gameobject.transform.localPosition = mol.Location;
		mol.Gameobject.transform.SetParent (Camera.main.transform);
		
		for (int i =0; i < mol.Bonds.Count; i++) {
			

			particles_bonds[i*2].size = scale;
			particles_bonds[i*2].position = mol.Atoms [mol.Bonds [i] [0]].Location;
			particles_bonds[i*2].rotation = 60.0f;
			particles_bonds[i*2].color = setColorAtm(mol.Atoms [mol.Bonds [i] [0]],color);

			particles_bonds[i*2+1].size = scale;
			particles_bonds[i*2+1].position = mol.Atoms [mol.Bonds [i] [1]].Location;
			particles_bonds[i*2+1].rotation = 60.0f;
			particles_bonds[i*2+1].color = setColorAtm(mol.Atoms [mol.Bonds [i] [1]],color);




			
		}
		GetComponent<ParticleSystem>().SetParticles(particles_bonds,mol.Bonds.Count*2);
		GetComponent<ParticleSystem>().GetComponent<Renderer>().enabled = true;



	}
	
	public override void UpdateMol(){
		
		switch(type){
		case TypeDisplay.Points : UpdateAtoms();break;
		case TypeDisplay.VDW : UpdateAtoms();break;
		//case TypeDisplay.Lines : UpdateBonds(temp);break;
		//case TypeDisplay.CPK : UpdateAtoms(temp);UpdateBonds(temp);break;
		default:break;
		}
	}


	public void UpdateAtoms(){

		for (int i=0; i<mol.Atoms.Count; i++){
			

			
			particles_atoms[i].position  = mol.Atoms[i].Location;
			
		}

		GetComponent<ParticleSystem>().SetParticles(particles_atoms,mol.Atoms.Count);
		GetComponent<ParticleSystem>().GetComponent<Renderer>().enabled = true;



	}





	void Update(){

		/*
		for (int i=0; i<mol.Bonds.Count; i++){
			

			
			particles_atoms[i].rotation  = 
			
		}
		GetComponent<ParticleSystem>().SetParticles(particles_atoms,mol.Atoms.Count);
		*/
	}



}
