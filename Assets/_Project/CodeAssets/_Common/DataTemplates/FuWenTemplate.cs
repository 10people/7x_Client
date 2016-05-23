using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class FuWenTemplate : XmlLoadManager {

	public int fuwenID;//符文id
	
	public int name;//名字id
	
	public int desc;//描述id
	
	public int type;//道具类型

	public int icon;//图标

	public int color;//边框颜色

	public int fuwenLevel;//符文等级

	public int shuxing;//属性

	public int shuxingValue;//属性值

	public int fuwenFront;//前一级符文

	public int fuwenNext;//后一级符文

	public int shuXingName;//属性名字

	public int inlayColor;//镶嵌对应颜色

    public int exp;

    public int lvlupExp;

	public int levelMax;  

	public int suipianID;

	public int suipianNum;

	public static List<FuWenTemplate> templates = new List<FuWenTemplate>();
    public struct NeedInfo
    {

        public int id;

        public int expId;

        public int level;

        public int needExp;
    }
    public static List<NeedInfo> m_listNeedInfo = new List<NeedInfo>();
    public static List<NeedInfo> m_listReduceInfo = new List<NeedInfo>();
    public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Fuwen.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
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
			t_has_items = t_reader.ReadToFollowing("Fuwen");
			
			if (!t_has_items)
			{
				break;
			}
			
			FuWenTemplate t_template = new FuWenTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.fuwenID = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.name = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.desc = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.color = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.fuwenLevel = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.shuxing = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.shuxingValue = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.fuwenFront = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.fuwenNext = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.shuXingName = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.inlayColor = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.exp = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.lvlupExp = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.levelMax = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.suipianID = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.suipianNum = int.Parse(t_reader.Value);

}
			
			//			t_template.Log();
			
			templates.Add(t_template);
		}
		while (t_has_items);
	}
	
	public static FuWenTemplate GetFuWenTemplateByFuWenId (int fuWenId)
	{
		foreach (FuWenTemplate template in templates)
		{
			if (template.fuwenID == fuWenId)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get FuWenTemplate with fuwenID " + fuWenId);
		
		return null;
	}

    public static FuWenTemplate GetGemExpByIdAndLevel(int fuWenId,int level)
    {
        foreach (FuWenTemplate template in templates)
        {
            if (template.fuwenID == fuWenId && template.fuwenLevel == level)
            {
                return template;
            }
        }
        return null;
    }

    public static FuWenTemplate GetGemByAttrubiteAndLevel(int attrubite, int level)
    {
        foreach (FuWenTemplate template in templates)
        {
            if (template.shuxing == attrubite && template.type == 7 && template.fuwenLevel == level)
            {
                return template;
            }
        }
        return null;
    }

    public static int GetMaxLevelByAddExp(int fuWenId, int expTotal, int levelNow)
    {
        List<FuWenTemplate> listNeedInfo = new List<FuWenTemplate>();
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].shuxing == GetFuWenTemplateByFuWenId(fuWenId).shuxing)
            {
                listNeedInfo.Add(templates[i]);
            }
        }
        int sum = 0;

        for (int i = 0; i < listNeedInfo.Count; i++)
        {
            if (listNeedInfo[i].fuwenLevel >= levelNow)
            {
                sum += listNeedInfo[i].lvlupExp;

                if (sum > expTotal)
                {

                    return listNeedInfo[i].fuwenLevel;
                }
                else if (sum == expTotal && listNeedInfo[i].fuwenLevel < GetFuWenTemplateByFuWenId(fuWenId).levelMax)
                {
                    return listNeedInfo[i].fuwenLevel + 1;
                }
            }

            if (listNeedInfo[i].lvlupExp == -1)
            {
                if (sum < expTotal)
                {
                    return listNeedInfo[i].fuwenLevel;
                }
            }
        }

        return listNeedInfo[listNeedInfo.Count - 1].fuwenLevel;

    }




    public static void GetUpgradeMaxLevel_ByExpidLevel(int fuWenId, int level, int CurrSumExp)
    {
        m_listNeedInfo.Clear();
        List<FuWenTemplate> listNeedInfo = new List<FuWenTemplate>();
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].shuxing == GetFuWenTemplateByFuWenId(fuWenId).shuxing 
                && templates[i].fuwenLevel >= level )
            {
                listNeedInfo.Add(templates[i]);
            }
        }
        int sum = 0;

        int size2 = listNeedInfo.Count;
        for (int j = 0; j < size2; j++)
        {
            NeedInfo ni = new NeedInfo();
            ni.id = listNeedInfo[j].fuwenID;
     
            ni.level = listNeedInfo[j].fuwenLevel;
            ni.needExp = listNeedInfo[j].lvlupExp;
            m_listNeedInfo.Add(ni);
            sum += listNeedInfo[j].lvlupExp;
            if (sum > CurrSumExp || listNeedInfo[j].lvlupExp < 0)
            {
                if (listNeedInfo[j].lvlupExp < 0)
                {
                    Debug.Log("sumsumsumsumsumsumsumsum ::" + sum);
                    sum++;
                }

                break;
            }
        }

        if (sum > 0 && listNeedInfo[size2 - 1].lvlupExp > 0)
        {
            EquipGrowthMaterialUseManagerment.CurrentExpIndex++;
            EquipGrowthMaterialUseManagerment.listCurrentAddExp.Add(EquipGrowthMaterialUseManagerment.CurrentExpIndex
                , sum - listNeedInfo[size2 - 1].lvlupExp);
        }
        else
        {

        }
    }

    public static void GetReduceMaxLevel_ByExpidLevel(int fuWenId, int reduceExp, int levelUnreal, int levelreal)
    {
 
        m_listReduceInfo.Clear();
 
        List<FuWenTemplate> listNeedInfo = new List<FuWenTemplate>();
        for (int i = 0; i < templates.Count; i++)
        {
            if (levelUnreal == levelreal)
            {
                if (templates[i].shuxing == GetFuWenTemplateByFuWenId(fuWenId).shuxing
                    && templates[i].fuwenLevel == levelreal)
                {
                    listNeedInfo.Add(templates[i]);
                }
            }
            else
            {
                if (templates[i].shuxing == GetFuWenTemplateByFuWenId(fuWenId).shuxing && templates[i].fuwenLevel < levelUnreal)
                {
                    listNeedInfo.Add(templates[i]);
                }
            }
        }
        int sum = 0;
        for (int j = listNeedInfo.Count - 1; j >= 0; j--)
        {
            NeedInfo ni = new NeedInfo();
            ni.id = listNeedInfo[j].fuwenID;
            ni.level = listNeedInfo[j].fuwenLevel;
            ni.needExp = listNeedInfo[j].lvlupExp;
            m_listReduceInfo.Add(ni);
            sum += listNeedInfo[j].lvlupExp;
            if (sum > reduceExp)
            {
                break;
            }
        }
    }
}
