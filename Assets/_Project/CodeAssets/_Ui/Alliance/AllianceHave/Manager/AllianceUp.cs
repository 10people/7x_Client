using UnityEngine;
using System.Collections;

public class AllianceUp : MonoBehaviour {

	public GameObject m_EffectObj;
	public UILabel UpInsrtruction;
	void Start () {
	
		UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_EffectObj, EffectIdTemplate.GetPathByeffectId(100020), null);

		StartCoroutine(WaitForUpgaade());
	}

	IEnumerator WaitForUpgaade()
	{
		yield return new WaitForSeconds(0.4f);
		UpInsrtruction.text = "联盟等级提升了1级！";
		
	}
	public void CloseUI()
	{

		Destroy (this.gameObject);

	}
}
