using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class LDNationBtn : MonoBehaviour {

	public UISprite btnBg;

	public UILabel nationName;

	private GuoInfo nationInfo;

	public GameObject ldManagerObj;

	private Vector3 pos;

	private bool isOnThisNation;

	private int btnIndex;

	public void GetNationBtnInfo (GuoInfo tempInfo,int index,Vector3 tempPos)
	{
//		Debug.Log ("国家id：" + tempInfo.guojiaId + "||" + tempInfo.hate + "||" + index);
		nationInfo = tempInfo;
		btnIndex = index;
		pos = tempPos;

		btnBg.spriteName = "nation_" + tempInfo.guojiaId;

		string nameStr = "";
		if (index == 0 || index == 1)
		{
			nameStr = NameIdTemplate.GetName_By_NameId (tempInfo.guojiaId) + "(敌)";
		}
		else
		{
			nameStr = NameIdTemplate.GetName_By_NameId (tempInfo.guojiaId);
		}

		nationName.text = nameStr;
	}

	void OnClick ()
	{
		if (!isOnThisNation)
		{
			if (!LueDuoData.Instance.IsStop)
			{
				LueDuoData.Instance.IsStop = true;

				LueDuoData.Instance.SetNationId = nationInfo.guojiaId;
				
				LDAllianceManager.isRefreshToTop = true;
				LueDuoManager ldManager = ldManagerObj.GetComponent<LueDuoManager> ();
				ldManager.BtnScaleAnimation (btnIndex);
				Debug.Log ("LueDuoData.Instance.GetAllianceStartPage:" + LueDuoData.Instance.GetAllianceStartPage);
				LueDuoData.Instance.LueDuoNextReq (LueDuoData.ReqType.Alliance,
				                                   nationInfo.guojiaId,
				                                   -1,
				                                   1,
				                                   LueDuoData.Direction.Default);
			}
		}
	}

	public void BtnAnimation (bool isAnimate)
	{
		isOnThisNation = isAnimate;

		//按钮背景颜色显示
		btnBg.color = isAnimate ? Color.white : Color.gray;
		nationName.color = isAnimate ? Color.white : Color.gray;

		float time = 0.5f;
		float size = 1.1f;

		Hashtable scale = new Hashtable ();
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		scale.Add ("time",time);
		scale.Add ("islocal",true);
		if (isAnimate)
		{
			scale.Add ("scale",new Vector3(size,size,size));
		}
		else
		{
			scale.Add ("scale",Vector3.one);
		}
		iTween.ScaleTo (this.gameObject,scale);
		
		Hashtable move = new Hashtable ();
		move.Add ("easetype",iTween.EaseType.easeOutQuart);
		move.Add ("time",time);
		move.Add ("islocal",true);
		if (isAnimate)
		{
			move.Add ("position",pos - new Vector3(5,0,0));
		}
		else
		{
			move.Add ("position",pos);
		}
		iTween.MoveTo (this.gameObject,move);
	}
}
