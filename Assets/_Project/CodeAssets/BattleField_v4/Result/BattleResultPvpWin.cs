using UnityEngine;
using System.Collections;

using qxmobile.protobuf;

public class BattleResultPvpWin : MonoBehaviour
{
	public UISprite spriteOld;

	public UISprite spriteNew;

	public UISprite spriteLabelOld;

	public UISprite spriteLabelNew;

	public GameObject newRecord;

	public UILabel labelOld;

	public UILabel labelNew;

	public UILabel labelRes;

	public UILabel labelWeiWang;

	public UILabel labelRMB;

	public GameObject layerNewRank;

	public UILabel labelNewRank;

	public UILabel labelAddrank;

	public GameObject effNewRank;

	public GameObject layerNewLevel;

	public UISprite spriteNewLevel;

	public GameObject effNewLevel;


	public static BaiZhanResultResp resp;


	public void refreshData(BaiZhanResultResp _resp)
	{
//		// 以下值是在战斗胜利的时候发送
//		optional int32 oldRank = 1;
//		optional int32 newRank = 2;
//		// 历史最高记录
//		optional int32 highest = 3;
//		// 获取到的威望值
//		optional int32 reciveWeiWang = 4;
//		// 是否超出今日所获得的最大威望值，true为超出
//		optional bool beyond = 5;
//		optional int32 oldJunXianLevel = 6;
//		optional int32 newJunXianLevel = 7;

//		Debug.Log ("旧排名：" + _resp.oldRank);
//		Debug.Log ("新排名：" + _resp.newRank);
//		Debug.Log ("老军衔：" + _resp.oldJunXianLevel);
//		Debug.Log ("新军衔：" + _resp.newJunXianLevel);

		Debug.Log ("PVP Result   " + _resp.oldRank + ", " + _resp.newRank + ", " + _resp.highest + ", " + _resp.yuanbao);

		newRecord.SetActive (_resp.newRank < _resp.highest);

		spriteOld.spriteName = "junxian" + _resp.oldJunXianLevel;

		spriteNew.spriteName = "junxian" + _resp.newJunXianLevel;

		spriteLabelOld.spriteName = "JunXian_" + _resp.oldJunXianLevel;

		spriteLabelNew.spriteName = "JunXian_" + _resp.newJunXianLevel;

		labelOld.text = "" + _resp.oldRank;

		labelNew.text = "" + _resp.newRank;

//		labelRes.text = "" + _resp.lostJianShe;

//		labelWeiWang.text = "" + _resp.reciveWeiWang;

		labelRMB.text = "" + _resp.yuanbao;

		//////////////////////////////////////////////////////////////////////////////

		layerNewRank.SetActive (false);

		if(_resp.newRank < _resp.oldRank)
		{
			layerNewRank.SetActive(true);

			labelNewRank.text = "" + _resp.oldRank;

			labelAddrank.text = "0";

//			StartCoroutine(newRankAction());
		}

		spriteNewLevel.spriteName = "junxian" + _resp.newJunXianLevel;

		layerNewLevel.SetActive (resp.oldJunXianLevel < resp.newJunXianLevel);

//		if(_resp.newRank < _resp.highest)
//		{
//			StartCoroutine(newLevelAction());
//		}
	}

	public void startAction()
	{
		if(resp.newRank < resp.oldRank)
		{
			StartCoroutine(newRankAction());
		}

		if(resp.oldJunXianLevel < resp.newJunXianLevel)
		{
			StartCoroutine(newLevelAction());
		}
	}

	IEnumerator newRankAction()
	{
		float tRank = (resp.oldRank - resp.newRank) / 20f;

		UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, effNewRank, EffectTemplate.getEffectTemplateByEffectId( 100191 ).path);
		
		for(int i = 0; i < 20; i++)
		{
			int nRank = (int)(resp.oldRank - tRank * i);

			if(nRank < resp.newRank)
			{
				nRank = resp.newRank;
			}

			labelNewRank.text = "" + nRank;

			int aRank = (int)(tRank * i);

			if(aRank > resp.oldRank - resp.newRank)
			{
				aRank = resp.oldRank - resp.newRank;
			}

			labelAddrank.text = "" + aRank;

			yield return new WaitForEndOfFrame();
		}

		labelNewRank.text = "" + resp.newRank;

		labelAddrank.text = "" + (resp.oldRank - resp.newRank);

		yield return new WaitForSeconds(5.33f);

		TweenAlpha.Begin (layerNewRank, .5f, 0);

		yield return new WaitForSeconds(.5f);

		layerNewRank.SetActive (false);
	}

	IEnumerator newLevelAction()
	{
		UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, effNewLevel, EffectTemplate.getEffectTemplateByEffectId( 100191 ).path);

		yield return new WaitForSeconds(6f);

		TweenAlpha.Begin (layerNewLevel, .5f, 0);
		
		yield return new WaitForSeconds(.5f);

		layerNewLevel.SetActive (false);
	}

}
