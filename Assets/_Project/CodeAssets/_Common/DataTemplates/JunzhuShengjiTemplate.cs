using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class JunzhuShengjiTemplate : XmlLoadManager 
{

	public int levelId;
	
	public int lv;
    public int gongji;
    public int fangyu;
    public int shengming;
    public int wqSH;
    public int wqJM;
    public int wqBJ;
    public int wqRX;
    public int jnSH;
    public int jnJM;
    public int jnBJ;
    public int jnRX;
    public int addTili;
    public int tilicao;
    public int exp;
    public int xishu;
	 

	public static List<JunzhuShengjiTemplate> templates = new List<JunzhuShengjiTemplate>();

    public struct LevelUpInfo
    {
        public string gongAdd;
        public string fangAdd;
        public string xueAdd;
        public string tiliAdd;
        public int tiliCao;
    };
 
    //public void Log(){
    //    Debug.Log( "JunzhuShengji -  levelId: " + levelId +
    //              " lv: " + lv + 
    //              " tongshui: " + tongshui + 
    //              " wuyi: " + wuyi +
    //              " zhimou: " + zhimou + 
    //              " gongji: " + gongji + 
    //              " fangyu: " + fangyu + 
    //              " shengming: " + shengming + 
    //              " tilicao: " + tilicao + 
    //              " exp: " + exp );
    //}
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "JunzhuShengji.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );

	}

	public static void CurLoad( ref WWW www, string path, Object obj ){
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
			t_has_items = t_reader.ReadToFollowing( "JunzhuShengji" );
			
			if( !t_has_items ){
				break;
			}
			
			JunzhuShengjiTemplate t_template = new JunzhuShengjiTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.levelId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.lv = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.gongji = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.fangyu = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.shengming = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.wqSH = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.wqJM = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.wqBJ = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.wqRX = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.jnSH = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.jnJM = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.jnBJ = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.jnRX = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.addTili = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.tilicao = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.exp = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.xishu = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static JunzhuShengjiTemplate GetJunZhuShengJi( int p_level ){
		for( int i = 0; i < templates.Count; i++ ){
			JunzhuShengjiTemplate t_item = templates[ i ];

			if( t_item.lv == p_level ){
				return t_item;
			}
		}

		Debug.LogError( "JunzhuShengjiTemplate not found: " + p_level );

		return null;
	}

	public static int GetMaxTili( int p_level ){
		for( int i = 0; i < templates.Count; i++ ){
			JunzhuShengjiTemplate t_item = templates[ i ];
			
			if( t_item.lv == p_level ){
				return t_item.tilicao;
			}
		}
		
		Debug.LogError( "JunzhuShengjiTemplate.GetMaxTili( " + p_level + " ) not found: " );
		
		return 0;
	}

    public static int GetNeedTili(int p_level)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].lv == p_level)
            {
                return templates[i].tilicao - templates[i - 1].tilicao;
            }
        }
        return 0;
    }
    public static int GetAddTili(int p_level)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            JunzhuShengjiTemplate t_item = templates[i];

            if (t_item.lv == p_level)
            {
                return t_item.addTili;
            }
        }

        Debug.LogError("JunzhuShengjiTemplate.GetMaxTili( " + p_level + " ) not found: ");

        return 0;
    }
    public static LevelUpInfo GetJunZhuLevelUpInfo(int level)
    {
        LevelUpInfo lvinfo = new LevelUpInfo();
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].lv == level)
            {
                if (i - 1 >= 0)
                {
                    lvinfo.gongAdd = (templates[i].gongji - templates[i - 1].gongji).ToString();
                    lvinfo.fangAdd = (templates[i].fangyu - templates[i - 1].fangyu).ToString();
                    lvinfo.xueAdd = (templates[i].shengming - templates[i - 1].shengming).ToString();
                }
                lvinfo.tiliAdd = templates[i].addTili.ToString();
                lvinfo.tiliCao = templates[i].tilicao;
                return lvinfo;
            }

        }
        return lvinfo;
    }




}
