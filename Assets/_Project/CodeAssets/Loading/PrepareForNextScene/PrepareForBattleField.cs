//#define DEBUG_PREPARE_FOR_BATTLE_FIELD

//#define SKIP_TEX_OPTIMIZE_ANDROID



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;



public class PrepareForBattleField : MonoBehaviour {

	#region Instance

	private static PrepareForBattleField m_instance = null;

	public static PrepareForBattleField Instance(){
		return m_instance;
	}

	#endregion



	#region Mono

	void Awake(){
		{
			m_instance = this;
		}

		Prepare_For_BattleField();
	}

	void OnRenderObject(){
		if( m_battle_res_step == 1 ){
			LoadingHelper.ItemLoaded( StaticLoading.m_loading_sections,
			                         CONST_BATTLE_RENDER, "Init" );
			
			m_battle_res_step++;
		}
	}

	void OnDestroy(){
		m_instance = null;
	}

	#endregion



	#region loading sections

	private void InitBattleLoading(){
		//		LoadingHelper.InitSectionInfo( StaticLoading.m_loading_sections, StaticLoading.CONST_COMMON_LOADING_SCENE, 1, -1 );
		
		LoadingHelper.InitSectionInfo( StaticLoading.m_loading_sections, CONST_BATTLE_LOADING_2D, 5, 1 );
		
		LoadingHelper.InitSectionInfo( StaticLoading.m_loading_sections, CONST_BATTLE_LOADING_NETWORK, 6, 1 );
		
		LoadingHelper.InitSectionInfo( StaticLoading.m_loading_sections, CONST_BATTLE_LOADING_DATA, 3, 1 );

		LoadingHelper.InitSectionInfo( StaticLoading.m_loading_sections, CONST_BATTLE_LOADING_3D, 50, 42 );

		LoadingHelper.InitSectionInfo( StaticLoading.m_loading_sections, CONST_BATTLE_LOADING_FX, 20, 55 );
		
		LoadingHelper.InitSectionInfo( StaticLoading.m_loading_sections, CONST_BATTLE_LOADING_SOUND, 5, 95 );
		
		LoadingHelper.InitSectionInfo( StaticLoading.m_loading_sections, CONST_BATTLE_CREATE_FLAGS, 2, 2 );
		
		LoadingHelper.InitSectionInfo( StaticLoading.m_loading_sections, CONST_BATTLE_RENDER, 5, 1 );
	}

	#endregion


	#region Prepare For Battle Field
	
	private int m_battle_res_step = 0;
	
	private const int BATTLE_RES_STEP_TOTAL	= 2;
	
	private GameObject temple2D;
	
	private GameObject temple3D;

	private void Prepare_For_BattleField(){
		#if DEBUG_PREPARE_FOR_BATTLE_FIELD
		Debug.Log( "Prepare_For_BattleField()" );
		#endif

		InitBattleLoading();
		
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.BATTLEFIELD_V4_2D_UI ), 
		                        Load2DCallback);
		
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.BATTLEFIELD_V4_3D_ROOT ),
		                        Load3DCallback);
	}
	
	public void Load2DCallback( ref WWW p_www, string p_path, Object p_object ){
		temple2D = p_object as GameObject;
		
		enterBattleField();
	}
	
	public void Load3DCallback( ref WWW p_www, string p_path, Object p_object ){
		temple3D = p_object as GameObject;
		
		enterBattleField();
	}
	
	private void enterBattleField(){
		LoadingHelper.ItemLoaded( StaticLoading.m_loading_sections,
		                         CONST_BATTLE_LOADING_2D, "EnterBattleField" );
		
		if (temple2D != null && temple3D != null){
			Prepare_For_BattleFieldCallback ();
		}
	}
	
	public void Prepare_For_BattleFieldCallback(){
		#if DEBUG_PREPARE_FOR_BATTLE_FIELD
		Debug.Log( "Prepare_For_BattleFieldCallback()" );
		#endif

		// preload templates
		{
			NameIdTemplate.ProcessAsset();
		}

		// origin coroutine
		{
			GameObject gc2d = (GameObject)Instantiate( temple2D );
			
			gc2d.SetActive( true );
			
			gc2d.transform.localScale = new Vector3(1, 1, 1);
			
			gc2d.transform.position = new Vector3(0, 500, 0);
			
			gc2d.name = "BattleField_V4_2D";
			
			{
				DontDestroyOnLoad( gc2d );
				
				LoadingHelper.RemoveWhenSceneDone( gc2d );
			}
			
			GameObject gc = (GameObject)Instantiate( temple3D );
			
			gc.SetActive( true );
			
			gc.transform.localScale = new Vector3(1, 1, 1);
			
			gc.transform.localPosition = Vector3.zero;
			
			gc.name = "BattleField_V4_3D";
			
			{
				DontDestroyOnLoad( gc );
				
				LoadingHelper.RemoveWhenSceneDone( gc );
			}
			
			//			if(CityGlobalData.m_tempSection == 0 
			//			   && CityGlobalData.m_tempLevel == 0 
			//			   && CityGlobalData.m_enterPvp == false)
			//			{
			////				EffectTemplate et = EffectTemplate.getEffectTemplateByEffectId( 66 );
			//
			//				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.LOGIN_CREATE_ROLE ),
			//				                        LoginCreateRoleLoadCallback );
			//			}
			
			BattleNet net = gc.GetComponentInChildren( typeof( BattleNet ) ) as BattleNet;
			
			net.getData();
		}
	}
	
	//	public void LoginCreateRoleLoadCallback( ref WWW p_www, string p_path, Object p_object ){
	//		GameObject createRoleObject = Instantiate( p_object ) as GameObject;
	//
	//		createRoleObject.SetActive( true );
	//		
	//		createRoleObject.transform.localScale = new Vector3(1, 1, 1);
	//		
	//		createRoleObject.transform.localPosition = new Vector3(0, -500, 0);
	//		
	//		createRoleObject.name = "BattleField_V4_CreateRole";
	//		
	//		{
	//			DontDestroyOnLoad( createRoleObject );
	//			
	//			RemoveWhenSceneDone( createRoleObject );
	//		}
	//	}
	
	private float m_battle_tex_time = 0.0f;
	
	public void BattleLoadDone(){
		m_battle_tex_time = Time.realtimeSinceStartup;
		
		m_battle_res_step++;

		bool t_exec_pre_load = true;

		#if SKIP_TEX_OPTIMIZE_ANDROID && UNITY_ANDROID
		t_exec_pre_load = false;
		#endif

		if( t_exec_pre_load ){
			Dictionary<string, GameObject> t_dict = BattleEffectControllor.Instance().GetEffectDict();
			
			List<Texture> t_list = new List<Texture>();
			
			{
				int t_index = 0;
				
				int t_tex_count = 0;
				
				int t_skip_count = 0;
				
				GameObject t_template_gb = ( GameObject )Instantiate( EnterNextScene.GetBackgroundImage().gameObject );
				
				{
					t_template_gb.transform.parent = EnterNextScene.GetBackgroundImage().gameObject.transform.parent;
					
					TransformHelper.CopyTransform( EnterNextScene.GetBackgroundImage().gameObject, t_template_gb );
					
					{
						UITexture t_tex = t_template_gb.GetComponent<UITexture>();
						
						t_tex.depth = t_tex.depth - 1;
					}
				}
				
				foreach( KeyValuePair< string, GameObject > t_pair in t_dict ){
					if( t_pair.Value == null ){
						t_index++;
						
						continue;
					}
					
					{
						t_index++;
					}

//					GameObject t_gb = ( GameObject )Instantiate( t_pair.Value );
					GameObject t_gb = FxHelper.ObtainFreeFxGameObject( t_pair.Key, t_pair.Value );
					
//					Debug.Log( t_pair.Key + ": " + t_gb.name );
					
					t_gb.SetActive( true );
					
					{
						ParticleSystem[] t_pss = t_gb.GetComponentsInChildren<ParticleSystem>();
						
						for( int i = 0; i < t_pss.Length; i++ ){
							ParticleSystem t_ps = t_pss[ i ];
							
							Texture t_tex = t_ps.GetComponent<Renderer>().material.mainTexture;
							
							if( t_tex == null ){
								continue;
							}
							
							if( t_list.Contains( t_tex ) ){
								t_skip_count++;
								
								continue;
							}
							else{
								t_list.Add( t_tex );
							}
							
							t_tex_count++;
							
							GameObject t_shadow_gb = ( GameObject )Instantiate( t_template_gb );
							
							t_shadow_gb.transform.parent = EnterNextScene.GetBackgroundImage().gameObject.transform;
							
							TransformHelper.CopyTransform( t_template_gb, t_shadow_gb );
							
							t_shadow_gb.name = t_index + " : " + t_tex.name + "_" + t_tex_count;
							
							UITexture t_ui_tex = t_shadow_gb.GetComponent<UITexture>();
							
							t_ui_tex.mainTexture = t_tex;
							
							t_ui_tex.SetDimensions( 16, 16 );
						}
					}
					
					t_gb.SetActive( false );
					
//					Destroy( t_gb );

					FxHelper.FreeFxGameObject( t_pair.Key, t_gb );
				}
				
				t_template_gb.SetActive( false );
				
				Destroy( t_template_gb );
			}
		}
		else{
			m_battle_res_step++;
		}
		
		{
			m_battle_tex_time = Time.realtimeSinceStartup - m_battle_tex_time;
		}
		
		StartCoroutine( CheckingResForBattleField() );
	}
	
	IEnumerator CheckingResForBattleField(){
		#if DEBUG_PREPARE_FOR_BATTLE_FIELD
		Debug.Log( "CheckingResForBattleField()" );
		#endif

		while ( m_battle_res_step < BATTLE_RES_STEP_TOTAL ){
			#if DEBUG_PREPARE_FOR_BATTLE_FIELD
			Debug.Log( "CheckingResForBattleField( " + 
			          m_battle_res_step + " / " + BATTLE_RES_STEP_TOTAL + 
			          " )" );
			#endif
			
			yield return new WaitForEndOfFrame();
		}

		#if DEBUG_PREPARE_FOR_BATTLE_FIELD
		Debug.Log( "CheckingResForBattleField.Wait.Done()" );
		#endif
		
		// report when battle field is ready
		{
			OperationSupport.ReportClientAction( OperationSupport.ClientAction.ENTER_GAME );
		}
		
		{
			//			SetAutoActivation( true );

			EnterNextScene.DirectLoadLevel();
		}
	}
	
	#endregion

	
	
	#region Loading BattleField
	
	public const string CONST_BATTLE_LOADING_2D			= "Battle_2D";
	
	public const string CONST_BATTLE_LOADING_NETWORK	= "Battle_Net";
	
	public const string CONST_BATTLE_LOADING_DATA		= "Battle_Data";
	
	public const string CONST_BATTLE_LOADING_3D			= "Battle_3D";
	
	public const string CONST_BATTLE_LOADING_FX			= "Battle_Fx";
	
	public const string CONST_BATTLE_LOADING_SOUND		= "Battle_Sound";
	
	public const string CONST_BATTLE_CREATE_FLAGS		= "Battle_Create_Flags";
	
	public const string CONST_BATTLE_RENDER				= "Battle_Render";
	
	#endregion
}
