using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class FuWenShow : MonoBehaviour {

	public UILabel mName;

	public UISprite Icon;

	public UISprite PinZhi;

	public UIEventListener mEventListener;

	public UIPanel mPanel;

	public GameObject my_Item;

	[HideInInspector]public LieFuActionResp m_LieFuActionInfo;

	int pinzhicolor;

	private bool Move;

	[HideInInspector]
	public GameObject IconSamplePrefab;

	float mTime = 0.1f;

	public delegate void CallBack();
	private CallBack m_CallBack;

	public List<int >m_Ids = new List<int>();
	public List<int >m_numbers = new List<int>();
	public bool mFuwenType ;//  fales 为其他东西调用，True 为符文功能调用

	void Awake()
	{
		mEventListener.onClick = Close;
	}

	void Start () {

	}
	public void Init(List <int > ids = null,List <int > Nunbers = null,CallBack mcallBack = null)
	{
		Move = false;
		if(mcallBack != null)
		{m_CallBack = mcallBack; }
		if(ids != null)
		{
			m_Ids = ids;
			m_numbers = Nunbers;
			mFuwenType = false;
			mEventListener.gameObject.SetActive(true);
		}
		else
		{
			mFuwenType = true;

			for(int i = 0; i < m_LieFuActionInfo.lieFuAwardList.Count; i ++)
			{
				if(m_LieFuActionInfo.lieFuAwardList[i].itemType == 7)
				{
					FuWenTemplate mFuwen = FuWenTemplate.GetFuWenTemplateByFuWenId (m_LieFuActionInfo.lieFuAwardList[i].itemId);
					
					pinzhicolor = mFuwen.color;
				}
				else
				{
					CommonItemTemplate mCommonItemTemplate = CommonItemTemplate.getCommonItemTemplateById (m_LieFuActionInfo.lieFuAwardList[i].itemId);
					
					pinzhicolor = mCommonItemTemplate.color;
				}
			}
			
			if(pinzhicolor >= 9)
			{
				//橙色品质框
				Move = false;
				mEventListener.gameObject.SetActive(true);
			}
			else
			{
				mEventListener.gameObject.SetActive(false);
				Move = true;
			}
		}

		LoadItem ();
	}
	void LoadItem()
	{	
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreatePuTong_Enemys);
		}
		else
		{
			WWW temp = null;
			OnCreatePuTong_Enemys(ref temp, null, IconSamplePrefab);
		}
	}
	private void OnCreatePuTong_Enemys(ref WWW p_www, string p_path, Object p_object)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
		if(mFuwenType)
		{
			StartCoroutine ("CreateAward");
		}
		else
		{
			CreateAtherAwards();
		}
	}
	void  CreateAtherAwards()
	{
		for(int i = 0; i < m_Ids.Count; i ++)
		{
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			
			iconSampleObject.SetActive (true);
			
			iconSampleObject.transform.parent = my_Item.transform;

			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

			CommonItemTemplate mCommonItemTemplate = CommonItemTemplate.getCommonItemTemplateById (m_Ids[i]);
			
			Icon.spriteName = mCommonItemTemplate.icon.ToString();
			
			//iconSampleManager.ShowAwardName(NameIdTemplate.GetName_By_NameId(mCommonItemTemplate.nameId)+"x"+m_LieFuActionInfo.lieFuAwardList[i].itemNum.ToString());
//			
//			string pinzhi = "pinzhi"+(mCommonItemTemplate.color-1).ToString();
//			
			string mname = NameIdTemplate.GetName_By_NameId(mCommonItemTemplate.nameId);
//			Debug.Log("m_numbers[i] = "+m_numbers[i]);

			iconSampleManager.SetIconByID(mCommonItemTemplate.id, m_numbers[i].ToString(), 3);
			iconSampleManager.SetIconBasic(3, mCommonItemTemplate.icon.ToString());
			var popTextTitle = NameIdTemplate.GetName_By_NameId(mCommonItemTemplate.nameId);
			DescIdTemplate mDesc = DescIdTemplate.getDescIdTemplateByNameId(mCommonItemTemplate.nameId);
			var popTextDesc = mDesc.description;
			iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
			iconSampleManager.SetAwardNumber(m_numbers[i]);
			iconSampleObject.transform.localPosition = new Vector3(i * 120 - (m_Ids.Count - 1) * 60, 0, 0);
		}
	}
	IEnumerator CreateAward()
	{
		if(Move) mTime = 0.5f;
		else{
			mTime = 0.01f;
		}
		for(int i = 0; i < m_LieFuActionInfo.lieFuAwardList.Count; i ++)
		{
			yield return new WaitForSeconds(mTime);
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			
			iconSampleObject.SetActive (true);
			
			iconSampleObject.transform.parent = my_Item.transform;


			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			if(m_LieFuActionInfo.lieFuAwardList[i].itemType == 8)
			{
				FuWenTemplate mFuwen = FuWenTemplate.GetFuWenTemplateByFuWenId (m_LieFuActionInfo.lieFuAwardList[i].itemId);
				
				string m_Name = mFuwen.fuwenLevel.ToString()+"级"+ NameIdTemplate.GetName_By_NameId(mFuwen.name);

				string pinzhi = "pinzhi"+(mFuwen.color-1).ToString();
				iconSampleManager.ShowAwardName(m_Name);
				
				iconSampleManager.SetIconType(IconSampleManager.IconType.FuWen);
				iconSampleManager.SetIconBasic(3, mFuwen.icon.ToString(),"",pinzhi);
				var popTextTitle = m_Name + " " + "LV" + mFuwen.fuwenLevel.ToString();
				var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mFuwen.desc).description;
				iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);

			}
			else
			{
				CommonItemTemplate mCommonItemTemplate = CommonItemTemplate.getCommonItemTemplateById (m_LieFuActionInfo.lieFuAwardList[i].itemId);
				
				Icon.spriteName = mCommonItemTemplate.icon.ToString();
				
				iconSampleManager.ShowAwardName(NameIdTemplate.GetName_By_NameId(mCommonItemTemplate.nameId)+"x"+m_LieFuActionInfo.lieFuAwardList[i].itemNum.ToString());

				string pinzhi = "pinzhi"+(mCommonItemTemplate.color-1).ToString();

				string mname = NameIdTemplate.GetName_By_NameId(mCommonItemTemplate.nameId)+"x"+m_LieFuActionInfo.lieFuAwardList[i].itemNum.ToString();
				iconSampleManager.SetIconType(IconSampleManager.IconType.FuWen);
				iconSampleManager.SetIconBasic(3, mCommonItemTemplate.icon.ToString(),"",pinzhi);
				var popTextTitle = NameIdTemplate.GetName_By_NameId(mCommonItemTemplate.nameId);
				DescIdTemplate mDesc = DescIdTemplate.getDescIdTemplateByNameId(mCommonItemTemplate.nameId);
				var popTextDesc = mDesc.description;
				iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);

			}
			Debug.Log("Move = "+Move);
			if(!Move)
			{
				iconSampleObject.transform.localPosition = new Vector3(i * 120 - (m_LieFuActionInfo.lieFuAwardList.Count - 1) * 60, 0, 0);
			}
			else
			{
				iconSampleObject.transform.localPosition = Vector3.zero;
				Movetodisplay(iconSampleObject);
				Destroy(iconSampleObject);
			}
		}
	}

	private void Movetodisplay(GameObject mobg)
	{
		GameObject clone = NGUITools.AddChild(mobg.transform.parent.gameObject, mobg);
		clone.AddComponent< TweenPosition>();
		clone.AddComponent<TweenAlpha>();
		clone.GetComponent<TweenPosition>().from = mobg.transform.localPosition;
		clone.GetComponent<TweenPosition>().to = mobg.transform.localPosition + Vector3.up * 250;
		clone.GetComponent<TweenPosition>().duration = 1.2f;
		clone.GetComponent<TweenAlpha>().from = 1.0f;
		clone.GetComponent<TweenAlpha>().to = 0;
		clone.GetComponent<TweenPosition>().duration = 1.2f;
		StartCoroutine(WatiFor(mobg));
		//		mobg.transform.Translate (); = new Vector3(0,mobg.transform.localPosition.y + 5, 0);
	}
	IEnumerator WatiFor(GameObject obj)
	{
		yield return new WaitForSeconds(0.8f);
		Destroy(obj);
		Close(this.gameObject);
	}
	public void setPosC()
	{
		float tempF = 1 - ((my_Item.transform.localPosition.y - 80)/ 240f);
		mPanel.alpha = tempF;
		if(tempF <= 0)
		{
			tempF = 0f;
			Close(this.gameObject);
		}
	}
	void Update () {
	
//		if(Move)
//		{
//			Movetodisplay();
//			setPosC();
//		}
	}
	public void Close(GameObject mbutton)
	{
		if(mFuwenType)
		{
			NewFuWenPage.Instance ().Init (NewFuWenPage.Instance ().mQueryFuwen.tab);
		}

		if(m_CallBack != null)
		{
			Debug.Log ("m_CallBack != null");
			m_CallBack();
			m_CallBack = null;
		}
		else
		{
			Debug.Log ("m_CallBack == null");
		}
		Destroy (this.gameObject);
	}
}
