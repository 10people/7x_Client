using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class SoundIdGroup : XmlLoadManager 
{
	//<RenWu id="100001" name="710001" funDesc="710001" type="1" condition="1" jiangli="9:900001:1000#9:900006:400#10:9101:1" />
	
	
	public struct SoundGroup 
	{
		public List<int> SoundID;
		public int playNum;
		public int id;
		public float pTime;
		public float nextTime;
		public float allNumTime;
	}

	public static List<SoundGroup> m_ListSoundGroup = new List<SoundGroup>();
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "SoundIdGroup.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
		{
			m_ListSoundGroup.Clear();
		}
		
		int iCurID = -1;
		
		XmlReader t_reader = null;
		
		if( obj != null ){
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}

		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "SoundIdGroup" );
			
			if( !t_has_items ){
				break;
			}
			
			{
				SoundGroup tempSoundGroup = new SoundGroup();

				t_reader.MoveToNextAttribute();
				tempSoundGroup.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				string tempData = t_reader.Value;
				string[] ids = tempData.Split('|');
				tempSoundGroup.SoundID = new List<int>();
				for(int i = 0; i < ids.Length; i ++)
				{
					tempSoundGroup.SoundID.Add(int.Parse(ids[i]));
				}
				t_reader.MoveToNextAttribute();
				tempSoundGroup.playNum = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				tempSoundGroup.nextTime = float.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				tempSoundGroup.allNumTime = float.Parse(t_reader.Value);

				m_ListSoundGroup.Add( tempSoundGroup );
			}
		}
		while( t_has_items );
	}

	public static SoundGroup getSoundGroup(int id)
	{
		for(int i = 0; i < m_ListSoundGroup.Count; i ++)
		{
			if(m_ListSoundGroup[i].SoundID.Contains(id))
			{
				return m_ListSoundGroup[i];
			}
		}
		SoundGroup temp = new SoundGroup();
		return temp;
	}
}
