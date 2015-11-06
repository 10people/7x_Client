using UnityEngine;
using System.Collections;

public class CreateRoleTips : MonoBehaviour {

	public GameObject winObj;
	
	public UILabel desLabel;
	
	public void ScaleAnim ()
	{
		Hashtable scale = new Hashtable ();
		
		scale.Add ("scale",Vector3.one);
		scale.Add ("time",0.5f);
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		scale.Add ("islocal",true);
		
		iTween.ScaleTo (winObj,scale);
	}
	
	public void GetDesLabelType (string tempMsg)
	{
		desLabel.text = tempMsg;
	}
	
	public void SureBtn ()
	{
		//重新随机一个名字
		NewSelectRole.selectRole.RandomNameBtn ();
		Destroy (this.gameObject);
	}
}
