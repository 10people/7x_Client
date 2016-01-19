using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;

public class BubblePopTemplate : XmlLoadManager 
{
	//<BubblePop levelId="100101" nodeId="101" triggerFunc="1" tp1="210001" tp2="0" tp3="0" textList="1|2|3" />

	public int levelId;

	public int nodeId;

	public int triggerFunc;

	public float tp1;

	public float tp2;

	public float tp3;

	public List<int> listTextId;

	public int triggerNum;

	public int triggerCurNum;


	public static List<BubblePopTemplate> templates;


	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		if(templates == null) templates = new List<BubblePopTemplate>();
		
		else templates.Clear();
		
		UnLoadManager.DownLoad( PathManager.GetUrl(m_LoadPath + "BubblePop.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false  );
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
		
		do{
			t_has_items = t_reader.ReadToFollowing( "BubblePop" );
			
			if( t_has_items == false ) break;
			
			BubblePopTemplate t_template = new BubblePopTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.levelId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.nodeId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.triggerFunc = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.tp1 = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.tp2 = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.tp3 = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.listTextId = new List<int>();
				string textList = t_reader.Value;
				string[] strs = textList.Split('|');
				foreach(string str in strs)
				{
					t_template.listTextId.Add(int.Parse(str));
				}

				t_reader.MoveToNextAttribute();
				t_template.triggerNum = int.Parse( t_reader.Value );
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public BubblePopTemplate getBubblePopByLevelIdAndNodeIdAndTriggerFunc(int _levelId, int _nodeId, int _triggerFunc)
	{
		foreach(BubblePopTemplate template in templates)
		{
			if(template.levelId == _levelId && template.nodeId == _nodeId && template.triggerFunc == _triggerFunc)
			{
				return template;
			}
		}

		return null;
	}

	public static bool haveBubblePopTemplateByLevelIdAndNodeId(int levelId, int nodeId)
	{
		foreach(BubblePopTemplate template in templates)
		{
			if(template.levelId == levelId && template.nodeId == nodeId)
			{
				return true;
			}
		}

		return false;
	}

	public static Dictionary<int, BubblePopTemplate> getBubblePopListByLevelIdAndNodeId(int levelId, int nodeId)
	{
		Dictionary<int, BubblePopTemplate> dict = null;

		foreach(BubblePopTemplate template in templates)
		{
			if(template.levelId == levelId && template.nodeId == nodeId)
			{
				if(dict == null) dict = new Dictionary<int, BubblePopTemplate>();

				if(dict.ContainsKey(template.triggerFunc) == false) 
				{
					dict.Add(template.triggerFunc, template);
				}
				else
				{
					Debug.LogError("XML ERROR: BubblePopTemplate have to much element with levelId " + levelId + " and nodeId " + nodeId + " and triggerFunc " + template.triggerFunc);
				}
			}
		}
		
		return dict;
	}

}
