using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JunZhuDiaoLuoItem : MonoBehaviour 
{
	public UILabel m_LabelName;
    public UILabel m_LabelChuanQi;
    public GameObject m_Mask;
 
      public delegate void OnClick_Touch(int id);
      OnClick_Touch touch_CallBack;
	public EventHandler m_TouchEvent;
    private bool _isChuangQi = false;
 
	private int sectionSend = 0;
    private int _levelid = 0;
	//public GameObject m_Map;
   // private int[] sectionNameID = {1, 30001,30002,30003,30004,30005,30006,30007,30008,30009,30010,30011,30012,30013,30014,30015};
	void Start () 
	{
	   m_TouchEvent.m_click_handler += SkipScence;
	}

    public void ShowInfo(JunZhuZhuangBeiInfo.DiaoLuoGuanQia diaoluoguanqia, OnClick_Touch callback)
    {
        _levelid = diaoluoguanqia._id;
     //   Debug.Log("sectionsectionsectionsection ::" + section);
        touch_CallBack = callback;
        sectionSend = diaoluoguanqia._SetionId;
  
     //   string []  ss = NameIdTemplate.GetName_By_NameId(sectionNameID[int.Parse(section)]).Split(' ');
        m_LabelName.text = diaoluoguanqia.GuanQiaName;//;ss[0];
        m_LabelChuanQi.gameObject.SetActive(diaoluoguanqia._isChuanQi);
        _isChuangQi = diaoluoguanqia._isChuanQi;
        m_Mask.SetActive(!diaoluoguanqia._isOpen);
    }
	GameObject DiaoluoShow;
	private void SkipScence(GameObject obj)
    {
		if (DeviceHelper.IsSingleTouching() )
        {
            if (_isChuangQi)
            {
                //判断当前攻打的是普通关卡还是传奇关卡 True为普通 false为传奇
                CityGlobalData.PT_Or_CQ = false;

            }
            else
            {
                //判断当前攻打的是普通关卡还是传奇关卡 True为普通 false为传奇
                CityGlobalData.PT_Or_CQ = true;
            }
            EnterGuoGuanmap.Instance().ShouldOpen_id = _levelid;

            touch_CallBack(_levelid);
            EnterGuoGuanmap.EnterPveUI(sectionSend);
        }
	}
}
