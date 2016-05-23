using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class QCLLayerSaoDang : MonoBehaviour {

	public List<UIEventListener> BtnList = new List<UIEventListener>(); 

	public UILabel SaodangLayer;

	[HideInInspector]public ChongLouSaoDangResp mQCL_saodangInfo;

	public GameObject AwardRoot;
	void Awake()
	{
		BtnList.ForEach (item => SetBtnMoth(item));
		
	}
	void SetBtnMoth(UIEventListener mUIEventListener)
	{
		mUIEventListener.onClick = BtnManagerMent;
	}
	
	void Start () {
		
	}

	public void Init(int mLayer,int currLayer)
	{
		string mStr = "";
		if(mLayer == 1 )
		{
			mStr = "扫荡层数：" +mLayer.ToString()+ "层";
		}
		else
		{
			mStr = "扫荡层数："+currLayer.ToString()+"—" +mLayer.ToString()+ "层";
		}
		SaodangLayer.text = mStr;

		InitAwardList ();
	}
	[HideInInspector]
	public GameObject IconSamplePrefab;
	void InitAwardList()
	{
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleCallBack);
		}
		else
		{
			WWW temp = null;
			OnIconSampleCallBack(ref temp, null, IconSamplePrefab);
		}
	}
	private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
	
		for (int i = 0; i < mQCL_saodangInfo.awards.Count; i++)
		{
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
		
			iconSampleObject.SetActive(true);
			
			iconSampleObject.transform.parent = AwardRoot.transform;
		
			//FirstAwardPos = iconSampleObject.transform.localPosition;
			
			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			var iconSpriteName = "";
			
			CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(mQCL_saodangInfo.awards[i].itemId);
			
			iconSpriteName = mItemTemp.icon.ToString();
			
			iconSampleManager.SetIconType(IconSampleManager.IconType.item);
			
			NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mQCL_saodangInfo.awards[i].itemId);
			
			string mdesc = DescIdTemplate.GetDescriptionById(mQCL_saodangInfo.awards[i].itemId);
			
			var popTitle = mNameIdTemplate.Name;
			
			var popDesc = mdesc;
			
			iconSampleManager.SetIconByID(mItemTemp.id, mQCL_saodangInfo.awards[i].itemNum.ToString(), 3);
			iconSampleManager.SetIconPopText(mQCL_saodangInfo.awards[i].itemId, popTitle, popDesc, 1);
			iconSampleObject.transform.localScale = new Vector3(0.6f,0.6f,1);

		}
		AwardRoot.GetComponent<UIGrid> ().repositionNow = true;

		float m_y = (mQCL_saodangInfo.awards.Count / 4+1) * 60;

		AwardRoot.GetComponent<BoxCollider> ().size = new Vector3 (290,m_y,0);
		AwardRoot.GetComponent<BoxCollider> ().center = new Vector3 (0,-(m_y-120)/2,0);
	}
	public void BtnManagerMent(GameObject mbutton)
	{
		Close ();
	}

	void Close()
	{
		Destroy (this.gameObject);
	}
}
