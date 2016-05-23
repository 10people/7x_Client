//#define DEBUG_SCHEDULE



using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class ScheduleHelper {


	#region Mono

	public static void OnUpdate(){
		Clean();

		Update();
	}

	#endregion



	#region delegate

	public delegate void VoidDelegate();

	#endregion



	#region Use

	private static List<ScheduleItem> m_schedule_item_list = new List<ScheduleItem>();

	public static void RegisterSchedule( VoidDelegate p_delegate, float p_time_interval ){
		ScheduleItem t_item = GetScheduleItem( p_delegate );

		if( t_item != null ){
			Debug.LogError( "Error, Already contained." );

			return;
		}

		#if DEBUG_SCHEDULE
		Debug.Log( "RegisterSchedule( " + p_delegate + " )" );
		#endif

		t_item = new ScheduleItem( p_delegate, p_time_interval );

		m_schedule_item_list.Add( t_item );
	}

	public static void UnRegisterSchedule( VoidDelegate p_delegate ){
		ScheduleItem t_item = GetScheduleItem( p_delegate );

		if( t_item != null ){
			#if DEBUG_SCHEDULE
			Debug.Log( "UnRegisterSchedule( " + p_delegate + " )" );
			#endif

			m_schedule_item_list.Remove( t_item );
		}
	}

	#endregion



	#region Utilities

	private static ScheduleItem GetScheduleItem( VoidDelegate p_delegate ){
		for( int i = m_schedule_item_list.Count - 1; i >= 0; i-- ){
			ScheduleItem t_item = m_schedule_item_list[ i ];

			if( t_item == null ){
				continue;
			}

			if( t_item.m_delegate == p_delegate ){
				return t_item;
			}
		}

		return null;
	}

	private static void Clean(){
		for( int i = m_schedule_item_list.Count - 1; i >= 0; i-- ){
			ScheduleItem t_item = m_schedule_item_list[ i ];

			if( !t_item.IsExist() ){
				#if DEBUG_SCHEDULE
				Debug.Log( "NotExist.Remove( " + t_item.m_delegate + " )" );
				#endif

				m_schedule_item_list.Remove( t_item );
			}
		}
	}

	private static void Update(){
		for( int i = m_schedule_item_list.Count - 1; i >= 0; i-- ){
			ScheduleItem t_item = m_schedule_item_list[ i ];

			t_item.Update();
		}
	}

	#endregion



	private class ScheduleItem{
		public VoidDelegate m_delegate;

		public float m_interval = 0.0f;

		private float m_last_update_time = 0.0f;

		public ScheduleItem( VoidDelegate p_delegate, float p_interval ){
			m_delegate = p_delegate;

			m_interval = p_interval;

			m_last_update_time = Time.realtimeSinceStartup;
		}

		public void Update(){
			if( IsTimeUp() ){
				#if DEBUG_SCHEDULE
				Debug.Log( "ScheduleItem.Execute( " + m_delegate + " )" );
				#endif

				Execute();
			}
		}

		public bool IsTimeUp(){
			if( Time.realtimeSinceStartup - m_last_update_time >= m_interval ){
				return true;
			}

			return false;
		}

		public void Execute(){
			m_last_update_time = Time.realtimeSinceStartup;

			if( m_delegate != null ){
				m_delegate();
			}
			else{
				Debug.LogError( "Delegate is null." );
			}
		}

		public bool IsExist(){
			if( m_delegate == null ){
				return false;
			}

			return true;
		}
	}
}
