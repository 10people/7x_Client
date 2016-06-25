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

	public UICamera m_Camera;

	void Awake()
	{
		mEventListener.onClick = Close;
	}

	void Start () {

	}
	public void Init(List <int > ids = null,List <int > Nunbers = null,CallBack mcallBack = null ,UICamera mCamera = null)
	{
		if(mCamera != null)
		{
			m_Camera = mCamera ;
		}
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

			iconSampleManager.SetIconByID(mCommonItemTemplate.id, m_numbers[i].ToString(), 3);

			iconSampleManager.SetIconPopText();

			iconSampleManager.ShowAwardName(mname);
			iconSampleObject.transform.localPosition = new Vector3(i * 120 - (m_Ids.Count - 1) * 60, 0, 0);
		}
		if(m_Camera != null)
		{
			EffectTool.SetUIBackgroundEffect (m_Camera.gameObject,true);
		}
	}
	IEnumerator CreateAward()
	{
		if(Move) mTime = 0.0f;
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
				
				string m_Name = NameIdTemplate.GetName_By_NameId(mFuwen.name);

				iconSampleManager.SetIconByID(mFuwen.fuwenID, m_LieFuActionInfo.lieFuAwardList[i].itemNum.ToString());

				iconSampleManager.SetIconPopText();

				iconSampleManager.ShowAwardName(m_Name);

			}
			else
			{
				CommonItemTemplate mCommonItemTemplate = CommonItemTemplate.getCommonItemTemplateById (m_LieFuActionInfo.lieFuAwardList[i].itemId);
				
				Icon.spriteName = mCommonItemTemplate.icon.ToString();
				
				iconSampleManager.ShowAwardName(NameIdTemplate.GetName_By_NameId(mCommonItemTemplate.nameId)+"x"+m_LieFuActionInfo.lieFuAwardList[i].itemNum.ToString());

				iconSampleManager.SetIconByID(mCommonItemTemplate.id, m_LieFuActionInfo.lieFuAwardList[i].itemNum.ToString(), 3);
			
				string mname = NameIdTemplate.GetName_By_NameId(mCommonItemTemplate.nameId)+"x"+m_LieFuActionInfo.lieFuAwardList[i].itemNum.ToString();

				iconSampleManager.SetIconPopText();

				iconSampleManager.ShowAwardName(mname);

			}
			Debug.Log("Move = "+Move);
			if(!Move)
			{
				if(m_Camera != null)
				{
					EffectTool.SetUIBackgroundEffect (m_Camera.gameObject,true);
				}
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
		transObj = clone;
		iTween.MoveTo (clone, iTween.Hash(
			"position", clone.transform.localPosition +Vector3.up * 250,
			"time", 1.5f,"islocal",true,
			"easetype", iTween.EaseType.easeInOutQuart
			));
//		clone.AddComponent< TweenPosition>();
//		clone.AddComponent<TweenAlpha>();
//		clone.GetComponent<TweenPosition>().from = mobg.transform.localPosition;
//		clone.GetComponent<TweenPosition>().to = mobg.transform.localPosition + Vector3.up * 250;
//
//		clone.GetComponent<TweenPosition>().duration = 1.2f;
//		clone.GetComponent<TweenAlpha>().from = 1.0f;
//		clone.GetComponent<TweenAlpha>().to = 0;
//		clone.GetComponent<TweenPosition>().duration = 1.2f;
		StartCoroutine(WatiFor(mobg));
		//		mobg.transform.Translate (); = new Vector3(0,mobg.transform.localPosition.y + 5, 0);
	}
//	void DestroyObj ()
//	{
//		Destroy(obj);
//		Close(this.gameObject);
//	}
	IEnumerator WatiFor(GameObject obj)
	{
		yield return new WaitForSeconds(1.30f);
		Destroy(obj);
		transObj = null;
		Destroy (this.gameObject);
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
	public UILabel m_ContinueLabel;
	GameObject transObj;
	float m_scale = 0.05f;
	float m_color;
	int index;
	void Update () {
	
		if(transObj)
		{
			float rewardAlpha = transObj.GetComponent<UIWidget> ().alpha;
			if (transObj.transform.localPosition.y <= 200)
			{
				//			rewardAlpha = 1 - (Mathf.Abs (transObj.transform.localPosition.y) / 100);//225
				rewardAlpha = 1;
			}
			else
			{
				rewardAlpha = 1 - (Mathf.Abs (transObj.transform.localPosition.y - 200) / 225);
			}
			transObj.GetComponent<UIWidget> ().alpha = rewardAlpha;
			if (transObj.transform.localPosition.y >= 200)
			{
				if (transObj.transform.localScale.x > 0.6f)
				{
					transObj.transform.localScale -= Vector3.one * m_scale;
					m_scale -= 0.0001f;
					if (transObj.transform.localScale.x <= 0.3f)
					{
						transObj.transform.localScale = Vector3.one * 0.3f;
					}
				}
			}
	
		}
		if(!Move)
		{
			if(m_color >= 1 )
			{
				index = -1;
			}
			if(m_color < 0.5f )
			{
				index = 1;
			}
			m_color += index*Time.deltaTime*0.90f;
			m_ContinueLabel.alpha = Mathf.Abs (m_color);
		}

	}
	public void Close(GameObject mbutton)
	{
//		if(mFuwenType)
//		{
//			NewFuWenPage.Instance ().Init (NewFuWenPage.Instance ().mQueryFuwen.tab);
//		}
		if(m_Camera != null)
		{
			EffectTool.SetUIBackgroundEffect (m_Camera.gameObject,false);
			m_Camera = null;
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
