using UnityEngine;
using System.Collections;

public class OpenMiBaioskillManger : MonoBehaviour {
	public UILabel Remainlabel;

	bool add = true;

	public UISprite m_skillIcon1;
	public UISprite m_skillIcon2;

	public GameObject skillIcon1;
	public GameObject skillIcon2;
	public GameObject EffectRoot;

	public UISprite m_SkillName;

	public UILabel mSkillDesc;

	float m_color;
	
	int index;

	void Start () {
	
//		Init ();
	}
	

	void Update () {


		if(m_color >= 1 )
		{
			index = -1;
		}
		if(m_color < 0.5f )
		{
			index = 1;
		}
		m_color += index*Time.deltaTime*0.90f;
		Remainlabel.alpha = Mathf.Abs (m_color);

	}
	public void Init ()
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
	public void Close()
	{
		NewMiBaoManager.Instance ().InitUI ();
		MainCityUI.TryRemoveFromObjectList(this.gameObject);
		Destroy (this.gameObject);
	}
}
