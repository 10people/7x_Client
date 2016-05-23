using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldMapDragger : MonoBehaviour {

	public float m_drag_delay = 0.8f;

	public iTween.EaseType m_ease_type = iTween.EaseType.easeOutQuad;


	public GameObject m_on_click_notice_target;

	public string m_onclick_notice_func_name;


	private Vector3 m_pos_last_drag_delta;

	private float[] m_time_last_drag = new float[ 4 ];


	private Dictionary<int, Vector2> m_last_touches = new Dictionary<int, Vector2>();


	// Use this for initialization
	void Start () {
//		Application.targetFrameRate =60;

		m_time_last_drag[ 0 ] = m_time_last_drag[ 1 ] = Time.time;
	}

	#region NGUI Event

	void OnPress( bool p_press ){
		SyncMultiTouch();

		if( !p_press ){
			if( UICamera.touchCount == 1 ){
				InertialDrag();
			}
		}
		else{
			iTween.Stop( Camera.main.gameObject );
		}
	}

	void OnDrag( Vector2 p_delta ){
		if( UICamera.touchCount == 2 ){
			ProcessScale();
		}
		else if( UICamera.touchCount == 1 ){
			ProcessDrag( p_delta );
		}
	}

	void OnClick(){
		if( m_on_click_notice_target != null ){
			m_on_click_notice_target.SendMessage( m_onclick_notice_func_name );
		}
	}

	#endregion

	#region Drag Utilities

	public bool IfReachable(){
		float[,] t_check_point = new float[ 4, 2 ];
		
		t_check_point[ 0, 0 ] = 0;
		t_check_point[ 0, 1 ] = 0;
		
		t_check_point[ 1, 0 ] = 0;
		t_check_point[ 1, 1 ] = Screen.height;
		
		t_check_point[ 2, 0 ] = Screen.width;
		t_check_point[ 2, 1 ] = 0;
		
		t_check_point[ 3, 0 ] = Screen.width;
		t_check_point[ 3, 1 ] = Screen.height;
		
		for( int i = 0; i < 4; i++ ){
			Ray t_ray = Camera.main.ScreenPointToRay( new Vector3( t_check_point[ i, 0 ], t_check_point[ i, 1 ], 10 ) );
			
			RaycastHit t_hit;
			
			if( !Physics.Raycast( t_ray.origin, t_ray.direction, out t_hit, 1000, 1 << LayerMask.NameToLayer( "Drag Collide Layer" ) ) ){
				return false;
			}
		}
		
		return true;
	}

	private void ProcessDrag( Vector2 p_delta ){
		m_pos_last_drag_delta = Vector3.zero;
		
		{
			for( int i = 0; i < m_time_last_drag.Length - 1; i++ ){
				m_time_last_drag[ i ] = m_time_last_drag[ i + 1 ];
			}
			
			m_time_last_drag[ m_time_last_drag.Length - 1 ] = Time.time;
		}
		
		ImmidiateDrag( new Vector2( p_delta.x, 0 ) );
		
		ImmidiateDrag( new Vector2( 0, p_delta.y ) );
	}

	private bool ImmidiateDrag( Vector2 p_delta ){
		Ray t_ray = Camera.main.ScreenPointToRay( new Vector3( UICamera.lastTouchPosition.x, UICamera.lastTouchPosition.y, 10 ) );
		
		RaycastHit t_hit;
		
		if( Physics.Raycast( t_ray.origin, t_ray.direction, out t_hit, 1000, 1 << LayerMask.NameToLayer( "Drag Collide Layer" ) ) ){
			Ray t_ray_new = Camera.main.ScreenPointToRay( new Vector3( UICamera.lastTouchPosition.x + p_delta.x, UICamera.lastTouchPosition.y + p_delta.y, 10 ) );
			
			RaycastHit t_hit_new;
			
			if( Physics.Raycast( t_ray_new.origin, t_ray_new.direction, out t_hit_new, 1000, 1 << LayerMask.NameToLayer( "Drag Collide Layer" ) ) ){
				Vector3 t_origin = Camera.main.transform.position;

				Camera.main.transform.position = Camera.main.transform.position + t_hit.point - t_hit_new.point;

				if( !IfReachable() ){
					Camera.main.transform.position = t_origin;

					return false;
				}
				else{
					m_pos_last_drag_delta += ( t_hit.point - t_hit_new.point );

					return true;
				}
			}
		}

		return false;
	}

	private void InertialDrag(){
		Vector3 t_origin = Camera.main.transform.position;

		Vector3 t_target_delta = m_pos_last_drag_delta * m_drag_delay / ( Time.time - m_time_last_drag[ 0 ] ) / 2;
		
		Camera.main.transform.position = Camera.main.transform.position + t_target_delta;
		
		if( !IfReachable() ){
			Camera.main.transform.position = t_origin;
			
			iTween.Stop( Camera.main.gameObject );
		}
		else{
			Camera.main.transform.position = t_origin;
			
			iTween.MoveTo( Camera.main.gameObject, iTween.Hash( 
			        "position", Camera.main.transform.position + t_target_delta, 
			        "time", m_drag_delay, 
			        "easeType", m_ease_type ) );
		}
	}

	#endregion


	#region Scale Utilities

	private void ProcessScale(){
		Vector2[] t_last_pos = new Vector2[ 2 ];
		Vector2[] t_cur_pos = new Vector2[ 2 ];

		// sample
		{
//			Dictionary< int, UICamera.MouseOrTouch > t_cur_touches = UICamera.GetTouches();

			int t_for_index = 0;
			foreach( KeyValuePair< int, Vector2 > t_touch in m_last_touches ){
				if( t_for_index < 2 ){
					t_last_pos[ t_for_index ] = t_touch.Value;

					t_cur_pos[ t_for_index ] = UICamera.GetTouch( t_touch.Key ).pos;

					t_for_index++;
				}
			}

			NGUIDebug.Log( "Origin: " + t_last_pos[ 0 ] + " , " + t_last_pos[ 1 ] );

			NGUIDebug.Log( "Cur: " + t_cur_pos[ 0 ] + " , " + t_cur_pos[ 1 ] );
		}

		// scale
		{
			float t_h_c = 0.0f;

			{
				Ray t_ray = Camera.main.ScreenPointToRay( new Vector3( ( t_last_pos[ 0 ].x + t_last_pos[ 1 ].x ) / 2, ( t_last_pos[ 0 ].y + t_last_pos[ 1 ].y ), 10 ) );

				RaycastHit t_hit;
				
				if( Physics.Raycast( t_ray.origin, t_ray.direction, out t_hit, 1000, 1 << LayerMask.NameToLayer( "Drag Collide Layer" ) ) ){
					t_h_c = t_hit.distance;
				}
				else{
					Debug.LogError( "Nothing Hit." );

					return;
				}
			}

			float t_w_c = 0.0f;

			float t_world_dis = 0.0f;

			{
				Vector3[] t_point = new Vector3[ 2 ];

				for( int i = 0; i < t_last_pos.Length; i++ ){
					Ray t_ray = Camera.main.ScreenPointToRay( new Vector3( t_last_pos[ i ].x, t_last_pos[ i ].y, 10 ) );
					
					RaycastHit t_hit;
					
					if( Physics.Raycast( t_ray.origin, t_ray.direction, out t_hit, 1000, 1 << LayerMask.NameToLayer( "Drag Collide Layer" ) ) ){
						t_point[ i ] = t_hit.point;
					}
					else{
						Debug.LogError( "Nothing Hit." );
						
						return;
					}
				}

				t_world_dis = ( t_point[ 1 ] - t_point[ 0 ] ).magnitude;

				t_w_c = Mathf.Abs( Screen.width * t_world_dis / ( t_last_pos[ 1 ] - t_last_pos[ 0 ] ).magnitude );
			}

			float t_w_c_n = 0.0f;

			{
				t_w_c_n = Mathf.Abs( Screen.width * t_world_dis / ( t_cur_pos[ 1 ] - t_cur_pos[ 0 ] ).magnitude );
			}

			float t_h_c_n = t_h_c / t_w_c * t_w_c_n;

			NGUIDebug.Log( "t_h_c_n: " + t_h_c_n + "   t_h_c: " + t_h_c + ", t_w_c: " + t_w_c + ", t_w_c_n: " + t_w_c_n );

			{
				Vector3 t_origin = Camera.main.transform.position;
				
				Camera.main.transform.Translate( new Vector3( 0, 0, -t_h_c_n + t_h_c ) );
				
				if( !IfReachable() ){
					Camera.main.transform.position = t_origin;
					
					iTween.Stop( Camera.main.gameObject );
				}
			}
		}

		// sync
		{
			SyncMultiTouch();
		}
	}

	private void SyncMultiTouch(){
		Debug.LogError( "Never Used When Update to NGUI 3.8.2." );

		m_last_touches.Clear();

//		List<UICamera.MouseOrTouch> t_cur_touches = UICamera.GetTouches();
//
//		foreach( UICamera.MouseOrTouch t_touch in t_cur_touches ){
//			m_last_touches.Add( t_touch.Key, t_touch.Value.pos );
//		}
	}

	#endregion

	// Update is called once per frame
	void Update () {

	}
}
