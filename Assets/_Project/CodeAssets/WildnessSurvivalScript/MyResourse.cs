using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MyResourse : MonoBehaviour {

//	public MibaoInfoResp my_MiBaoInfo;
//
//	public HuangYeResource my_HuangYeResource;//
//
//	public UISprite mLock;
//
//	public UISprite MibaoskillIcon;
//
//	public GameObject EnemyRoot;
//
//	public UILabel ResInstrction;
//
//	public UILabel Zhanli;
//
//	public UILabel Res_Name;
//
//	public ResourceNpcInfo m_ResInfo;
//
//	[HideInInspector]
//
//	public int ZhaliNum;
//
//	public List<IconSampleManager> GuyongbingList = new List<IconSampleManager>();
//
//	[HideInInspector]
//
//	public GameObject IconSamplePrefab;
//
//	private readonly Vector3 HeroPos = new Vector3(-364, 0, 0);
//
//	private readonly Vector3 MercenaryPos = new Vector3(-240, 0, 0);
//
//	private const int IconBasicDepth = 15;
//
//	public BattleResouceResp mHYResourseBattleResp;
//
//	void Start () {
//	
//	}
//
//	public void Init()
//	{
//		HuangyeTemplate mHuangyeTemplate = HuangyeTemplate.getHuangyeTemplate_byid (my_HuangYeResource.fileId);//m描述
//		
//		string mDescIdTemplate = DescIdTemplate.GetDescriptionById (mHuangyeTemplate.descId);
//		
//		string mName = NameIdTemplate.GetName_By_NameId (mHuangyeTemplate.nameId);
////
////		char[] separator = new char[]{'#'};
////		
////		string[] s = mDescIdTemplate.Split (separator);
////		
////		string desText = "";
////		
////		for(int j = 0; j < s.Length; j++ )
////		{
////			desText += s[j]+"\r\n                    ";
////		}
////		
//		Res_Name.text = mName;
//
//		ResInstrction.text = LanguageTemplate.GetText(LanguageTemplate.Text.HUANGYE_69);
//		
//		init_Hero_GuYongBing();
//		
//		Zhanli.text = ZhaliNum.ToString();//显示敌方战力
//
//		ShowMibaoSkill ();
//	}
//	void init_Hero_GuYongBing()
//	{
//		foreach (IconSampleManager GYB in GuyongbingList)
//		{
//			Destroy(GYB.gameObject);
//		}
//		
//		GuyongbingList.Clear();
//
//		if (IconSamplePrefab == null)
//		{
//			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
//			                        
//			                        IconSampleLoadCallBack);
//		}
//		else
//		{
//			WWW tempWww = null;
//			
//			IconSampleLoadCallBack(ref tempWww, null, IconSamplePrefab);
//		}
//	}
//
//	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
//	{
//		//store loaded prefab.
//		if (IconSamplePrefab == null)
//		{
//			IconSamplePrefab = p_object as GameObject;
//		}
//
//		
//		GameObject EnemyHero = Instantiate(IconSamplePrefab) as GameObject;
//		
//		EnemyHero.transform.parent = EnemyRoot.transform;
//		
//		EnemyHero.transform.localPosition = HeroPos;
//		
//		EnemyHero.SetActive(true);
//		IconSampleManager enemyHeroIconSample = EnemyHero.GetComponent<IconSampleManager>();
//		
//		enemyHeroIconSample.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
//		
//		HuangyePvpNpcTemplate mHYPVP = HuangyePvpNpcTemplate.getHuangyepvpNPCTemplate_by_Npcid(m_ResInfo.bossId);
//
//		enemyHeroIconSample.SetIconDecoSprite(null, "boss");
//
//		enemyHeroIconSample.SetIconBasic(IconBasicDepth, mHYPVP.icon.ToString(), mHYPVP.level.ToString());
//		
//		for (int i = 0; i < m_ResInfo.yongBingId.Count; i++)//显示敌方的雇佣兵
//		{
//			GameObject EnemyMercenary = Instantiate(IconSamplePrefab) as GameObject;
//			
//			EnemyMercenary.transform.parent = EnemyRoot.transform;
//			
//			EnemyMercenary.transform.localPosition = MercenaryPos + new Vector3(120 * i, 0, 0);
//			
//			EnemyMercenary.SetActive(true);
//			
//			HY_GuYongBingTempTemplate mGuYongBingTempTemplate = HY_GuYongBingTempTemplate.GetHY_GuYongBingTempTemplate_By_id(m_ResInfo.yongBingId[i]);
//			
//			int yongbing_Lv = mGuYongBingTempTemplate.level;
//			
//			int yongbing_Sprite = mGuYongBingTempTemplate.icon;
//			
//			int yongbing_Pinzhi = mGuYongBingTempTemplate.quality;
//			
//			string leftTopSpriteName;
//			if (mGuYongBingTempTemplate.profession == 1)
//			{
//				leftTopSpriteName = "dun";//盾
//			}
//			else if (mGuYongBingTempTemplate.profession == 4)
//			{
//				leftTopSpriteName = "piccar";//车
//			}
//			else if (mGuYongBingTempTemplate.profession == 3)
//			{
//				leftTopSpriteName = "gong";//弓箭
//			}
//			else if (mGuYongBingTempTemplate.profession == 5)
//			{
//				leftTopSpriteName = "hours";//Horse////马
//			}
//			else if (mGuYongBingTempTemplate.profession == 2)
//			{
//				leftTopSpriteName = "gang";//Horse////枪
//			}
//			else
//			{
//				leftTopSpriteName = null;
//			}
//			var rightButtomSpriteName = "";
//
//			IconSampleManager enemyMercenaryIconSample = EnemyMercenary.GetComponent<IconSampleManager>();
//			
//			enemyMercenaryIconSample.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
//			
//			enemyMercenaryIconSample.SetIconBasic(IconBasicDepth, yongbing_Sprite.ToString(), yongbing_Lv.ToString());
//			
//			enemyMercenaryIconSample.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
//			
//			GuyongbingList.Add(enemyMercenaryIconSample);
//		}
//	}
//
//	void ShowMibaoSkill()
//	{
//		my_MiBaoInfo = MiBaoGlobleData.Instance().G_MiBaoInfo;
//
//		int Pinzhi = 0;
//
//		for(int i = 0 ; i < my_MiBaoInfo.mibaoGroup.Count; i++)
//		{
//			if(my_MiBaoInfo.mibaoGroup[i].zuheId == mHYResourseBattleResp.zuheId)
//			{
//				for(int j = 0 ; j < my_MiBaoInfo.mibaoGroup[i].mibaoInfo.Count; j++)
//				{
//					if(my_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].level > 0 && !my_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].isLock)
//					{
//						Pinzhi += 1;
//					}
//				}
//			}
//		}
//
//		if(Pinzhi < 2||mHYResourseBattleResp.zuheId < 1 || mHYResourseBattleResp.zuheId > 4)
//		{
//			mLock.gameObject.SetActive(true);
//			
//			MibaoskillIcon.spriteName = "";
//			
//			return;
//		}
//		mLock.gameObject.SetActive(false);
//		
//		MiBaoSkillTemp mSkill = MiBaoSkillTemp.getMiBaoSkillTempByZuHe_Pinzhi (  mHYResourseBattleResp.zuheId,Pinzhi );
//		
//		MibaoskillIcon.spriteName = mSkill.icon.ToString();
//	}
//	public void Closed()
//	{
//		GameObject HuangYe = GameObject.Find ("HuangYe_TiaoZhanZhengRong(Clone)");
//		
//		if(HuangYe)
//		{
//			Destroy(HuangYe);
//		}
//		Destroy (this.gameObject);
//	}
//
//	public void mBack()
//	{
//		Destroy(this.gameObject);
//	}
}
