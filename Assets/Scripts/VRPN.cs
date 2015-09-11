using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using MoleculeData;

namespace VRPNData{


	/// <summary>
	/// VRPN Tracker Report Structure   
	/// </summary>
	/// <description> Report data structure of VRPN</description>
	
	#if UNITY_STANDALONE_WIN
	[ StructLayout( LayoutKind.Sequential, Pack=0 )]
	#else
	[ StructLayout( LayoutKind.Sequential, Pack=1 )] //we add one byte
	#endif
	public struct VRPNReport
	{
		public TimeVal msg_time;
		public int sensor;
		[ MarshalAs( UnmanagedType.ByValArray, SizeConst=3 )]
		public double[] pos;
		[ MarshalAs( UnmanagedType.ByValArray, SizeConst=4 )]
		public double[] quat;

	}





	/// <summary>
	/// VRPN Button Report Structure   
	/// </summary>
	/// <description> Report data structure of VRPN</description>
	
	#if UNITY_STANDALONE_WIN
	[ StructLayout( LayoutKind.Sequential, Pack=0 )]
	public struct VRPNButtonReport
	{
		
		public IntPtr msg_time;
		public int button;
		public int state;
	}
	#else
	[ StructLayout( LayoutKind.Sequential, Pack=1 )]
	public struct VRPNButtonReport
	{
		
		public TimeVal msg_time;
		public int button;
		public int state;
	}
	#endif

	/// <summary>
	/// Time structure
	/// </summary>
	/// <description> Unknown use</description>
	[ StructLayout( LayoutKind.Sequential )]
	public struct TimeVal
	{
		public long tv_sec;
		public long tv_usec;
	}









	/// <summary>
	/// Device class
	/// </summary>
	/// <description> This class manages the different devices used by the application</description>
	public class Device{

		/// <summary>
		/// Raw position of a device
		/// </summary>
		private double[] temp_pos = new double[3];
		/// <summary>
		/// Raw rotation of a device
		/// </summary>
		private double[] temp_quat = new double[4];
		/// <summary>
		/// Device position
		/// </summary>
		private Vector3 trackerPos = Vector3.zero;
		/// <summary>
		///Device initial position
		/// </summary>
		/// <description>Device initial position for initial calibration</description>
		private Vector3 trackerInitPos = Vector3.zero;
		/// <summary>
		/// Device rotation
		/// </summary>
		private Quaternion trackerQuat = Quaternion.identity;
		/// <summary>
		/// Device initial rotation
		/// </summary>
		/// <description>Device initial rotation for initial calibration</description>
		private Quaternion trackerInitQuat = Quaternion.identity;
		/// <summary>
		/// Device velocity
		/// </summary>
		/// <remarks>It is not used</remarks>
		private Vector3 trackerVelocity = Vector3.zero;
		/// <summary>
		/// Device top button
		/// </summary>
		private bool button0 = false;
		/// <summary>
		/// Device bottom button
		/// </summary>

		private bool button1 = false;
		/// <summary>
		/// Is the device Initialized ?
		/// </summary>
		/// <description>Boolean to check if the device is initialized</description>
		private bool trackerInitialized =false;

		/// <summary>
		/// Maximum number of reports
		/// </summary>
		/// <description>Maximum number of report that can be recieved</description>
		private int maxNumberOfReports = 1000;
		/// <summary>
		/// Tracker reports
		/// </summary>
		/// <description>Data sent by the tracker component of the device. To be used with VRPNTrackerPosReports</description>
		private IntPtr[] reports;
		/// <summary>
		/// Button reports
		/// </summary>
		/// <description>Data sent by the button component of the device. To be used with VRPNButtonReports</description>
		private IntPtr[] bReports;
		/// <summary>
		/// Tracker report
		/// </summary>
		/// <description>Data sent by the tracker component of the device. To be used with VRPNTrackerPosReport or VRPNTrackerPosReportAlt</description>
		private IntPtr trackReport;
		/// <summary>
		/// Tracker report
		/// </summary>
		/// <description>Data sent by the tracker component of the device. To be used with VRPNTrackerVelReport or VRPNTrackerVelReportAlt</description>
		private IntPtr velReport;
		/// <summary>
		/// Initial position of the Game object associated
		/// </summary>
		private Vector3 initPos;
		/// <summary>
		/// Initial rotation of the Game object associated
		/// </summary>
		private Quaternion initRot;

		/// <summary>
		/// Moving scale
		/// </summary>
		public float baseScale = 60.0f;
		/// <summary>
		/// Game object associated
		/// </summary>
		public GameObject obj;
		/// <summary>
		/// Device name
		/// </summary>
		private string name;
		/// <summary>
		/// Server IP address
		/// </summary>
		private string server;

		/// <summary>
		/// color of the device
		/// </summary>
		public Color32 c;


		/// <summary>
		/// Device prototype
		/// </summary>
		/// <param name="name">Device Name: The device name. Must map the name set when we launch the vrpn server. example: Omni</param>
		/// <param name="location">Server address : Address of the vrpn server </param>
		/// <description>Create a new device instance with a random color</description>
		public Device(string name,string location){

			this.name = name;
			this.server = location;
			this.c = new Color32((byte)UnityEngine.Random.Range(10,255),(byte)UnityEngine.Random.Range(10,255),(byte)UnityEngine.Random.Range(10,255),255);
		}



		public double[] Temp_pos{
			get{return temp_pos;}
			set{temp_pos =value;}
		}
		public double[] Temp_quat{
			get{return temp_quat;}
			set{temp_quat =value;}
		}


		public Vector3 TrackerPos{
			get{return trackerPos;}
			
		}
		
		public Quaternion TrackerQuat{
			get{return trackerQuat;}
			
		}
		
		public Vector3 TrackerVelocity{
			get{return trackerVelocity;}
			
		}
		public bool Button0{
			get{return button0;}
			
		}
		
		public bool Button1{
			get{return button1;}
			
		}
		
		public int MaxNumberOfReports{
			get{return maxNumberOfReports;}
			set{maxNumberOfReports = value;}
		}

		public IntPtr[] Reports{
			get{return reports;}
			set{reports = value;}
		}
		public IntPtr[] Breports{
			get{return bReports;}
			set{bReports = value;}
		}

		public IntPtr TrackReport{
			get{return trackReport;}
			set{trackReport = value;}
		}

		public IntPtr VelReport{
			get{return velReport;}
			set{velReport = value;}
		}


		public string Name{
			get{return name;}
			set{name = value;}
		}

		public string Server{
			get{return server;}
			set{server = value;}
		}


		/// <summary>
		/// Reports initialisation
		/// </summary>
		/// <description>Initialiation and allocation of the memory for reports. The process involves Marshaling, in order to exchange data between Unity and the C++ VRPN wrapper</description>
		protected void initializeReports ()
		{

			reports = new IntPtr[maxNumberOfReports];
			bReports = new IntPtr[maxNumberOfReports];
			VRPNReport report = new VRPNReport();
			VRPNButtonReport bReport = new VRPNButtonReport();
			
			report.sensor = 0;
			report.pos = new double[3];
			report.quat = new double[4];
			report.quat[3] = 1.0f;
			bReport.button = 0;
			bReport.state = 0;
			
			
			for(int i = 0; i < maxNumberOfReports; i++)
			{
				//Allocation of the memory
				reports[i] = Marshal.AllocHGlobal(Marshal.SizeOf (typeof(VRPNReport)));
				// Copy the report struct to unmanaged memory (reports array)
				Marshal.StructureToPtr(report, reports[i], true);

				//Allocation of the memory
				bReports[i] = Marshal.AllocHGlobal(Marshal.SizeOf (typeof(VRPNButtonReport)));
				// Copy the report struct to unmanaged memory (reports array)
				Marshal.StructureToPtr(bReport, bReports[i], true);
				
				
				
			}
			
			trackReport = Marshal.AllocHGlobal(Marshal.SizeOf (typeof(VRPNReport)));
			Marshal.StructureToPtr (report, trackReport, true);
			
			
			velReport = Marshal.AllocHGlobal(Marshal.SizeOf (typeof(VRPNReport)));
			Marshal.StructureToPtr (report, velReport, true);
		}


		/// <summary>
		/// Device initialisation
		/// </summary>
		/// <description>Initialiation of the device class</description>
		public void Init(){
			
			if ((name != null) && (server != null)) {

				initPos = obj.transform.localPosition;
				initRot = obj.transform.localRotation;
				obj.SetActive(true);
				initializeReports ();

			}
			
		}

		/// <summary>
		/// Position and rotation report
		/// </summary>
		/// <param name="pos">The new postition of the device</param>
		/// <param name="quat">The new rotation of the device</param>
		/// <description>Applies the new postition and rotation to the GameObject</description>
		public void ReportPos(double[] pos,double[] quat){
			
			

			//Debug.Log ((float)pos[0] + "  " + (float)pos[1] + "  "+ (float)pos[2] +"\n" + (float)quat[0] + "  " + (float)quat[1] + "  "+ (float)quat[2] + " "+ (float)quat[3]);
			
			
			if(!trackerInitialized){
				trackerInitPos.x = (float)pos [0] * baseScale;
				trackerInitPos.y = (float)pos [1] * baseScale;
				trackerInitPos.z = -(float)pos [2] * baseScale;
				trackerInitQuat.x = -(float)quat [0];
				trackerInitQuat.y = -(float)quat [1];
				trackerInitQuat.z = (float)quat [2];
				trackerInitQuat.w = (float)quat [3];
				trackerInitialized=true;
				
				
			}
			trackerPos.x = (float)pos [0] * baseScale;
			trackerPos.y = (float)pos [1] * baseScale;
			trackerPos.z = -(float)pos [2] * baseScale;
			trackerQuat.x = -(float)quat [0];
			trackerQuat.y = -(float)quat [1];
			trackerQuat.z = (float)quat [2];
			trackerQuat.w = (float)quat [3];
			
			obj.transform.localRotation = initRot*Quaternion.Inverse(trackerInitQuat)*trackerQuat;
			obj.transform.localPosition = (trackerPos-trackerInitPos)+initPos;
			
			
			
		}





		/// <summary>
		/// Position and rotation report
		/// </summary>
		/// <param name="rep">The report of the device</param>
		/// <description>Applies the new postition and rotation to the GameObject</description>
		public void ReportVel(double[] vel){

			trackerVelocity.x = (float)vel[0];
			trackerVelocity.y = (float)vel[1];
			trackerVelocity.z = -(float)vel[2];

		}
	

		/// <summary>
		/// Position and rotation report
		/// </summary>
		/// <param name="rep">The report of the device</param>
		/// <description>Applies the new postition and rotation to the GameObject</description>
		public void ReportButtons(VRPNButtonReport brep){

				
			//Debug.Log (brep.button + " " + brep.state);
			          
				if(brep.button == 0 && brep.state == 1)
				{
					
					button0 = true;
				}
				
				if(brep.button == 0 && brep.state == 0)
				{
					
					button0 = false;
				}
				
				if(brep.button == 1 && brep.state == 1)
				{
					
					button1 = true;
				}
				
				if(brep.button == 1 && brep.state == 0)
				{
					
					button1 = false;
				}
				
				
				
				
			
		}




	}


	/// <summary>
	/// VRPN class
	/// </summary>
	/// <description>Manages the connection between VRPN server and the Unity main application</description>
	public class VRPN : MonoBehaviour {

		/// <summary>
		/// VRPNServerStart
		/// </summary>
		/// <description>Connect to the server at the given location</description>
		[DllImport ("UnityVRPN")]
		private static extern void VRPNServerStart(string cfg,string location);

		/// <summary>
		/// VRPNServerStop
		/// </summary>
		/// <description>Stop the server or close the connection with it.</description>
		[DllImport ("UnityVRPN")]
		private static extern void VRPNServerStop();
		
		/// <summary>
		/// VRPNServerLoop
		/// </summary>
		/// <description>As its name does not indicate, it handles any incoming message.</description>
		[DllImport ("UnityVRPN")]
		private static extern void VRPNServerLoop();

		/// <summary>
		/// VRPNTrackerStart wrapper API function
		/// </summary>
		/// <param name="name">Device Name: The device name. Must map the vrpn server configuration file. example: Omni</param>
		/// <param name="deriv">Derivation: Not used in the wrapper test scene, or not investigated enough. Must be 1.</param>
		/// <description>Initialize a "tracker", a kind of client/listener, in the wrapper library (client) linked the vrpn server, for a provided <paramref name="name">device</paramref></description>
		[DllImport ("UnityVRPN")]
		private static extern void VRPNTrackerStart(string name, int deriv, int max = 1000);
		
		
		/// <summary>
		/// VRPNTrackerPosReport wrapper API function
		/// </summary>
		/// <param name="name">Device name</param>
		/// <param name="rep">Rep: report struct pointer</param>
		/// <param name="ts">Ts: </param>
		/// <param name="sensor">Sensor: </param>
		/// <description>Fill the <paramref name="rep">rep structure</paramref> with data for the given <paramref name="name">device</paramref></description>
		[DllImport ("UnityVRPN")]
		private static extern void VRPNTrackerPosReport(string name, [In,Out] IntPtr rep, [Out] IntPtr ts, int sensor = 0);


		/// <summary>
		/// VRPNTrackerPosReportAlt wrapper API function
		/// </summary>
		/// <param name="name">Device name</param>
		/// <param name="pos">Pos: Device position</param>
		/// <param name="quat">Quat: Device rotation</param>
		/// <param name="ts">Ts: </param>
		/// <param name="sensor">Sensor: </param>
		/// <description>Get the position and the rotation for the given <paramref name="name">device</paramref></description>
		[DllImport ("UnityVRPN")]
		private static extern void VRPNTrackerPosReportAlt(string name, [In,Out] double[] pos, [In,Out] double[] quat, [Out] IntPtr ts, int sensor = 0);
		
		/// <summary>
		/// VRPNTrackerNumPosReports wrapper API function
		/// </summary>
		/// <returns>The number of reports sent by the server</returns>
		/// <param name="name">Name: Device name</param>
		/// <description>Provides the number of reports the server sent for a given <paramref name="name">device</paramref>.</description>
		[DllImport ("UnityVRPN")]
		private static extern int VRPNTrackerNumPosReports(string name);
		
		/// <summary>
		/// VRPNTrackerPosReports wrapper API function
		/// </summary>
		/// <param name="name">Name: Device name</param>
		/// <param name="repsPtr">Reps ptr: Array of pointers to reports</param>
		/// <param name="nmbr">Nmbr: Int pointer to the total number of reports</param>
		/// <description>Fill the array pointed by <paramref name="repsPtr">rep</paramref> with server reports for the given <paramref name="name">device</paramref>. Set <paramref name="nmbr">nmbr</paramref> to the number of available reports.</description>
		[DllImport ("UnityVRPN")]
		private static extern void VRPNTrackerPosReports(string name, [In,Out] IntPtr[] repsPtr, [In,Out] ref int nmbr);
		/// <summary>
		/// VRPNTrackerPosReport wrapper API function
		/// </summary>
		/// <param name="name">Device name</param>
		/// <param name="report">report struct pointer</param>
		/// <param name="ts">Ts: </param>
		/// <param name="sensor">Sensor: </param>
		/// <description>Fill the <paramref name="rep">rep structure</paramref> with data for the given <paramref name="name">device</paramref></description>
		[DllImport ("UnityVRPN")]
		private static extern void VRPNTrackerVelReport(string name,  [In,Out] IntPtr report, IntPtr ts, int sensor = 0);
		/// <summary>
		/// VRPNTrackerPosReport wrapper API function
		/// </summary>
		/// <param name="name">Device name</param>
		/// <param name="vel">Device velocity</param>
		/// <param name="ts">Ts: </param>
		/// <param name="sensor">Sensor: </param>
		/// <description>Get the velocity for the given <paramref name="name">device</paramref></description>
		[DllImport ("UnityVRPN")]
		private static extern void VRPNTrackerVelReportAlt(string name,  [In,Out] double[] vel, IntPtr ts, int sensor = 0);



		/// <summary>
		/// VRPNButtonStart wrapper API function
		/// </summary>
		/// <param name="name">Name: Device name</param>
		/// <param name="max">Max: The max number of retrieved reports</param>
		/// <description>Initialize a button "tracker" in the wrapper library for a provided <paramref name="name">device</paramref></description>
		[DllImport ("UnityVRPN")]
		private static extern void VRPNForceFeedbackInit(string name);

		[DllImport ("UnityVRPN")]
		private static extern void VRPNForceFeedbackSetForce(string name,float force_x, float force_y, float force_z);
		
		/// <summary>
		/// VRPNButtonStart wrapper API function
		/// </summary>
		/// <param name="name">Name: Device name</param>
		/// <param name="max">Max: The max number of retrieved reports</param>
		/// <description>Initialize a button "tracker" in the wrapper library for a provided <paramref name="name">device</paramref></description>
		[DllImport ("UnityVRPN")]
		private static extern void VRPNButtonStart(string name, int max);
		
		/// <summary>
		/// VRPNButtonReports wrapper API function
		/// </summary>
		/// <param name="name">Name: Device name</param>
		/// <param name="reportsPtr">Reports ptr: Array of pointers to reports</param>
		/// <param name="cnt">Count: Number of available reports</param>
		/// <param name="ts">Ts: TimeVal</param>
		/// <param name="btn_num">Btn num: the number of the button we want to track</param>
		/// <param name="clearReport">If set to <c>true</c> clear useless reports.</param>
		/// <description>Retrieve reports by filling the <paramref name="reportsPtr">array</paramref>. The trick with this wrapper function is not to use the ts parameter by setting it to a null pointer.</description>
		[DllImport ("UnityVRPN")]
		private static extern void VRPNButtonReports(string name, [In,Out] IntPtr[] reportsPtr, [In,Out] ref int cnt, IntPtr ts, int btn_num, bool clearReport);
		
		/// <summary>
		/// VRPNButtonNumReports
		/// </summary>
		/// <returns>The number of available reports.</returns>
		/// <param name="name">Name: Device name</param>
		[DllImport ("UnityVRPN")]
		private static extern int VRPNButtonNumReports(string name);


		/// <summary>
		/// List of all Devices
		/// </summary>
		private List<Device> devices;
		/// <summary>
		/// Are we connected to a server ?
		/// </summary>
		private bool serverStarted = false;
		public float magnetFeedbackScale = 1f;
		// Stiffness, i.e. k value, of the sphere.  Higher stiffness results
		// in a harder surface.
		public float sphereStiffness = 0.5f;
		public float linearForceFeedbackFactor;
		
		private Vector3[] lastPosition;
		private Vector3[] lastBarycentre;
		private List<GameObject> arrow;
		private float[] forces;
		private int[] atom_id;

		private IMD imd;
		private SelectAtoms s;
		private int frame;

		public bool ServerStarted{
			get{return serverStarted;}
			set{serverStarted = value;}
		}

		public List<Device> Devices{
			get{return devices;}
			set{devices = value;}
		}

		/// <summary>
		/// Set parameters for a new device
		/// </summary>
		/// <param name="t">Name: Device name</param>
		/// <description> Parse data from the application to add a new device</description>
		public void setParams(string t){

			devices = new List<Device> ();
			string[] sl;
			sl = t.Split (new Char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

			for (int i=0; i<sl.Length; i++) {
				string[] sl2;
				sl2 = sl[i].Split (";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				devices.Add (new Device(sl2[0],sl2[1]));
			}

		}


		void Start(){
			
			lastPosition = new Vector3[Main.MAX_NUM_DEVICES];
			lastBarycentre = new Vector3[Main.MAX_NUM_DEVICES];
			imd = GetComponent<IMD> ();
			s = GetComponent<SelectAtoms> ();

		}

		/// <summary>
		/// Initialisation of VRPN
		/// </summary>
		/// <description>Initialize the devices and connect to the server</description>
		public void Init(){



			//List<string> s_temp =new List<string>();
			VRPNServerStart ("",devices[0].Server);
			for (int i=0; i<devices.Count; i++) {
				/*String s = s_temp.Find(x => x == devices[i].Server);
				if(s == null){
					

					s_temp.Add(devices[i].Server);
					
				}
				*/


				

				devices[i].obj = (GameObject)Instantiate (Resources.Load("Prefabs/Picker") as GameObject, new Vector3(0,0,10), Quaternion.identity);
				devices[i].obj.transform.SetParent(Camera.main.transform,false);
				devices[i].obj.GetComponentInChildren<Renderer>().material.color = devices[i].c;

				devices[i].Init();

				VRPNTrackerStart (devices[i].Name, 1, devices[i].MaxNumberOfReports);
				VRPNButtonStart(devices[i].Name, devices[i].MaxNumberOfReports);
				VRPNForceFeedbackInit(devices[i].Name);

			}
			serverStarted = true;

		}


		/// <summary>
		/// VRPN Loop
		/// </summary>
		/// <description>Main VRPN Loop</description>
		void Update () {
			
			
			if (serverStarted) {
				
				VRPNServerLoop ();



				for (int i=0; i<devices.Count; i++) {

					if (VRPNTrackerNumPosReports (devices[i].Name) > 0) {


						//Note : For a unknown reason, we can't Marshall the data back from the unmanged memory when a array is inside a struct
						// This is why we don't use the VRPNReport struct but directly an array
						//VRPNReport rep;
						//VRPNTrackerPosReport (devices[i].Name, devices[i].TrackReport, IntPtr.Zero, 0);
						//rep = (VRPNReport)Marshal.PtrToStructure (devices[i].TrackReport, typeof(VRPNReport));

						VRPNTrackerPosReportAlt (devices[i].Name,devices[i].Temp_pos,devices[i].Temp_quat, IntPtr.Zero, 0);
						devices[i].ReportPos(devices[i].Temp_pos,devices[i].Temp_quat);
					}
					//VRPNReport rep2;
					//VRPNTrackerVelReport(devices[i].Name, devices[i].VelReport, IntPtr.Zero, 0);
					//rep2 = (VRPNReport)Marshal.PtrToStructure(devices[i].VelReport, typeof(VRPNReport));
					VRPNTrackerVelReportAlt(devices[i].Name,devices[i].Temp_pos, IntPtr.Zero, 0);
					devices[i].ReportVel(devices[i].Temp_pos);



					if (VRPNButtonNumReports(devices[i].Name) > 0) {
						VRPNButtonReport brep;
						int numrep =devices[i].MaxNumberOfReports;
						VRPNButtonReports(devices[i].Name, devices[i].Breports,ref numrep, IntPtr.Zero, -1, true);
						for (int j = 0; j < numrep; j++) {
							
							brep = (VRPNButtonReport)Marshal.PtrToStructure(devices[i].Breports[j], typeof(VRPNButtonReport));
							devices[i].ReportButtons(brep);

						}

					}
				}

				s.ClosestAtom(Devices);
				applyForcesVector();
					
					



			}

		}











		
		



		/// <summary>
		/// Manages all the forces to send 
		/// </summary>
		public void applyForcesVector(){
			
			
			
			for (int z=0; z<Devices.Count; z++) {
				

				if (ServerStarted && !imd.IsIMDRunning ()) {
					setForceForAtomPosition(z);
				}

				else if (ServerStarted && imd.IsIMDRunning ()) {

					List<Atom> l = new List<Atom> ();
					
					switch (GetComponent<Main> ().molecules [MainUI.current_mol].select) {
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


					if (Devices[z].Button0 && !s.Selects[z].Listen2) {
						
						s.Selects[z].Listen2 = true;
						s.Selects[z].LastPressedTime2 = Time.realtimeSinceStartup;
						lastPosition[z] = Devices[z].obj.transform.position;
						lastBarycentre[z] = Vector3.zero;
						for(int i = 0; i < l.Count;i++)
							lastBarycentre[z]+=l[i].Location[Main.current_frame];
						
						lastBarycentre[z] /= l.Count;
						
					}
					
					if (s.Selects[z].Listen2) {
						
						float timeDiff = Time.realtimeSinceStartup - s.Selects[z].LastPressedTime;
						
						if (timeDiff > 1.0f) {
							
							
							if (l.Count > 0) {

								setLinearForceForVector(l,z);

							}
							
						}
						
					}
					else {
						setForceForAtomPosition(z);
					}

					
					
					if (!Devices[z].Button0 && s.Selects[z].Listen2) {
						
						if (l.Count > 0) {
							
							resetForce(l,z);
							
						}
						
						s.Selects[z].Listen2 = false;
						
					}
					
					
				}
				
			}
		}


		/// <summary>
		/// Set force magnetic forces for the device
		/// </summary>
		public void setForceForAtomPosition(int device_id)
		{
			if (!ServerStarted) return;
			
			
			
			
			
			Vector3 minAtm = s.Selects[device_id].MinDistAtom.Location[Main.current_frame];
			float radius =  s.Selects[device_id].MinDistAtom.AtomRadius;
			Vector3 pos = Devices[device_id].obj.transform.position;
			
			//Attraction
			
			// Compute the distance between the atom and the picker for each axis
			Vector3 forceFactor = (minAtm - pos);
			// Compute the absolute distance between the atom and the picker
			float dist = Vector3.Distance (minAtm, pos);
			// Compute a gaussian factor
			float gaussian = Mathf.Exp(-((Mathf.Pow ((dist - 1.8f) / 0.8f, 2))) / 2);
			forceFactor /= dist;
			forceFactor *= gaussian * magnetFeedbackScale*radius;
			
			Vector3 feedbackForce = Camera.main.transform.worldToLocalMatrix * forceFactor;
			
			feedbackForce.x -= Devices[device_id].TrackerVelocity.x;
			feedbackForce.y -= Devices[device_id].TrackerVelocity.y;
			feedbackForce.z -= Devices[device_id].TrackerVelocity.z;
			
			
			
			//Touch
			
			/*

		float dist = Vector3.Magnitude(pos - minAtm);
		Vector3 feedbackForce = Vector3.zero;



		// If the user is within the sphere -- i.e. if the distance from the user to 
		// the center of the sphere is less than the sphere radius -- then the user 
		// is penetrating the sphere and a force should be commanded to repel him 
		// towards the surface.
		if(dist <radius)
		{

			float penetrationDistance = radius-dist;

			Vector3 forceDirection = (pos-minAtm)/dist;
			// Use F=kx to create a force vector that is away from the center of 
			// the sphere and proportional to the penetration distance, and scsaled 
			// by the object stiffness.  
			// Hooke's law explicitly:

			feedbackForce = sphereStiffness*penetrationDistance*forceDirection;

		}
*/
			//Debug.Log (feedbackForce);
			VRPNForceFeedbackSetForce (Devices[device_id].Name, feedbackForce.x, feedbackForce.y, -feedbackForce.z);
			
			
		}









		/// <summary>
		/// Set the forces forces to send to the atoms and the device 
		/// </summary>
		public void setLinearForceForVector(List<Atom> l,int device_id) {



				Vector3 barycentre = Vector3.zero;
				for(int i = 0; i < l.Count;i++)
					barycentre+=l[i].Location[Main.current_frame];
				
				barycentre /= l.Count;
				
				
			//Vector3 force_atoms = barycentre-lastBarycentre[device_id];
				
			float xbase = 0.5f;
			Vector3 force_util = Devices[device_id].obj.transform.position - lastPosition[device_id];
			float dist = Vector3.Distance(Devices[device_id].obj.transform.position,lastPosition[device_id]);
				
			if (force_util.magnitude < xbase)
				force_util = new Vector3(0,0,0);
			else
			{
				force_util -= force_util/force_util.magnitude*xbase;
			}



				forces = new float[l.Count * 3];
				atom_id = new int[l.Count];
				
				for(int i = 0; i < l.Count;i++){
					
					forces[i*3] = -force_util.x;
					forces[i*3+1] = force_util.y;
					forces[i*3+2] = force_util.z;
					atom_id[i] = l[i].Number;
					
				}
				
				
				
				
				


			//float gaussian = Mathf.Exp((Mathf.Pow (distance, 2)) / -2);
			// Compute a gaussian factor
			float gaussian = Mathf.Exp(-((Mathf.Pow (Vector3.Distance(Devices[device_id].obj.transform.position,lastPosition[device_id]), 2))) / 100) -1;
	
			force_util *= gaussian;


			force_util *= linearForceFeedbackFactor*Mathf.Pow(Vector3.Distance(barycentre,lastBarycentre[device_id]),2);
			Debug.Log (force_util);
			
			Vector3 nv = Camera.main.transform.worldToLocalMatrix * force_util;

		
		//nv.Normalize();

			VRPNForceFeedbackSetForce (Devices[device_id].Name, nv.x, nv.y, -nv.z);
			imd.setForces(atom_id,forces);


			//Application to the virtual arrows 
			switch (GetComponent<Main> ().molecules [MainUI.current_mol].select) {
			case SelectDisplay.Atom :
				
				for(int i = 0; i <s.Selects[device_id].selectedAtoms.Count;i++){
					s.Selects[device_id].selectedAtoms[i].ForceGameobject[device_id].transform.localPosition = s.Selects[device_id].selectedAtoms[i].Location[Main.current_frame];
					s.Selects[device_id].selectedAtoms[i].ForceGameobject[device_id].transform.up = -force_util;
					s.Selects[device_id].selectedAtoms[i].ForceGameobject[device_id].transform.localScale = new Vector3(dist/50.0f,dist/8.0f,dist/50.0f);
					
					
				}
				
				break;
			case SelectDisplay.Residue :
				for(int i=0;i<s.Selects[device_id].selectedResidues.Count;i++){
					s.Selects[device_id].selectedResidues[i].ForceGameobject[device_id].transform.localPosition = s.Selects[device_id].selectedResidues[i].Location[Main.current_frame];
					s.Selects[device_id].selectedResidues[i].ForceGameobject[device_id].transform.up = -force_util;
					s.Selects[device_id].selectedResidues[i].ForceGameobject[device_id].transform.localScale = new Vector3(dist/50.0f,dist/8.0f,dist/50.0f);
					
					
					
				}
				
				break;
			case SelectDisplay.Chain :
				for(int i=0;i<s.Selects[device_id].selectedChains.Count;i++){
					s.Selects[device_id].selectedChains[i].ForceGameobject[device_id].transform.localPosition = s.Selects[device_id].selectedChains[i].Location[Main.current_frame];
					s.Selects[device_id].selectedChains[i].ForceGameobject[device_id].transform.up = -force_util;
					s.Selects[device_id].selectedChains[i].ForceGameobject[device_id].transform.localScale = new Vector3(dist/50.0f,dist/8.0f,dist/50.0f);
					
				}
				
				break;
			default:break;
				
			}


		}



		/// <summary>
		/// Set all the forces to zero
		/// </summary>
		public void resetForce(List<Atom> l,int device_id)
		{



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
			
			switch (GetComponent<Main> ().molecules [MainUI.current_mol].select) {
			case SelectDisplay.Atom :
				
				for(int i = 0; i <s.Selects[device_id].selectedAtoms.Count;i++){
					s.Selects[device_id].selectedAtoms[i].ForceGameobject[device_id].transform.localScale =  Vector3.zero;
					
					
				}
				
				break;
			case SelectDisplay.Residue :
				for(int i=0;i<s.Selects[device_id].selectedResidues.Count;i++){
					s.Selects[device_id].selectedResidues[i].ForceGameobject[device_id].transform.localScale =  Vector3.zero;
					
					
					
				}
				
				break;
			case SelectDisplay.Chain :
				for(int i=0;i<s.Selects[device_id].selectedChains.Count;i++){
					s.Selects[device_id].selectedChains[i].ForceGameobject[device_id].transform.localScale =  Vector3.zero;
					
				}
				
				break;
			default:break;
				
			}
			
			
			VRPNForceFeedbackSetForce (Devices[device_id].Name, 0, 0, 0);
			imd.setForces(atom_id,forces);
			imd.resetForces();



		}


		/// <summary>
		/// Stop the devices (public)
		/// </summary>
		public void Stop () {

			OnDestroy ();
		}






		/// <summary>
		/// Stop the devices
		/// </summary>
		/// <description>Close the connections and free the memory</description>
		void OnDestroy () {

			if (serverStarted) {
				VRPNServerStop ();
				serverStarted = false;
				//obj.SetActive(false);
				for (int j=0; j<devices.Count; j++) {

					Destroy (devices[j].obj);

					Marshal.FreeHGlobal (devices[j].TrackReport);

					Marshal.FreeHGlobal (devices[j].VelReport);
					for (int i = 0; i < devices[j].MaxNumberOfReports; i++) {
						Marshal.FreeHGlobal (devices[j].Reports [i]);
					}

				}
			}
		}


	}
}
