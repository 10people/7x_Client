using UnityEngine;
using System.Collections;

public class SceneFxConfig : MonoBehaviour {

	public enum SceneFxType{
		None,
		Torch_Fire,
		Range_Smoke,
		Flowing_Fog,
	}

	public SceneFxType m_scene_fx_type;

	#region Mono

	void Awake(){
		if( true ){
			return;
		}

		if( Quality_SceneFx.IsSceneFxNone() ){
			SetActive( false );

			return;
		}

		switch( m_scene_fx_type ){
		case SceneFxType.None:
			Debug.LogError( "Error, Type not setted." );

			SetActive( false );
			break;

		case SceneFxType.Torch_Fire:
			SetActive( false );
			break;

		case SceneFxType.Range_Smoke:
//			if( QualityTool.IsSceneFxHigh() ){
//				SetActive( false );
//			}

			SetActive( false );
			break;

		case SceneFxType.Flowing_Fog:
			SetActive( false );
			break;
		}
	}

	#endregion



	#region Utilities

	private void SetActive( bool p_is_active ){
		gameObject.SetActive( p_is_active );
	}

	#endregion
}
