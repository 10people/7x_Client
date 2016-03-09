using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class SkillTemplate : XmlLoadManager
{
	//<SkillTemplate id="200001" name="600001" desc1="600001" zhiye="1" skillType="1" 
	//value1="180" value2="3.5" value3="0" value4="0" value5="0" value6="0" timePeriod="0" 
	//value7="0" endTime="0" zhudong="1" />

	public int id;

	public int skillName;

	public int funDesc;

	public int zhiye;

	public int skillType;

	public float value1;

	public float value2;

	public float value3;

	public float value4;

	public float value5;

	public float value6;

	public int timePeriod;

	public string value7;

	public float endTime;

	public bool zhudong;

	/// ModelId#Scale#offsetY#Color#Coef#TimeOffset#TimeLen
	/// 3002#2.0#2.0#ff0000#0.74#0.0#10.0
	public string Fx3D;

	public int immediately;

	public static List<SkillTemplate> templates= new List<SkillTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "SkillTemplate.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );

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
			t_has_items = t_reader.ReadToFollowing( "SkillTemplate" );
			
			if( !t_has_items ){
				break;
			}
			
			SkillTemplate t_template = new SkillTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.skillName = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.funDesc = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.zhiye = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.skillType = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.value1 = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.value2 = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.value3 = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.value4 = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.value5 = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.value6 = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.timePeriod = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.value7 = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.endTime = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				int t_zhudong = int.Parse( t_reader.Value );
				t_template.zhudong = t_zhudong == 1;

				t_reader.MoveToNextAttribute();
				t_template.Fx3D = t_reader.Value;

				t_template.immediately = XmlLoadManager.ReadNextInt(t_reader);
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
		for(int i = 0; i < templates.Count; i ++)
		{
			try
			{
				if(templates[i].value7 != "0")
				{
					setFirstLoadEffID(templates[i].value7);
				}
			}
			catch (System.Exception e)
			{
				//Debug.LogError(templates[i].value7);
				//Debug.Log(e.Message);
				//Debug.Log(e);
			}

		}
	}

	public static SkillTemplate getSkillTemplateById(int id)
	{
		foreach(SkillTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get SkillTemplate with id " + id);
		
		return null;
	}

    public static List<SkillTemplate> GetSkillOfJunZhu(int skillType)
    {
        List<SkillTemplate> templateList = new List<SkillTemplate>();

        foreach (SkillTemplate template in templates)
        {
            if (template.zhiye == skillType)
            {
                templateList.Add(template);
            }
        }
        return templateList;
    }

	public static List<int> setFirstLoadEffID(string tempString)
	{
//		Debug.Log (tempString);
		List<int> tempLoadEffID = new List<int>();
		Global.NextCutting(ref tempString);

		int otherSkillNum = int.Parse(Global.NextCutting(ref tempString));
		for (int i = 0; i < otherSkillNum; i ++)
		{
			Global.NextCutting(ref tempString);
		}
		Global.NextCutting(ref tempString);
		otherSkillNum = int.Parse(Global.NextCutting(ref tempString));
		for (int i = 0; i < otherSkillNum; i ++)
		{
			Global.NextCutting(ref tempString);
		}
		otherSkillNum = int.Parse(Global.NextCutting(ref tempString));
		for (int i = 0; i < otherSkillNum; i ++)
		{
			Global.NextCutting(ref tempString);
		}
		Global.NextCutting(ref tempString);

		HeroSkill.USETYPE iUseType = (HeroSkill.USETYPE)int.Parse(Global.NextCutting(ref tempString));//技能触发类型
		
		//		Debug.Log(iUseType);
		//		Debug.Log(tempString);
		
		switch(iUseType)
		{
		case HeroSkill.USETYPE.DISTANCE:
			Global.NextCutting(ref tempString);
			break;
		case HeroSkill.USETYPE.ATK:
			Global.NextCutting(ref tempString);
			break;
		case HeroSkill.USETYPE.BYATK:
			Global.NextCutting(ref tempString);
			break;
		case HeroSkill.USETYPE.SKILLATK:
			Global.NextCutting(ref tempString);
			break;
		case HeroSkill.USETYPE.HP:
			Global.NextCutting(ref tempString);
			Global.NextCutting(ref tempString);
			int tempNum = int.Parse(Global.NextCutting(ref tempString));
			if(tempNum > 0)
			{
				for(int i = 0; i < tempNum; i ++)
				{
					Global.NextCutting(ref tempString);
				}
			}
			break;
		case HeroSkill.USETYPE.POSDIS:
			Global.NextCutting(ref tempString);
			break;
		case HeroSkill.USETYPE.DISNUM:
			Global.NextCutting(ref tempString);
			Global.NextCutting(ref tempString);
			Global.NextCutting(ref tempString);
			break;
		case HeroSkill.USETYPE.DISROUND:
			Global.NextCutting(ref tempString);
			Global.NextCutting(ref tempString);
			break;
		}
		
		
		
		int tempValue = int.Parse(Global.NextCutting(ref tempString));
		for(int i = 0; i < tempValue; i ++)
		{
			Global.NextCutting(ref tempString);
		}
		
		tempValue = int.Parse(Global.NextCutting(ref tempString));
		for(int i = 0; i < tempValue; i ++)
		{
			Global.NextCutting(ref tempString);
		}
		
		//		m_CollStateType = (COLLSTATETYPE)int.Parse(Global.NextCutting(ref tempString));//作用范围类型

		tempValue = int.Parse(Global.NextCutting(ref tempString));
		for(int i = 0; i < tempValue; i ++)
		{
			Global.NextCutting(ref tempString);
			Global.NextCutting(ref tempString);
			Global.NextCutting(ref tempString);
		}

		//作用范围
		switch((HeroSkill.COLLSTATETYPE)int.Parse(Global.NextCutting(ref tempString)))
		{
		case HeroSkill.COLLSTATETYPE.ROUND:
			Global.NextCutting(ref tempString);
			break;
		case HeroSkill.COLLSTATETYPE.ANGLE:
			Global.NextCutting(ref tempString);
			Global.NextCutting(ref tempString);
			break;
		case HeroSkill.COLLSTATETYPE.VECTOR:
			Global.NextCutting(ref tempString);
			Global.NextCutting(ref tempString);
			Global.NextCutting(ref tempString);
			Global.NextCutting(ref tempString);
			Global.NextCutting(ref tempString);
			Global.NextCutting(ref tempString);
			break;
		}
		Global.NextCutting (ref tempString);
//		;
		Global.NextCutting(ref tempString);
		Global.NextCutting(ref tempString);
		
		
		
		//		m_ATTTYPE = (ATTTYPE);//技能播放目标单体群体
		//目标
		switch((HeroSkill.ATTTYPE)int.Parse(Global.NextCutting(ref tempString)))
		{
		case HeroSkill.ATTTYPE.ONE:
			Global.NextCutting(ref tempString);
			break;
		case HeroSkill.ATTTYPE.SOME:
			Global.NextCutting(ref tempString);
			//			Debug.Log("m_iTargetValue0="+m_iTargetValue0);
			break;
		case HeroSkill.ATTTYPE.RANDOM:
			Global.NextCutting(ref tempString);
			Global.NextCutting(ref tempString);
			break;
		}
		tempLoadEffID.Add(int.Parse(Global.NextCutting(ref tempString)));
		tempLoadEffID.Add(int.Parse(Global.NextCutting(ref tempString)));
		
		Global.NextCutting(ref tempString);
		Global.NextCutting(ref tempString);
		tempLoadEffID.Add(int.Parse(Global.NextCutting(ref tempString)));
		Global.NextCutting(ref tempString);
		tempLoadEffID.Add(int.Parse(Global.NextCutting(ref tempString)));
		Global.NextCutting(ref tempString);
		Global.NextCutting(ref tempString);
		Global.NextCutting(ref tempString);
		Global.NextCutting(ref tempString);
		//		m_fEffEndTime = float.Parse(Global.NextCutting(ref temptemptempString));
		//		Debug.Log("temptemptempString="+temptemptempString);
		tempValue = int.Parse(Global.NextCutting(ref tempString));
//				Debug.Log(tempValue);
		for(int i = 0; i < tempValue; i ++)
		{
			//			Debug.Log(temptemptempString);
			HeroSkill.SKILLELEMENTTYPE tempSKILLELEMENTTYPE = (HeroSkill.SKILLELEMENTTYPE)int.Parse(Global.NextCutting(ref tempString));
			//			Debug.Log(tempSKILLELEMENTTYPE);
			//			m_look.Add(tempSKILLELEMENTTYPE);
			SkillDataBead tempSkillDataBead;
			switch(tempSKILLELEMENTTYPE)
			{
			case HeroSkill.SKILLELEMENTTYPE.LOSTHP:
				tempSkillDataBead = new SkillShanghai(null);
				break;
			case HeroSkill.SKILLELEMENTTYPE.BUFF:
				tempSkillDataBead = new SkillBuff(null);
				break;
			case HeroSkill.SKILLELEMENTTYPE.SUMMON:
				tempSkillDataBead = new SkillSummon(null);
				break;
			case HeroSkill.SKILLELEMENTTYPE.COPY:
				tempSkillDataBead = new SkillCopy(null);
				break;
			case HeroSkill.SKILLELEMENTTYPE.RESURRECTION:
				tempSkillDataBead = new SkillResurrection(null);
				break;
			case HeroSkill.SKILLELEMENTTYPE.BACK:
				tempSkillDataBead = new SkillBack(null);
				break;
			case HeroSkill.SKILLELEMENTTYPE.ADDHP:
				tempSkillDataBead = new SkillAddHP(null);
				break;
			case HeroSkill.SKILLELEMENTTYPE.FALL:
				tempSkillDataBead = new SkillFall(null);
				break;
			case HeroSkill.SKILLELEMENTTYPE.BYATK:
				tempSkillDataBead = new SkillByAtk(null);
				break;
			case HeroSkill.SKILLELEMENTTYPE.DELETEBUFF:
				tempSkillDataBead = new SkillDeleteBuff(null);
				break;
			case HeroSkill.SKILLELEMENTTYPE.YINLI:
				tempSkillDataBead = new SkillYinli(null);
				break;
			default:
				tempSkillDataBead = new SkillSummon(null);
				break;
			}
			tempSkillDataBead.setData(ref tempString);
			if(tempSKILLELEMENTTYPE == HeroSkill.SKILLELEMENTTYPE.BUFF)
			{
				SkillBuff tempBuff = tempSkillDataBead as SkillBuff;
				tempLoadEffID.Add(tempBuff.m_iEffID);
			}
		}
		return tempLoadEffID;
	}

	public static SkillTemplate getSkillTemplateByJiNengPeiYangId(int jiNengPeiYangId)
	{
		HeroSkillUpTemplate jinengpeiyangTemplate = HeroSkillUpTemplate.GetHeroSkillUpByID (jiNengPeiYangId);

		return getSkillTemplateById (jinengpeiyangTemplate.m_iSkillID);
	}

	public static SkillTemplate getSkillTemplateBySkillLevelIndex(CityGlobalData.skillLevelId skillLevelIndex, KingControllor kingNode)
	{
		return getSkillTemplateByJiNengPeiYangId(kingNode.skillLevel [(int)skillLevelIndex]);
	}

	public static int getSkillLevelByJiNengPeiYangId(int jiNengPeiYangId)
	{
		HeroSkillUpTemplate jinengpeiyangTemplate = HeroSkillUpTemplate.GetHeroSkillUpByID (jiNengPeiYangId);

		return jinengpeiyangTemplate.m_iQuality;
	}

	public static int getSkillLevelBySkillLevelIndex(CityGlobalData.skillLevelId skillLevelIndex, KingControllor kingNode)
	{
		return getSkillLevelByJiNengPeiYangId (kingNode.skillLevel [(int)skillLevelIndex]);
	}


	#region Fx3D

	public bool HaveFx3D(){
		return Fx3D.Length > 0;
	}

	#endregion
}
