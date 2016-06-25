//#define OPEN_CONSOLE

//#define DEBUG_CONSOLE



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Text;
using System.IO;

using qxmobile;
using qxmobile.protobuf;



/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.9.2
 * @since:		Unity 5.1.3
 * Function:	Assist team to develop.
 * 
 * Notes:
 * None.
 */ 
public class ConsoleTool : MonoBehaviour, SocketProcessor, SocketListener {

	#region Instance

	private static ConsoleTool m_instance = null;
	
	public static ConsoleTool Instance(){
		if( m_instance == null ){
			GameObject t_gameObject = GameObjectHelper.GetDontDestroyOnLoadGameObject();

			ComponentHelper.AddIfNotExist( t_gameObject, typeof(ConsoleTool) );

			m_instance = t_gameObject.GetComponent<ConsoleTool>();
		}
		
		return m_instance;
	}

	#endregion



	#region Mono

	void Awake(){
		{
			SocketTool.RegisterMessageProcessor( this );
			
			SocketTool.RegisterSocketListener( this );
		}
	}

	// Use this for initialization
	void Start () {
		{
			RegisterCommands();
		}
	}

	void OnDestroy(){
		{
			m_instance = null;
		}
		
		{
			SocketTool.UnRegisterMessageProcessor( this );
			
			SocketTool.UnRegisterSocketListener( this );
		}
	}

	private string m_text = "";

	public void OnGUI(){
		{
			bool t_enable_console = IsOpenConsole();
			
			if( !t_enable_console ) {
				return;
			}
		}

		{
			ShowConsole ();
			
			Console_SetConfig.OnGUI();
			
			Console_SetCam.OnGUI();
		}
	}

//	void LateUpdate(){
//		ManualLateUpdate();
//	}

	#endregion



	#region Mono's sub

	private bool IsOpenConsole(){
		bool t_enable_console = false;
		
		t_enable_console = ConfigTool.GetBool( ConfigTool.CONST_SHOW_CONSOLE );
		
		#if OPEN_CONSOLE
		t_enable_console = true;
		#endif

		return t_enable_console;
	}

	/// <summary>
	/// Manuals called LateUpdate, for Camera Reset Use.
	/// </summary>
	public void ManualLateUpdate(){
		{
			bool t_enable_console = IsOpenConsole();

			if( !t_enable_console ) {
				return;
			}
		}
		
		{
			Console_SetCam.LateUpdate();
		}
	}

	private void ShowConsole(){
		m_text = GUI.TextField ( new Rect ( ScreenHelper.GetX( 0.2f ), 0, ScreenHelper.GetWidth( 0.5f ), ScreenHelper.GetHeight( 0.1f ) ), m_text );
		
		if( GUI.Button( new Rect( 0, 0, ScreenHelper.GetWidth( 0.15f ), ScreenHelper.GetHeight( 0.1f ) ), "go" ) ){
			OnChatContent( m_text );
		}
	}

	#endregion



	#region Interact

	private delegate void CommandDelegate( string[] p_params );

	private Dictionary<string, CommandDelegate> m_command_delegate_dict = new Dictionary<string, CommandDelegate>();

	private void RegisterCommands(){
		// release feature
		{
			RegisterCommand ( PING_COMMAND, Console_SetNetwork.OnPing );

			RegisterCommand ( LOG_DEVICE_INFO, DeviceHelper.LogDeviceInfo );
		}

		if( ThirdPlatform.IsThirdPlatform () && !IsOpenConsole() ){
			return;
		}

		// reserved feature
		{
			{
				RegisterCommand ( DEBUG_FUNCTION_COMMAND, Console_DebugFunction.OnDebugFunction );
			}

			{
				RegisterCommand ( LOG_RED_SPOT_DATA_COMMAND, Console_RedSpot.OnLogRedSpotData );
				
				RegisterCommand ( SET_RED_SPOT_DATA_COMMAND, Console_RedSpot.OnSetRedSpotData );
				
				RegisterCommand ( SET_RED_SPOT_COUNT_DOWN_COMMAND, Console_RedSpot.OnRedSpotCountDown );
				
				RegisterCommand ( GET_RED_SPOT_CHILD_COMMAND, Console_RedSpot.OnGetRedSpotChild );



				RegisterCommand ( SET_LOCAL_PUSH, Console_RedSpot.OnSetLocalPush );
			}

			{
				RegisterCommand ( LOG_SOCKET_PROCESSOR_COMMAND, Console_SetNetwork.LogSocketProcessor );
				
				RegisterCommand ( LOG_SOCKET_LISTENER_COMMAND, Console_SetNetwork.LogSocketListener );
			}
			
			{
				RegisterCommand ( COMPONENT_COUNT, Console_Component.ComponentCount );
				
				RegisterCommand ( FIND_GAMEOBJECT, Console_GameObject.FindGameObject );
				
				RegisterCommand ( DESTROY_GAMEOBJECT, Console_GameObject.DestroyGameObject );
				
				RegisterCommand ( DESTROY_COMPONENT, Console_Component.DestroyComponent );
				
				RegisterCommand ( ENABLE_COMPONENT, Console_Component.EnableComponent );
				
				RegisterCommand ( DISABLE_COMPONENT, Console_Component.DisableComponent );

				RegisterCommand ( FIND_OBJECT, Console_Component.FindObject );

				RegisterCommand ( DESTROY_OBJECT, Console_Component.DestroyObject );

				RegisterCommand ( LOG_REFS, Console_Component.LogRefs );
			}

			{
				RegisterCommand ( SET_CONFIG_TOOL, Console_SetConfig.SetConfigTool );
				
				RegisterCommand( LOG_FPS, Console_SetConfig.LogFps );
				
				RegisterCommand( LOG_CONFIG, Console_SetConfig.LogConfig );
			}

			{
				RegisterCommand( LOG_QUALITY, Console_SetQuality.LogQuality );

				RegisterCommand( SET_QUALITY, Console_SetQuality.SetQuality );

				RegisterCommand( SET_LIGHT, Console_SetQuality.SetLight );
				
				RegisterCommand( SET_BLOOM, Console_SetQuality.SetBloom );

				RegisterCommand( SWITCH_SCENE_CAMERA_FX, Console_SetQuality.SwitchCameraFx );


				RegisterCommand( SET_WEIGHT, Console_SetQuality.SetWeight );

				RegisterCommand( LOG_ROOT_AUTO_RELEASE, Console_SetQuality.LogRootAutoRelease );

				RegisterCommand( SET_ATTACK_FX, Console_SetBattleFieldFx.SetAttackFx );
				
				RegisterCommand( SET_SKILL_FX, Console_SetBattleFieldFx.SetSkillFx );
				
				RegisterCommand( SET_BLOODLABEL, Console_SetBattleFieldFx.setBloodLabel );
			}

			{
				RegisterCommand( SET_FPS, Console_SetQuality.SetFPS );
				
				RegisterCommand( SET_SYNC, Console_SetQuality.SetSync );
				
				RegisterCommand( GC, Console_SetSystem.GC );

				RegisterCommand( CONST_LOG_SCREEN, Console_SetSystem.LogScreen );

				RegisterCommand( CONST_VIBRATE, Console_SetSystem.Vibrate );

				RegisterCommand( CONST_DEBUG_CAMERA, Console_SetCam.DebugCamera );

				RegisterCommand( CREATE_RES, Console_SetSystem.CreateRes );

				RegisterCommand( LOAD_RES, Console_SetSystem.LoadRes );

				RegisterCommand( UNLOAD_RES, Console_SetSystem.UnloadRes );

				RegisterCommand( LOG_MODEL_AUTO, ModelAutoActivator.LogModelAuto );
			}

			{
				RegisterCommand( SET_CAM_TOUR, Console_SetCam.SetCamTour );
				
				RegisterCommand ( LOG_MAIN_CAMERA, Console_SetCam.LogMainCamera );
			}

			{
				RegisterCommand( SET_PRE_RUN_C, Console_SetNetwork.SetPreRunC );

				RegisterCommand( SET_VALID_RUN_C, Console_SetNetwork.SetValidRunC );
			}

			{
				RegisterCommand( INIT_MSDK, Console_ThirdPlatform.OnInitMSDK );

				RegisterCommand( MSDK_LOGIN_QQ, Console_ThirdPlatform.OnMSDKLoginQQ );

				RegisterCommand( MSDK_LOGIN_WX, Console_ThirdPlatform.OnMSDKLoginWX );

				RegisterCommand( MSDK_LOGIN_GUEST, Console_ThirdPlatform.OnMSDKLoginGuest );

				RegisterCommand( MSDK_AUTO_LOGIN, Console_ThirdPlatform.OnMSDKAutoLogin );

				RegisterCommand( MSDK_LOGOUT, Console_ThirdPlatform.OnMSDKLogOut );
			}

			{
				RegisterCommand( PLAY_VIDEO, Console_PlayVideo.PlayVideo );
			}

			{
				RegisterCommand( UI_BG_EF, Console_Effect.UIBackgroundEffect );

				RegisterCommand( STR_EF, Console_Effect.StrEffect );

				RegisterCommand( CLOSE_STR_EF, Console_Effect.CloseStrEffect );
			}
		}
	}

	private void RegisterCommand( string p_command, CommandDelegate p_delegate ){
		if( m_command_delegate_dict.ContainsKey( p_command ) ){
			Debug.LogError( "Error, key already exist: " + p_command );

			return;
		}

		if( p_delegate == null ){
			Debug.LogError( "delegate = null." );

			return;
		}

		m_command_delegate_dict.Add( p_command, p_delegate );
	}

	/// <summary>
	/// true, if binggo, and never send to server.
	/// </summary>
	/// <param name="p_chat_content"></param>
	/// <returns></returns>
    public bool OnChatContent( string p_chat_content ){
		#if DEBUG_CONSOLE
		Debug.Log( "ConsoleTool.OnChatContent( " + p_chat_content + " )" );
		#endif

		string[] t_contents = p_chat_content.Split ( ' ' );

		if ( t_contents.Length <= 0 ) {
			return false;
		}

		#if DEBUG_CONSOLE
		LogStringArray( t_contents );
		#endif

		foreach( KeyValuePair<string, CommandDelegate> t_kv in m_command_delegate_dict ){
			if( StringHelper.IsLowerEqual( t_contents[ 0 ], t_kv.Key ) ){
				t_kv.Value( t_contents );

				return true;
			}
		}


		#if DEBUG_CONSOLE
		Debug.LogError( "Command not found: " + p_chat_content );
		#endif

	    return false;
	}

	#endregion



	#region Network

	public bool OnSocketEvent( QXBuffer p_message ){
		if( p_message == null ){
			return false;
		}
		
		switch( p_message.m_protocol_index ){
		default:
			return false;
		}
	}

	public bool OnProcessSocketMessage( QXBuffer p_message ){
		// ping ret
		if( p_message.m_protocol_index == ProtoIndexes.DELAY_RET ){
			ErrorMessage t_msg = new ErrorMessage();
			
			ProtoHelper.DeserializeProto( t_msg, p_message );

			Console_SetNetwork.OnPingReceive( p_message, t_msg );

			return true;
		}
				
		return false;
	}

	#endregion



	#region Utilities

	public void JavaInvoke( string p_param ){
		Debug.Log( "JavaInVoke( " + p_param + " )" );
	}

	public static System.Type GetComponentType( string p_component_type_string ){
		System.Type t_type = null;

		if( StringHelper.IsLowerEqual( p_component_type_string, COM_TYPE_SCENE_GUIDE_MANAGER ) ){
			t_type = typeof(SceneGuideManager);
		}
		else if( StringHelper.IsLowerEqual( p_component_type_string, COM_TYPE_UI_SPRITE ) ){
			t_type = typeof(UISprite);
		}
		else if( StringHelper.IsLowerEqual( p_component_type_string, COM_TYPE_UI_TEXTURE ) ){
			t_type = typeof(UITexture);
		}
		if( StringHelper.IsLowerEqual( p_component_type_string, COM_TYPE_UI_SPRITE ) ){
			t_type = typeof(UISprite);
		}
		else if( StringHelper.IsLowerEqual( p_component_type_string, COM_UI_LABEL ) ){
			t_type = typeof(UILabel);
		}
		else if( StringHelper.IsLowerEqual( p_component_type_string, COM_TYPE_PARTICLE_SYSTEM ) ){
			t_type = typeof(ParticleSystem);
		}
		else if( StringHelper.IsLowerEqual( p_component_type_string, COM_TYPE_MESH_RENDERER ) ){
			t_type = typeof(MeshRenderer);
		}
		else if( StringHelper.IsLowerEqual( p_component_type_string, COM_TYPE_CAMERA ) ){
			t_type = typeof(Camera);
		}
		else if( StringHelper.IsLowerEqual( p_component_type_string, COM_TYPE_UI_ATLAS ) ){
			t_type = typeof(UIAtlas);
		}
		else if( StringHelper.IsLowerEqual( p_component_type_string, COM_TYPE_ANIMATION_CLIP ) ){
			t_type = typeof(AnimationClip);
		}
		else if( StringHelper.IsLowerEqual( p_component_type_string, COM_TYPE_ANIMATOR_CONTROLLER ) ){
			t_type = typeof(RuntimeAnimatorController);
		}
		else if( StringHelper.IsLowerEqual( p_component_type_string, COM_TYPE_ANIMATOR ) ){
			t_type = typeof(Animator);
		}
		else if( StringHelper.IsLowerEqual( p_component_type_string, COM_TYPE_MATERIAL ) ){
			t_type = typeof(Material);
		}
		else if( StringHelper.IsLowerEqual( p_component_type_string, COM_TYPE_RENDERER ) ){
			t_type = typeof(Renderer);
		}
		else if( StringHelper.IsLowerEqual( p_component_type_string, COM_SKINNED_MESH_RENDERER ) ){
			t_type = typeof(SkinnedMeshRenderer);
		}

		return t_type;
	}

	#endregion



	#region Console Command

	public const string DEBUG_FUNCTION_COMMAND			= "/Debug";


	public const string LOG_RED_SPOT_DATA_COMMAND		= "/LogRedSpot";

	public const string SET_RED_SPOT_DATA_COMMAND		= "/SetRedSpot";

	public const string SET_RED_SPOT_COUNT_DOWN_COMMAND = "/RedSpotCountDown";

	public const string GET_RED_SPOT_CHILD_COMMAND		= "/RedSpotChildCount";


	
	public const string PING_COMMAND					= "/ping";

	public const string LOG_SOCKET_PROCESSOR_COMMAND	= "/LogSocketProcessor";

	public const string LOG_SOCKET_LISTENER_COMMAND		= "/LogSocketListener";



	public const string FIND_GAMEOBJECT 				= "/FindGameObject";

	public const string DESTROY_GAMEOBJECT				= "/DestroyGameObject";

	public const string COMPONENT_COUNT					= "/ComponentCount";

	public const string DESTROY_COMPONENT				= "/DestroyComponent";

	public const string ENABLE_COMPONENT				= "/EnableComponent";

	public const string DISABLE_COMPONENT				= "/DisableComponent";

	public const string FIND_OBJECT						= "/FindObject";

	public const string DESTROY_OBJECT					= "/DestroyObject";

	public const string LOG_REFS						= "/LogRefs";



	public const string SET_CONFIG_TOOL 				= "/SetConfig";

	public const string LOG_FPS							= "/LogFps";
	
	public const string LOG_CONFIG						= "/LogConfig";



	public const string LOG_DEVICE_INFO					= "/LogDeviceInfo";

	public const string SET_LIGHT 						= "/SetLight";

	public const string SET_BLOOM						= "/SetBloom";

	public const string SWITCH_SCENE_CAMERA_FX			= "/SwitchCameraFx";



	public const string LOG_QUALITY						= "/LogQuality";

	public const string SET_WEIGHT						= "/SetWeight";

	public const string SET_QUALITY						= "/SetQuality";

	public const string LOG_ROOT_AUTO_RELEASE			= "/LogRootAutoRelease";



	public const string SET_ATTACK_FX					= "/SetAttackFx";

	public const string SET_SKILL_FX					= "/SetSkillFx";

	public const string SET_BLOODLABEL 					= "/SetBloodLabel";

	public const string SET_FPS							= "/SetFPS";

	public const string SET_SYNC						= "/SetSync";

	public const string GC								= "/GC";

	public const string CREATE_RES						= "/CreateRes";

	public const string LOAD_RES						= "/LoadRes";

	public const string UNLOAD_RES						= "/UnloadRes";

	public const string LOG_MODEL_AUTO					= "/LogModelAuto";



	public const string SET_CAM_TOUR					= "/SetCamTour";

	public const string LOG_MAIN_CAMERA					= "/LogMainCam";

	public const string CONST_LOG_SCREEN				= "/LogScreen";

	public const string CONST_VIBRATE					= "/Vibrate";

	public const string CONST_DEBUG_CAMERA				= "/DebugCamera";



	public const string SET_PRE_RUN_C					= "/SetPreRunC";

	public const string SET_VALID_RUN_C					= "/SetValidRunC";



	public const string SET_LOCAL_PUSH					= "/SetLocalPush";




	public const string INIT_MSDK						= "/InitMSDK";

	public const string MSDK_LOGIN_QQ					= "/MSDKLoginQQ";

	public const string MSDK_LOGIN_WX					= "/MSDKLoginWX";

	public const string MSDK_LOGIN_GUEST				= "/MSDKLoginGUEST";

	public const string MSDK_AUTO_LOGIN					= "/MSDKAutoLogin";

	public const string MSDK_LOGOUT						= "/MSDKLogOut";



	public const string PLAY_VIDEO						= "/PlayVideo";



	public const string UI_BG_EF						= "/UIBgEf";

	public const string STR_EF							= "/StrEff";

	public const string CLOSE_STR_EF					= "/CloseStrEff";


	#endregion



	#region Component Type

	public const string COM_TYPE_SCENE_GUIDE_MANAGER 	= "SceneGuideManager";

	public const string COM_TYPE_MESH_RENDERER			= "MeshRenderer";

	public const string COM_TYPE_UI_SPRITE				= "UISprite";

	public const string COM_TYPE_UI_TEXTURE 			= "UITexture";

	public const string COM_UI_LABEL 					= "UILabel";

	public const string COM_TYPE_PARTICLE_SYSTEM 		= "ps";

	public const string COM_TYPE_CAMERA	 				= "Camera";

	public const string COM_TYPE_LIGHT					= "Light";

	public const string COM_TYPE_UI_ATLAS				= "UIAtlas";

	public const string COM_TYPE_ANIMATION_CLIP			= "AnimationClip";

	public const string COM_TYPE_ANIMATOR_CONTROLLER	= "AnimatorController";

	public const string COM_TYPE_ANIMATOR				= "Animator";

	public const string COM_TYPE_MATERIAL				= "Material";

	public const string COM_TYPE_RENDERER				= "Renderer";

	public const string COM_SKINNED_MESH_RENDERER		= "SkinnedMeshRenderer";

	#endregion
}
