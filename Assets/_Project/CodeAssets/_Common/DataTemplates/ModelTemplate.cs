using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class ModelTemplate : XmlLoadManager 
{
	//<ModelTemplate  modelId="1001" path="_3D/Fx/Prefabs/BattleField/Player_0_ceshi" effects="0" />

	public int modelId;

	public string path;

	// origin text
	public string effect;

	public List<int> effects;

	// orgin text
	public string sound;

	public List<int> sounds;

	public float radius;

	public float height;

	public int icon;


	public static List<ModelTemplate> m_templates = new List<ModelTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "ModelId.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "ModelTemplate" );
			
			if( !t_has_items ){
				break;
			}
			
			ModelTemplate t_template = new ModelTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.modelId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.path = t_reader.Value;

				// effects
				{
					t_template.effects = new List<int>();

					t_reader.MoveToNextAttribute();

					t_template.effect = t_reader.Value;
					
					string[] strs = t_template.effect.Split(',');
					
					foreach(string s in strs){
						t_template.effects.Add(int.Parse(s));
					}
				}
				
				// sounds
				{
					t_template.sounds = new List<int>();

					t_reader.MoveToNextAttribute();

					t_template.sound = t_reader.Value;
					
					if( !string.IsNullOrEmpty( t_template.sound ) )
					{
						string[] t_items = t_template.sound.Split( '|' );
						
						foreach( string t_sound in t_items )
						{
							t_template.sounds.Add( int.Parse( t_sound ) );
						}
					}
				}

				t_reader.MoveToNextAttribute();
				t_template.radius = float.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.height = float.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse(t_reader.Value);
			}
			
			//			t_template.Log();
			
			m_templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static ModelTemplate getModelTemplateByModelId( int p_model_id, bool p_will_log_error = true ){
		foreach( ModelTemplate t_template in m_templates ){
			if(t_template.modelId == p_model_id){
				return t_template;
			}
		}

		if( p_will_log_error ){
			Debug.LogError("XML ERROR: Can't get ModelTemplate with modelId " + p_model_id);
		}
		
		return null;
	}

	public static string GetResPathByModelId( int p_model_id ){
		foreach( ModelTemplate t_template in m_templates ){
			if( t_template.modelId == p_model_id ){
				return t_template.path;
			}
		}
		
		Debug.LogError( "XML ERROR: Can't get res path with modelId " + p_model_id + "   " + m_templates.Count );
		
		return "";
	}

    public static int GetModelIdByPath(string path)
    {
        foreach (ModelTemplate t_template in m_templates)
        {
            if (t_template.path == path)
            {
                return t_template.modelId;
            }
        }
        return 0;
    }

}
