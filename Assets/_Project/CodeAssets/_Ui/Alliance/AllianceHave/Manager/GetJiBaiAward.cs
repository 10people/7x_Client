using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GetJiBaiAward : MonoBehaviour {

	[HideInInspector]
	public GameObject IconSamplePrefab;
	public List<Award> m_OneKeyAward = new List<Award>();
	public GameObject AwardUitroot;
	public UILabel UIName;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Init()
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
		if(m_OneKeyAward.Count <= 1)
		{
			UIName.text = "祭拜";
		}
		else
		{
			UIName.text = "一键祭拜";
		}
		for(int i = 0; i < m_OneKeyAward.Count ; i ++)
		{
			if (IconSamplePrefab == null)
			{
				IconSamplePrefab = p_object as GameObject;
			}
			
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			
			iconSampleObject.SetActive(true);
			
			iconSampleObject.transform.parent = AwardUitroot.transform;
			
			//iconSampleObject.transform.localScale = new Vector3(0.7f,0.7f,1);
			
			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			var iconSpriteName = "";
			
			CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(m_OneKeyAward[i].itemId);
			
			iconSpriteName = mItemTemp.icon.ToString();
			
			iconSampleManager.SetIconType(IconSampleManager.IconType.item);
			
			NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mItemTemp.nameId);
			
			string mdesc = DescIdTemplate.GetDescriptionById(mItemTemp.descId);
			
			var popTitle = mNameIdTemplate.Name;
			
			var popDesc = mdesc;
			
			iconSampleManager.SetIconByID(mItemTemp.id, m_OneKeyAward[i].itemNumber.ToString(), 10);
			iconSampleManager.SetIconPopText(mItemTemp.id, popTitle, popDesc, 1);
			iconSampleObject.transform.localScale = Vector3.one * 0.65f;
			//iconSampleManager.SetAwardNumber(m_OneKeyAward[i].pieceNumber);
		}
		if(m_OneKeyAward.Count <= 1)
		{
			AwardUitroot.GetComponent<UIGrid> ().m_x_offset = 0;
		}
		if(m_OneKeyAward.Count == 2)
		{
			AwardUitroot.GetComponent<UIGrid> ().m_x_offset = -40;
		}
		if(m_OneKeyAward.Count == 3)
		{
			AwardUitroot.GetComponent<UIGrid> ().m_x_offset = -75;
		}
		if(m_OneKeyAward.Count > 4)
		{
			AwardUitroot.GetComponent<UIGrid> ().m_y_offset = 74;
		}
		AwardUitroot.GetComponent<UIGrid> ().repositionNow = true;
	}
	public void Close()
	{
		Destroy (this.gameObject);
	}
}
