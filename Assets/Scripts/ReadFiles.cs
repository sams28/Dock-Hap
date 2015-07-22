﻿using UnityEngine;
using System;
using System.Collections;
using System.Text;	
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using MoleculeData;

namespace LoadData{

	public class ReadFiles{
	
	
		static private int nowline;
		static private int nowchain;
		static private int nowresidue;
		static private int nbatom;
		static private Vector3 vect;
		static private string s;
		static private	float x;
		static private	float y;
		static private	float z;
		static private string atomname;
		static private string resname;
		static private int resID;
		static private int lastresID;
		static private string chainID;
		static private string lastchainID;



		public static Molecule ReadPDB(TextReader sr){

			Molecule mol = new Molecule ();
			Chain c = new Chain();
			Residue r = new Residue();
			lastresID =-1;
			lastchainID = "NONE";
			nowline = 0;
			nowresidue = -1;
			nowchain = -1;
			nbatom = 0;
			
			while((s=sr.ReadLine())!=null) {
				if(s.StartsWith("ENDMDL"))
					break;
				
				if(s.Length>4) {
					if(s.StartsWith("ATOM")) {


						chainID = s.Substring(21,1).Trim();


						if(String.Compare(lastchainID,chainID) !=0){

							c=new Chain("ATOM",chainID);
							mol.Chains.Add(c);
							nowchain++;
							nowresidue =-1;

							lastchainID = chainID;
							
						}

						resID = int.Parse(s.Substring(22,4));

						if((lastresID != resID)){
							resname=s.Substring(17,3).Trim();
							r = new Residue(resname,resID,c);
							mol.Chains[nowchain].Residues.Add(r);
							mol.Residues.Add(r);
							nowresidue++;
							lastresID = resID;
							
						}



						
						atomname=s.Substring(12,4).Trim();

						//Unity has a left-handed coordinates system while PDBs are right-handed
						//So we have to reverse the X coordinates
						x=-float.Parse(s.Substring(30,8));
						y=float.Parse(s.Substring(38,8));
						z=float.Parse(s.Substring(46,8));
						vect = new Vector3(x,y,z);
						Atom at = new Atom(vect,atomname,0.0f,nbatom,r,c);
						mol.Chains[nowchain].Residues[nowresidue].Atoms.Add(at);
						mol.Chains[nowchain].Atoms.Add(at);
						mol.Atoms.Add(at);
						nbatom++;
					
						
						
					}
					
					if(s.StartsWith("HETATM")) {
							
							

						
						chainID = s.Substring(21,1).Trim();
						
						
						if(String.Compare(lastchainID,chainID) !=0){
							
							c=new Chain("HETATM",chainID);
							mol.Chains.Add(c);
							nowchain++;
							nowresidue =-1;
							
							lastchainID = chainID;
							
						}
						
						resID = int.Parse(s.Substring(22,4));
						
						if((lastresID != resID)){
							resname=s.Substring(17,3).Trim();
							r = new Residue(resname,resID,c);
							mol.Chains[nowchain].Residues.Add(r);
							mol.Residues.Add(r);
							nowresidue++;
							lastresID = resID;
							
						}
						
						
						
						
						atomname=s.Substring(12,4).Trim();
						
						//Unity has a left-handed coordinates system while PDBs are right-handed
						//So we have to reverse the X coordinates
						x=-float.Parse(s.Substring(30,8));
						y=float.Parse(s.Substring(38,8));
						z=float.Parse(s.Substring(46,8));
						vect = new Vector3(x,y,z);
						Atom at = new Atom(vect,atomname,0.0f,nbatom,r,c);
						mol.Chains[nowchain].Residues[nowresidue].Atoms.Add(at);
						mol.Chains[nowchain].Atoms.Add(at);
						mol.Atoms.Add(at);
						nbatom++;

						
						
					}

					nowline++;
				}
			}
			
			sr.Close ();
		
			Debug.Log ("atoms:" + mol.Atoms.Count);
			return mol;
			
		}


		public static Molecule ReadGRO(TextReader sr){
			lastresID =-1;
			lastchainID = "NONE";
			nowresidue = -1;
			nowchain = 0;
			
			nbatom=0;

			Molecule mol = new Molecule ();
			Residue r =new Residue();
			chainID = "None";
			mol.Chains.Add(new Chain("ATOM",chainID));
			mol.Chains.Add(new Chain("HETATM",chainID));
			//We skip the first two lines
			sr.ReadLine ();
			sr.ReadLine ();

			while ((s=sr.ReadLine())!=null) {

				if(s.Length>50) {



					resID = int.Parse(s.Substring(0,5));

					if((lastresID != resID)){
						resname = s.Substring(5,5).Trim();

						if(resname != "SOL" &&resname != "DLC"){
							r =new Residue(resname,resID,mol.Chains[0]);
							/*everything after dlc is a hetatm
							if(nowchain != 0){
								nowresidue = -1;
							}
							nowchain =0;
							*/
							mol.Chains[nowchain].Residues.Add(r);
							mol.Residues.Add(r);

						}
						else{
							r =new Residue(resname,resID,mol.Chains[1]);
							if(nowchain != 1){
								nowresidue = -1;
							}
							nowchain =1;
							mol.Chains[nowchain].Residues.Add(r);
							mol.Residues.Add(r);
						}

						nowresidue++;
						lastresID = resID;
						
					}
					
					
					
					
					atomname = s.Substring(10,5).Trim ();
					
					//Unity has a left-handed coordinates system while PDBs are right-handed
					//So we have to reverse the X coordinates
					x = -float.Parse (s.Substring(20,8));
					y = float.Parse (s.Substring(28,8));
					z = float.Parse (s.Substring(36,8));
					//convert nanometers to Angstorms
					x*=10;
					y*=10;
					z*=10;
					vect = new Vector3(x,y,z);

					if(resname != "SOL" &&resname != "DLC" ){
						Atom at =new Atom(vect,atomname,0.0f,nbatom,r,mol.Chains[0]); 
						mol.Chains[nowchain].Residues[nowresidue].Atoms.Add(at);
						mol.Chains[nowchain].Atoms.Add(at);
						mol.Atoms.Add(at);
					}
					else{
						Atom at =new Atom(vect,atomname,0.0f,nbatom,r,mol.Chains[1]); 
						mol.Chains[nowchain].Residues[nowresidue].Atoms.Add(at);
						mol.Chains[nowchain].Atoms.Add(at);
						mol.Atoms.Add(at);
					}
						
					nbatom++;

					

					
					
					
				}



				nowline++;

			}


			sr.Close ();
			Debug.Log ("atoms:" + mol.Atoms.Count);
			return mol;




		}







		/*


		public static Molecule ReadMOL2(TextReader sr){
			nowline = 0;
			nbatoms=0;
				
			Molecule mol = new Molecule ();
			bool t = false;
			string[] sl;
			float ch;
				
			while ((s=sr.ReadLine())!=null) {
				
				if (s.StartsWith ("@<TRIPOS>BOND")) {
					t = false;
					
				}
				
				
				if (t == true) {
					sl = s.Split (new Char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
					
					
					
					atomname = sl [1];
					
					
					
					//we have to reverse the Z coordinates
					x = float.Parse (sl [2]);
					y = float.Parse (sl [3]);
					z = -float.Parse (sl [4]);
					
					vect = new Vector3 (x, y, z);
					
					ch = float.Parse (sl [8]);
					
					a = new Atom (vect, atomname, ch);
					mol.Atoms.Add (a);
					
					nbatoms ++;
					
					
					
				}
				if (s.StartsWith ("@<TRIPOS>ATOM")) {
					t = true;
					
				}
				
				
				nowline++;
				
			}
			
			sr.Close ();
			Debug.Log ("atoms:" + mol.Atoms.Count);

			return mol;
		}

		*/

	}
}