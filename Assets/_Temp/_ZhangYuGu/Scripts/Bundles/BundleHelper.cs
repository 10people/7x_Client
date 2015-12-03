#define DEBUG_BUNDLE_HELPER

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

using SimpleJSON;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.11.30
 * @since:		Unity 5.1.3
 * Function:	Helper class for Build Bundles.
 * 
 * Notes:
 * 1.Created in Unity 5
 */ 
public class BundleHelper : MonoBehaviour{

	#region Build

	#if UNITY_EDITOR
	// Build All Bundles in Unity5.
	public static void BuildAll( string p_path ){
		if( string.IsNullOrEmpty( p_path ) ){
			Debug.LogError( "path is null or empty." );

			return;
		} 

		BuildPipeline.BuildAssetBundles( p_path,
		                                BuildAssetBundleOptions.None,
		                                BuildTarget.Android );
	}
	#endif

	#endregion



	#region Logs

	#if UNITY_EDITOR
	/// Log all configged bundles
	public static void LogAllBundleConfigs(){
		string[] t_names = AssetDatabase.GetAllAssetBundleNames();
		foreach( string t_name in t_names ){
			Debug.Log( "Asset Bundle: " + t_name );
		}
	}
	#endif

	#endregion



	#region Clean

	public static void CleanCache(){
		Debug.Log( "Clean Cache." );
		
		Caching.CleanCache();
	}

	#endregion

	
	
	#region Bundle Loader

	private Dictionary<string, BundleContainer> m_bundle_dict	= new Dictionary<string, BundleContainer>();

	/// TODO, p_url will be removed.
	public UnityEngine.Object LoadBundleAsset( string p_url, string p_asset_name ){
		if( !m_bundle_dict.ContainsKey( p_url ) ){
			Debug.LogError( "Bundle not loaded." );
			
			return null;
		}
		
		return m_bundle_dict[ p_url ].LoadBundleAsset( p_asset_name );
	}
	
	public void LoadBundle( string p_url, int p_version ){
		if( !m_bundle_dict.ContainsKey( p_url ) ){
			StartCoroutine( StartLoadBundle( p_url, p_version ) );
		}
		else{
			Debug.Log( "Bundle Already Loaded." );
		}
	}
	
	private IEnumerator StartLoadBundle( string p_url, int p_version ){
		#if DEBUG_BUNDLE_HELPER
		Debug.Log( "StartLoadBundle( " + p_url + ", " + p_version + " )" );
		#endif
		
		using( WWW www = WWW.LoadFromCacheOrDownload ( p_url, p_version ) ){
			yield return www;
			
			if (www.error != null){
				Debug.LogError( "Error in WWW.Download: " + www.error );
				
				yield return null;
			}

			#if DEBUG_BUNDLE_HELPER
			Debug.Log( "Bundle Loaded: " + p_url + "   - " + p_version );
			#endif
			
			AssetBundle t_bundle = www.assetBundle;
			
			if( t_bundle != null ){
				BundleContainer t_container = new BundleContainer( p_url, p_version, t_bundle );
				
				m_bundle_dict.Add( p_url, t_container );

				#if DEBUG_BUNDLE_HELPER
				t_container.LogAllAssetNames();

				t_container.LogAllScenePaths();
				#endif
			}
			else{
				Debug.LogError( "Bundle is null." );
			}
		}
	}
	
	#endregion



	private class BundleContainer{
		private string m_url;
		
		private int m_version;
		
		private AssetBundle m_bundle;
		
		public BundleContainer( string p_url, int p_version, AssetBundle p_bundle ){
			if( string.IsNullOrEmpty( p_url ) ){
				Debug.LogError( "Url not specified." );
				
				return;
			}
			
			if( p_version < 0 ){
				Debug.LogError( "version error." );
				
				return;
			}
			
			if( p_bundle == null ){
				Debug.LogError( "bundle is null." );
				
				return;
			}
			
			m_url = p_url;
			
			m_version = p_version;
			
			m_bundle = p_bundle;
		}
		
		public AssetBundle GetBundle(){
			return m_bundle;
		}
		
		public UnityEngine.Object LoadBundleAsset( string p_asset_name ){
			if( string.IsNullOrEmpty( p_asset_name ) ){
				return m_bundle.mainAsset;
			}
			else{
				return m_bundle.LoadAsset( p_asset_name );
			}
		}

		public string GetBundleDescription(){
			return m_url + "   - " + m_version;
		}
		
		public void LogAllAssetNames(){
			string[] t_assets = m_bundle.GetAllAssetNames();
			
			if( t_assets == null ){
				Debug.LogError( "Error, no asset exist: " + GetBundleDescription() );
				
				return;
			}
			
			for( int i = 0; i < t_assets.Length; i++ ){
				Debug.Log( "Asset " + i + " : " + t_assets[ i ] );
			}
		}

		public void LogAllScenePaths(){
			string[] t_paths = m_bundle.GetAllScenePaths();

			if( t_paths == null ){
				Debug.LogError( "Error, no path exist: " + GetBundleDescription() );

				return;
			}

			for( int i = 0; i < t_paths.Length; i++ ){
				Debug.Log( "Scene " + i + " : " + t_paths[ i ] );
			}
		}
	}
}