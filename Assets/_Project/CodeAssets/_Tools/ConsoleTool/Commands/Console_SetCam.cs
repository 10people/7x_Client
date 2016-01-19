//#define DEBUG_CAM_TOUR

using UnityEngine;
using System.Collections;
using System;

public class Console_SetCam {

	#region Mono

	private static Camera m_main_cam 	= null;

	void OnDestroy(){
		m_main_cam = null;
	}

	public static void OnGUI(){
		if( !m_enable_cam_tour ){
			return;
		}

		{
			m_main_cam = Camera.main;
			
			if( m_main_cam == null ){
				return;
			}
		}

		{
			GUI_Move();

			GUI_Rot();
		}

		{
			UpdateCamTourTransform();
		}
	}

	public static void LateUpdate(){
		if( !m_enable_cam_tour ){
			return;
		}

		{
			m_main_cam = Camera.main;
			
			if( m_main_cam == null ){
				return;
			}
		}

		{
			CheckResetCam();
		}

		{
			UpdateMainCamTransform();
		}
	}

	#endregion



	#region GUI Items

	private static void GUI_Move(){
		if( GUI.Button( GUIHelper.GetJoyStickRect( GUIHelper.GUIJoyStickType.Left, GUIHelper.GUIJoyStickButton.Up ), 
		               "↑" ) ){
			m_main_cam.transform.localPosition += m_main_cam.transform.forward * m_move_unit;
		}

		if( GUI.Button( GUIHelper.GetJoyStickRect( GUIHelper.GUIJoyStickType.Left, GUIHelper.GUIJoyStickButton.Down ), 
		               "↓" ) ){
			m_main_cam.transform.localPosition += m_main_cam.transform.forward * -m_move_unit;
		}

		if( GUI.Button( GUIHelper.GetJoyStickRect( GUIHelper.GUIJoyStickType.Left, GUIHelper.GUIJoyStickButton.Left ), 
		               "←" ) ){
			m_main_cam.transform.localPosition += m_main_cam.transform.right * -m_move_unit;
		}

		if( GUI.Button( GUIHelper.GetJoyStickRect( GUIHelper.GUIJoyStickType.Left, GUIHelper.GUIJoyStickButton.Right ), 
		               "→" ) ){
			m_main_cam.transform.localPosition += m_main_cam.transform.right * m_move_unit;
		}
	}

	private static void GUI_Rot(){
		if( GUI.Button( GUIHelper.GetJoyStickRect( GUIHelper.GUIJoyStickType.Right, GUIHelper.GUIJoyStickButton.Up ), 
		               "↑" ) ){
			m_main_cam.transform.Rotate( new Vector3( -m_rotate_unit, 0, 0 ) );
		}
		
		if( GUI.Button( GUIHelper.GetJoyStickRect( GUIHelper.GUIJoyStickType.Right, GUIHelper.GUIJoyStickButton.Down ), 
		               "↓" ) ){
			m_main_cam.transform.Rotate( new Vector3( m_rotate_unit, 0, 0 ) );
		}
		
		if( GUI.Button( GUIHelper.GetJoyStickRect( GUIHelper.GUIJoyStickType.Right, GUIHelper.GUIJoyStickButton.Left ), 
		               "←" ) ){
			m_main_cam.transform.Rotate( new Vector3( 0, -m_rotate_unit, 0 ) );
		}
		
		if( GUI.Button( GUIHelper.GetJoyStickRect( GUIHelper.GUIJoyStickType.Right, GUIHelper.GUIJoyStickButton.Right ), 
		               "→" ) ){
			m_main_cam.transform.Rotate( new Vector3( 0, m_rotate_unit, 0 ) );
		}
	}

	#endregion



	#region Main Camera
	
	public static void LogMainCamera( string[] p_params ){
		if( p_params.Length <= 0 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		m_main_cam = Camera.main;
		
		if( m_main_cam == null ){
			Debug.LogError( "main cam = null." );
			
			return;
		}
		
		GameObjectHelper.LogGameObjectHierarchy( m_main_cam.gameObject, "Main Cam " );
		
		Debug.Log( "pos: " + m_main_cam.gameObject.transform.position );
		
		Debug.Log( "localRot:" + m_main_cam.gameObject.transform.localRotation );
	}
	
	
	#endregion



	#region Set Cam Tour

	/// 1.Enable/Disable Cam Tour:
	/// SetCamTour true
	/// SetCamTour false
	/// 
	/// 
	/// 2.Set Tour Move/Rotate Unit:
	/// SetCamTour mu 5
	/// SetCamTour ru 15
	public static void SetCamTour( string[] p_params ){
		if( IsEnOrDisable( p_params ) ){
			return;
		}

		if( IsSetUnit( p_params ) ){
			return;
		}

		if( IsResetCam( p_params ) ){
			return;
		}

		{
			StringHelper.LogStringArray( p_params );
		}
	}

	#endregion



	#region Cam Tour Body

	private static Vector3 m_local_pos		= Vector3.zero;
	
	private static Quaternion m_local_rot	= Quaternion.identity;

	private static void CheckResetCam(){
		if( IsResetCamTour() ){
			SetResetCamTour( false );

			{
				UpdateCamTourTransform();
			}
		}
	}

	private static void ResetAllToIdentity(){
		#if DEBUG_CAM_TOUR
		Debug.Log( "ResetAllToIdentity()" );
		#endif

		{
			m_local_pos = Vector3.zero;
			
			m_local_rot = Quaternion.identity;
		}

		{
			UpdateMainCamTransform();
		}
	}

	private static void UpdateCamTourTransform(){
		m_local_pos = m_main_cam.transform.localPosition;
		
		m_local_rot = m_main_cam.transform.localRotation;
	}

	private static void UpdateMainCamTransform(){
		m_main_cam.transform.localPosition = m_local_pos;

		m_main_cam.transform.localRotation = m_local_rot;
	}

	#endregion



	#region Enable/Disable Cam Tour

	private static bool m_enable_cam_tour = false;

	private static bool IsEnOrDisable( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return false;
		}
		
		bool t_param_1_enable = false;
		
		try{
			t_param_1_enable = bool.Parse( p_params[ 1 ] );
			
			EnableCamTour( t_param_1_enable );
			
			return true;
		}
		catch( Exception e ){
			return false;
		}
	}
	
	private static void EnableCamTour( bool p_enable ){
		#if DEBUG_CAM_TOUR
		Debug.Log( "EnableCamTour( " + p_enable + " )" );
		#endif

		m_enable_cam_tour = p_enable;

		if( p_enable ){
			SetResetCamTour( true );
		}
	}

	#endregion



	#region Set Cam Tour Unit

	private static float m_move_unit 		= 5.0f;
	
	private static float m_rotate_unit 		= 5.0f;

	private static bool IsSetUnit( string[] p_params ){
		if( p_params.Length <= 2 ){
			return false;
		}
		
		if( !StringHelper.IsLowerEqual( p_params[ 1 ], MOVE_UNIT ) &&
		   !StringHelper.IsLowerEqual( p_params[ 1 ], ROT_UNIT ) ){
			return false;
		}
		
		float t_param_1_unit = 0.0f;
		
		try{
			t_param_1_unit = float.Parse( p_params[ 2 ] );
			
			if( StringHelper.IsLowerEqual( p_params[ 1 ], MOVE_UNIT ) ){
				SetMoveUnit( t_param_1_unit );
			}
			else if( StringHelper.IsLowerEqual( p_params[ 1 ], ROT_UNIT ) ){
				SetRotUnit( t_param_1_unit );
			}
			else{
				return false;
			}

			return true;
		}
		catch( Exception e ){
			return false;
		}
	}

	private static void SetMoveUnit( float p_move_unit ){
		#if DEBUG_CAM_TOUR
		Debug.Log( "SetMoveUnit( " + p_move_unit + " )" );
		#endif

		m_move_unit = p_move_unit;
	}

	private static void SetRotUnit( float p_rot_unit ){
		#if DEBUG_CAM_TOUR
		Debug.Log( "SetRotUnit( " + p_rot_unit + " )" );
		#endif

		m_rotate_unit = p_rot_unit;
	}

	#endregion



	#region Reset Cam Tour
	
	private static bool m_enable_reset_cam = false;
	
	private static bool IsResetCam( string[] p_params ){
		if( p_params.Length <= 1 ){
			return false;
		}
		
		if( StringHelper.IsLowerEqual( p_params[ 1 ], RESET_POT ) ){
			SetResetCamTour( true );

			return true;
		}

		return false;
	}
	
	private static void SetResetCamTour( bool p_is_reset ){
		#if DEBUG_CAM_TOUR
		Debug.Log( "SetResetCamTour( " + p_is_reset + " )" );
		#endif

		m_enable_reset_cam = p_is_reset;

//		if( p_is_reset ){
//			ResetAllToIdentity();
//		}
	}

	private static bool IsResetCamTour(){
		return m_enable_reset_cam;
	}
	
	#endregion



	#region Const

	private const string MOVE_UNIT		= "mu";
	
	private const string ROT_UNIT		= "ru";

	private const string RESET_POT		= "reset";

	#endregion
}
