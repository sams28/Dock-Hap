using UnityEngine;
using UnityEngine.UI; 
using MoleculeData;
using System.Collections;
using System.Collections.Generic;
using VRPNData;



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



	private Vector2 deg;
	private Vector3 trans;
	private Vector2 pos;
	private Quaternion rot;
	
	
	public Vector3 center= new Vector3 (0, 0, 0);
	public float sensitivityX = 0.5f;
	public float sensitivityY = 0.5f;





	private Camera canvasCamera;
	private bool openMenu, typeMenu,colorMenu,optionsMenu;
	private List<SubMenu> subMenus;
	private ColorDisplay color;
	private TypeDisplay type;
	private RenderDisplay render;

	public GameObject system;
	public GameObject contents;


	static public int current_mol;
	private List<GameObject> labels_mol;
	private GameObject start;
	private GameObject menu;
	private GameObject perm;
	private GameObject prefab_toggle;
	private Transform axes;
	// Use this for initialization
	void Start () {

		openMenu=false;

		labels_mol = new List<GameObject> ();

		start = transform.FindChild ("Start").gameObject;
		menu = transform.FindChild ("Menu").gameObject;
		perm = transform.FindChild ("Permanent").gameObject;
		axes = perm.transform.FindChild ("Axe").FindChild ("axes");
		prefab_toggle = (Resources.Load ("Prefabs/Toggle_content") as GameObject);


		subMenus = new List<SubMenu>();
		subMenus.Add(new SubMenu(menu.transform.FindChild("Render").gameObject));
		subMenus.Add(new SubMenu(menu.transform.FindChild("Type").gameObject));
		subMenus.Add(new SubMenu(menu.transform.FindChild("Color").gameObject));
		subMenus.Add(new SubMenu(menu.transform.FindChild("VRPN").gameObject));
		subMenus.Add(new SubMenu(menu.transform.FindChild("IMD").gameObject));
		subMenus.Add(new SubMenu(menu.transform.FindChild("Select").gameObject));
		subMenus.Add(new SubMenu(menu.transform.FindChild("Options").gameObject));

		GameObject c = (GameObject)Instantiate (prefab_toggle,new Vector3(0,prefab_toggle.transform.localPosition.y,0), Quaternion.identity);
		c.GetComponent<Toggle> ().isOn = true;
		c.transform.SetParent (contents.transform, false);
		c.GetComponent<Toggle> ().group = contents.GetComponent<ToggleGroup>();
		c.transform.FindChild("Label").GetComponent<Text>().text = "all";
		c.GetComponent<Toggle> ().onValueChanged.AddListener (delegate { SetCurrentMol (1);});

		labels_mol.Add (c);

		current_mol = 1;

		menu.SetActive (false);
		//start.transform.FindChild ("LoadMolecule").GetComponent<InputField> ().text = Application.dataPath+"/Resources/dyna1.gro";
		start.transform.FindChild ("LoadMolecule").GetComponent<InputField> ().text = "/Users/Samba/Documents/molecules/imdgroup.gro";
		//start.transform.FindChild ("LoadMolecule").GetComponent<InputField> ().text = Application.dataPath+"/Resources/1BRS.pdb";
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!start.activeInHierarchy) {

			MoveMolecule(menu.activeInHierarchy);
		}

		if (Input.GetKeyDown(KeyCode.Escape) )
		{
			OpenMenu();
		}


	}




	public void AddContents(InputField passField){

		if (system.GetComponent<Main> ().SetMolecule (passField.text,true)) {

			int count = system.GetComponent<Main> ().molecules.Count-1;

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
			system.GetComponent<Main> ().SetColors (current_mol);
			system.GetComponent<Main> ().SetMaterials (current_mol);
			system.GetComponent<Main> ().DisplayMolecules (current_mol);
		}
	}

	public void SetCurrentMol(int i){

		current_mol = i;


		for(int k=0;k<subMenus[0].Elements.Count;k++){
			if(StringToRenderDisplay(subMenus[0].Elements[k].name) ==  system.GetComponent<Main> ().molecules [current_mol].render)
				subMenus[0].Elements[k].GetComponent<Toggle>().isOn = true;
			else
				subMenus[0].Elements[k].GetComponent<Toggle>().isOn = false;
		}
		for(int k=0;k<subMenus[1].Elements.Count;k++){
			if(StringToTypeDisplay(subMenus[1].Elements[k].name) ==  system.GetComponent<Main> ().molecules [current_mol].type)
				subMenus[1].Elements[k].GetComponent<Toggle>().isOn = true;
			else
				subMenus[1].Elements[k].GetComponent<Toggle>().isOn = false;
		}
		for(int k=0;k<subMenus[2].Elements.Count;k++){
			if(StringToColorDisplay(subMenus[2].Elements[k].name) ==  system.GetComponent<Main> ().molecules [current_mol].color)
				subMenus[2].Elements[k].GetComponent<Toggle>().isOn = true;
			else
				subMenus[2].Elements[k].GetComponent<Toggle>().isOn = false;
		}

		menu.transform.FindChild ("Atoms").FindChild ("InputField").GetComponent<InputField> ().text = labels_mol [current_mol - 1].transform.FindChild ("Label").GetComponent<Text> ().text;

	}

	public void ApplyFrames (InputField passField){
	
		system.GetComponent<Main> ().SetFrames (passField.text);

	}



	public void ApplyMol (InputField passField){
		if (system.GetComponent<Main> ().SetMolecule (passField.text,false,current_mol)) {
			labels_mol[current_mol-1].transform.FindChild ("Label").GetComponent<Text> ().text =passField.text;
			system.GetComponent<Main> ().DisplayMolecules (current_mol);
		}

	}
	public void DeleteMol (){

		if (current_mol > 1) {
			Destroy(labels_mol[current_mol-1]);
			labels_mol.RemoveAt(current_mol-1);
			for(int i=0;i<Main.total_frames;i++){

				Destroy(system.GetComponent<Main> ().molecules[current_mol].Gameobject[i]);

			}
			system.GetComponent<Main> ().molecules.RemoveAt(current_mol);
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
		system.GetComponent<Main> ().molecules [current_mol].render = StringToRenderDisplay(s);

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
		system.GetComponent<Main> ().molecules [current_mol].color = StringToColorDisplay(s);
		system.GetComponent<Main> ().SetColors (current_mol);
		system.GetComponent<Main> ().SetMaterials (current_mol);

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
		system.GetComponent<Main> ().molecules [current_mol].type = StringToTypeDisplay(s);

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
		system.GetComponent<Main> ().molecules [current_mol].select = StringToSelectDisplay(s);
		system.GetComponent<SelectAtoms> ().FlushSelect ();

		
	}


	public void SetFileMol(InputField passField)
	{
		system.GetComponent<Main> ().molecule_file = passField.text;
	}

	public void SetFileTraj(InputField passField)
	{
		system.GetComponent<Main> ().trajectory_file = passField.text;
	}

	public void Load()
	{
		start.SetActive (false);
		system.GetComponent<Main> ().Init ();
	}



	public void SetVRPNParam(InputField passField)
	{
		system.GetComponent<VRPN> ().setParams(passField.text);
	}


	public void SetIMDServer(InputField passField)
	{
		system.GetComponent<IMD> ().Server = passField.text;
		
	}
	public void SetIMDPort(InputField passField)
	{
		system.GetComponent<IMD> ().Port = int.Parse (passField.text);
		
	}




	public void VRPNStart(){
		system.GetComponent<SelectAtoms> ().Init ();
		system.GetComponent<VRPN> ().Init ();

	}

	public void VRPNStop(){

		system.GetComponent<SelectAtoms> ().Delete ();
		system.GetComponent<VRPN> ().Stop ();
		
	}



	public void OpenMenu() //opens the menu.
	{
		openMenu=!openMenu;
		menu.gameObject.SetActive(openMenu); 
	}


	public void ChangeSize(Slider s)
	{
		Main.globalScale =s.value;
		
	}


	public void ShowFPS()
	{
		Main.options.showFPS = !Main.options.showFPS;
		subMenus.Find (x => x.Name == "Options").Elements.Find (x => x.name == "FPS_Toggle").GetComponent<Toggle>().isOn = Main.options.showFPS;
		perm.transform.FindChild ("FPS").gameObject.SetActive(Main.options.showFPS);

	}

	public void UseOldUnityObj()
	{
		Main.options.oldUnityObj = !Main.options.oldUnityObj;
		subMenus.Find (x => x.Name == "Options").Elements.Find (x => x.name == "UnityObj").GetComponent<Toggle>().isOn = Main.options.oldUnityObj;
	}


	public void UseVsync()
	{
		Main.options.activateV_sync = !Main.options.activateV_sync;
		subMenus.Find (x => x.Name == "Options").Elements.Find (x => x.name == "V-sync").GetComponent<Toggle>().isOn = Main.options.activateV_sync;
		if (Main.options.activateV_sync) {
			QualitySettings.vSyncCount = 1;
		} else {
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = -1;
			
		}


	}

	public void UseFakeCharges()
	{
		Main.options.activateFakeCharges = !Main.options.activateFakeCharges;
		subMenus.Find (x => x.Name == "Options").Elements.Find (x => x.name == "Charges").GetComponent<Toggle>().isOn = Main.options.activateFakeCharges;
		
	}

	public void UseBones()
	{
		Main.options.activateBones = !Main.options.activateBones;
		subMenus.Find (x => x.Name == "Options").Elements.Find (x => x.name == "Bones").GetComponent<Toggle>().isOn = Main.options.activateBones;
		
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
	public void MoveMolecule(bool menu){

		bool b;
		
		
		if(menu)
			b = Input.mousePosition.x > Screen.width * 0.18f;
		else
			b= true;
		
		
		if (Input.GetMouseButton (0) && b) {

			deg.x += Input.GetAxis ("Mouse X") * sensitivityX;
			if (Input.mousePosition.x < Screen.width * 0.85f && Input.mousePosition.y < Screen.height * 0.85f && Input.mousePosition.y > Screen.height * 0.15f) {	
				deg.y += -Input.GetAxis ("Mouse Y") * sensitivityY;
			}
			else{
				deg.y += Input.GetAxis ("Mouse Y") * sensitivityY;
			}

			
		} 
		else if (Input.GetMouseButton (2)) {
			trans.x += Input.GetAxis ("Mouse X") * sensitivityX;
			trans.y += Input.GetAxis ("Mouse Y") * sensitivityY;
			pos.x = trans.x;
			pos.y = trans.y;
		}
		
		else {
			
			deg.x=0;
			deg.y=0;
			trans.x=0;
			trans.y=0;
			
		}
		
		
		
		
		if (!Camera.main.orthographic){
			trans.z = Input.GetAxis ("Mouse ScrollWheel")*5;//
			
		}


		axes.transform.rotation *= Quaternion.AngleAxis(-deg.x,Camera.main.transform.up)*Quaternion.AngleAxis(-deg.y,Camera.main.transform.right);

		Camera.main.transform.RotateAround (new Vector3 (pos.x+center.x, pos.y+center.y,center.z), Camera.main.transform.up, deg.x);
		Camera.main.transform.RotateAround (new Vector3 (pos.x+center.x, pos.y+center.y,center.z), Camera.main.transform.right, deg.y);
		Camera.main.transform.Translate (new Vector3 (trans.x, trans.y, trans.z*10), Space.Self);
		
		
		
	}




}
