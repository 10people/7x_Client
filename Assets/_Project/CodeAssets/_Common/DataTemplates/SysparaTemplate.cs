using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using qxmobile;
using qxmobile.protobuf;
public class SysparaTemplate : XmlLoadManager 
{
    public string word;
	private static List<SysparaTemplate> templates = new List<SysparaTemplate>();
    
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "syspara.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
    }

	private static TextAsset m_templates_text = null;

    public static void CurLoad(ref WWW www, string path, Object obj){
		if (obj == null) {
			Debug.LogError ("Asset Not Exist: " + path);
			
			return;
		}
		
		m_templates_text = obj as TextAsset;
	}

	private static void ProcessAsset(){
		if( templates.Count > 0 ) {
			return;
		}
		
		if( m_templates_text == null ) {
			Debug.LogError( "Error, Asset Not Exist." );
			
			return;
		}
		
		XmlReader t_reader = XmlReader.Create( new StringReader( m_templates_text.text ) );

		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "SensitiveWord" );
			
			if( !t_has_items ){
				break;
			}
			
			SysparaTemplate t_template = new SysparaTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.word = t_reader.Value;
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );

		{
			m_templates_text = null;
		}
    }

    public static bool CompareSyeParaWord(string word){
		{
			ProcessAsset();
		}

        foreach (SysparaTemplate tem in templates)
        {
            if (tem.word.Equals(word))
            {
                
                return true;
            }
        }

        return false;
    }
}
