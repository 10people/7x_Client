using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetBundleMemory : MonoBehaviour {

	public List<AssetBundle> m_bundles = new List<AssetBundle>();
			
	#region Mono

	void OnGUI(){
		int t_btn_index = 0;
		
		int t_left_offset = 0;
		
		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Load Bundle Res" ) ){
			Debug.Log( "Load Bundle Res." );
			
//			StartCoroutine( BundleTool.LoadBundle<GameObject>(
//				null,
//				"file://E:/WorkSpace_External/_Debug_Empty/Assets/StreamAssets/Debug_AssetBundle.unity3d",
//				0,
//				"2D_Equip_Growth",
//				OnBundleAssetLoadDone ) );
		}

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Bundle Unload" ) ){
			Debug.Log( "Unload UnloadUnusedAssets." );
			
			for( int i = m_bundles.Count - 1; i >= 0; i-- ){
				AssetBundle	t_bundle = m_bundles[ i ];

				t_bundle.Unload( true );

				m_bundles.Remove( t_bundle );
			}
		}

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Load From Loaded Bundle" ) ){
			Debug.Log( "Load From Loaded Bundle." );
			
			for( int i = m_bundles.Count - 1; i >= 0; i-- ){
				AssetBundle	t_bundle = m_bundles[ i ];
				
				t_bundle.LoadAsset( "2D_Equip_Growth", typeof(GameObject) );
			}
		}
		
		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Unload Unused" ) ){
			Debug.Log( "Unload UnloadUnusedAssets." );
			
			Resources.UnloadUnusedAssets();		
		}
		
		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Switch Scene" ) ){
			Debug.Log( "Switch Scene" );
			
			Application.LoadLevel( 0 );
		}
	}
	
	#endregion
	


	#region Bundle Stuff

	public void OnBundleAssetLoadDone( AssetBundle p_bundle, Object p_obj ){
		Debug.Log( "OnBundleAssetLoadDone( " + p_bundle + " - " + p_obj + " )" );

		if( p_bundle != null ){
			if( !m_bundles.Contains( p_bundle ) ){
				Debug.Log( "Add Bundle: " + p_bundle.name );

				m_bundles.Add( p_bundle );
			}

			Debug.Log( "Total Bundle: " + m_bundles.Count );
		}
	} 


	#endregion


	
	#region Interaction
	
	
	
	#endregion
	
	
	
	#region Utilities
	
	private Rect GetRect( int p_x, int p_index_y ){
		return new Rect( p_x, 75 * p_index_y, 200, 75 );
	}
	
	#endregion
}
