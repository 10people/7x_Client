//#define DEBUG_LOADING



using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;



public class LoadingTemplate : XmlLoadManager
{
    // <LoadingTemplate priority="1" function="1" param="1-1" resid="100" />

	public enum LoadingFunctions{
		PVE_GUO_GUAN = 1,	// ok
		PVE_SHI_LIAN = 2,	// ok
		PVP_BAI_ZHAN = 3,	// ok
		PVP_YUN_BIAO = 4,	// ok
		PVP_LUE_DUO = 5,	// ok
		
		PVP_HUANG_YE = 6,	// ok
		SPECIAL_FIRST_DAY = 10,	// ok
		COMMON = 11,		// ok


		ENTER_LOGIN = 12,		// to check
		ENTER_GUIDE_LEVEL = 7,	// ok
		ENTER_CREATE_ROLE = 8,	// ok
		ENTER_MAIN_CITY = 9,	// ok
	}

	private static LoadingFunctions m_loading_function = LoadingFunctions.COMMON;

	private static string m_loading_param = "";

    public int m_function_id;
	
	public string m_param;
	
	public int m_res_id;
	
    private static List<LoadingTemplate> m_templates = new List<LoadingTemplate>();


    private static LoadingTemplate m_instance = null;

    public static LoadingTemplate Instance(){
        if( m_instance == null ){
            m_instance = new LoadingTemplate();
        }

        return m_instance;
    }



	#region Load

    public static void LoadTemplates(EventDelegate.Callback p_callback = null){
        //		Debug.Log( "Res2DTemplate.LoadTemplates()" );

        UnLoadManager.DownLoad( PathManager.GetUrl(m_LoadPath + "LoadingTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false );
    }

	public static void CurLoad( ref WWW www, string path, UnityEngine.Object obj ){
        if ( m_templates.Count > 0 ){
            return;
        }

        XmlReader t_reader = null;

        if ( obj != null ){
            TextAsset t_text_asset = obj as TextAsset;

            t_reader = XmlReader.Create(new StringReader(t_text_asset.text));
        }
        else
        {
            t_reader = XmlReader.Create(new StringReader(www.text));
        }

        bool t_has_items = true;

        do
        {
			t_has_items = t_reader.ReadToFollowing("LoadingTemplate");

            if (!t_has_items)
            {
                break;
            }

            LoadingTemplate t_template = new LoadingTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.m_function_id = int.Parse(t_reader.Value);
                
				t_reader.MoveToNextAttribute();
                t_template.m_param = t_reader.Value;

				t_reader.MoveToNextAttribute();
                t_template.m_res_id = int.Parse(t_reader.Value);
            }

            m_templates.Add( t_template );
        }
        while (t_has_items);
    }

	#endregion



	#region Use

	public static void SetCurFunction( LoadingFunctions p_cur_function, string p_param = "" ){
		#if DEBUG_LOADING
		Debug.Log( "SetCurFunction( " + p_cur_function + "   " + p_param + " )" );
		#endif

		m_loading_function = p_cur_function;

		m_loading_param = p_param;
	}

	// TODO
	public static string GetResPath(){
		if( m_templates == null ||
			m_templates.Count == 0 ){
			return "";
		}

		string t_res_path = "";

		#if DEBUG_LOADING
		Debug.Log( "Using Loading Bg Type: " + m_loading_function );
		#endif

		switch( m_loading_function ){
		case LoadingFunctions.ENTER_LOGIN:
		case LoadingFunctions.ENTER_GUIDE_LEVEL:
		case LoadingFunctions.ENTER_CREATE_ROLE:
		case LoadingFunctions.ENTER_MAIN_CITY:
		case LoadingFunctions.PVE_GUO_GUAN:
		case LoadingFunctions.PVE_SHI_LIAN:
		case LoadingFunctions.PVP_BAI_ZHAN:
		case LoadingFunctions.PVP_YUN_BIAO:
		case LoadingFunctions.PVP_LUE_DUO:
		case LoadingFunctions.PVP_HUANG_YE:
			t_res_path = GetTargetRes( m_loading_function, m_loading_param );
			break;


		case LoadingFunctions.SPECIAL_FIRST_DAY:
		case LoadingFunctions.COMMON:
			t_res_path = GetCommonUILoading();
			break;

		default:
			Debug.LogError( "Error, no condition found." );

			t_res_path = GetTargetRes( m_loading_function, m_loading_param );
			break;
		}

		#if DEBUG_LOADING
		Debug.Log( "Using Loading Bg: " + t_res_path );
		#endif

		return t_res_path;
	}

	private static int m_cur_common_ui_index 	= -1;

	private static string GetCommonUILoading(){
		List<int> m_target_common_ui_ids = new List<int>();

		for( int i = 0; i < m_templates.Count; i++ ){
			if( m_templates[ i ].m_function_id == (int)LoadingFunctions.COMMON ){
				m_target_common_ui_ids.Add( m_templates[ i ].m_res_id );
			}
		}

		bool t_is_first_day = IsFirstDay();

		#if DEBUG_LOADING
		Debug.Log( "Is First Day: " + t_is_first_day );
		#endif

		if( t_is_first_day ){
			for( int i = 0; i < m_templates.Count; i++ ){
				if( m_templates[ i ].m_function_id == (int)LoadingFunctions.SPECIAL_FIRST_DAY ){
					m_target_common_ui_ids.Add( m_templates[ i ].m_res_id );
				}
			}
		}

		if( m_target_common_ui_ids.Count == 0 ){
//			Debug.Log( "Error, no common ui res found." );

			return "";
		}

		{
			// init random
			if( m_cur_common_ui_index < 0 ){
				m_cur_common_ui_index = MathHelper.GetRandom( 0, m_target_common_ui_ids.Count - 1 );

				#if DEBUG_LOADING
				Debug.Log( "First Time use Common, Random Init: " + m_cur_common_ui_index );
				#endif
			}
		}

		#if DEBUG_LOADING
		Debug.Log( "Using Common Bg Index : " + m_cur_common_ui_index );
		#endif

		int t_res_id = m_target_common_ui_ids[ m_cur_common_ui_index ];

		m_cur_common_ui_index++;

		if( m_cur_common_ui_index >= m_target_common_ui_ids.Count ){
			m_cur_common_ui_index = 0;
		}

		#if DEBUG_LOADING
		Debug.Log( "Using Common Bg Id : " + t_res_id );
		#endif

		return Res2DTemplate.GetResPath( t_res_id );
	}

	#endregion



	#region Utilities

	public static LoadingTemplate GetFirstTemplate( LoadingFunctions p_function_id ){
		int p_target_id = (int)p_function_id;

		for( int i = 0; i < m_templates.Count; i++ ){
			if( m_templates[ i ].m_function_id == p_target_id ){
				return m_templates[ i ];
			}
		}

		return null;
	}

	public static string GetTargetRes( LoadingFunctions p_function_id, string p_param ){
		int p_target_id = (int)p_function_id;

		int p_res_id = -1;

		for( int i = 0; i < m_templates.Count; i++ ){
			if( m_templates[ i ].m_function_id == p_target_id ){
				if( string.IsNullOrEmpty( m_templates[ i ].m_param ) ){
					p_res_id = m_templates[ i ].m_res_id;	

					#if DEBUG_LOADING
					Debug.Log( "Find Target, param is NullorEmpty: " + p_res_id );
					#endif

					break;
				}
				else{
					if( m_templates[ i ].m_param == p_param ){
						p_res_id = m_templates[ i ].m_res_id;

						#if DEBUG_LOADING
						Debug.Log( "Find Target: " + p_res_id + "   param: " + p_param );
						#endif

						break;
					}
				}
			}
		}

		#if DEBUG_LOADING
		Debug.Log( "Target Res id : " + p_res_id );
		#endif

		// default common Bg
		if( p_res_id < 0 ){
			#if DEBUG_LOADING
			Debug.Log( "Use Common Bg Now." );
			#endif

			string t_target_path = GetCommonUILoading();

			return t_target_path;
		}

		return Res2DTemplate.GetResPath( p_res_id ); 
	}

	public static bool IsFirstDay(){
		int t_register_h = int.Parse( PlayerInfoCache.GetRegisterTimeString( "HH" ) );

		int t_first_day_end_delta_h = 0;

		int t_param_target_h = 4;

		{
			LoadingTemplate t_template = GetFirstTemplate( LoadingFunctions.SPECIAL_FIRST_DAY );

			if( t_template == null ){
				Debug.LogError( "Error, No Data found for First Day." );

				return false;
			}
			else{
				t_param_target_h = int.Parse( t_template.m_param );

				#if DEBUG_LOADING
				Debug.Log( "Target Hour of Day: " + t_param_target_h );
				#endif
			}
		}

		if( t_register_h >= 4 ){
			t_first_day_end_delta_h = 24 + 4 - t_register_h;
		}
		else{
			t_first_day_end_delta_h = 4 - t_register_h;
		}

		// 2008-05-01 07:34:42
		// "yyyy-MM-dd-HHmmss"

		DateTime t_first_day_end = DateTime.Parse( PlayerInfoCache.GetRegisterTimeString( "yyyy-MM-dd HH" ) + ":00:00" );

//		Debug.Log( "t_first_day_end_delta_h: " + t_first_day_end_delta_h + "   " + t_first_day_end.ToString( TimeHelper.DEFAULT_DATETIME_FORMAT ) );

		t_first_day_end = t_first_day_end.AddHours( t_first_day_end_delta_h );

//		Debug.Log( "First Day End Time: " + t_first_day_end.ToString( TimeHelper.DEFAULT_DATETIME_FORMAT ) );

		int t_compare = TimeHelper.GetCurServerDateTime().CompareTo( t_first_day_end );

		if( t_compare > 0 ){
			return false;
		}
		else{
			return true;
		}
	}

	#endregion
}