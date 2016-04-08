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
	}

}
