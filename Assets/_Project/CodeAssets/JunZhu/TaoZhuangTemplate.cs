using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class TaoZhuangTemplate : XmlLoadManager
{
	public int id;
	
	public int type;

	public int condition;
	
	public int shuxing1;

	public int num1;
	
	public int shuxing2;

	public int num2;
	
	public int shuxing3;

	public int num3;

    public int targetShow;
    public int Maxcondition;

    public static List<TaoZhuangTemplate> templates = new List<TaoZhuangTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "TaoZhuang.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "TaoZhuang" );
			
			if( !t_has_items ){
				break;
			}
			
			TaoZhuangTemplate t_template = new TaoZhuangTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.condition = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.shuxing1 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.num1 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.shuxing2 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.num2 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.shuxing3 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.num3 = int.Parse( t_reader.Value );
 
                t_reader.MoveToNextAttribute();
                t_template.targetShow = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.Maxcondition = int.Parse(t_reader.Value);
                
            }	
			templates.Add( t_template );
		}
		while( t_has_items );
	}

    public static TaoZhuangTemplate GetTaoZhuangById(int id)
    {
        foreach (TaoZhuangTemplate _template in templates)
        {
            if (_template.id == id)
            {
                return _template;
            }
        }

        return null;
    }
    public static TaoZhuangTemplate GetNextTaoZhuangById(int id)
    {
        int size = templates.Count;
        for (int i = 0; i < size; i++)
        {
            if (templates[i].id == id)
            {
                if (i + 1 < size)
                {
                    return templates[i + 1];
                }
            }
        }

        return null;
    }
}
