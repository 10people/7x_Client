using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ObjectName : MonoBehaviour {
    public GameObject m_ObjController;
	public UILabel m_playerName;
    public UILabel m_LabAllianceName;
    public UISprite m_SpriteChengHao;
    public UILabel m_playerVip;
    public UISprite m_SpriteVip;
    public GameObject m_ObjVip;
    public UISprite m_SpriteZH;
    [HideInInspector]
    public string m_NameSend = "";
    protected Transform m_transform;
    public bool m_isShowChengHao = false;
    private GameObject _ObjEffect = null;
    public string _Designation = null;

	void Start()
	{
		m_transform = this.GetComponent<Transform>();
	}
    public void Init_Maincity_PlayerHeadInfo(PlayersManager.OtherPlayerInfo u_info)//初始化玩家名字
    {
        m_NameSend = u_info._Name;
        if (!string.IsNullOrEmpty(u_info._AllianceName)  && !u_info._AllianceName.Equals("***"))
        {
            m_LabAllianceName.text = MyColorData.getColorString(12, "<" + u_info._AllianceName + ">  " + FunctionWindowsCreateManagerment.GetIdentityById(u_info._Duty));
        }
        else
        {
            m_LabAllianceName.text = MyColorData.getColorString(12, "<" +LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT)+">");
        }

        if (!string.IsNullOrEmpty(u_info._Designation) && !u_info._Designation.Equals("-1"))
        {
            m_isShowChengHao = true;
            m_SpriteChengHao.gameObject.SetActive(true);
            m_SpriteChengHao.spriteName = u_info._Designation;

            if (!string.IsNullOrEmpty(_Designation)
                )
            {
                CreateEffect(int.Parse(u_info._Designation));
            }
            else if (!_Designation.Equals(u_info._Designation))
            {
                CreateEffect(int.Parse(u_info._Designation));
            }

            if (string.IsNullOrEmpty(_Designation))
            {
                _Designation = u_info._Designation;
            }
        }
        else
        {
            //if (_ObjEffect)
            //{
            //    Destroy(_ObjEffect);
            //}
            m_SpriteChengHao.gameObject.SetActive(false);
        }

        if (!string.IsNullOrEmpty(u_info._VInfo))
        {
            if (int.Parse(u_info._VInfo) > 0)
            {
                m_ObjVip.transform.localPosition = new Vector3(-55 - (FunctionWindowsCreateManagerment.DistanceCount(u_info._Name) - 1) *13, -11, 0);
                m_ObjVip.SetActive(true);
                m_SpriteVip.spriteName = "v" + u_info._VInfo;
                m_playerName.text = MyColorData.getColorString(9, "[b]" + u_info._Name + "[/b]");
            }
            else
            {
                m_ObjVip.gameObject.SetActive(false);
                m_playerName.text = MyColorData.getColorString(9, "[b]" + u_info._Name + "[/b]");
            }
        }
        else
        {
            m_ObjVip.gameObject.SetActive(false);
            m_playerName.text = MyColorData.getColorString(9, "[b]" + u_info._Name + "[/b]");
        }

        ShowOrHide(true);
    }

	#region TreasureCity
	private bool isPlayer = true;
	public bool IsPlayer { set{isPlayer = value;} get{return isPlayer;} }

	public void InItTreasureCityName (PlayersManager.OtherPlayerInfo u_info)//初始化十连副本玩家名字
	{
		m_NameSend = u_info._Name;
		if (!string.IsNullOrEmpty(u_info._AllianceName)  && !u_info._AllianceName.Equals("***"))
		{
			m_LabAllianceName.text = MyColorData.getColorString(12, "<" + u_info._AllianceName + ">  " + FunctionWindowsCreateManagerment.GetIdentityById(u_info._Duty));
		}
		else
		{
			m_LabAllianceName.text = MyColorData.getColorString(12, "<" +LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT)+">");
		}
		
		if (!string.IsNullOrEmpty(u_info._Designation) && !u_info._Designation.Equals("-1"))
		{
			m_isShowChengHao = true;
			m_SpriteChengHao.gameObject.SetActive(true);
			m_SpriteChengHao.spriteName = u_info._Designation;
			
			if (!string.IsNullOrEmpty(_Designation)
			    )
			{
				CreateEffect(int.Parse(u_info._Designation));
			}
			else if (!_Designation.Equals(u_info._Designation))
			{
				CreateEffect(int.Parse(u_info._Designation));
			}
			
			if (string.IsNullOrEmpty(_Designation))
			{
				_Designation = u_info._Designation;
			}
		}
		else
		{
			//if (_ObjEffect)
			//{
			//    Destroy(_ObjEffect);
			//}
			m_SpriteChengHao.gameObject.SetActive(false);
		}
		
		if (!string.IsNullOrEmpty(u_info._VInfo))
		{
			if (int.Parse(u_info._VInfo) > 0)
			{
				m_ObjVip.transform.localPosition = new Vector3(-55 - (FunctionWindowsCreateManagerment.DistanceCount(u_info._Name) - 1) *13, -11, 0);
				m_ObjVip.SetActive(true);
				m_SpriteVip.spriteName = "v" + u_info._VInfo;
				m_playerName.text = MyColorData.getColorString(9, "[b]" + u_info._Name + "[/b]");
			}
			else
			{
				m_ObjVip.gameObject.SetActive(false);
				m_playerName.text = MyColorData.getColorString(9, "[b]" + u_info._Name + "[/b]");
			}
		}
		else
		{
			m_ObjVip.gameObject.SetActive(false);
			m_playerName.text = MyColorData.getColorString(9, "[b]" + u_info._Name + "[/b]");
		}
		
		ShowOrHide(true);
	}
	#endregion

    public void UpdateZH(string info)
    {
        if (m_isShowChengHao)
        {
            m_SpriteZH.transform.localPosition = new Vector3(0, 110, 0);
        }
        else
        {
            m_SpriteZH.transform.localPosition = new Vector3(0, 50, 0);
        }
        string[] zH = info.ToString().Split('|');
        m_SpriteZH.gameObject.SetActive(true);
        StartCoroutine(Wiat(float.Parse(zH[1]) / 1000));
    }
    IEnumerator Wiat(float time)
    {
        yield return new WaitForSeconds(time);
        m_SpriteZH.gameObject.SetActive(false);
    }
	public void ShowOrHide(bool tempState) //显示或隐藏玩家名字
	{
		m_playerName.gameObject.SetActive(tempState);
	}

    public void WetherHide(bool is_show)
    {
     //   m_playerName.gameObject.SetActive(is_show);
        m_LabAllianceName.gameObject.SetActive(is_show);
        m_SpriteChengHao.gameObject.SetActive(is_show);
    }

    void CreateEffect(int Designation)
    {
        if (_ObjEffect)
        {
            Destroy(_ObjEffect);
        }

        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(ChenghaoTemplate.GetChenghaoColor(Designation)), UIBoxLoad_SelectCountry);
    }
    public void UIBoxLoad_SelectCountry(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_SpriteChengHao != null)
        {
            GameObject tempObject = Instantiate(p_object) as GameObject;
            _ObjEffect = tempObject;
            tempObject.transform.parent = m_SpriteChengHao.transform;
            tempObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            tempObject.transform.localPosition = Vector3.zero;
            tempObject.transform.localScale = Vector3.one;
        }
        else
        {
            p_object = null;
        }

    }
}
