using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class XmlLoadManager{
	public const string m_LoadPath = "_Data/Design/";


	#region Read 

	public static bool ReadNextBool( XmlReader p_reader ){
		p_reader.MoveToNextAttribute();
		
		return bool.Parse( p_reader.Value );
	}

	public static int ReadNextInt( XmlReader p_reader ){
		p_reader.MoveToNextAttribute();

		return int.Parse( p_reader.Value );
	}

	public static float ReadNextFloat( XmlReader p_reader ){
		p_reader.MoveToNextAttribute();
		
		return float.Parse( p_reader.Value );
	}

	public static string ReadNextString( XmlReader p_reader ){
		p_reader.MoveToNextAttribute();

		return p_reader.Value;
	}

	#endregion



	#region Write

	public static string GetFileHead(){
		return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<dataset>\n";
	}

	public static string GetFileEnd(){
		return "</dataset>";
	}

	#endregion



	#region Log


	#endregion
}