using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class BattleWinTemplate : XmlLoadManager 
{
	//<BattleWin winType="0" BossNum="0" destinationx="0" destinationy="0" destinationz="0" 
	//destinationRadius="0" />

	public int winId;

	public BattleWinFlag.WinType winType;

	public int killNum;

	public Vector3 destination;

	public float destinationRadius;

	public int showOnUI;

	public int protectNodeId;


	public static List<BattleWinTemplate> templates;


	private static List<int> unpassList = new List<int>();
	

	public static void LoadTemplates( int chapterId, EventDelegate.Callback p_callback = null )
	{
		if(templates == null) templates = new List<BattleWinTemplate>();

		else templates.Clear();

		UnLoadManager.DownLoad( PathManager.GetUrl( "_Data/BattleField/BattleFlags/Win_" + chapterId + ".xml" ), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false  );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj )
	{
		XmlReader t_reader = null;
		
		if( obj != null )
		{
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
		}
		else
		{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}

		bool t_has_items = true;

		unpassList.Clear ();

		do{
			t_has_items = t_reader.ReadToFollowing( "BattleWin" );
			
			if( t_has_items == false ) break;
			
			BattleWinTemplate t_template = new BattleWinTemplate();

			{
				t_reader.MoveToNextAttribute();
				t_template.winId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.winType = (BattleWinFlag.WinType)int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.killNum = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				float x = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				float y = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				float z = float.Parse( t_reader.Value );

				t_template.destination = new Vector3(x, y, z);

				t_reader.MoveToNextAttribute();
				t_template.destinationRadius = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.showOnUI = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.protectNodeId = int.Parse( t_reader.Value );
			}

			bool f = true;

			foreach(int unpass in unpassList)
			{
				if(unpass == t_template.winId)
				{
					f = false;
				}
			}

			if(f == true) unpassList.Add(t_template.winId);

			templates.Add( t_template );
		}
		while( t_has_items );

		refreshDesc ();

		if( m_load_callback != null )
		{
			m_load_callback();
			
			m_load_callback = null;
		}
	}

	private static void refreshDesc()
	{
		if (BattleUIControlor.Instance () == null) return;

		foreach(BattleWinTemplate template in templates)
		{
			if(template.showOnUI != 0)
			{
				BattleUIControlor.Instance().setShowWinDesc(template);

				return;
			}
		}

		BattleUIControlor.Instance().setShowWinDesc(null);
	}

	private static Global.LoadResourceCallback m_load_callback = null;
	
	public static void SetLoadDoneCallback( Global.LoadResourceCallback p_callback )
	{
		m_load_callback = p_callback;
	}

	public static BattleWinTemplate getWinTemplateContainsType(BattleWinFlag.WinType targetWinType)
	{
		foreach(BattleWinTemplate template in templates)
		{
			if(targetWinType == template.winType)
			{
				return template;
			}
		}

		return null;
	}

	//return: true-win beside reachTime, false-notWin
	public static bool reachType(int winId)
	{
		foreach(int unpass in unpassList)
		{
			if(unpass == winId)
			{
				unpassList.Remove(winId);

				break;
			}
		}

		if (unpassList.Count == 0)
		{
			return true;
		}
		else
		{
			foreach(BattleWinTemplate template in templates)
			{
				if(template.winType == BattleWinFlag.WinType.Reach_Time)
				{
					if(unpassList.Count == 1) return true;

					break;
				}
			}
		}

		return false;
	}

}
