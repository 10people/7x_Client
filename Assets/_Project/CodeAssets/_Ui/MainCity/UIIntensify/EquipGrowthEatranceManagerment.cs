using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipGrowthEatranceManagerment : MonoBehaviour 
{
	public List<EventIndexHandle> listEvent;
	public GameObject m_SharedParts;
	public UILabel m_PlayerName;
	public UILabel m_Occupation;
	public UILabel m_Des;
    public UIFont titleFont;//标题字体
    public UIFont btn1Font;//按钮1字体
    public UIFont btn2Font;//按钮2字体

	void Start () 
	{
		listEvent.ForEach (p => p.m_Handle += ShowInfo);
        m_PlayerName.text = NameIdTemplate.GetName_By_NameId(990001);
        m_Des.text = DescIdTemplate.GetDescriptionById(990001);
       // m_PlayerName.text = JunZhuData.Instance().m_junzhuInfo.name;
	}


	public void ShowInfo(int index)
	{
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            if (FreshGuide.Instance().IsActive(100050) && TaskData.Instance.m_TaskInfoDic[100050].progress >= 0)
            {
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100050];
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
            else if (FreshGuide.Instance().IsActive(100060) && TaskData.Instance.m_TaskInfoDic[100060].progress >= 0)
            {
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100060];
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }

            else if (FreshGuide.Instance().IsActive(100125) && TaskData.Instance.m_TaskInfoDic[100125].progress >= 0)
            {
                Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100125];
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
            else if (FreshGuide.Instance().IsActive(100170) && TaskData.Instance.m_TaskInfoDic[100170].progress >= 0)
            {
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100170];
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
            else
            {
                CityGlobalData.m_isRightGuide = true;
            }
        }

		
        m_PlayerName.text = NameIdTemplate.GetName_By_NameId(990001);
        m_Des.text = DescIdTemplate.GetDescriptionById(990001);
       // Debug.Log("sdadsasdasddddddddddddddddddd : " + DescIdTemplate.GetDescriptionById(990001));
        if (!FunctionOpenTemp.GetWhetherContainID(1210) && index == 1)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                      UIBoxLoadCallback);
        }
        else
        {
            this.gameObject.SetActive(false);
           m_SharedParts.SetActive(true);
           m_SharedParts.GetComponent<EquipGrowthShowInfoManagerment>().ShowSharedPartsInfo(index);
        }
	}

    public void UIBoxLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.HUANGYE_19);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
 
        string str1 = NameIdTemplate.GetName_By_NameId(990043) + ZhuXianTemp.GeTaskTitleById(FunctionOpenTemp.GetMissionIdById(1210)) + NameIdTemplate.GetName_By_NameId(990044) + "!";;

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null, titleFont, btn1Font);
    }
}
