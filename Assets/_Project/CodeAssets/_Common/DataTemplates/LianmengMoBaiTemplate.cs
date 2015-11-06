using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using qxmobile;
using qxmobile.protobuf;
public class LianmengMoBaiTemplate : XmlLoadManager
{

    public int type;
    public int nameID;
    public string desc;
    public int needNum;
    public string award;
    public int tili;
    public int gongxian;
    public int buffNum;
    public string awardShow;
	public static List<LianmengMoBaiTemplate> templates = new List<LianmengMoBaiTemplate>();


    public static void LoadTemplates( EventDelegate.Callback p_callback = null )
    {
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LianmengMobai.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
    }

    public static void CurLoad(ref WWW www, string path, Object obj){
		{
			templates.Clear();
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
			t_has_items = t_reader.ReadToFollowing( "LianmengMobai" );
			
			if( !t_has_items ){
				break;
			}
			
			LianmengMoBaiTemplate t_template = new LianmengMoBaiTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.nameID = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.desc = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.needNum = int.Parse( t_reader.Value );


				t_reader.MoveToNextAttribute();
				t_template.award = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.tili = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.gongxian = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.buffNum = int.Parse( t_reader.Value );

                t_reader.MoveToNextAttribute();
                t_template.awardShow = t_reader.Value;
                
            }
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
    }

    public static LianmengMoBaiTemplate GetShowInfoByType(int type)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].type == type)
            {
                return templates[i];
            }
        }
        return null;
      
    }
}
