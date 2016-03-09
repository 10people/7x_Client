using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FxCacheItem : MonoBehaviour{

	public string m_fx_path = "";

	#region Register & UnRegister

	void Awake(){
		
	}

	void OnEnable(){
		
	}

	void OnDisable(){
		
	}

	public void FreeFx(){
		FxHelper.FreeFxGameObject( m_fx_path, gameObject );
	}

	#endregion



	#region Utilities

	public static void AutoFxCache( string p_fx_path, GameObject p_gb ){
		if( p_gb == null ){
			Debug.Log( "gb is null: " + p_gb );

			return;
		}

		FxCacheItem t_fx = (FxCacheItem)ComponentHelper.AddIfNotExist( p_gb, typeof(FxCacheItem) );

		t_fx.m_fx_path = p_fx_path;
	}

	#endregion
}