using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class EffectIdGroup : XmlLoadManager
{
	//<EffectIdGroup Id="1" Group="1000|1010" Count="2" Time="0.8" />

	public int id;

	public List<int> idGroup;

	public int count;

	public float time;


	private int countLow;

	private int countMed;

	private int countHigh;


	private static List<EffectIdGroup> templates = new List<EffectIdGroup>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "EffectIdGroup.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	public static void CurLoad( ref WWW www, string path, Object obj )
	{
		templates.Clear();
		
		XmlReader t_reader = null;
		
		if( obj != null )
		{
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else
		{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "EffectIdGroup" );
			
			if( !t_has_items ) break;
			
			{
				EffectIdGroup template = new EffectIdGroup();
				
				t_reader.MoveToNextAttribute();
				template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				string tempData = t_reader.Value;
				string[] ids = tempData.Split('|');
				template.idGroup = new List<int>();
				for(int i = 0; i < ids.Length; i ++)
				{
					template.idGroup.Add(int.Parse(ids[i]));
				}

				t_reader.MoveToNextAttribute();
				template.countLow = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				template.countMed = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				template.countHigh = int.Parse(t_reader.Value);

				if(QualityTool.Instance.IsLowQuality() == true) template.count = template.countLow;

				else if(QualityTool.Instance.IsMediumQuality() == true) template.count = template.countMed;

				else template.count = template.countHigh;

				t_reader.MoveToNextAttribute();
				template.time = float.Parse(t_reader.Value);
				
				templates.Add( template );
			}
		}
		while( t_has_items );
	}

	public static EffectIdGroup getGroudById(int effectId)
	{
		foreach(EffectIdGroup eig in templates)
		{
			foreach(int id in eig.idGroup)
			{
				if(id == effectId)
				{
					return eig;
				}
			}
		}

		return null;
	}

}
