using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoleculeData;

public class ApplyForces : MonoBehaviour {





	
	public float gaussianMean = 1.8f;
	public float gaussianDeviation = 0.8f;
	public float magnetFeedbackScale = 0.7f;
	
	public float linearForceFeedbackFactor = 6.0f;
	private Vector3[] lastPosition;
	private List<GameObject> arrow;
	private VRPN vrpn;
	private IMD imd;
	private SelectAtoms s;
	private float[] forces;
	private int[] atom_id;

	void Start(){

		lastPosition = new Vector3[10];
		vrpn = GetComponent<VRPN> ();
		imd = GetComponent<IMD> ();
		s = GetComponent<SelectAtoms> ();


	}


	private float Gaussian(float distance, float mean, float deviation) {
		return Mathf.Exp(-((Mathf.Pow ((distance - mean) / deviation, 2))) / 2);

	}




	// Magnetic force
	public void setForceForAtomPosition(Molecule m)
	{
		if (!vrpn.ServerStarted) return;

		for (int i=0; i<vrpn.Devices.Count; i++) {




			Vector3 minAtm = s.Selects[i].MinDistAtom.Location;

			// Compute the distance between the atom and the picker for each axis
			Vector3 forceFactor = (minAtm - vrpn.Devices[i].obj.transform.position);
			// Compute the absolute distance between the atom and the picker

			// Compute a gaussian factor
			float gaussian = Gaussian (Vector3.Distance (minAtm, vrpn.Devices[i].obj.transform.position), gaussianMean, gaussianDeviation);
		
		
		
			forceFactor /= Vector3.Distance (minAtm, vrpn.Devices[i].obj.transform.position);
			forceFactor *= gaussian * magnetFeedbackScale;

			Vector3 feedbackForce = Camera.main.transform.worldToLocalMatrix * forceFactor;

			feedbackForce.x -= vrpn.Devices[i].TrackerVelocity.x;
			feedbackForce.y -= vrpn.Devices[i].TrackerVelocity.y;
			feedbackForce.z -= vrpn.Devices[i].TrackerVelocity.z;
		
			/*
	
		if (distance < 0.5f) {

			forceFactor *= 0f;

		} else{


			forceFactor = 120.0f*forceFactor.normalized/(Mathf.Pow (distance, 2));

		}
*/

			vrpn.SetForce (feedbackForce, vrpn.Devices[i]);

		}

	}




	public void applyForcesVector(){


		
		for (int z=0; z<vrpn.Devices.Count; z++) {


			List<Atom> l = new List<Atom> ();

			switch (GetComponent<Main> ().molecules [0].select) {
			case SelectDisplay.Atom :
					l= s.Selects[z].selectedAtoms;
				
				break;
			case SelectDisplay.Residue :
				for(int i=0;i<s.Selects[z].selectedResidues.Count;i++){
					l.AddRange(s.Selects[z].selectedResidues[i].Atoms);
					
					
				}

				break;
			case SelectDisplay.Chain :
				for(int i=0;i<s.Selects[z].selectedChains.Count;i++){
					l.AddRange(s.Selects[z].selectedChains[i].Atoms);
				}
				
				break;
			default:break;
				
			}



			if (vrpn.ServerStarted && imd.IsIMDRunning ()) {
				
				if (vrpn.Devices[z].Button0 && !s.Selects[z].Listen2) {
					
					s.Selects[z].Listen2 = true;
					s.Selects[z].LastPressedTime2 = Time.realtimeSinceStartup;
					lastPosition[z] = vrpn.Devices[z].obj.transform.position;
				}
				
				if (s.Selects[z].Listen2) {
					
					float timeDiff = Time.realtimeSinceStartup - s.Selects[z].LastPressedTime;
					
					if (timeDiff > 1.0f) {
						
						
						if (l.Count > 0) {



							Vector3 barycentre = Vector3.zero;
							for(int i = 0; i < l.Count;i++)
								barycentre+=l[i].Location;

							barycentre /= l.Count;

							Vector3 force = vrpn.Devices[z].obj.transform.position - lastPosition[z];
							float dist = Vector3.Distance(vrpn.Devices[z].obj.transform.position,lastPosition[z]);
							

							forces = new float[l.Count * 3];
							atom_id = new int[l.Count];

							for(int i = 0; i < l.Count;i++){

								forces[i*3] = -force.x;
								forces[i*3+1] = force.y;
								forces[i*3+2] = force.z;
								atom_id[i] = l[i].Number;

							}


							switch (GetComponent<Main> ().molecules [0].select) {
							case SelectDisplay.Atom :

								for(int i = 0; i <s.Selects[z].selectedAtoms.Count;i++){
									s.Selects[z].selectedAtoms[i].ForceGameobject[z].transform.localPosition = s.Selects[z].selectedAtoms[i].Location;
									s.Selects[z].selectedAtoms[i].ForceGameobject[z].transform.up = force;
									s.Selects[z].selectedAtoms[i].ForceGameobject[z].transform.localScale = new Vector3(dist/50.0f,dist/8.0f,dist/50.0f);


								}

								break;
							case SelectDisplay.Residue :
								for(int i=0;i<s.Selects[z].selectedResidues.Count;i++){
									s.Selects[z].selectedResidues[i].ForceGameobject[z].transform.localPosition = s.Selects[z].selectedResidues[i].Location;
									s.Selects[z].selectedResidues[i].ForceGameobject[z].transform.up = force;
									s.Selects[z].selectedResidues[i].ForceGameobject[z].transform.localScale = new Vector3(dist/50.0f,dist/8.0f,dist/50.0f);

									
									
								}
								
								break;
							case SelectDisplay.Chain :
								for(int i=0;i<s.Selects[z].selectedChains.Count;i++){
									s.Selects[z].selectedChains[i].ForceGameobject[z].transform.localPosition = s.Selects[z].selectedChains[i].Location;
									s.Selects[z].selectedChains[i].ForceGameobject[z].transform.up = force;
									s.Selects[z].selectedChains[i].ForceGameobject[z].transform.localScale = new Vector3(dist/50.0f,dist/8.0f,dist/50.0f);

								}
								
								break;
							default:break;
								
							}

							//Debug.Log (force);
							setLinearForceForVector(-force,vrpn.Devices[z]);
							imd.setForces(atom_id,forces);
							
						}
						
					}
					
				}
				
				
				if (!vrpn.Devices[z].Button0 && s.Selects[z].Listen2) {

					if (l.Count > 0) {

						forces = new float[l.Count * 3];
						int[] atom_id = new int[l.Count];
						for(int i = 0; i < l.Count;i++){
							
							//Vector3 force = vrpn.obj.transform.position - l[i].Gameobject.transform.position;
							//float dist = Vector3.Distance(vrpn.obj.transform.position,l[i].Gameobject.transform.position);
							forces[i] = 0.0f;
							forces[i+1] = 0.0f;
							forces[i+2] = 0.0f;
							atom_id[i] = l[i].Number;

							
						}

						switch (GetComponent<Main> ().molecules [0].select) {
						case SelectDisplay.Atom :
							
							for(int i = 0; i <s.Selects[z].selectedAtoms.Count;i++){
								s.Selects[z].selectedAtoms[i].ForceGameobject[z].transform.localScale =  Vector3.zero;
								
								
							}
							
							break;
						case SelectDisplay.Residue :
							for(int i=0;i<s.Selects[z].selectedResidues.Count;i++){
								s.Selects[z].selectedResidues[i].ForceGameobject[z].transform.localScale =  Vector3.zero;
								
								
								
							}
							
							break;
						case SelectDisplay.Chain :
							for(int i=0;i<s.Selects[z].selectedChains.Count;i++){
								s.Selects[z].selectedChains[i].ForceGameobject[z].transform.localScale =  Vector3.zero;
								
							}
							
							break;
						default:break;
							
						}


						resetForce(vrpn.Devices[z]);
						imd.setForces(atom_id,forces);
						imd.resetForces();
						
					}

					s.Selects[z].Listen2 = false;
					
				}


			}

		}
	}

	
	public void setLinearForceForVector(Vector3 v,Device d) {
		if (!vrpn.ServerStarted) return;
		
		
		
		
		Vector3 nv = Camera.main.transform.worldToLocalMatrix * v;
		float distance = Mathf.Sqrt (Mathf.Pow (nv.x, 2) + Mathf.Pow (nv.y, 2) + Mathf.Pow (nv.z, 2));
		nv.Normalize();
		
		nv *= 1 / Mathf.Pow(1 + Mathf.Exp(-(distance - 4)), 2);
		nv *= linearForceFeedbackFactor;
		
		
		vrpn.SetForce(nv,d);
	}
	
	public void resetForce(Device d)
	{
		if (!vrpn.ServerStarted) return;
		vrpn.SetForce(Vector3.zero,d);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
