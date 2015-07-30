#if UNITY_STANDALONE
	#define IMPORT_GLENABLE
	#endif
	
using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class GLParam : MonoBehaviour 
{
	const UInt32 GL_VERTEX_PROGRAM_POINT_SIZE = 0x8642;
	const UInt32 GL_POINT_SMOOTH = 0x0B10;
	const UInt32 GL_LINE_SMOOTH = 0x0B20;
	const UInt32 GL_SMOOTH = 0x1D01;


	const string LibGLPath =
		#if UNITY_STANDALONE_WIN
		"opengl32.dll";
	#elif UNITY_STANDALONE_OSX
	"/System/Library/Frameworks/OpenGL.framework/OpenGL";
	#elif UNITY_STANDALONE_LINUX
	"/usr/lib/x86_64-linux-gnu/libGL.so";  
	#else
	null;   // OpenGL ES platforms don't require this feature
	#endif
	
	#if IMPORT_GLENABLE
	[DllImport(LibGLPath)]
	public static extern void glEnable(UInt32 cap);
	[DllImport(LibGLPath)]
	public static extern void glLineWidth(float f);
	[DllImport(LibGLPath)]
	public static extern void glPointSize(float f);

	private bool mIsOpenGL;

	void Start()
	{
		mIsOpenGL = SystemInfo.graphicsDeviceVersion.Contains("OpenGL");
	}
	
	void OnPreRender()
	{
		if (mIsOpenGL)
			glEnable(GL_VERTEX_PROGRAM_POINT_SIZE);
		glEnable(GL_POINT_SMOOTH);
		//obligatory to control the line width (no geometry shader)
		glLineWidth (Main.globalScale);
		glPointSize (Main.globalScale);
		glEnable(GL_LINE_SMOOTH);
		//GL.wireframe = true;
	}

	void OnPostRender() {
		GL.wireframe = false;
	}


	#endif
}
