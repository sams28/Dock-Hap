using UnityEngine;

using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using System.IO;




// Report data structure from UART
// VRPN Tracker Report Structure   
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

// Button report data structure from UART

#if UNITY_STANDALONE_WIN
[ StructLayout( LayoutKind.Sequential, Pack=0 )]
#else
[ StructLayout( LayoutKind.Sequential, Pack=1 )]
#endif
public struct VRPNButtonReport
{
	public TimeVal msg_time;
	public int button;
	public int state;
}


[ StructLayout( LayoutKind.Sequential )]
public struct TimeVal
{
	public UInt32 tv_sec;
	public UInt32 tv_usec;
}










public class Device{

	private Vector3 trackerPos = Vector3.zero;
	private Vector3 trackerInitPos = Vector3.zero;
	private Quaternion trackerQuat = Quaternion.identity;
	private Quaternion trackerInitQuat = Quaternion.identity;
	private Vector3 trackerVelocity = Vector3.zero;
	
	private bool button0 = false;
	private bool button1 = false;
	
	private bool trackerInitialized =false;

	private int maxNumberOfReports = 1000;
	private IntPtr[] reports;
	private IntPtr[] bReports;
	private IntPtr trackReport;
	private IntPtr velReport;
	private IntPtr buttonReport;
	private Vector3 initPos;
	private Quaternion initRot;
	public float baseScale = 60.0f;
	private Vector3 coef;
	private Vector3 min;
	private Vector3 max;
	public GameObject obj;
	private string name;
	private string server;
	
	public Color32 c;

	public Device(string n,string location){

		name = n;
		server = location;
		c = new Color32((byte)UnityEngine.Random.Range(10,255),(byte)UnityEngine.Random.Range(10,255),(byte)UnityEngine.Random.Range(10,255),255);
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

	public IntPtr ButtonReport{
		get{return buttonReport;}
		set{buttonReport = value;}
	}

	public string Name{
		get{return name;}
		set{name = value;}
	}

	public string Server{
		get{return server;}
		set{server = value;}
	} 


	protected void initializeReports ()
	{
		// Allocate and initialize memory for reports
		// The process involves Marshaling, in order to exchange data between Unity and the C++ VRPN wrapper
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
			reports[i] = Marshal.AllocHGlobal(Marshal.SizeOf (typeof(VRPNReport)));
			// Copy the report struct to unmanaged memory (reports array)
			Marshal.StructureToPtr(report, reports[i], true);
			
			bReports[i] = Marshal.AllocHGlobal(Marshal.SizeOf (typeof(VRPNButtonReport)));
			// Copy the report struct to unmanaged memory (reports array)
			Marshal.StructureToPtr(bReport, bReports[i], true);
			
			
			
		}
		
		trackReport = Marshal.AllocHGlobal(Marshal.SizeOf (typeof(VRPNReport)));
		Marshal.StructureToPtr (report, trackReport, true);
		
		
		velReport = Marshal.AllocHGlobal(Marshal.SizeOf (typeof(VRPNReport)));
		Marshal.StructureToPtr (report, velReport, true);
	}



	public void Init(){
		
		if ((name != null) && (server != null)) {


			coef = new Vector3 (baseScale, baseScale, baseScale);
			initPos = obj.transform.localPosition;
			initRot = obj.transform.localRotation;
			obj.SetActive(true);
			initializeReports ();

		}
		
	}




	public void ReportPos(VRPNReport rep){

		if(!trackerInitialized){
			trackerInitPos.x = (float)rep.pos [0] * coef.x;
			trackerInitPos.y = (float)rep.pos [1] * coef.y;
			trackerInitPos.z = -(float)rep.pos [2] * coef.z;
			trackerInitQuat.x = -(float)rep.quat [0];
			trackerInitQuat.y = -(float)rep.quat [1];
			trackerInitQuat.z = (float)rep.quat [2];
			trackerInitQuat.w = (float)rep.quat [3];
			trackerInitialized=true;
			
			
		}
		// Store position in a Vector3
		trackerPos.x = (float)rep.pos [0] * coef.x;
		trackerPos.y = (float)rep.pos [1] * coef.y;
		trackerPos.z = -(float)rep.pos [2] * coef.z;
		trackerQuat.x = -(float)rep.quat [0];
		trackerQuat.y = -(float)rep.quat [1];
		trackerQuat.z = (float)rep.quat [2];
		trackerQuat.w = (float)rep.quat [3];
		
		obj.transform.localRotation = initRot*Quaternion.Inverse(trackerInitQuat)*trackerQuat;
		//obj.transform.localRotation
		obj.transform.localPosition = (trackerPos-trackerInitPos)+initPos;



	}

	public void ReportVel(VRPNReport rep){

		trackerVelocity.x = (float)rep.pos[0];
		trackerVelocity.y = (float)rep.pos[1];
		trackerVelocity.z = -(float)rep.pos[2];

	}

	public void ReportButtons(VRPNButtonReport brep){

			
			
			
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



public class VRPN : MonoBehaviour {



	[DllImport ("UnityVRPN")]
	private static extern void VRPNServerStart(string file, string location);

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
	
	[DllImport ("UnityVRPN")]
	private static extern void VRPNTrackerVelReport(string name, [In,Out] IntPtr report, IntPtr ts, int sensor = 0);
	
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
	
	
	//public delegate void ButtonAction(VRPNButtonReport report);
	//public static event ButtonAction OnButton;









	private List<Device> devices;
	private bool serverStarted = false;



	public bool ServerStarted{
		get{return serverStarted;}
		set{serverStarted = value;}
	}
	public List<Device> Devices{
		get{return devices;}
		set{devices = value;}
	}

	public void setParams(string t){

		devices = new List<Device> ();
		string[] sl;
		sl = t.Split (new Char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

		for (int i=0; i<sl.Length; i++) {
			string[] sl2;
			sl2 = sl[i].Split (new Char[] {'@'}, StringSplitOptions.RemoveEmptyEntries);
			devices.Add (new Device(sl2[0],sl2[1]));
		}

	}





	public void Init(){

		for (int i=0; i<devices.Count; i++) {
			 
			devices[i].obj = (GameObject)Instantiate (Resources.Load("Prefabs/Picker") as GameObject, new Vector3(0,0,10), Quaternion.identity);
			devices[i].obj.transform.SetParent(Camera.main.transform,false);
			devices[i].obj.GetComponentInChildren<Renderer>().material.color = devices[i].c;
			devices[i].Init();

			VRPNTrackerStart (devices[i].Name, 1, devices[i].MaxNumberOfReports);
			VRPNButtonStart(devices[i].Name, devices[i].MaxNumberOfReports);
			VRPNForceFeedbackInit(devices[i].Name);
			serverStarted = true;

		}




		VRPNServerStart ("", devices[0].Server);
		for (int i=0; i<devices.Count; i++) {

			
			VRPNTrackerStart (devices[i].Name, 1, devices[i].MaxNumberOfReports);
			VRPNButtonStart(devices[i].Name, devices[i].MaxNumberOfReports);
			VRPNForceFeedbackInit(devices[i].Name);

			
		}
		serverStarted = true;
		
	}
	
	void Update () {
		
		
		if (serverStarted) {
			
			VRPNServerLoop ();



			for (int i=0; i<devices.Count; i++) {

				VRPNReport rep;
				if (VRPNTrackerNumPosReports (devices[i].Name) > 0) {
					
					
					//VRPNTrackerPosReports("Phantom",reports,ref num);
					VRPNTrackerPosReport (devices[i].Name, devices[i].TrackReport, IntPtr.Zero, 0);
					//rep = (VRPNReport)Marshal.PtrToStructure(reports[0], typeof(VRPNReport));
					rep = (VRPNReport)Marshal.PtrToStructure (devices[i].TrackReport, typeof(VRPNReport));
					devices[i].ReportPos(rep);
				}
				
				VRPNTrackerVelReport(devices[i].Name, devices[i].VelReport, IntPtr.Zero, 0);
				rep = (VRPNReport)Marshal.PtrToStructure(devices[i].VelReport, typeof(VRPNReport));
				devices[i].ReportVel(rep);
				
				
				
				
				VRPNButtonReport brep;

				if (VRPNButtonNumReports(devices[i].Name) > 0) {
					
					//VRPNButtonReports(devices[i].Name, devices[i].Reports, ref devices[i].MaxNumberOfReports, IntPtr.Zero, -1, true);
					int numrep =devices[i].MaxNumberOfReports;
					VRPNButtonReports(devices[i].Name, devices[i].Breports,ref numrep, IntPtr.Zero, -1, true);
					for (int j = 0; j < numrep; j++) {
						
						brep = (VRPNButtonReport)Marshal.PtrToStructure(devices[i].Breports[j], typeof(VRPNButtonReport));
						devices[i].ReportButtons(brep);

					}

				}
			}
		}

	}
	


	public void Stop()
	{
		if (serverStarted) {
			VRPNServerStop ();
			
			serverStarted = false;

			//obj.SetActive(false);
			for (int j=0; j<devices.Count; j++) {
				Destroy(devices[j].obj);
				Marshal.FreeHGlobal (devices[j].TrackReport);
				
				Marshal.FreeHGlobal (devices[j].VelReport);
				for (int i = 0; i < devices[j].MaxNumberOfReports; i++) {
					Marshal.FreeHGlobal (devices[j].Reports [i]);
				}
				
			}
		}
		
	}











	// Update is called once per frame

	// Send the desired feedback to server
	// z axis is inverted
	public void SetForce(Vector3 force,Device d)
	{
		VRPNForceFeedbackSetForce (d.Name, force.x, force.y, -force.z);

	}


	void OnDestroy () {

		if (serverStarted) {
			VRPNServerStop ();

			serverStarted = false;
			//obj.SetActive(false);
			for (int j=0; j<devices.Count; j++) {
				Marshal.FreeHGlobal (devices[j].TrackReport);

				Marshal.FreeHGlobal (devices[j].VelReport);
				for (int i = 0; i < devices[j].MaxNumberOfReports; i++) {
					Marshal.FreeHGlobal (devices[j].Reports [i]);
				}

			}
		}
	}


}
