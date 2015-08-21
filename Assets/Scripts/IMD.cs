using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using MoleculeData;

public class IMD : MonoBehaviour {


	
	//Lets make our calls from the Plugin
	[DllImport ("imd-unity")]
	private static extern void IMD_init(string hostname, int port);
	
	[DllImport ("imd-unity")]
	private static extern int IMD_start();
	
	[DllImport ("imd-unity")]
	private static extern int IMD_stop();
	
	[DllImport ("imd-unity")]
	private static extern bool IMD_isConnected();	
	
	[DllImport ("imd-unity")]
	private static extern void IMD_setNbParticles(int nbParticles);
	
	[DllImport ("imd-unity")]
	private static extern void IMD_pause();
	
	[DllImport ("imd-unity")]
	private static extern void IMD_play();
	
	[DllImport ("imd-unity")]
	private static extern void IMD_setForces(int nbforces, int[] atomslist, float[] forceslist);
	
	[DllImport ("imd-unity")]
	private static extern void IMD_resetForces();
	
	[DllImport ("imd-unity")]
	private static extern void IMD_getThings([In, Out] float[] verts,[In, Out] ref IMDEnergies energies);
	

	private bool init = false;
	private bool pause=false;
	private int index;
	private float[] temp_pos;
	private string server = "localhost";
	private int port = 3000;
	private List<Molecule> molecules;
	private IMDEnergies energies;
	public string Server{
		get{return server;}
		set{server = value;}
	} 
	public int Port{
		get{return port;}
		set{port = value;}
	} 

	
	public void initIMD(){

		Main.current_frame = Main.total_frames;
		Main.total_frames += 1;


		molecules = GetComponent<Main> ().molecules;
		energies= new IMDEnergies();
		temp_pos = new float[molecules[0].Atoms.Count*3];
		if(IMD_isConnected())
			IMD_stop();
		
		IMD_init(server, port);
		IMD_setNbParticles(molecules[0].Atoms.Count);

		for (int i =0; i<molecules.Count; i++) {
			molecules[i].Gameobject[Main.current_frame]=molecules[i].Gameobject[Main.current_frame-1];
			for(int j=0;j<molecules[i].Atoms.Count;j++){
				molecules[i].Atoms[j].Gameobject[Main.current_frame]=molecules[i].Atoms[j].Gameobject[Main.current_frame-1];
			}
		}
		init = true;

	}
	
	public void setForces(int[] atoms, float[] forces)
	{
		IMD_setForces(atoms.Length, atoms, forces);
	}
	
	public void resetForces()
	{
		IMD_resetForces();
	}

	public void PauseIMD(){

		if (init) {

			if (!pause) {
				IMD_pause ();
				pause = true;
			} else {
				IMD_init (server, port);
				IMD_setNbParticles (molecules[0].Atoms.Count);
				pause = false;

			}

		}
	}

	public bool IsIMDRunning(){

		return(IMD_isConnected () && init && !pause);
	}



	void Update() {

		if (IsIMDRunning ()) {

			IMD_getThings (temp_pos, ref energies);



			molecules[0].Update(temp_pos);
			molecules[0].Energies = energies;
			for(int i =1;i<molecules.Count;i++){

				molecules[i].Update(temp_pos);
				molecules[i].Energies = energies;

				molecules[i].Gameobject[Main.current_frame]=molecules[i].Gameobject[Main.current_frame-1];
				molecules[i].Gameobject[Main.current_frame].GetComponent<DisplayMolecule>().UpdateMol();
			}

		}
			
	}
		



	void OnDisable () {
		if (IMD_isConnected ()) {

			Debug.Log ("Exiting sim Disable");
			Debug.Log ("Stop: " + IMD_stop ());

		}
	}
	

}