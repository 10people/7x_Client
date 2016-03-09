using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PveStarAward : MonoBehaviour {
	
	[HideInInspector] public List<PveStarItem> PveStarItemList = new List<PveStarItem> ();

	private float Dis = 100;

	public GameObject UIgrid;

	[HideInInspector]
	public Level mLevel;

	[HideInInspector]
	public GameObject IconSamplePrefab;

	void Start () {
	
	}

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
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
	}
	private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		foreach(PveStarItem mPveStarItem in PveStarItemList)
		{
			Destroy(mPveStarItem.gameObject);
		}
		PveStarItemList.Clear ();
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
		if(CityGlobalData.PT_Or_CQ)
		{
			for(int i = 0 ; i < mLevel.starInfo.Count; i ++)
			{
				if(mLevel.starInfo[i].finished && !mLevel.starInfo[i].getRewardState)
				{
					GameObject iconSampleObject = Instantiate( IconSamplePrefab )as GameObject;
					
					iconSampleObject.SetActive(true);
					
					iconSampleObject.transform.parent = UIgrid.transform;
					
					var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
					
					var iconSpriteName = "";
					string[] Awardlist = PveStarTemplate.GetAwardInfo (mLevel.starInfo[i].starId);
					
					PveStarTemplate mPveStarTemplate = PveStarTemplate.getPveStarTemplateByStarId (mLevel.starInfo[i].starId);
					
					CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(int.Parse(Awardlist[1]));
					
					iconSpriteName = mItemTemp.icon.ToString();
					
					iconSampleManager.SetIconType(IconSampleManager.IconType.item);
					
					NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mItemTemp.nameId);
					
					string mdesc = DescIdTemplate.GetDescriptionById(int.Parse(Awardlist[1]));
					
					var popTitle = mNameIdTemplate.Name;
					
					var popDesc = mdesc;
					
					iconSampleManager.SetIconByID(mItemTemp.id, Awardlist[2], 3);
					iconSampleManager.SetIconPopText(int.Parse(Awardlist[1]), popTitle, popDesc, 1);
					iconSampleObject.transform.localScale = new Vector3(0.5f,0.5f,1);
				}
				
			}
		}
		else{
			for(int i = 0 ; i < mLevel.cqStarInfo.Count; i ++)
			{
				if(mLevel.cqStarInfo[i].finished && !mLevel.cqStarInfo[i].getRewardState)
				{
					GameObject iconSampleObject = Instantiate( IconSamplePrefab )as GameObject;
					
					iconSampleObject.SetActive(true);
					
					iconSampleObject.transform.parent = UIgrid.transform;
					
					var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
					
					var iconSpriteName = "";
					string[] Awardlist = PveStarTemplate.GetAwardInfo (mLevel.cqStarInfo[i].starId);
					
					PveStarTemplate mPveStarTemplate = PveStarTemplate.getPveStarTemplateByStarId (mLevel.cqStarInfo[i].starId);
					
					CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(int.Parse(Awardlist[1]));
					
					iconSpriteName = mItemTemp.icon.ToString();
					
					iconSampleManager.SetIconType(IconSampleManager.IconType.item);
					
					NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mItemTemp.nameId);
					
					string mdesc = DescIdTemplate.GetDescriptionById(int.Parse(Awardlist[1]));
					
					var popTitle = mNameIdTemplate.Name;
					
					var popDesc = mdesc;
					
					iconSampleManager.SetIconByID(mItemTemp.id, Awardlist[2], 3);
					iconSampleManager.SetIconPopText(int.Parse(Awardlist[1]), popTitle, popDesc, 1);
					iconSampleObject.transform.localScale = new Vector3(0.5f,0.5f,1);
				}
				
			}
		}

		UIgrid.GetComponent<UIGrid> ().repositionNow = true;
	}
	public void SendLingQu()
	{
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
		if(CityGlobalData.PT_Or_CQ)
		{
			for (int i = 0; i < mLevel.starInfo.Count; i ++) {
				if(mLevel.starInfo[i].finished && !mLevel.starInfo[i].getRewardState)
				{
					MemoryStream t_tream = new MemoryStream();
					
					QiXiongSerializer t_qx = new QiXiongSerializer();
					
					GetPveStarAward award = new GetPveStarAward();
					
					award.s_starNum = mLevel.starInfo[i].starId;
					
					award.guanQiaId = mLevel.guanQiaId;

					award.isChuanQi = false;

					t_qx.Serialize(t_tream, award);
					
					byte[] t_protof;
					
					t_protof = t_tream.ToArray();
					
					SocketTool.Instance().SendSocketMessage(ProtoIndexes.PVE_STAR_REWARD_GET, ref t_protof);
				}
			}
		}
		else{
			for (int i = 0; i < mLevel.cqStarInfo.Count; i ++) {

				if(mLevel.cqStarInfo[i].finished && !mLevel.cqStarInfo[i].getRewardState)
				{
					MemoryStream t_tream = new MemoryStream();
					
					QiXiongSerializer t_qx = new QiXiongSerializer();
					
					GetPveStarAward award = new GetPveStarAward();
					
					award.s_starNum = mLevel.cqStarInfo[i].starId;
					
					award.guanQiaId = mLevel.guanQiaId;

					award.isChuanQi = true;

					t_qx.Serialize(t_tream, award);
					
					byte[] t_protof;
					
					t_protof = t_tream.ToArray();
					
					SocketTool.Instance().SendSocketMessage(ProtoIndexes.PVE_STAR_REWARD_GET, ref t_protof);
				}
			}
		}

	
		Destroy (this.gameObject);
	}
	public void CloseUI()
	{
		NewPVEUIManager.Instance().ShowPVEGuid ();
		Destroy (this.gameObject);
	}
}
