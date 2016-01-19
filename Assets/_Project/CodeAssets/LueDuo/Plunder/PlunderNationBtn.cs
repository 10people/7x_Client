using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PlunderNationBtn : MonoBehaviour {

	private GuoInfo nationInfo;

	public UISprite btnBg;
	public UILabel nationName;

	private Vector3 pos;
	private int btnIndex;

	/// <summary>
	/// Ins it nation button.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	/// <param name="tempIndex">Temp index.</param>
	/// <param name="tempPos">Temp position.</param>
	public void InItNationBtn (GuoInfo tempInfo,int tempIndex,Vector3 tempPos)
	{
		nationInfo = tempInfo;
		btnIndex = tempIndex;
		pos = tempPos;
		
		btnBg.spriteName = "nation_" + tempInfo.guojiaId;
		nationName.text = tempIndex == 0 || tempIndex == 1 ? 
			NameIdTemplate.GetName_By_NameId (tempInfo.guojiaId) + "(敌)" : NameIdTemplate.GetName_By_NameId (tempInfo.guojiaId);
	}

	/// <summary>
	/// Buttons the animation.
	/// </summary>
	/// <param name="isAnimate">If set to <c>true</c> is animate.</param>
	public void BtnAnimation (bool isAnimate)
	{	
		//按钮背景颜色显示
		btnBg.color = isAnimate ? Color.white : Color.gray;
		nationName.color = isAnimate ? Color.white : Color.gray;
		
		float time = 0.5f;
		float size = 1.1f;
		
		Hashtable scale = new Hashtable ();
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		scale.Add ("time",time);
		scale.Add ("islocal",true);
		scale.Add ("scale",isAnimate ? new Vector3(size,size,size) : Vector3.one);
		iTween.ScaleTo (this.gameObject,scale);
		
		Hashtable move = new Hashtable ();
		move.Add ("easetype",iTween.EaseType.easeOutQuart);
		move.Add ("time",time);
		move.Add ("islocal",true);
		move.Add ("position",isAnimate ? pos - new Vector3(5,0,0) : pos);
		iTween.MoveTo (this.gameObject,move);
	}
}
