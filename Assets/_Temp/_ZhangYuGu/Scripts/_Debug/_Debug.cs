using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


//#if UNITY_EDITOR 
//using UnityEditor;
//using UnityEditorInternal;
//#endif

public class _Debug : MonoBehaviour {

	public Animator m_animator = null;

	private static _Debug m_instance = null;

	public Color m_temp_color;

	#region Mono

	void Awake(){
		Debug.Log ( "_Debug.Awake()" );

		m_instance = this;

		DontDestroyOnLoad( gameObject );
	}

	// Use this for initialization
	void Start () {
		Debug.Log ( "_Debug.Start()" );

	}

	void OnGUI(){
		{
			GUIHelper.GUILayoutVerticalSpace( 0.1f );
		}
		
		GUILayout.BeginVertical();
		
		{
			string t_path = "_3D/Fx/Prefabs/BattleEffect/BOSSwudi";

			if( GUILayout.Button( "Global Fx" ) ){
				FxTool.PlayGlobalFx( t_path, FxLoadDelegate );
			}

			if( GUILayout.Button( "Global Fx X" ) ){
				FxTool.PlayGlobalFx( t_path, FxLoadDelegate, new Vector3( 10, 10, 10 ), new Vector3( 30, 30, 30 ) );
			}
			
			if( GUILayout.Button( "Local Fx" ) ){
				FxTool.PlayLocalFx( t_path, gameObject, FxLoadDelegate );
			}

			if( GUILayout.Button( "Local Fx" ) ){
				FxTool.PlayLocalFx( t_path, gameObject, FxLoadDelegate, new Vector3( -10, -10, -10 ), new Vector3( -30, -30, -30 ) );
			}
		}
		
		GUILayout.EndVertical();
	}

	public void FxLoadDelegate( GameObject p_fx ){
		Debug.Log( "FxLoadDelegate( " + p_fx + " )" );

		GameObjectHelper.LogGameObjectInfo( p_fx );
	}


	void Update () {
//		Debug.Log ( "Update.TimeScale: " + Time.timeScale );

	}

	void LateUpdate() {

	}

	void OnDestroy(){
		Debug.Log ( "_Debug.OnDestroy()" );

		m_instance = null;
	}

	#endregion



	#region Utilities


	#endregion
}
