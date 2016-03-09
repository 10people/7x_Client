using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class LMMuBiaoAward : MonoBehaviour {

	//[HideInInspector] public List<PveStarItem> PveStarItemList = new List<PveStarItem> ();

	private float Dis = 100;
	
	public GameObject UIgrid;

	[HideInInspector]
	public GameObject IconSamplePrefab;

	[HideInInspector]
	public int  m_LMMBLevel;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Init(int lv)
	{
		m_LMMBLevel = lv;
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleCallBack);
		}
		else
		{
			WWW temp = null;
			OnIconSampleCallBack(ref temp, null, IconSamplePrefab);
		}
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
	}
	private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
	{
//		foreach(PveStarItem mPveStarItem in PveStarItemList)
//		{
//			Destroy(mPveStarItem.gameObject);
//		}
//		PveStarItemList.Clear ();
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
		LianMengTemplate mLM = LianMengTemplate.GetLianMengTemplate_by_lv (m_LMMBLevel);

		string m_award = mLM.targetAward;

		string [] s = m_award.Split (':');


		GameObject iconSampleObject = Instantiate( IconSamplePrefab )as GameObject;
		
		iconSampleObject.SetActive(true);

		iconSampleObject.transform.parent = UIgrid.transform;
		iconSampleObject.transform.localPosition = new Vector3(0,10,0);
		var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
		
		var iconSpriteName = "";

		
		CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(int.Parse(s[1]));
		
		iconSpriteName = mItemTemp.icon.ToString();
		
		iconSampleManager.SetIconType(IconSampleManager.IconType.item);
		
		NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mItemTemp.nameId);
		
		string mdesc = DescIdTemplate.GetDescriptionById(int.Parse(s[1]));
		
		var popTitle = mNameIdTemplate.Name;
		
		var popDesc = mdesc;
		
		iconSampleManager.SetIconByID(mItemTemp.id, s[2], 3);
		iconSampleManager.SetIconPopText(int.Parse(s[1]), popTitle, popDesc, 1);
		iconSampleObject.transform.localScale = new Vector3(1f,1f,1);

		//UIgrid.GetComponent<UIGrid> ().repositionNow = true;
	}
	public void Close()
	{
	
		Destroy (this.gameObject);
	}
}
