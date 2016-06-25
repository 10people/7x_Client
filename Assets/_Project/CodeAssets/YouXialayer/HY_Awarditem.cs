using UnityEngine;
using System.Collections;

public class HY_Awarditem : MYNGUIPanel {

	public NGUILongPress EnergyDetailLongPress;
	void Start () {
	
		EnergyDetailLongPress.LongTriggerType = NGUILongPress.TriggerType.Press;
		EnergyDetailLongPress.NormalPressTriggerWhenLongPress = false;
		EnergyDetailLongPress.OnLongPressFinish = OnCloseDetail;
		EnergyDetailLongPress.OnLongPress = OnEnergyDetailClick;

	}

	public void ShowTips()
	{

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
	public void OnEnergyDetailClick(GameObject go)//显示体力恢复提示
	{
		string id = this.GetComponent<UISprite>().spriteName;
		int a = int.Parse (id);
		
		CommonItemTemplate m_com = CommonItemTemplate.getCommonItemTemplateById(a);
	//	Debug.Log (" m_com.id = "+ m_com.id);
		ShowTip.showTip( m_com.id);
		//        RecoverToliCips.transform.localScale = new Vector3(1, 1, 1);
		//        Invoke("diseCoverTiLiClips", 1.5f);
	}
	private void OnCloseDetail(GameObject go)
	{
		ShowTip.close();
	}
}
