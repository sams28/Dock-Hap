using UnityEngine;
using UnityEngine.UI; 
using MoleculeData;
using System.Collections;
using System.Collections.Generic;




public class SubMenu{

	private string name;
	private bool isActive;
	private GameObject gameobject;
	private List<Transform> elements;

	public SubMenu(GameObject g){
		gameobject = g;
		elements = new List<Transform> ();
		Transform t = g.transform.FindChild ("Elements");
		for (int i =0; i<t.childCount; i++) {
			elements.Add(t.GetChild(i));

		}

		name = g.name;
		isActive = false;


	}

	public string Name{
		get{return name;}
		set{name = value;}
	} 

	public bool IsActive{
		get{return isActive;}
		set{isActive = value;}
	} 
	public GameObject Gameobject{
		get{return gameobject;}
		set{gameobject = value;}
	} 


	public List<Transform> Elements{
	get{return elements;}
	set{elements = value;}
	} 

}





public class MainUI : MonoBehaviour {




	private Camera canvasCamera;
	private bool openMenu,showFPS, typeMenu,colorMenu;
	private List<SubMenu> subMenus;
	private ColorDisplay color;
	private TypeDisplay type;
	private RenderDisplay render;
	public GameObject ui;
	public GameObject contents;


	private int current_mol;
	private List<GameObject> labels_mol;
	private GameObject start;
	private GameObject menu;
	private GameObject perm;
	private GameObject prefab_toggle;

	// Use this for initialization
	void Start () {

		showFPS = true;
		openMenu=false;

		labels_mol = new List<GameObject> ();

		start = ui.transform.FindChild ("Start").gameObject;
		menu = ui.transform.FindChild ("Menu").gameObject;
		perm = ui.transform.FindChild ("Permanent").gameObject;
		perm.transform.FindChild ("FPS").gameObject.SetActive(showFPS);
		prefab_toggle = (Resources.Load ("Prefabs/Toggle_content") as GameObject);


		subMenus = new List<SubMenu>();
		subMenus.Add(new SubMenu(menu.transform.FindChild("Render").gameObject));
		subMenus.Add(new SubMenu(menu.transform.FindChild("Type").gameObject));
		subMenus.Add(new SubMenu(menu.transform.FindChild("Color").gameObject));
		subMenus.Add(new SubMenu(menu.transform.FindChild("VRPN").gameObject));
		subMenus.Add(new SubMenu(menu.transform.FindChild("IMD").gameObject));
		subMenus.Add(new SubMenu(menu.transform.FindChild("Select").gameObject));


		GameObject c = (GameObject)Instantiate (prefab_toggle,new Vector3(0,prefab_toggle.transform.localPosition.y,0), Quaternion.identity);
		c.GetComponent<Toggle> ().isOn = true;
		c.transform.SetParent (contents.transform, false);
		c.GetComponent<Toggle> ().group = contents.GetComponent<ToggleGroup>();
		c.transform.FindChild("Label").GetComponent<Text>().text = "all";
		c.GetComponent<Toggle> ().onValueChanged.AddListener (delegate { SetCurrentMol (1);});

		labels_mol.Add (c);

		current_mol = 1;

		menu.SetActive (false);
		//start.transform.FindChild ("LoadFile").GetComponent<InputField> ().text = Application.dataPath+"/Resources/dyna1.gro";
		start.transform.FindChild ("LoadFile").GetComponent<InputField> ().text = "/Users/Samba/Documents/molecules/imdgroup.gro";
		//start.transform.FindChild ("LoadFile").GetComponent<InputField> ().text = Application.dataPath+"/Resources/1BRS.pdb";
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.Escape) )
		{
			OpenMenu();
		}
	}


	public void AddContents(InputField passField){

		if (GetComponent<Main> ().SetMolecule (passField.text,true)) {

			int count = GetComponent<Main> ().molecules.Count-1;

			if (count > 4) {
				contents.GetComponent<RectTransform> ().sizeDelta = new Vector2 (contents.GetComponent<RectTransform> ().sizeDelta.x, 20 * (count-1));
			}

			GameObject c = (GameObject)Instantiate (prefab_toggle, new Vector3 (0, prefab_toggle.transform.localPosition.y - 20 * (count-1), 0), Quaternion.identity);




			c.transform.SetParent (contents.transform, false);
			c.GetComponent<Toggle> ().group = contents.GetComponent<ToggleGroup> ();
			c.GetComponent<Toggle> ().isOn = true;
			c.transform.FindChild ("Label").GetComponent<Text> ().text = passField.text;
			labels_mol.Add (c);

			c.GetComponent<Toggle> ().onValueChanged.AddListener (delegate {
				SetCurrentMol (count);
			});




			current_mol = count;
			GetComponent<Main> ().SetColors (current_mol);
			GetComponent<Main> ().SetMaterials (current_mol);
			GetComponent<Main> ().DisplayMolecules (current_mol);
		}
	}

	public void SetCurrentMol(int i){

		current_mol = i;


		for(int k=0;k<subMenus[0].Elements.Count;k++){
			if(StringToRenderDisplay(subMenus[0].Elements[k].name) ==  GetComponent<Main> ().molecules [current_mol].render)
				subMenus[0].Elements[k].GetComponent<Toggle>().isOn = true;
			else
				subMenus[0].Elements[k].GetComponent<Toggle>().isOn = false;
		}
		for(int k=0;k<subMenus[1].Elements.Count;k++){
			if(StringToTypeDisplay(subMenus[1].Elements[k].name) ==  GetComponent<Main> ().molecules [current_mol].type)
				subMenus[1].Elements[k].GetComponent<Toggle>().isOn = true;
			else
				subMenus[1].Elements[k].GetComponent<Toggle>().isOn = false;
		}
		for(int k=0;k<subMenus[2].Elements.Count;k++){
			if(StringToColorDisplay(subMenus[2].Elements[k].name) ==  GetComponent<Main> ().molecules [current_mol].color)
				subMenus[2].Elements[k].GetComponent<Toggle>().isOn = true;
			else
				subMenus[2].Elements[k].GetComponent<Toggle>().isOn = false;
		}

		menu.transform.FindChild ("Atoms").FindChild ("InputField").GetComponent<InputField> ().text = labels_mol [current_mol - 1].transform.FindChild ("Label").GetComponent<Text> ().text;

	}

	public void ApplyFrames (InputField passField){
	
		GetComponent<Main> ().SetFrames (passField.text);

	}



	public void ApplyMol (InputField passField){
		if (GetComponent<Main> ().SetMolecule (passField.text,false,current_mol)) {
			labels_mol[current_mol-1].transform.FindChild ("Label").GetComponent<Text> ().text =passField.text;
			GetComponent<Main> ().DisplayMolecules (current_mol);
		}

	}
	public void DeleteMol (){

		if (current_mol > 1) {
			Destroy(labels_mol[current_mol-1]);
			labels_mol.RemoveAt(current_mol-1);
			for(int i=0;i<Main.total_frames;i++){

			Destroy(GetComponent<Main> ().molecules[current_mol].Gameobject[i]);
			}
			GetComponent<Main> ().molecules.RemoveAt(current_mol);
			Resources.UnloadUnusedAssets ();
			current_mol -=1;


		}



		
	}


	public void ShowSubMenu(string s)
	{

		for (int i=0; i < subMenus.Count; i++) {
			if (subMenus[i].Name == s){

				subMenus[i].IsActive = !subMenus[i].IsActive;

			}
			else
				subMenus[i].IsActive = false;
			subMenus[i].Gameobject.transform.FindChild("Elements").gameObject.SetActive (subMenus[i].IsActive);

		}


	}
	


	public RenderDisplay StringToRenderDisplay(string s){
		switch (s) {
		case "UnityObjects": return RenderDisplay.UnityObjects;
		case "Particles": return RenderDisplay.Particles;
		case "Meshs": return RenderDisplay.Meshs;
		default:return RenderDisplay.UnityObjects;
			
		}
		
		
		
	}
	
	
	
	public void ShowRender(string s)
	{
		GetComponent<Main> ().molecules [current_mol].render = StringToRenderDisplay(s);

	}





	public ColorDisplay StringToColorDisplay(string s){
		switch (s) {
		case "Name": return ColorDisplay.Name;
		case "ResName": return ColorDisplay.ResName;
		case "ChainID": return ColorDisplay.ChainID;
		case "ResID": return ColorDisplay.ResID;
		case "Charges": return ColorDisplay.Charges;
		default:return ColorDisplay.Name;

		}



	}



	public void ShowColor(string s)
	{
		GetComponent<Main> ().molecules [current_mol].color = StringToColorDisplay(s);
		GetComponent<Main> ().SetColors (current_mol);
		GetComponent<Main> ().SetMaterials (current_mol);

	}

	public TypeDisplay StringToTypeDisplay(string s){
		switch (s) {
		case "ChainID": return TypeDisplay.ChainID;
		case "CPK": return TypeDisplay.CPK;
		case "Lines" : return TypeDisplay.Lines;
		case "Points": return TypeDisplay.Points;
		case "Surface": return TypeDisplay.Surface;
		case "Trace": return TypeDisplay.Trace;
		case "VDW": return TypeDisplay.VDW;
		default:return TypeDisplay.Points;
			
		}
		
		
		
	}
	
	
	
	public void ShowType(string s)
	{
		GetComponent<Main> ().molecules [current_mol].type = StringToTypeDisplay(s);

	}




	public SelectDisplay StringToSelectDisplay(string s){
		switch (s) {
		case "Atom": return SelectDisplay.Atom;
		case "Residue": return SelectDisplay.Residue;
		case "Chain": return SelectDisplay.Chain;
		case "Molecule": return SelectDisplay.Molecule;
		default:return SelectDisplay.Atom;
			
		}
		
		
		
	}
	
	
	
	public void ShowSelect(string s)
	{
		GetComponent<Main> ().molecules [0].select = StringToSelectDisplay(s);
		GetComponent<SelectAtoms> ().FlushSelect ();

		
	}





	/*

	public void ShowWaterAtm()
	{

		showWater = !showWater;

		GetComponent<Main> ().molecules [0].showWater = showWater;
		GetComponent<Main> ().DisplayWater ();
		
	}

	public void ShowHetAtm(Toggle t)
	{
		showHet = !showHet;

		GetComponent<Main> ().molecules [0].showHetAtoms = showHet;
		GetComponent<Main> ().DisplayHetAtm ();

		if (!showHet)
			t.interactable =false;
		else
			t.interactable =true;

		
	}*/

	public void SetFile(InputField passField)
	{
		GetComponent<Main> ().resource_name = passField.text;
	}
	public void Load(InputField passField)
	{
		GetComponent<Main> ().resource_name = passField.text;
		start.SetActive (false);
		GetComponent<Main> ().Init ();
	}



	public void SetVRPNParam(InputField passField)
	{
		GetComponent<VRPN> ().setParams(passField.text);
	}


	public void SetIMDServer(InputField passField)
	{
		GetComponent<IMD> ().Server = passField.text;
		
	}
	public void SetIMDPort(InputField passField)
	{
		GetComponent<IMD> ().Port = int.Parse (passField.text);
		
	}




	public void VRPNStart(){
		GetComponent<SelectAtoms> ().Init ();
		GetComponent<VRPN> ().Init ();

	}

	public void VRPNStop(){

		GetComponent<SelectAtoms> ().Delete ();
		GetComponent<VRPN> ().Stop ();
		
	}



	public void OpenMenu() //opens the menu.
	{
		openMenu=!openMenu;
		menu.gameObject.SetActive(openMenu); 
	}

	public void ShowFPS()
	{
		showFPS = !showFPS;

		perm.transform.FindChild ("FPS").gameObject.SetActive(showFPS);

	}

	public void ResetGame(){

		Application.LoadLevel (0);
	}


	public void QuitGame()
	{
		Application.Quit();
	}


	//assigns the main camera to all canvasis that are not set to "Screen Space-Overlay".
	/*void OnLevelWasLoaded()  
	{
		canvasCamera=Camera.main;
		menu.gameObject.SetActive(true);
		Canvas[] X=transform.GetComponentsInChildren<Canvas>();
		foreach (Canvas x in X)
		{
			if (x.worldCamera==null)
				x.worldCamera=canvasCamera;
		}
		menu.gameObject.SetActive(false);


	}
*/

}
