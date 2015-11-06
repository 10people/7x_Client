//#define DEBUG_POP_UP_LABEL

//#define DEBUG_UI

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PopUpLabelTool : MonoBehaviour, IUIRootAutoActivator {

	public GameObject m_ngui_parent = null;



	private static PopUpLabelTool m_instance = null;
	
	public static PopUpLabelTool Instance(){
		if( m_instance == null ){
			#if DEBUG_POP_UP_LABEL
			Debug.Log( "PopUpLabelTool.Instance( " + Res2DTemplate.GetResPath( Res2DTemplate.Res.POP_UP_LABEL_TOOL ) + " )" );
			#endif
			
			string t_ui_path = Res2DTemplate.GetResPath( Res2DTemplate.Res.POP_UP_LABEL_TOOL );

			Global.ResourcesDotLoad( t_ui_path, ResourceLoadCallback );
		}
		
		return m_instance;
	}
	
	public static void ResourceLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		GameObject t_gb = (GameObject)GameObject.Instantiate( p_object );
		
		if( t_gb == null ){
			Debug.LogError( "Instantiate to null." );
			
			return;
		}

		DontDestroyOnLoad( t_gb );
	}

	#region Mono
	
	void Awake(){
		m_instance = this;
		
		{
			UIRootAutoActivator.RegisterAutoActivator( this );
		}
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		UpdatePopUpLabels();
	}

	#if DEBUG_UI

	public UILabel m_lb_by_hand = null;

	public UILabel m_lb_up = null;

	public UIPanel m_panel_down = null;

	void OnGUI(){
		{
			ConfigTool.Instance.m_btn_rect_params[ 0 ] = Screen.width * 0.8f;
			
			ConfigTool.Instance.m_btn_rect_params[ 1 ] = Screen.height * 0.2f;
			
			ConfigTool.Instance.m_btn_rect_params[ 2 ] = Screen.width * 0.2f;
			
			ConfigTool.Instance.m_btn_rect_params[ 3 ] = Screen.height * 0.08f;
			
			ConfigTool.Instance.m_btn_rect_params[ 4 ] = 0;
			
			ConfigTool.Instance.m_btn_rect_params[ 5 ] = Screen.height * 0.095f;
		}
		
		int t_button_index = 0;
		
		if( GUI.Button( UtilityTool.GetGUIRect( t_button_index++, ConfigTool.Instance.m_btn_rect_params ), "By Hand", ConfigTool.Instance.m_gui_btn_style ) ){
			PopUpLabelTool.Instance().AddPopLabelWatcher_ManageByHand( m_lb_by_hand.gameObject, Vector3.zero );
		}

		if( GUI.Button( UtilityTool.GetGUIRect( t_button_index++, ConfigTool.Instance.m_btn_rect_params ), "Label", ConfigTool.Instance.m_gui_btn_style ) ){
			PopUpLabelTool.Instance().AddPopLabelWatcher( 
			                                             m_lb_up.gameObject, new Vector3( Screen.width / 4, 0, 0.0f ),
			                                             new Vector2( -Screen.width / 2, Screen.height / 4 ), iTween.EaseType.linear,
			                                             -1.0f, iTween.EaseType.easeOutQuart,
			                                             3.0f );
		}

		if( GUI.Button( UtilityTool.GetGUIRect( t_button_index++, ConfigTool.Instance.m_btn_rect_params ), "Panel", ConfigTool.Instance.m_gui_btn_style ) ){
			PopUpLabelTool.Instance().AddPopLabelWatcher( 
			                                             m_panel_down.gameObject, new Vector3( -Screen.width / 4, 0, 0.0f ),
			                                             new Vector2( Screen.width / 2, -Screen.height / 4 ), iTween.EaseType.linear,
			                                             -1.0f, iTween.EaseType.easeOutQuart,
			                                             1.0f );
		}
	}

	#endif
	
	void OnDestroy(){
		#if DEBUG_POP_UP_LABEL
		Debug.Log( "PopUpLabelTool.OnDestroy()" );
		#endif
		
		{
			m_instance = null;
		}

		{
			UIRootAutoActivator.UnregisterAutoActivator( this );
		}
	}
	
	#endregion



	#region Fx Watcher
	
	public List<PopUpLabelWatcher> m_pop_up_label_list = new List<PopUpLabelWatcher>();

	/// Desc:
	/// Build a PopUp Label with Prefab or Res.Load's Asset, and update Pos&Rot By Yourself.
	/// All Local Pos And Rot will be set to Identity.
	/// 
	/// Params:
	/// 1.p_ngui_gb: PreBuild Prefabs or Resources.Load's Assets.
	public GameObject AddPopLabelWatcher_ManageByHand( GameObject p_ngui_gb, Vector3 p_ngui_pos ){
		return AddPopLabelWatcher( p_ngui_gb, p_ngui_pos,
		                   Vector2.zero, iTween.EaseType.linear, 
		                   0.0f, iTween.EaseType.linear, 
		                   0.0f );
	}

	/// Desc:
	/// Build a PopUp Label with Prefab or Res.Load's Asset, managed by params.
	/// All Local Pos And Rot will be set to Identity.
	/// 
	/// Params:
	/// 1.p_ngui_gb: PreBuild Prefabs or Resources.Load's Assets;
	/// 2.EasyTye: http://easings.net/zh-cn;
	/// 3.p_delta_alpha: from 0.0f to 1.1f;
	public GameObject AddPopLabelWatcher( GameObject p_ngui_gb, Vector3 p_ngui_pos,
	                                     Vector2 p_delta_pos, iTween.EaseType p_pos_type,
	                                     float p_delta_alpha, iTween.EaseType p_alpha_type,
	                                     float p_delta_time ){
		if( p_ngui_gb == null ){
			Debug.LogError( "Error, p_ngui_gb = null." );
			
			return null;
		}
		
		PopUpLabelWatcher t_watcher = new PopUpLabelWatcher( p_ngui_gb, p_ngui_pos,
		                                                    p_delta_pos, p_pos_type, 
		                                                    p_delta_alpha, p_alpha_type, 
		                                                    p_delta_time );
		
		m_pop_up_label_list.Add( t_watcher );
		
		return t_watcher.m_target_ngui_gb;
	}



	private PopUpLabelWatcher GetPopLabelWatcher( GameObject p_ngui_gb ){
		for( int i = 0; i < m_pop_up_label_list.Count; i++ ){
			PopUpLabelWatcher t_watcher = m_pop_up_label_list[ i ];
			
			if( t_watcher.m_target_ngui_gb == p_ngui_gb ){
				return t_watcher;
			}
		}
		
		return null;
	}

	public void UpdatePopUpLabels(){
		int t_count = m_pop_up_label_list.Count;
		
		for( int i = t_count - 1; i >= 0; i-- ){
			PopUpLabelWatcher t_watcher = m_pop_up_label_list[ i ];
			
			t_watcher.Update();
		}
	}
	
	public void ClearPopUpLabel( GameObject p_target_gb ){
		#if DEBUG_POP_UP_LABEL
		Debug.Log( "ClearPopUpLabel( " + p_target_gb + " )" );
		#endif
		
		PopUpLabelWatcher t_watcher = GetPopLabelWatcher( p_target_gb );
		
		if( t_watcher == null ){
			Debug.Log( "No Pop Label Exist for: " + p_target_gb );
			
			return;
		}
		
		t_watcher.Clear();
	}
	
	public class PopUpLabelWatcher{
		public GameObject m_target_ngui_gb = null;

		private Vector2 m_delta_pos = Vector2.zero;

		private iTween.EaseType m_pos_type = iTween.EaseType.linear;

		private float m_delta_alpha = 0.0f;

		private iTween.EaseType m_alpha_type = iTween.EaseType.linear;

		private float m_delta_time = 0.0f;



		private UILabel m_lb = null;

		private UIPanel m_panel = null;

		private Vector3 m_origin_pos = Vector2.zero;

		private float m_origin_alpha = 0.0f;


		private bool m_manage_by_hand = false;

		private bool m_time_out = false;

		public PopUpLabelWatcher( GameObject p_target_ngui_gb, Vector3 p_ngui_pos,
		                         Vector2 p_delta_pos, iTween.EaseType p_pos_type,
		                         float p_delta_alpha, iTween.EaseType p_alpha_type,
		                         float p_delta_time ){
			if( p_target_ngui_gb == null ){
				Debug.LogError( "p_target_ngui_gb = null." );

				return;
			}

			{
				m_target_ngui_gb = (GameObject)Instantiate( p_target_ngui_gb );

				m_delta_pos = p_delta_pos;

				m_pos_type = m_pos_type;

				m_delta_alpha = p_delta_alpha;

				m_alpha_type = p_alpha_type;

				m_delta_time = p_delta_time;
			}

			{
				m_target_ngui_gb.SetActive( true );

				{
					m_target_ngui_gb.transform.parent = PopUpLabelTool.Instance().m_ngui_parent.transform;
					
					UtilityTool.SetGameObjectLayer( PopUpLabelTool.Instance().m_ngui_parent, m_target_ngui_gb );
				}

				{
					UtilityTool.ResetLocalPosAndLocalRotAndLocalScale( m_target_ngui_gb );
					
					m_target_ngui_gb.transform.localPosition = p_ngui_pos;
				}

				{
					m_lb = m_target_ngui_gb.GetComponent<UILabel>();
					
					m_panel = m_target_ngui_gb.GetComponent<UIPanel>();
					
					if( m_lb == null && m_panel == null ){
						Debug.LogError( "Error No Component Found: UILabel & UIPanel." );
					}

					if( m_lb != null ){
						m_origin_alpha = m_lb.alpha;
					}

					if( m_panel != null ){
						m_origin_alpha = m_panel.alpha;
					}

					m_origin_pos = new Vector3( p_ngui_pos.x, p_ngui_pos.y, 0.0f );
				}

				{
					if( Mathf.Approximately( m_delta_pos.magnitude, 0.0f ) &&
					   Mathf.Approximately( m_delta_alpha, 0.0f ) ){
						m_manage_by_hand = true;
					}
					
					#if DEBUG_POP_UP_LABEL
					Debug.Log( "Manage By Hand: " + m_manage_by_hand );
					#endif
				}
			}

			{
				InitTweens();
			}
		}

		private void InitTweens(){
			if( m_delta_time < 0.0f ){
				Debug.LogError( "Error In Time: " + m_delta_time );

				return;
			}

			if( m_manage_by_hand ){
				return;
			}

			{
				UIEventListener t_listener = m_target_ngui_gb.AddComponent<UIEventListener>();
				
				t_listener.onDrag = OnUpdatePos;

				t_listener.onScroll = OnUpdateAlpha;

				t_listener.onSubmit = OnUpdateComplete;
			}

			if( !Mathf.Approximately( m_delta_pos.magnitude, 0.0f ) ){
				#if DEBUG_POP_UP_LABEL
				Debug.Log( "InitTweens for Pos: " + m_delta_pos );
				#endif

				iTween.ValueTo( m_target_ngui_gb, iTween.Hash( 
				                                        "from", Vector2.zero,
				                                        "to", m_delta_pos,
				                                        "time", m_delta_time,
				                                        "easetype", m_pos_type,
														"onupdate", "OnDrag",
				                             			"oncomplete", "OnSubmit" ) );
			}

			if( !Mathf.Approximately( m_delta_alpha, 0.0f ) ){
				#if DEBUG_POP_UP_LABEL
				Debug.Log( "InitTweens for Alpha: " + m_delta_alpha );
				#endif

				iTween.ValueTo( m_target_ngui_gb, iTween.Hash( 
				                                        "from", 0.0f,
				                                        "to", m_delta_alpha,
				                                        "time", m_delta_time,
				                                        "easetype", m_alpha_type,
				                                  		"onupdate", "OnScroll",
				                           				"oncomplete", "OnSubmit" ) );
			}
		}

		public void OnUpdatePos( GameObject p_gb, Vector2 p_delta ){
			#if DEBUG_POP_UP_LABEL
			Debug.Log( "OnUpdatePos( " + p_delta + " )" );
			#endif

			m_target_ngui_gb.transform.localPosition = m_origin_pos + new Vector3( p_delta.x, p_delta.y, 0.0f );
		}

		public void OnUpdateAlpha( GameObject p_gb, float p_delta ){
			#if DEBUG_POP_UP_LABEL
			Debug.Log( "OnUpdateAlpha( " + p_delta + " )" );
			#endif
			
			if( m_lb != null ){
				m_lb.alpha = m_origin_alpha + p_delta;
			}

			if( m_panel != null ){
				m_panel.alpha = m_origin_alpha + p_delta;
			}
		}

		public void OnUpdateComplete( GameObject p_gb ){
			SetTimeOut( true );
		}

		public void Clear(){
			{
				m_target_ngui_gb.SetActive( false );

				Destroy( m_target_ngui_gb );
			}

			PopUpLabelTool.Instance().m_pop_up_label_list.Remove( this );
		}
		
		public void Update(){
			if( ShouldClear() ){
				Clear();
			}
			
			UpdateTransform();
		}
		
		private void UpdateTransform(){
			if( m_target_ngui_gb == null ){
				return;
			}

			// TODO
		}
		
		public bool ShouldClear(){
			if( m_target_ngui_gb == null ){
				return true;
			}

			if( m_time_out ){
				return true;
			}
			
			return false;
		}

		private void SetTimeOut( bool p_is_time_out ){
			m_time_out = p_is_time_out;
		}
	}
	
	#endregion



	#region IUIRootAutoActivator
	
	public bool IsNGUIVisible(){
		if( m_pop_up_label_list == null ){
			return false;
		}
		
		return m_pop_up_label_list.Count > 0 ? true : false;
	}
	
	#endregion
}
