using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class EffectIdTemplate : XmlLoadManager 
{
    public int effectId;
    public string path;
    public string sound;
	public float m_fHeight;

	public float ratio;

    public static List<EffectIdTemplate> templates = new List<EffectIdTemplate>();

    public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "EffectId.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
	}

    public static void CurLoad(ref WWW www, string path, Object obj)
    {
        {
            templates.Clear();
        }

        XmlReader t_reader = null;

        if (obj != null)
        {
            TextAsset t_text_asset = obj as TextAsset;

            t_reader = XmlReader.Create(new StringReader(t_text_asset.text));

            //			Debug.Log( "Text: " + t_text_asset.text );
        }
        else
        {
            t_reader = XmlReader.Create(new StringReader(www.text));
        }

        bool t_has_items = true;

        do
        {
            t_has_items = t_reader.ReadToFollowing("EffectTemplate");

            if (!t_has_items)
            {
                break;
            }

            EffectIdTemplate t_template = new EffectIdTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.effectId = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.path = t_reader.Value;


                t_reader.MoveToNextAttribute();
                t_template.sound = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.m_fHeight = float.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.ratio = float.Parse(t_reader.Value);

                templates.Add(t_template);
   
            }
        }
        while (t_has_items);
    }

    public static string GetPathByeffectId(int id)
    {
        //Debug.Log("GetPathByeffectIdGetPathByeffectIdGetPathByeffectIdGetPathByeffectId ::" + templates.Count);
        foreach (EffectIdTemplate template in templates)
        {
            if (template.effectId == id)
            {
                return template.path;
            }
        }

		Debug.LogError( "XML ERROR: Can't get EffectIdTemplate with Id: " + id );

        return null;
    } 

	public static EffectIdTemplate getEffectTemplateByEffectId( int effectId, bool p_will_log_error = true ){
		foreach( EffectIdTemplate template in templates ){
			if( template.effectId == effectId ){
				return template;
			}
		}
			
		if( p_will_log_error ){
			Debug.LogError("XML ERROR: Can't get EffectIdTemplate with effectId " + effectId);
		}
			
		return null;
	}

	public static EffectIdTemplate getEffectTemplateByEffectPath( string p_effect_path, bool p_will_log_error = true ){
		foreach( EffectIdTemplate template in templates ){
			if( template.path == p_effect_path ){
				return template;
			}
		}
		
		if( p_will_log_error ){
			Debug.LogError( "XML ERROR: Can't get EffectIdTemplate with path: " + p_effect_path );
		}
		
		return null;
	}

	public bool HaveSound(){
		return !sound.Equals( "-1" );
	} 

	private static string GetLine( int p_effect_id ){
		EffectIdTemplate t_template = getEffectTemplateByEffectId( p_effect_id );

		if( t_template == null ){
			return "";
		}

		return "<EffectTemplate  effectId=\"" + t_template.effectId + "\"" + 
			" path=\"" + t_template.path + "\"" +
			" sound=\"" + t_template.sound + "\"" + 
			" position=\"" + 0 + "\" />\n";
	}

	public static string ComposeXmlText(){
		string t_text = GetFileHead();

		for( int i = 0; i < templates.Count; i++ ){
			t_text = t_text + GetLine( templates[ i ].effectId );
		}

		t_text = t_text + GetFileEnd();

		return t_text;
	}

	public static float GetHeight(int id)
	{
		if(id == 0)
		{
			return 0;
		}
		return getEffectTemplateByEffectId(id).m_fHeight;
	}
}
