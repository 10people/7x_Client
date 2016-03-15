using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Debug_UIMultiAnim : MonoBehaviour {

	public int m_ui_source_effect_id = 223;

	public int m_ui_mirror_effect_id = 224;

	public int m_ui_fx_id = 225;

	#region Mono

	// Use this for initialization
	void Start () {
		EffectTool.OpenMultiUIEffect_ById( gameObject, m_ui_source_effect_id, m_ui_mirror_effect_id, m_ui_fx_id );
	}

	void OnDestroy(){
		EffectTool.CloseMultiUIEffect_ById( gameObject, m_ui_source_effect_id, m_ui_mirror_effect_id, m_ui_fx_id );
	}

	#endregion



	#region Utilities

	#endregion
}