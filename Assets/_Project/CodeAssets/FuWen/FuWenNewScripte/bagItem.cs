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
	private FuWenTemplate mFW;
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
		Debug.Log ("mfwbg.exp = "+mfwbg.exp);
		ShowTip.showTip (mfwbg.itemId,TipItemData.createTipItemData(TipItemData.ScreenPosition.LEFT,mfwbg.exp));
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
	    mFW = FuWenTemplate.GetFuWenTemplateByFuWenId (mfwbg.itemId);

		//Debug.Log ("mFW.icon = "+mFW.icon);
		Icon.spriteName = mFW.icon.ToString ();
		pinzhi.spriteName = "pinzhi"+(mFW.color - 1).ToString ();
		Numbers.text = mfwbg.cnt.ToString();

		m_fwbg = new FuwenInBag();
		cur = 0;
		ChooseTps.SetActive (false);
	}

	int mUseNumber;
	public bool isOneKey;
	public void Choose()
	{

		if(InfoIsOpen)
		{

			if(cur < mfwbg.cnt)
			{
				FuWenTemplate FuwenInfo = FuWenTemplate.GetFuWenTemplateByFuWenId(FuWenInfoShow.Instance().mFuWenlanwei.itemId);

	
				if(FuwenInfo.fuwenLevel >= FuwenInfo.levelMax)
				{
					ClientMain.m_UITextManager.createText("符文等级已达最高，不能再进行熔合了！");
					return;
				}

				if(isOneKey)
				{
					isOneKey = false;
					//FuwenInBagResp m_FuwenInBag = (FuwenInBagResp)NewFuWenPage.Instance().mFuwenInBag.Public_MemberwiseClone;
					if( FuwenInfo.color < mFW.color )
					{
						return;
					}
					ChooseTps.SetActive(true);
					if(!FuWenInfoShow.Instance().fuwensinBag.Contains(m_fwbg))
					{
						if(FuWenInfoShow.Instance().mCurrFuWenlanwei.exp < FuwenInfo.lvlupExp)
						{
							int Max_exp = FuwenInfo.lvlupExp - FuWenInfoShow.Instance().mCurrFuWenlanwei.exp ;
							if( Max_exp >= mfwbg.cnt * mfwbg.exp )
							{
								m_fwbg.cnt = mfwbg.cnt;

								cur = mfwbg.cnt;
							}
							else
							{
								for(int i = 1 ; i < mfwbg.cnt+1; i++)
								{
									if( i * mfwbg.exp >= Max_exp)
									{
										m_fwbg.cnt = i;
										cur = m_fwbg.cnt;
										break;
									}
								}
							}

							m_fwbg.bagId = mfwbg.bagId;
							m_fwbg.itemId = 0;
							m_fwbg.exp = mfwbg.exp;
						}
						int m_EXp = mFW.exp + m_fwbg.exp;
						FuWenInfoShow.Instance().fuwensinBag.Add(m_fwbg);
						FuWenInfoShow.Instance().CreateLifeMoveNow(m_EXp*m_fwbg.cnt);
					}
					else
					{
					
						foreach(FuwenInBag mfuweninbag in FuWenInfoShow.Instance().fuwensinBag)
						{
							if(mfuweninbag.bagId == m_fwbg.bagId)
							{
								if(FuWenInfoShow.Instance().mCurrFuWenlanwei.exp < FuwenInfo.lvlupExp)
								{
									int Max_exp = FuwenInfo.lvlupExp - FuWenInfoShow.Instance().mCurrFuWenlanwei.exp ;
									if( Max_exp > mfwbg.cnt * mfwbg.exp )
									{
										mfuweninbag.cnt = mfwbg.cnt;
										
										cur = mfwbg.cnt;
									}
									else
									{
										for(int i = 1 ; i < mfwbg.cnt+1; i++)
										{
											if( i * mfwbg.exp >= Max_exp)
											{
												mfuweninbag.cnt = i;
												cur = mfwbg.cnt;
												break;
											}
										}
									}
									
								}
							}
							int m_EXp = mfuweninbag.exp + mFW.exp;
							FuWenInfoShow.Instance().CreateLifeMoveNow(m_EXp*cur);
						}
					}
					Numbers.text = cur.ToString()+"/"+mfwbg.cnt.ToString();
				} // 单次选择
				else
				{
					NewFuWenPage.Instance().OnekeyXiangqiang = false;
					if(FuwenInfo.color < mFW.color)
					{
						ClientMain.m_UITextManager.createText("低品质符文不能熔合高品质符文！");
						return;
					}
//					if(FuWenInfoShow.Instance().mCurrFuWenlanwei.exp >= FuwenInfo.lvlupExp)
//					{
//						ClientMain.m_UITextManager.createText("不能点了，等浩南配置提示语！");
//						return;
//					}
					ChooseTps.SetActive(true);
					cur += 1;
					Numbers.text = cur.ToString()+"/"+mfwbg.cnt.ToString();
					
					mUseNumber = cur;
					
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
					int m_EXp = m_fwbg.exp + mFW.exp;
					FuWenInfoShow.Instance().CreateLifeMoveNow(m_EXp);
				}

			}
		}

	}

	public void BackChoose()
	{
		if(cur > 0)
		{
			NewFuWenPage.Instance().OnekeyXiangqiang = false;
			cur --;
			if(cur == 0)
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
						if(mfuweninbag.cnt > 1)
						{
							mfuweninbag.cnt -= 1;
							int m_EXp = m_fwbg.exp + mFW.exp;
							FuWenInfoShow.Instance()._DeleltLifeMoveNow(m_EXp);
						}
						else
						{
							mfuweninbag.cnt -= 1;
							int m_EXp = m_fwbg.exp + mFW.exp;
							FuWenInfoShow.Instance()._DeleltLifeMoveNow(m_EXp);
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
