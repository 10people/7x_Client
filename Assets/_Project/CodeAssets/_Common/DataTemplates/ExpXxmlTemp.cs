using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class ExpXxmlTemp : XmlLoadManager {
	
	
	//<AwardTemp id="800001" awardId="1" itemId="111001" itemType="0" itemNum="1" weight="100" />
	
	public int id;
	
	public int expId;
	
	public int level;
	
	public int needExp;
	
    public struct NeedInfo
    {

        public int id;

        public int expId;

        public int level;

        public int needExp;
    }
    public static List<NeedInfo> m_listNeedInfo = new List<NeedInfo>();
    public static List<NeedInfo> m_listReduceInfo = new List<NeedInfo>();
	
	private static List<ExpXxmlTemp> templates = new List<ExpXxmlTemp>();
	
	
	public void Log(){
		Debug.Log( "ExpXxmlTemp.Log( id: " + id +
		          " awardId : " );
	}
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "ExpTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	private static TextAsset m_templates_text = null;
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
		if ( obj == null) {
			Debug.LogError ("Asset Not Exist: " + path );
			
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
			t_has_items = t_reader.ReadToFollowing( "ExpTemp" );
			
			if( !t_has_items ){
				break;
			}
			
			ExpXxmlTemp t_template = new ExpXxmlTemp();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.expId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.level = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.needExp = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );

		{
			m_templates_text = null;
		}
	}
	
	public static ExpXxmlTemp getExpXxmlTempById(int id){
		{
			ProcessAsset();
		}

		foreach( ExpXxmlTemp template in templates )
		{
			if( template.id == id )
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get ExpXxmlTemp with id " + id);
		
		return null;
	}
	
	public static ExpXxmlTemp getExpXxmlTemp_By_expId( int expId_id,int lv ){
		{
			ProcessAsset();
		}

		foreach( ExpXxmlTemp template in templates )
		{
			if( template.expId == expId_id &&template.level == lv)
			{
				return template;
			}
		}
		
		Debug.LogError( "ExpXxmlTemp not found with award id+ lv: " + expId_id +lv);
		
		return null;
	}

    //public static int GetUpgradeExpNeed_ByExpidLevel(int expId,int level)
    //{
    //    for (int i = 0; i < templates.Count; i++)
    //    {
    //        if (templates[i].id == expId && templates[i].level == level)
    //        {
    //            return templates[i].needExp;
    //        }
    //    }
    //    return 0;
    //}

    public static void GetUpgradeMaxLevel_ByExpidLevel(int expId, int level, int CurrSumExp, int MaxLevel){
		{
			ProcessAsset();
		}

        m_listNeedInfo.Clear();
        //Debug.Log("expIdexpIdexpIdexpIdexpIdexpId ::" + expId);
        //Debug.Log("MaxLevelMaxLevelMaxLevelMaxLevel ::" + MaxLevel);
        //Debug.Log("CurrSumExpCurrSumExpCurrSumExp :: " + CurrSumExp);
        List<ExpXxmlTemp> listNeedInfo = new List<ExpXxmlTemp>();
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].expId == expId && templates[i].level >= level /*&& templates[i].level <= JunZhuData.Instance().m_junzhuInfo.level */&& templates[i].level <= MaxLevel)
            {
                listNeedInfo.Add(templates[i]);

              //  Debug.Log("templates[i].leveltemplates[i].level :: " + templates[i].level);
            } 
        }
        int sum = 0;

        int size2 = listNeedInfo.Count;
        for (int j = 0; j < size2; j++)
        {
            NeedInfo ni = new NeedInfo();
            ni.id = listNeedInfo[j].id;
            ni.expId = listNeedInfo[j].expId;
            ni.level = listNeedInfo[j].level;
            ni.needExp = listNeedInfo[j].needExp;
            m_listNeedInfo.Add(ni);
            sum += listNeedInfo[j].needExp;
            if (sum > CurrSumExp || listNeedInfo[j].needExp < 0)
            {
              //  Debug.Log("listNeedInfo[i].levellistNeedInfo[i].level :: " + listNeedInfo[j].level);
                if (listNeedInfo[j].needExp < 0)
                {
                    sum++;
                }
         
                break ;
            }
        }

        if (sum > 0 && listNeedInfo[size2 - 1].needExp > 0)
        {
            EquipGrowthMaterialUseManagerment.CurrentExpIndex++;
            EquipGrowthMaterialUseManagerment.listCurrentAddExp.Add(EquipGrowthMaterialUseManagerment.CurrentExpIndex, sum - listNeedInfo[size2 - 1].needExp);
        }
        else
        { 
        
        }
    }

    public static void GetReduceMaxLevel_ByExpidLevel(int expId, int reduceExp, int levelUnreal, int levelreal){
		{
			ProcessAsset();
		}

        m_listReduceInfo.Clear();
       // Debug.Log("levelUnreallevelUnreallevelUnreal :::" + levelUnreal);

        List<ExpXxmlTemp> listNeedInfo = new List<ExpXxmlTemp>();
        for (int i = 0;i < templates.Count; i++)
        {
            if (levelUnreal == levelreal)
            {
                if (templates[i].expId == expId && templates[i].level == levelreal)
                {
                    listNeedInfo.Add(templates[i]);
                }
            }
            else
            {
                if (templates[i].expId == expId && templates[i].level < levelUnreal)
                {
                    listNeedInfo.Add(templates[i]);
                }
            }
        }
        int sum = 0;
        for (int j = listNeedInfo.Count - 1; j >= 0; j--)
        {
            NeedInfo ni = new NeedInfo();
            ni.id = listNeedInfo[j].id;
            ni.expId = listNeedInfo[j].expId;
            ni.level = listNeedInfo[j].level;
            ni.needExp = listNeedInfo[j].needExp;
            m_listReduceInfo.Add(ni);
            sum += listNeedInfo[j].needExp;
            if (sum > reduceExp)
            {
                break;
            }
        }
       // Debug.Log("sumsumsumsumsumsumsumsum:::" + sum);
     
    }

    public static int GetMaxLevelByAddExp(int expId ,int expTotal,int levelNow){
		{
			ProcessAsset();
		}

     //Debug.Log("expTotalexpTotalexpTotalexpTotal :::" + expTotal);
        List<ExpXxmlTemp> listNeedInfo = new List<ExpXxmlTemp>();
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].expId == expId)
            {
                listNeedInfo.Add(templates[i]);
            }
        }
        int sum = 0;
        for (int i = 0; i < listNeedInfo.Count; i++)
        {
            if (listNeedInfo[i].level >= levelNow)
            {
                sum += listNeedInfo[i].needExp;

                if (sum > expTotal)
                {

                    return listNeedInfo[i].level;
                }
                else if (sum == expTotal && listNeedInfo[i].level < 100)
                {
                    return listNeedInfo[i].level + 1;
                }
            }
        
            if (listNeedInfo[i].needExp == -1)
            {
                if (sum < expTotal)
                {
                    return listNeedInfo[i].level;
                }
            }
        }

        return listNeedInfo[listNeedInfo.Count - 1].level;
        
    }

  //  public static int GetNeedMaxExpByExpId(int expId){
		//{
		//	ProcessAsset();
		//}

  //      List<ExpXxmlTemp> listNeedInfo = new List<ExpXxmlTemp>();
  //      int sum = 0;
  //      for (int i = 0; i < templates.Count; i++)
  //      {
  //          if (JunZhuData.Instance().m_junzhuInfo.level < 100)
  //          {
  //              if (templates[i].expId == expId && templates[i].level <= JunZhuData.Instance().m_junzhuInfo.level)
  //              {
  //                  sum += templates[i].needExp; 
  //              }
  //          }
  //          else
  //          {
  //              if (templates[i].expId == expId && templates[i].level < 100)
  //              {
  //                  sum += templates[i].needExp;
  //              }
  //          }
 
  //      }
  //      return sum;

  //  }
    public static int GetCurrentRealAdd(int expId, int Exptotal, int Expnow){
		{
			ProcessAsset();
		}

        List<ExpXxmlTemp> listNeedInfo = new List<ExpXxmlTemp>();
        int sum = 0;
        for (int i = 0; i < templates.Count; i++)
        {
            if (JunZhuData.Instance().m_junzhuInfo.level < 100)
            {
                if (templates[i].expId == expId /*&& templates[i].level < JunZhuData.Instance().m_junzhuInfo.level*/)
                {
                    sum += templates[i].needExp;
                }
            }
            else
            {
                if (templates[i].expId == expId && templates[i].level < 100)
                {
                    sum += templates[i].needExp;
                }
            }

        }
        if (Expnow >= sum)
        { 
            return sum;
        }
        else
        {
            return Expnow - (Exptotal - sum);
        }
    }
}
