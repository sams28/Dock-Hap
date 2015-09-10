using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using MoleculeData;


/// <summary>
/// IMD class
/// </summary>
/// <description>Manages the connection between the simulation server and the Unity main application</description>
public class IMD : MonoBehaviour {


	/// <summary>
	/// Connection to a server
	/// </summary>
	/// <param name="hostname">Server address</param>
	/// <param name="port">Server port </param>
	/// <description>Connects to the given server</description>
	[DllImport ("imd-unity")]
	private static extern void IMD_init(string hostname, int port);
	/// <summary>
	///  Stop the IMD simulation
	/// </summary>
	/// <description>Send a signal to stop the simulation and cuts the connection</description>
	[DllImport ("imd-unity")]
	private static extern int IMD_stop();
	/// <summary>
	/// Is IMD Connected ?
	/// </summary>
	/// <description>Checks if there is a connection opened</description>
	[DllImport ("imd-unity")]
	private static extern bool IMD_isConnected();	

	/// <summary>
	/// Set number of atoms
	/// </summary>
	/// <description>Set the number of atoms used in the simulation</description>
	[DllImport ("imd-unity")]
	private static extern void IMD_setNbParticles(int nbParticles);

	/// <summary>
	/// Pauses the simulation
	/// </summary>
	[DllImport ("imd-unity")]
	private static extern void IMD_pause();
	
	[DllImport ("imd-unity")]
	private static extern void IMD_play();

	/// <summary>
	/// Set the forces
	/// </summary>
	/// <param name="nbforces">Number of atoms to apply the forces</param>
	/// <param name="atomslist">list of the atoms id to apply the forces</param>
	/// <param name="forceslist">list of the forces to apply to each atom</param>
	/// <description>Set the forces to be applied to the atoms</description>
	[DllImport ("imd-unity")]
	private static extern void IMD_setForces(int nbforces, int[] atomslist, float[] forceslist);

	
	/// <summary>
	/// Reset forces
	/// </summary>
	/// <description>Set the forces applied of all atoms to zero</description>
	[DllImport ("imd-unity")]
	private static extern void IMD_resetForces();

	/// <summary>
	/// Get the data of the simulation
	/// </summary>
	/// <param name="verts">List of the atoms positions</param>
	/// <param name="energies">System energies</param>
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
				IMD_stop();
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