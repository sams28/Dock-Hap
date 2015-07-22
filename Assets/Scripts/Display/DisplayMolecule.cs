using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MoleculeData;

public abstract class DisplayMolecule : MonoBehaviour {

	

	public float scale=1;

	protected TypeDisplay type;
	protected ColorDisplay color;
	protected Material mat;
	protected Molecule mol;
	protected float minCharges;
	protected float maxCharges;




	protected static Color[] tableColor = {
		/* X  */ Color.black,Color.white, Color.magenta,Color.magenta,Color.magenta, Color.magenta,  
		/* C  */ Color.green, Color.blue, Color.red, Color.magenta, Color.magenta, 
		/* Na */ Color.magenta, Color.magenta, Color.magenta, Color.magenta, new Color(0.5f,0.5f,0.2f),
		/* S  */ Color.yellow, Color.magenta, Color.magenta, Color.magenta, Color.magenta, Color.magenta,

	};
	
	
	
	
	
	
	
	
	
	
	
	
	public void Init(Molecule mo,Material ma){

		mol = mo;
		mat= ma;
		GradientCharges ();

	}


	abstract public void DisplayMol (ColorDisplay color,TypeDisplay type);
	abstract public void UpdateMol ();
	abstract public void DisplayHetAtm (bool showHetAtoms);
	abstract public void DisplayWater (bool showWater);






	public void GradientCharges(){
		minCharges=0.0f;
		maxCharges=0.0f;
		
		for (int i =0; i < mol.Atoms.Count; i++) {
			
			if (mol.Atoms[i].AtomCharge > maxCharges) {
				minCharges = mol.Atoms[i].AtomCharge;
			}
			if (mol.Atoms[i].AtomCharge < minCharges) {
				maxCharges = mol.Atoms[i].AtomCharge;
			}
			
			
		}


	}


		protected Color setColorAtm(Atom a,ColorDisplay color){
			
		switch (color) {

		case ColorDisplay.Name: return a.ObjColor;
		case ColorDisplay.Charges:
			if(a.AtomCharge >= 0){
				return (new Color((a.AtomCharge)/maxCharges,0f,1f,1f));
			}
			else{
				return(new Color((a.AtomCharge)/-minCharges,0f,1f,1f));
			}
		case ColorDisplay.ResName: return a.AtomResidue.ObjColor;
		case ColorDisplay.ResID:  return a.AtomResidue.ObjColor;
		case ColorDisplay.ChainID:return a.AtomChain.ObjColor;
	
		default:return Color.white;
			

			}
			
		}

	
	protected Material setMaterialAtm(Atom a,ColorDisplay color){
		
		switch (color) {
			
		case ColorDisplay.Name: return a.ObjMaterial;
		case ColorDisplay.Charges:
			Material m =new Material(mat);
			if(a.AtomCharge >= 0){

				m.color = (new Color((a.AtomCharge)/maxCharges,0f,1f,1f));
				return m;
			}
			else{
				m.color =(new Color((a.AtomCharge)/-minCharges,0f,1f,1f));
				return m;
			}
		case ColorDisplay.ResName: return a.AtomResidue.ObjMaterial;
		case ColorDisplay.ResID:  return a.AtomResidue.ObjMaterial;
		case ColorDisplay.ChainID:return a.AtomChain.ObjMaterial;
			
		default: return mat;
			
			
		}
		
	}
	



















}
