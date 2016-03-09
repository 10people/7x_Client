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

	public bool isWin;

	public BattleWinFlag.EndType winType;

	public int killNum;

	public Vector3 destination;

	public float destinationRadius;

	public int activeNum;
	
	public List<int> activeList = new List<int> ();

	public int showOnUI;

	public int protectNum;

	public List<int> protectList = new List<int>();


	public static List<BattleWinTemplate> templates;


	private static List<int> unwinList = new List<int>();

	private static List<int> unloseList = new List<int>();


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

		unwinList.Clear ();

		unloseList.Clear ();

		do{
			t_has_items = t_reader.ReadToFollowing( "BattleWin" );
			
			if( t_has_items == false ) break;
			
			BattleWinTemplate t_template = new BattleWinTemplate();

			{
				t_reader.MoveToNextAttribute();
				t_template.winId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.isWin = int.Parse( t_reader.Value ) == 1;

				t_reader.MoveToNextAttribute();
				t_template.winType = (BattleWinFlag.EndType)int.Parse( t_reader.Value );

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
				t_template.activeNum = int.Parse( t_reader.Value );

				//activeList
				{
					t_reader.MoveToNextAttribute();
					string strActiveList = t_reader.Value;
					
					string[] sActiveList = strActiveList.Split(',');
					
					t_template.activeList = new List<int>();
					
					foreach(string st in sActiveList)
					{
						int i = int.Parse(st);

						if(i != 0) t_template.activeList.Add(i);
					}
				}

				t_reader.MoveToNextAttribute();
				t_template.showOnUI = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.protectNum = int.Parse( t_reader.Value );

				//protectList
				{
					t_reader.MoveToNextAttribute();
					string strProtectist = t_reader.Value;
					
					string[] sProtectist = strProtectist.Split(',');
					
					t_template.protectList = new List<int>();
					
					foreach(string st in sProtectist)
					{
						int i = int.Parse(st);

						if(i != 0) t_template.protectList.Add(i);
					}
				}
			}

			bool f = true;

			if(t_template.isWin == true)
			{
				foreach(int unwin in unwinList)
				{
					if(unwin == t_template.winId)
					{
						f = false;
					}
				}

				if(f == true) unwinList.Add(t_template.winId);
			}
			else
			{
				foreach(int unlose in unloseList)
				{
					if(unlose == t_template.winId)
					{
						f = false;
					}
				}
				
				if(f == true) unloseList.Add(t_template.winId);
			}

			templates.Add( t_template );
		}
		while( t_has_items );

		refreshDesc ();

		refreshDestinationEffect ();

		if( m_load_callback != null )
		{
			m_load_callback();
			
			m_load_callback = null;
		}
	}

	private static void refreshDesc()
	{
		if (BattleUIControlor.Instance() == null) return;

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

	private static void refreshDestinationEffect()
	{
		if (BattleUIControlor.Instance() == null) return;

		foreach(BattleWinTemplate template in templates)
		{
			if(template.winType == BattleWinFlag.EndType.Reach_Destination)
			{
				BattleEffectControllor.Instance().LoadEffectByEffectId(600157, loadDestinationEffectLoadCallback);

				return;
			}
		}
	}

	private static void loadDestinationEffectLoadCallback()
	{
		foreach(BattleWinTemplate template in templates)
		{
			if(template.winType == BattleWinFlag.EndType.Reach_Destination)
			{
				GameObject gc = BattleEffectControllor.Instance().PlayEffect(600157, template.destination, new Vector3(1, 0, 0), 9999);

				gc.transform.parent = BattleControlor.Instance().transform;
			}
		}
	}

	private static Global.LoadResourceCallback m_load_callback = null;
	
	public static void SetLoadDoneCallback( Global.LoadResourceCallback p_callback )
	{
		m_load_callback = p_callback;
	}

	public static BattleWinTemplate getWinTemplateContainsType(BattleWinFlag.EndType targetWinType, bool isWin)
	{
		foreach(BattleWinTemplate template in templates)
		{
			if(targetWinType == template.winType && isWin == template.isWin)
			{
				return template;
			}
		}

		return null;
	}

	//return: true-win beside reachTime, false-notWin
	public static bool reachTypeWin(int winId)
	{
		foreach(int unwin in unwinList)
		{
			if(unwin == winId)
			{
				unwinList.Remove(winId);

				break;
			}
		}

		if (unwinList.Count == 0)
		{
			return true;
		}
		else
		{
			foreach(BattleWinTemplate template in templates)
			{
				if(template.isWin == true && template.winType == BattleWinFlag.EndType.Reach_Time)
				{
					if(unwinList.Count == 1) return true;

					break;
				}
			}
		}

		return false;
	}

	public static bool reachTypeLose(int loseId)
	{
		foreach(int unlose in unloseList)
		{
			if(unlose == loseId)
			{
				unloseList.Remove(loseId);
				
				break;
			}
		}
		
		if (unloseList.Count == 0)
		{
			return true;
		}
		else
		{
			foreach(BattleWinTemplate template in templates)
			{
				if(template.isWin == false && template.winType == BattleWinFlag.EndType.Reach_Time)
				{
					if(unloseList.Count == 1) return true;
					
					break;
				}
			}
		}
		
		return false;
	}

	public static BattleWinTemplate getNextWinTemplate()
	{
		BattleWinTemplate template = null;

		int endId = -9999;

		foreach(BattleWinTemplate _temp in templates)
		{
			if(_temp.winId > endId)
			{
				endId = _temp.winId;

				template = _temp;
			}
		}

		return template;
	}

}
