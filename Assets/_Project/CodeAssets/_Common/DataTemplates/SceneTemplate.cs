using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class SceneTemplate : XmlLoadManager
{
	//<SceneTemplate sceneId="0" sceneName="Game_Scene_Bundle_Loader" isCheck="1" />

	public enum SceneEnum
	{
		BUNDLE_LOADER = 0,
		REGISTER_N_LOGIN = 1,
		LOADING = 2,
		CREATE_ROLE = 3,
		MAIN_CITY = 4,
		MAIN_CITY_YE_WAN = 5,
		HOUSE = 6,
		ALLIANCE_CITY = 7,
		ALLIANCE_CITY_YE_WAN = 8,
		CARRIAGE = 9,
		ALLIANCE_BATTLE = 10,
		BATTLE_FIELD_1 = 100,
		BATTLE_FIELD_2 = 101,
		BATTLE_FIELD_3 = 201,
		BATTLE_FIELD_4 = 202,
		BATTLE_FIELD_5 = 203,
		BATTLE_FIELD_6 = 204,
		BATTLE_FIELD_7 = 205,
		BATTLE_FIELD_8 = 207,
		BATTLE_FIELD_9 = 301,
		BATTLE_FIELD_10 = 302,
		BATTLE_FIELD_11 = 303,
		BATTLE_FIELD_12 = 304,
		BATTLE_FIELD_13 = 401,
		BATTLE_FIELD_14 = 406,
		BATTLE_FIELD_15 = 407,
		BATTLE_FIELD_16 = 503,
		BATTLE_FIELD_17 = 504,
		BATTLE_FIELD_18 = 506,
		BATTLE_FIELD_19 = 704,
		BATTLE_FIELD_20 = 1503,
		BATTLE_FIELD_21 = 2008,
		BATTLE_FIELD_22 = 900,
	}

	public int sceneId;

	public string sceneName;

	public int isCheck;


	private static List<SceneTemplate> m_templates = new List<SceneTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "SceneTemplate.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
	}

	public static void CurLoad( ref WWW www, string path, Object obj ){
		if (m_templates.Count > 0){
			return;
		}
		
		XmlReader t_reader = null;
		
		if (obj != null)
		{
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
			t_has_items = t_reader.ReadToFollowing("SceneTemplate");
			
			if (!t_has_items)
			{
				break;
			}
			
			SceneTemplate t_template = new SceneTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.sceneId = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.sceneName = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.isCheck = int.Parse(t_reader.Value);
			}
			
			m_templates.Add(t_template);
		}
		while (t_has_items);

		{
			UtilityTool.LoadBox();
		}
	}

	public static string GetScenePath( SceneEnum p_enum ){
		int t_id = (int)p_enum;

		return GetScenePath( t_id );
	}

	public static string GetScenePath( int p_scene_id ){
		for (int i = 0; i < m_templates.Count; i++){
			SceneTemplate t_template = m_templates[i];
			
			if ( t_template.sceneId == p_scene_id ){
				return t_template.sceneName;
			}
		}

//		#if UNITY_EDITOR
//		Debug.LogWarning( "XML ERROR: Can't get SceneTemplate with SceneId: " + p_scene_id );
//		#endif

		return "";
	}
}
