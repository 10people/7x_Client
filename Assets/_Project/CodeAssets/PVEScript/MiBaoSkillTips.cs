using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class MiBaoSkillTips : MYNGUIPanel {

	public NGUILongPress EnergyDetailLongPress1;
	public int Skillid;
	public UISprite Skillicon;
	public string mibao_name = "";
	void Awake()
	{
		EnergyDetailLongPress1.LongTriggerType = NGUILongPress.TriggerType.Press;
		EnergyDetailLongPress1.NormalPressTriggerWhenLongPress = false;
		EnergyDetailLongPress1.OnLongPressFinish = OnCloseDetail;
		EnergyDetailLongPress1.OnLongPress = OnEnergyDetailClick1;
	}
	void Start () {
	
	}
	public void Init()
	{

		Skillicon.spriteName = mibao_name;
	}
	private void OnCloseDetail(GameObject go)
	{
		ShowTip.close();
	}
	public void OnEnergyDetailClick1(GameObject go)//显示体力恢复提示
	{
		if(Skillid <= 0)
		{
			return;
		}
		MiBaoSkillTemp mMiBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempBy_Icon (Skillid);
		int id = mMiBaoSkillTemp.id;

		ShowTip.showTip (id);
	}
	void Update () {
	
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
