using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class RankItem : MonoBehaviour {

	public UILabel JunZhuName;
	public UILabel mRank;
	public UILabel mDamage;
	public DamageInfo mDamageInfo;

	public UISprite myself;

	public bool iSMyself = false;

	[HideInInspector]
	public GameObject IconSamplePrefab;

	public UILabel Award;

	void Start () {
	
	}
	

	void Update () {
	
	}
	public void Init()
	{
		JunZhuName.text = mDamageInfo.junZhuName;

		mRank.text = mDamageInfo.rank.ToString();

		mDamage.text = mDamageInfo.damage.ToString();

		myself.gameObject.SetActive(iSMyself);

		InitAwards ();
	}
	void InitAwards()
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
	List<int> t_items = new List<int>();
	List<int> _t_items_Num = new List<int>();
	float m_Dis = 50;
	private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		t_items.Clear ();
		_t_items_Num.Clear ();
//		if (IconSamplePrefab == null)
//		{
//			IconSamplePrefab = p_object as GameObject;
//		}
		HYRankAwardTemplate mHYRank = HYRankAwardTemplate.getHYRankAwardTemplateTemplateBy_Rank (mDamageInfo.rank);

		string m_Award = mHYRank.award;

		char[] t_items_delimiter = { '#' };
		
		char[] t_item_id_delimiter = { ':' };
		
		string[] t_item_strings = m_Award.Split(t_items_delimiter);
		
		for (int i = 0; i < t_item_strings.Length; i++)
		{
			string t_item = t_item_strings[i];
			
			string[] t_finals = t_item.Split(t_item_id_delimiter);
			
			if(t_finals[0] != "" && !t_items.Contains(int.Parse(t_finals[0])))
			{
				t_items.Add(int.Parse(t_finals[1]));
				_t_items_Num.Add(int.Parse(t_finals[2]));
			}
		}

		List<AwardTemp> mAwardTemp = new List<AwardTemp> ();
		int AwardCount = 0;
		Award.text = _t_items_Num [0].ToString ();
//		for (int n = 0; n < t_items.Count; n++)
//		{
//			AwardCount++;
//			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
//			
//			iconSampleObject.SetActive(true);
//			
////			iconSampleObject.transform.parent = AwardRoot.transform;
//			
//			iconSampleObject.transform.localPosition = new Vector3((n) * m_Dis-(t_items.Count - 1)* 25, 0, 0);
//			
//			//FirstAwardPos = iconSampleObject.transform.localPosition;
//			
//			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
//			
//			var iconSpriteName = "";
//			
//			CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(t_items[n]);
//			
//			iconSpriteName = mItemTemp.icon.ToString();
//			
//			iconSampleManager.SetIconType(IconSampleManager.IconType.item);
//			
//			NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(t_items[n]);
//			
//			string mdesc = DescIdTemplate.GetDescriptionById(t_items[n]);
//			
//			var popTitle = mNameIdTemplate.Name;
//			
//			var popDesc = mdesc;
//			
//			iconSampleManager.SetIconByID(mItemTemp.id, _t_items_Num[n].ToString(), 3);
//			iconSampleManager.SetIconPopText(t_items[n], popTitle, popDesc, 1);
//			iconSampleObject.transform.localScale = new Vector3(0.4f,0.4f,1);
//		
//		}
		
	}
}
