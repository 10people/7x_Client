using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class bagItem : MYNGUIPanel {

	[HideInInspector]public FuwenInBag mfwbg;
	public UILabel Numbers;

	public UISprite Icon;

	public UISprite pinzhi;

	private bool InfoIsOpen = false;

	public GameObject ChooseTps;
	int cur; // 剩余的符文个数
	public NGUILongPress EnergyDetailLongPress1;

	public UIButton mbtn;

	private FuwenInBag m_fwbg;

	void Awake()
	{
		EnergyDetailLongPress1.LongTriggerType = NGUILongPress.TriggerType.Press;
		EnergyDetailLongPress1.NormalPressTriggerWhenLongPress = false;
		EnergyDetailLongPress1.OnLongPressFinish = OnCloseDetail;
		EnergyDetailLongPress1.OnLongPress = OnEnergyDetailClick1;
	}
	private void OnCloseDetail(GameObject go)
	{
		ShowTip.close();
	}
	public void OnEnergyDetailClick1(GameObject go)//显示体力恢复提示
	{
		if(InfoIsOpen)
			return;
		ShowTip.showTip (mfwbg.itemId);
	}
	void Start () {
	
	}

	void Update () {
	
		InfoIsOpen = NewFuWenPage.Instance ().RongHeUIisOpen;
		if(InfoIsOpen)
		{
			mbtn.enabled = true;
		}
		else{

			mbtn.enabled = false;
		}
	}

	public void init()
	{
		//Debug.Log ("mfwbg.itemId = "+mfwbg.itemId);
		isOneKey = false;
		FuWenTemplate mFW = FuWenTemplate.GetFuWenTemplateByFuWenId (mfwbg.itemId);

		//Debug.Log ("mFW.icon = "+mFW.icon);
		Icon.spriteName = mFW.icon.ToString ();
		pinzhi.spriteName = "pinzhi"+(mFW.color - 1).ToString ();
		Numbers.text = mfwbg.cnt.ToString();
		cur = mfwbg.cnt;

		m_fwbg = new FuwenInBag();

		ChooseTps.SetActive (false);
	}
	public void OnKeyIN()
	{
		cur = 0;
		ChooseTps.SetActive (true);
		Numbers.text = cur.ToString()+"/"+mfwbg.cnt.ToString();
	}
	int mUseNumber;
	public bool isOneKey;
	public void Choose()
	{
		if(InfoIsOpen)
		{
			ChooseTps.SetActive(true);
			if(cur > 0)
			{
				if(isOneKey)
				{
					//FuwenInBagResp m_FuwenInBag = (FuwenInBagResp)NewFuWenPage.Instance().mFuwenInBag.Public_MemberwiseClone;

					if(!FuWenInfoShow.Instance().fuwensinBag.Contains(m_fwbg))
					{
						m_fwbg.cnt = mfwbg.cnt;
						m_fwbg.bagId = mfwbg.bagId;
						m_fwbg.itemId = 0;
						m_fwbg.exp = mfwbg.exp;
						FuWenInfoShow.Instance().fuwensinBag.Add(m_fwbg);
						FuWenInfoShow.Instance().CreateLifeMoveNow(m_fwbg.exp*m_fwbg.cnt);
					}
					else
					{
						foreach(FuwenInBag mfuweninbag in FuWenInfoShow.Instance().fuwensinBag)
						{
							if(mfuweninbag.bagId == m_fwbg.bagId)
							{
								mfuweninbag.cnt =  mfwbg.cnt;
							}
						}
						FuWenInfoShow.Instance().CreateLifeMoveNow(m_fwbg.exp*cur);

					}
					cur = 0;
					Numbers.text = cur.ToString()+"/"+mfwbg.cnt.ToString();
				}
				else
				{
					cur -= 1;
					Numbers.text = cur.ToString()+"/"+mfwbg.cnt.ToString();
					
					mUseNumber = mfwbg.cnt - cur;
					
					if(!FuWenInfoShow.Instance().fuwensinBag.Contains(m_fwbg))
					{
						m_fwbg.cnt = mUseNumber;
						m_fwbg.bagId = mfwbg.bagId;
						m_fwbg.itemId = 0;
						m_fwbg.exp = mfwbg.exp;
						FuWenInfoShow.Instance().fuwensinBag.Add(m_fwbg);
					}
					else
					{
						foreach(FuwenInBag mfuweninbag in FuWenInfoShow.Instance().fuwensinBag)
						{
							if(mfuweninbag.bagId == m_fwbg.bagId)
							{
								mfuweninbag.cnt += 1;
								//Debug.Log("1111111mfuweninbag.cnt = " +mfuweninbag.cnt);
							}
						}
					}

					FuWenInfoShow.Instance().CreateLifeMoveNow(m_fwbg.exp);
				}

			}
		}

	}

	public void BackChoose()
	{
		if(cur < mfwbg.cnt)
		{
			cur ++;
			if(cur == mfwbg.cnt)
			{
				ChooseTps.SetActive(false);
				Numbers.text = mfwbg.cnt.ToString();
			}
			else
			{
				Numbers.text = cur.ToString()+"/"+mfwbg.cnt.ToString();
			}
//			if(cur ==  mfwbg.cnt)
			{
				foreach(FuwenInBag mfuweninbag in FuWenInfoShow.Instance().fuwensinBag)
				{
					if(mfuweninbag.bagId == mfwbg.bagId)
					{
						//Debug.Log("mfuweninbag.cnt = " +mfuweninbag.cnt);
						if(mfuweninbag.cnt > 0)
						{
							mfuweninbag.cnt -= 1;
							FuWenInfoShow.Instance()._DeleltLifeMoveNow(mfwbg.exp);
						}
						else
						{
							FuWenInfoShow.Instance().fuwensinBag.Remove(mfuweninbag);
						}
						break;
					}
				}
			}
		}
	}
	#region fulfil my ngui panel
	
	/// <summary>
	/// my click in my ngui panel
	/// </summary>
	/// <param name="ui"></param>
	public override void MYClick(GameObject ui)
	{
		
	}
	
	public override void MYMouseOver(GameObject ui)
	{
		
	}
	
	public override void MYMouseOut(GameObject ui)
	{
		
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
		
	}
	
	public override void MYelease(GameObject ui)
	{
		
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
		
	}
	#endregion
}
