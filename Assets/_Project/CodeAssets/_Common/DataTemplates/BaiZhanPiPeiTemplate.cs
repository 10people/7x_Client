using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class BaiZhanPiPeiTemplate : XmlLoadManager {

//	<BaizhanPipei id="1" roomNum="101" row="1" param1="0" param2="0.25" param3="0.5" param4="0.75" param5="1" />
	public int id;

	public int roomNum;

	public int row;

	public float param1;

	public float param2;

	public float param3;

	public float param4;

	public float param5;

	private static List<BaiZhanPiPeiTemplate> m_templates = new List<BaiZhanPiPeiTemplate>();

	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "BaizhanPipei.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
		
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
		{
			m_templates.Clear();
		}
		
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
			t_has_items = t_reader.ReadToFollowing( "BaizhanPipei" );
			
			if( !t_has_items ){
				break;
			}
			
			BaiZhanPiPeiTemplate t_template = new BaiZhanPiPeiTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.roomNum = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.row = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.param1 = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.param2 = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.param3 = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.param4 = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.param5 = float.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			m_templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static Dictionary<int,List<BaiZhanPiPeiTemplate>> GetBaiZhanPiPeiTemplatesDic ()
	{
		Dictionary<int,List<BaiZhanPiPeiTemplate>> baiZhanPiPeiDic = new Dictionary<int, List<BaiZhanPiPeiTemplate>> ();
		for (int i = 0;i < m_templates.Count;i ++)
		{
			if (!baiZhanPiPeiDic.ContainsKey (m_templates[i].row))
			{
				List<BaiZhanPiPeiTemplate> piPeiTempList = new List<BaiZhanPiPeiTemplate>();
				piPeiTempList.Add (m_templates[i]);
				baiZhanPiPeiDic.Add (m_templates[i].row,piPeiTempList);
			}
			else
			{
				baiZhanPiPeiDic[m_templates[i].row].Add (m_templates[i]);
			}
		}

		return baiZhanPiPeiDic;
	}

	public static BaiZhanPiPeiTemplate GetBaiZhanPiPeiTempByRoomId (int tempRoomId)
	{
		foreach (BaiZhanPiPeiTemplate template in m_templates)
		{
			if (template.roomNum == tempRoomId)
			{
				return template;
			}
		}

		Debug.LogError ("Can not get BaiZhanPiPeiTemplate by roomId!");
		return null;
	}
}
