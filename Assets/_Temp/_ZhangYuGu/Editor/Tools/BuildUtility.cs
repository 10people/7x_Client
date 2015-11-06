using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class BuildUtility : MonoBehaviour {

	public static void DirectoryDelete( string p_full_path, bool p_recursive ){
		DirectoryInfo t_dir = new DirectoryInfo( p_full_path );

		if ( !t_dir.Exists ) {
//			Debug.LogError( "Delete Target not exist: " + p_full_path );

			return;
		}

		Directory.Delete ( p_full_path, p_recursive );
	}

	public static void DirectoryMove( string p_src, string p_dest ){
		Directory.Move( p_src, p_dest );
	}
	
	public static void DirectoryCopy( string p_src, string p_dest ){
		DirectoryInfo t_src_dir = new DirectoryInfo( p_src );
		
		DirectoryInfo[] t_src_dirs = t_src_dir.GetDirectories();
		
		if( !t_src_dir.Exists ){
			Debug.LogError( "Not Exist: " + p_src );
			
			return;
		}
		
		if( !Directory.Exists( p_dest ) ){
			Directory.CreateDirectory( p_dest );
		}
		
		FileInfo[] t_files = t_src_dir.GetFiles();
		
		foreach( FileInfo t_file in t_files ){
			string t_path = Path.Combine( p_dest, t_file.Name );
			
			t_file.CopyTo( t_path, true );
		}
		
		foreach( DirectoryInfo t_dir in t_src_dirs ){
			string t_path = Path.Combine( p_dest, t_dir.Name );
			
			DirectoryCopy( t_dir.FullName, t_path );
		}
	}

	public static void FileCopy( string p_src, string p_dest ){
		if ( File.Exists ( p_dest ) ) {
			File.Delete ( p_dest );
		}

		File.Copy( p_src, p_dest );
	}
}
