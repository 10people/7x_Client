using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Debug_UIAnim : MonoBehaviour {

	public int m_ui_source_effect_id = 223;

	#region Mono

	// Use this for initialization
	void Start () {
		EffectTool.OpenUIEffect_ById( gameObject, m_ui_source_effect_id );
	}

	void OnDestroy(){
		EffectTool.CloseUIEffect_ById( gameObject, m_ui_source_effect_id );
	}

	#endregion



	#region Utilities

	#endregion
}