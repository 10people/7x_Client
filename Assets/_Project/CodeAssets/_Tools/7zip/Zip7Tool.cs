

using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

public class Zip7Tool : MonoBehaviour {


}

namespace SevenZip
{
	using CommandLineParser;
	
	public class BundleEncoder
	{
		
		#region 7-Zip
		
		/** Params:
		 * 
		 * 1.p_path_name:	"Assets/StreamingAssets/UIResources/MemoryTrace/MemoryTrace_-adf113"
		 */
		public static void EncodeBundleFile(string p_path_name)
		{
			string t_file_path = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
			
			string t_in_file_path = t_file_path + p_path_name;
			
			string t_temp_path = t_in_file_path + "_7z";
			
			string t_out_file_path = t_in_file_path;
			
			{
				FileInfo t_input = new FileInfo(t_in_file_path);
				
				t_input.MoveTo(t_temp_path);
				
				{
					Encode7Zip(t_temp_path, t_out_file_path);
				}
			}
			
			{
				FileInfo t_out = new FileInfo(t_out_file_path);
				
				// TODO, Local Bundle Delete
				//				t_out.Delete();
			}
			
			//			Debug.Log( "EncodeBundleFile.done: " + p_path_name );
		}
		
		public static void Decode7Zip(string p_in_file_path, string p_out_file_path)
		{
			//			Debug.Log( "Decode7Zip.p_in_file_path: " + p_in_file_path );
			//
			//			Debug.Log( "Decode7Zip.p_out_file_path: " + p_out_file_path );
			
			SevenZip.Compression.LZMA.Decoder t_coder = new SevenZip.Compression.LZMA.Decoder();
			
			// prepare dir
			{
				string t_folder = p_out_file_path.Substring(0, p_out_file_path.LastIndexOf("/"));
				
				DirectoryInfo t_out_dir = new DirectoryInfo(t_folder);
				
				if (!t_out_dir.Exists)
				{
					t_out_dir.Create();
				}
			}
			
			FileStream t_input = new FileStream(p_in_file_path, FileMode.Open);
			
			FileStream t_output = new FileStream(p_out_file_path, FileMode.Create);
			
			// Read the decoder properties
			byte[] t_properties = new byte[5];
			
			t_input.Read(t_properties, 0, 5);
			
			// Read in the decompress file size.
			byte[] fileLengthBytes = new byte[8];
			
			t_input.Read(fileLengthBytes, 0, 8);
			
			long fileLength = System.BitConverter.ToInt64(fileLengthBytes, 0);
			
			t_coder.SetDecoderProperties(t_properties);
			
			t_coder.Code(t_input, t_output, t_input.Length, fileLength, null);
			
			t_output.Flush();
			
			t_output.Close();
			
			//			Debug.Log( "EncodeBundleFile.done: " + p_out_file_path );
		}
		
		public static void Encode7Zip(string p_in_file_path, string p_out_file_path)
		{
			SevenZip.Compression.LZMA.Encoder t_coder = new SevenZip.Compression.LZMA.Encoder();
			
			FileStream t_input = new FileStream(p_in_file_path, FileMode.Open);
			
			FileStream t_output = new FileStream(p_out_file_path, FileMode.Create);
			
			// properties
			{
				CoderPropID[] t_propIDs = 
				{
					CoderPropID.DictionarySize,
					CoderPropID.PosStateBits,
					CoderPropID.LitContextBits,
					CoderPropID.LitPosBits,
					CoderPropID.Algorithm,
					CoderPropID.NumFastBytes,
					CoderPropID.MatchFinder,
					CoderPropID.EndMarker
				};
				
				object[] t_properties = 
				{
					1 << 21,
					2,
					3,
					0,
					2,
					273,
					"bt4",
					false
				};
				
				t_coder.SetCoderProperties(t_propIDs, t_properties);
				
				t_coder.WriteCoderProperties(t_output);
			}
			
			// Write the decompressed file size.
			t_output.Write(System.BitConverter.GetBytes(t_input.Length), 0, 8);
			
			// Encode the file.
			t_coder.Code(t_input, t_output, t_input.Length, -1, null);
			
			t_output.Flush();
			
			t_output.Close();
		}
		
		#endregion
	}
}
