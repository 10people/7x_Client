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
			GameObject t_gameObject = UtilityTool.GetDontDestroyOnLoadGameObject();
			
			m_instance = t_gameObject.AddComponent( typeof( ConsoleTool ) ) as ConsoleTool;
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

	private string m_text = "";



	public void OnGUI(){
		ShowConsole ();

		ShowPause ();

		ShowQuickFx ();
	}

	// Update is called once per frame
	void Update () {

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

	#endregion



	#region Mono's sub

	private void ShowPause(){
		bool t_show = ConfigTool.GetBool( ConfigTool.CONST_QUICK_PAUSE );
		
		if ( !t_show ) {
			return;
		}
		
		if( GUI.Button( new Rect( 0, ScreenTool.GetY( 0.1f ), ScreenTool.GetX( 0.15f ), ScreenTool.GetY( 0.1f ) ), 
		               ( Time.timeScale > 0 ? "Pause" : "Resume" ) ) ){
			ExecPause();
		}
	}

	private bool m_show_fx = true;

	private void ShowQuickFx(){
		bool t_show = ConfigTool.GetBool( ConfigTool.CONST_QUICK_FX );
		
		if ( !t_show ) {
			return;
		}
		
		if( GUI.Button( new Rect( ScreenTool.GetX( 0.2f ), ScreenTool.GetY( 0.1f ), ScreenTool.GetX( 0.2f ), ScreenTool.GetY( 0.1f ) ), 
		               m_show_fx ? "Fx.Hide" : "Fx.Show" ) ){
			m_show_fx = !m_show_fx;

			string[] t_params = { "", COM_TYPE_PARTICLE_SYSTEM };

			if( m_show_fx ){
				t_params[ 0 ] = ENABLE_COMPONENT;
				Console_Components.EnableComponent( t_params );
			}
			else{
				t_params[ 0 ] = DISABLE_COMPONENT;

				Console_Components.DisableComponent( t_params );
			}
		}
	}

	private void ExecPause(){
		if ( Time.timeScale > 0 ) {
			Time.timeScale = 0;
		}
		else {
			Time.timeScale = 1;
		}

//		Debug.Log ( "Switch TimeScale to: " + Time.timeScale );
	}
	
	private void ShowConsole(){
		bool t_show_console = false;
		
		t_show_console = ConfigTool.GetBool( ConfigTool.CONST_SHOW_CONSOLE );
		
		#if OPEN_CONSOLE
		t_show_console = true;
		#endif
		
		if( !t_show_console ) {
			return;
		}
		
		m_text = GUI.TextField ( new Rect ( ScreenTool.GetX( 0.2f ), 0, ScreenTool.GetX( 0.5f ), ScreenTool.GetY( 0.1f ) ), m_text );
		
		if( GUI.Button( new Rect( 0, 0, ScreenTool.GetX( 0.15f ), ScreenTool.GetY( 0.1f ) ), "go" ) ){
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
			RegisterCommand ( PING_COMMAND, OnPing );

			RegisterCommand ( LOG_DEVICE_INFO, DeviceHelper.LogDeviceInfo );
		}

		if ( ThirdPlatform.IsThirdPlatform () ) {
			return;
		}

		// reserve feature
		{
			RegisterCommand ( LOG_RED_SPOT_DATA_COMMAND, Console_RedSpot.OnLogRedSpotData );
			
			RegisterCommand ( SET_RED_SPOT_DATA_COMMAND, Console_RedSpot.OnSetRedSpotData );
			
			RegisterCommand ( SET_RED_SPOT_COUNT_DOWN_COMMAND, Console_RedSpot.OnRedSpotCountDown );
			
			RegisterCommand ( GET_RED_SPOT_CHILD_COMMAND, Console_RedSpot.OnGetRedSpotChild );
			
			RegisterCommand ( LOG_SOCKET_PROCESSOR_COMMAND, LogSocketProcessor );
			
			RegisterCommand ( LOG_SOCKET_LISTENER_COMMAND, LogSocketListener );
			
			RegisterCommand ( COMPONENT_COUNT, Console_Components.ComponentCount );
			
			RegisterCommand ( FIND_GAMEOBJECT, FindGameObject );

			RegisterCommand ( DESTROY_GAMEOBJECT, DestroyGameObject );
			
			RegisterCommand ( DESTROY_COMPONENT, Console_Components.DestroyComponent );

			RegisterCommand ( ENABLE_COMPONENT, Console_Components.EnableComponent );

			RegisterCommand ( DISABLE_COMPONENT, Console_Components.DisableComponent );
			
			RegisterCommand ( LOG_MAIN_CAMERA, LogMainCamera );
			
			RegisterCommand ( SET_CONFIG_TOOL, SetConfigTool );

			RegisterCommand( SET_LIGHT, SetLight );

			RegisterCommand( LOG_FPS, LogFps );

			RegisterCommand( SET_BLOOM, Console_SetQuality.SetBloom );

			RegisterCommand( LOG_CONFIG, LogConfig );

			RegisterCommand( LOG_QUALITY, LogQuality );

			RegisterCommand( SET_WEIGHT, Console_SetQuality.SetWeight );

			RegisterCommand( LOG_ROOT_AUTO_RELEASE, LogRootAutoRelease );

			RegisterCommand( SET_ATTACK_FX, Console_SetBattleFieldFx.SetAttackFx );

			RegisterCommand( SET_SKILL_FX, Console_SetBattleFieldFx.SetSkillFx );

			RegisterCommand( SET_BLOODLABEL, Console_SetBattleFieldFx.setBloodLabel );

			RegisterCommand( SET_FPS, Console_SetQuality.SetFPS );

			RegisterCommand( SET_SYNC, Console_SetQuality.SetSync );

			RegisterCommand( GC, Console_SetSystem.GC );
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
			if( StringHelper.IsLowerEqual( t_contents[ 0], t_kv.Key ) ){
				t_kv.Value( t_contents );

				return true;
			}
		}


		#if DEBUG_CONSOLE
		Debug.LogError( "Command not found." );
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
			MemoryStream t_stream = new MemoryStream( p_message.m_protocol_message, 0, p_message.position );
			
			ErrorMessage t_msg = new ErrorMessage();
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			t_qx.Deserialize( t_stream, t_msg, t_msg.GetType() );

			OnPingReceive( t_msg );

			return true;
		}
				
		return false;
	}

	#endregion



	#region Ping

	private float m_on_ping_time = 0.0f;

	private void OnPing( string[] p_params ){
		#if DEBUG_CONSOLE
		Debug.Log( "OnPing() " + Time.realtimeSinceStartup );
		#endif

		ErrorMessage t_msg = new ErrorMessage ();
		
		{
			t_msg.cmd = 1;

			t_msg.errorCode = 10;

			t_msg.errorDesc = Time.realtimeSinceStartup + "";
		}

		{
			m_on_ping_time = Time.realtimeSinceStartup;
		}

		UtilityTool.SendQXMessage( t_msg, ProtoIndexes.DELAY_REQ );
	}

	void OnPingReceive( ErrorMessage p_msg ){
		#if DEBUG_CONSOLE
		Debug.Log( Time.realtimeSinceStartup + " OnPingReceive( " + p_msg.cmd + 
		          "   - " + p_msg.errorCode +
		          "   - " + p_msg.errorDesc + " )" );
		#endif

		Debug.Log ( "Ping Duration: " + ( Time.realtimeSinceStartup - m_on_ping_time ) );


        {
            ChatPct tempChatPct = new ChatPct();
            
            tempChatPct.senderName = "Sys";

            tempChatPct.content = "Delay: " + (Time.realtimeSinceStartup - m_on_ping_time);

            tempChatPct.channel = ChatWindow.s_ChatWindow.CurrentChannel;

            ChatWindow.s_ChatWindow.GetChannelFrame(ChatWindow.s_ChatWindow.CurrentChannel).m_ChatBaseDataHandler.OnChatMessageReceived(tempChatPct );
        }

	    

//		Global.CreateBox( "Ping",
//		                 "Duration: " + ( Time.realtimeSinceStartup - m_on_ping_time ),
//		                 "",
//		                 null,
//		                 "OK",
//		                 null,
//		                 OnCloseBoxCallback );
	}

	public void OnCloseBoxCallback( int p_index ){
		#if DEBUG_CONSOLE
		Debug.Log( "OnCloseBoxCallback()" );
		#endif
	}

	#endregion



	#region Socket 

	private void LogSocketProcessor( string[] p_params ){
		SocketTool.LogSocketProcessor ();
	}

	private void LogSocketListener( string[] p_params ){
		SocketTool.LogSocketListener ();
	}

	#endregion



	#region Find GameObject

	private void FindGameObject( string[] p_params ){
		FindGameObjectAndReturn( p_params );
	}

	private GameObject FindGameObjectAndReturn( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return null;
		}
		
		string t_param_1_name = "";
		
		try{
			t_param_1_name = p_params[ 1 ];
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return null;
		}
		
		GameObject t_gb = GameObject.Find( t_param_1_name );
		
		if( t_gb != null ){
			GameObjectHelper.LogGameObjectHierarchy( t_gb );
		}
		else{
			Debug.LogError( "GameObject not found: " + t_param_1_name );
		}

		return t_gb;
	}

	#endregion



	#region Destroy GameObject

	private void DestroyGameObject( string[] p_params ){
		GameObject t_obj = FindGameObjectAndReturn( p_params );

		if( t_obj == null ){
			return;
		}

		Destroy( t_obj );

		{
			GameObjectHelper.LogGameObjectHierarchy( t_obj, "Destroy first found " );

		}
	}

	#endregion



	#region Main Camera

	private void LogMainCamera( string[] p_params ){
		if( p_params.Length <= 0 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		Camera t_cam = Camera.main;

		if( t_cam == null ){
			Debug.LogError( "main cam = null." );

			return;
		}

		GameObjectHelper.LogGameObjectHierarchy( t_cam.gameObject, "Main Cam " );

		Debug.Log( "pos: " + t_cam.gameObject.transform.position );

		Debug.Log( "localRot:" + t_cam.gameObject.transform.localRotation );
	}


	#endregion


	#region Set Config Tool

	/// Emp:
	/// SetConfigTool ShowConsole false
	private void SetConfigTool( string[] p_params ){
		if( p_params.Length < 3 ){
			Debug.Log( "Length Not Enough." );
			
			return;
		}
		
		string t_target_key = p_params[ 1 ].ToLowerInvariant();

		foreach( KeyValuePair<string, ConfigTool.ConfigValue> t_pair in ConfigTool.m_config_value_dict ){
			string t_key = t_pair.Key.ToLowerInvariant();
			
			if( t_key == t_target_key ){
				Debug.Log( "Config.Set( " + t_key + " - " + p_params[ 2 ] + " )" );
				
				t_pair.Value.AutoSet( p_params[ 2 ] );
				
				return;
			}
		}
		
		{
			Debug.LogError( "Key Not Found: " + p_params[ 1 ] );
		}
	}
	
	#endregion



	#region Set Light

	private void SetLight( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}

		bool t_param_1_show = false;
		
		try{
			t_param_1_show = bool.Parse( p_params[ 1 ] );
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}

		{
			QualityTool.ConfigLights( t_param_1_show );
			
			QualityTool.m_quality_dict[ QualityTool.CONST_IN_CITY_SHADOW ].AutoSet( p_params[ 1 ] );

			QualityTool.m_quality_dict[ QualityTool.CONST_BATTLE_FIELD_SHADOW ].AutoSet( p_params[ 1 ] );
		}
	}

	#endregion



	#region Log Device Info

	public static void LogDeviceInfo(){
		DeviceHelper.LogDeviceInfo( null );
	}

	#endregion



	#region Log Fps
	
	private void LogFps( string[] p_params ){
		ComponentHelper.AddIfNotExist( UtilityTool.GetDontDestroyOnLoadGameObject(), 
		                              typeof(FPSCounter_CS) );
	
		Debug.Log( "TargetFrameRate: " + Application.targetFrameRate );

		Debug.Log( "vSyncCount: " + QualitySettings.vSyncCount );
	}
	
	#endregion



	#region Log Config
	
	private void LogConfig( string[] p_params ){
		ConfigTool.LogConfigs();
	}
	
	#endregion



	#region Log Quality
	
	private void LogQuality( string[] p_params ){
		QualityTool.LogQualityItems();
	}
	
	#endregion



	#region LogRootAutoRelease

	private void LogRootAutoRelease( string[] p_params ){
		UIRootAutoActivator.Log();
	}

	#endregion





	#region Utilities

	/// p_ops: < or >
	/// p_w_x_h: Wxh
	public static bool IsWHCompareStatisfy( float p_w, float p_h, string p_ops, string p_w_x_h ){
		if( string.IsNullOrEmpty( p_ops ) ){
			return true;
		} 
		
		if( string.IsNullOrEmpty( p_w_x_h ) ){
			return true;
		}
		
		string[] t_w_h = p_w_x_h.Split( new char[]{'x'} );
		
		if( t_w_h.Length != 2 ){
			Debug.LogError( "WxH error: " + p_w_x_h );
			
			return false;
		}
		
		float t_w = 0;
		
		float t_h = 0;
		
		try{
			t_w = float.Parse( t_w_h[ 0 ] );
			
			t_h = float.Parse( t_w_h[ 1 ] );
		}
		catch( Exception e ){
			Debug.LogError( "Error, params error: " + e );
			
			return false;
		}
		
		if( StringHelper.IsLowerEqual( p_ops, "<" ) ){
			if( p_w < t_w && p_h < t_h ){
				return true;
			}
		}
		else if( StringHelper.IsLowerEqual( p_ops, ">" ) ){
			if( p_w > t_w && p_h > t_h ){
				return true;
			}
		}
		else{
			Debug.Log( "compare error: " + p_ops );
		}
		
		return false;
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

		return t_type;
	}

	#endregion



	#region Console Command

	private const string PING_COMMAND					= "/ping";



	private const string LOG_RED_SPOT_DATA_COMMAND		= "/LogRedSpot";

	private const string SET_RED_SPOT_DATA_COMMAND		= "/SetRedSpot";

	private const string SET_RED_SPOT_COUNT_DOWN_COMMAND = "/RedSpotCountDown";

	private const string GET_RED_SPOT_CHILD_COMMAND		= "/RedSpotChildCount";



	private const string LOG_SOCKET_PROCESSOR_COMMAND	= "/LogSocketProcessor";

	private const string LOG_SOCKET_LISTENER_COMMAND	= "/LogSocketListener";



	private const string FIND_GAMEOBJECT 				= "/FindGameObject";

	private const string DESTROY_GAMEOBJECT				= "/DestroyGameObject";

	private const string COMPONENT_COUNT				= "/ComponentCount";

	private const string DESTROY_COMPONENT				= "/DestroyComponent";

	private const string ENABLE_COMPONENT				= "/EnableComponent";

	private const string DISABLE_COMPONENT				= "/DisableComponent";

	private const string LOG_MAIN_CAMERA				= "/LogMainCam";

	private const string SET_CONFIG_TOOL 				= "/SetConfig";

	private const string LOG_DEVICE_INFO				= "/LogDeviceInfo";

	private const string SET_LIGHT 						= "/SetLight";

	private const string SET_BLOOM						= "/SetBloom";

	private const string LOG_FPS						= "/LogFps";

	private const string LOG_CONFIG						= "/LogConfig";

	private const string LOG_QUALITY					= "/LogQuality";

	private const string SET_WEIGHT						= "/SetWeight";

	private const string LOG_ROOT_AUTO_RELEASE			= "/LogRootAutoRelease";

	private const string SET_ATTACK_FX					= "/SetAttackFx";

	private const string SET_SKILL_FX					= "/SetSkillFx";

	private const string SET_BLOODLABEL 				= "/SetBloodLabel";

	private const string SET_FPS						= "/SetFPS";

	private const string SET_SYNC						= "/SetSync";

	private const string GC								= "/GC";

	#endregion



	#region Component Type

	private const string COM_TYPE_SCENE_GUIDE_MANAGER 	= "SceneGuideManager";

	private const string COM_TYPE_MESH_RENDERER			= "MeshRenderer";

	private const string COM_TYPE_UI_SPRITE				= "UISprite";

	private const string COM_TYPE_UI_TEXTURE 			= "UITexture";

	private const string COM_UI_LABEL 					= "UILabel";

	private const string COM_TYPE_PARTICLE_SYSTEM 		= "ps";

	private const string COM_TYPE_CAMERA	 			= "Camera";

	private const string COM_TYPE_LIGHT					= "Light";

	#endregion
}
