using UnityEngine;
using System.Collections;

public class OpenMiBaioskillManger : MonoBehaviour {
	public UILabel Remainlabel;

	bool add = true;

	public GameObject skillIcon1;
	public GameObject skillIcon2;
	public GameObject EffectRoot;
	void Start () {
	
	}
	

	void Update () {


//		float aph = 0f;
//	
//		if(aph < 1f)
//		{
//
//			aph += 0.01f;
//		}
//		Remainlabel.alpha = aph;
	}
	void OnEnable()
	{
		Init ();
		//StartCoroutine ("ShowEffect");
	}
	void Init ()
	{
		StartCoroutine ("showOpenMiBaoskillEffect");
	}
	IEnumerator showOpenMiBaoskillEffect()
	{
		yield return new WaitForSeconds (0.1f);
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
		UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,skillIcon1.gameObject,EffectIdTemplate.GetPathByeffectId(100108));
		//	UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,OPenSkillICon.gameObject,EffectIdTemplate.GetPathByeffectId(100178));
		yield return new WaitForSeconds (0.5f);
		//Remainlabel.alpha = 0f;
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
		UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,skillIcon1.gameObject,EffectIdTemplate.GetPathByeffectId(100107));
		UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,skillIcon1.gameObject,EffectIdTemplate.GetPathByeffectId(620215));
	}
	IEnumerator ShowEffect()
	{
		yield return new WaitForSeconds (0.1f);
		//EffectTool.OpenUIEffect_ById (skillIcon1,229,230,225);
		//EffectTool.OpenUIEffect_ById (skillIcon2,229,230,225);
	}
	void OnDisable()
	{
		UI3DEffectTool.ClearUIFx(skillIcon1.gameObject);
	}
}
