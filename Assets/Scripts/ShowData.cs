using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MoleculeData;

public class ShowData : MonoBehaviour {



	private Text text;
	public GameObject molecule;
	private Molecule mol;
	private string format;

	// Use this for initialization
	void Start () {

		text = this.GetComponent<Text>();
		mol = molecule.GetComponent<Molecule>();
	
	}
	
	// Update is called once per frame
	void Update () {


		format = "Step :"+mol.Energies.tstep.ToString() + "\n" + 
				"Temp :"+mol.Energies.T.ToString() + "\n" + 
				"Total Energy :"+mol.Energies.Etot.ToString() + "\n" +
				"Potential Energy :"+mol.Energies.Epot.ToString() + "\n" +
				"Van der Walls Energy :"+mol.Energies.Evdw.ToString() + "\n" +
				"Electrostatic Energy :"+mol.Energies.Eelec.ToString() + "\n";
		text.text = format;  
	}
}
