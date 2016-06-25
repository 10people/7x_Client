using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class JiBaiAward : MYNGUIPanel {
	public UISprite icon;

	public UISprite IsChoosed;

	public GameObject Box;

	public int Itemid;

	public Award mAwardinfo;

	public string PinZhi = "pinzhi";

	public UISprite PinZhikuang;

	public NGUILongPress EnergyDetailLongPress1;

	public UILabel Num;

	void Awake()
	{
		EnergyDetailLongPress1.LongTriggerType = NGUILongPress.TriggerType.Press;
		EnergyDetailLongPress1.NormalPressTriggerWhenLongPress = false;
		EnergyDetailLongPress1.OnLongPressFinish = OnCloseDetail;
		EnergyDetailLongPress1.OnLongPress = OnEnergyDetailClick1;
	}
	void Start () {
	
	}
	
	private void OnCloseDetail(GameObject go)
	{
		ShowTip.close();
	}
	public void OnEnergyDetailClick1(GameObject go)//显示体力恢复提示
	{
		
		int Star_Id = mAwardinfo.itemId;
//		Debug.Log ("Star_Id = "+Star_Id);
		ShowTip.showTip (Star_Id);
	}

	public void Init()
	{
		CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(mAwardinfo.itemId);
		Num.text = mAwardinfo.itemNumber.ToString ();
		icon.spriteName = mItemTemp.icon.ToString();
		PinZhikuang.spriteName = mItemTemp.color !=0 ?PinZhi+(mItemTemp.color-1).ToString():"";
		if (mAwardinfo.miBaoStar == 0) {
			Box.SetActive (false);
		} else
		{
			Box.SetActive (true);
		}
	}
	public void GetDataByChouJiang()
	{
		StartCoroutine (ShowEffect());
	}
	IEnumerator ShowEffect()
	{
		float time = 1f;
		IsChoosed.gameObject.SetActive (true);
		int i = 0;
		while(time>0)
		{
			time -= 0.25f;
			i ++;
			if(i%2 == 0)
			{
				IsChoosed.alpha = 1;
			}
			else
			{
				IsChoosed.alpha = 0;
			}
			yield return new WaitForSeconds(0.2f);
		}
		IsChoosed.alpha = 0;
		IsChoosed.gameObject.SetActive (false);
		Box.SetActive (true);
	}
	public void  ChangeBoxPostion(GameObject m_obg)
	{
		m_obg.transform.localPosition = this.transform.localPosition;
	}
	public void CHoseShowAnimation()
	{
		StartCoroutine (ShowAnimation());
	}
	IEnumerator ShowAnimation()
	{
		IsChoosed.gameObject.SetActive (true);
		yield return new WaitForSeconds(0.01f);
		IsChoosed.gameObject.SetActive (false);
	}
	#region fulfil my ngui panel
	
	/// <summary>
	/// my click in my ngui panel
	/// </summary>
	/// <param name="ui"></param>
	public override void MYClick(GameObject ui)
	{
		
	}
	
	public override void MYMouseOver(GameObject ui)
	{
		
	}
	
	public override void MYMouseOut(GameObject ui)
	{
		
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
		
	}
	
	public override void MYelease(GameObject ui)
	{
		
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
		
	}
	
	#endregion
}
