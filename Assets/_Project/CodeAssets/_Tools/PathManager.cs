using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class PathManager
{
	public static string GetUrl(string strRelativePath)
	{
		return strRelativePath.Substring( 0, strRelativePath.IndexOf( "." ) );

//		if (ClientMain.M_BRESLOAD)
//		{
//			return strRelativePath.Substring(0, strRelativePath.IndexOf("."));
//		}
//		else
//		{
//			string strFileFullName = LOCAL_RES_PATH_URL + strRelativePath;
//			if (!System.IO.File.Exists (LOCAL_RES_PATH + strRelativePath)) 
//			{
//				strFileFullName = LOCAL_RES_URL + strRelativePath;
//			}
//			return strFileFullName;
//		}
	}

	#if  UNITY_EDITOR || UNITY_STANDALONE
	public static readonly string LOCAL_RES_URL = "file://" + Application.dataPath + "/StreamAssets" + "/Windows/";
	#elif UNITY_IPHONE
	public static readonly string LOCAL_RES_URL = "file://" + Application.dataPath + "/Raw"+ "/iPhone/";
	#elif UNITY_ANDROID
	public static readonly string LOCAL_RES_URL =  "jar:file://" + Application.dataPath + "!/assets" + "/Android/";
	#endif

	#if DEBUG_MODE
	public static readonly string LOCAL_RES_PATH = LOCAL_RES_DATAPATH;
	#elif  UNITY_EDITOR || UNITY_STANDALONE
	public static readonly string LOCAL_RES_PATH = Application.persistentDataPath + "/Windows/";
	#elif UNITY_IPHONE
	public static readonly string LOCAL_RES_PATH = Application.persistentDataPath + "/iPhone/";
	#elif UNITY_ANDROID
	public static readonly string LOCAL_RES_PATH = Application.persistentDataPath + "/Android/";
	#endif
	
	#if DEBUG_MODE
	public static readonly string LOCAL_RES_PATH_URL = "file:///" + LOCAL_RES_DATAPATH;
	#elif  UNITY_EDITOR || UNITY_STANDALONE
	public static readonly string LOCAL_RES_PATH_URL = "file:///" + Application.persistentDataPath + "/Windows/";
	#elif UNITY_IPHONE
	public static readonly string LOCAL_RES_PATH_URL = "file://" + Application.persistentDataPath + "/iPhone/";
	#elif UNITY_ANDROID
	public static readonly string LOCAL_RES_PATH_URL = "file://" + Application.persistentDataPath + "/Android/";
	#endif

}