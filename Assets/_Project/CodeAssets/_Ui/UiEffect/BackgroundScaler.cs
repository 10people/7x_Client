using UnityEngine;
using System.Collections;

public class BackgroundScaler : MonoBehaviour {

	public int m_designed_w		= 960;

	public int m_designed_h		= 640;

	#region Mono

	void Start(){
		ScaleBg();
	}

	void OnEnable(){
//		UpdateMainC( false );
	}

	void OnDisable(){
//		UpdateMainC( true );
	}

	void OnDestroy(){
		
	}

	#endregion



	#region UpdateMainC

//	private void UpdateMainC( bool p_enable ){
//		CameraHelper.SetMainCamera( p_enable );
//	}

	#endregion



	#region Utilities

	public bool IsFullScreenUI(){
		UITexture t_tex = GetComponent<UITexture>();

		if( t_tex != null ){
			return true;
		}

		return false;
	}

	private void ScaleBg(){
		float t_s = ScreenHelper.GetBGScale( m_designed_w, m_designed_h );

		{
			UITexture t_tex = GetComponent<UITexture>();
			
			if( t_tex != null ){
				t_tex.width = (int)( m_designed_w * t_s ) + 6;
				
				t_tex.height = (int)( m_designed_h * t_s ) + 4;

				return;
			}
		}


		{
			UISprite t_sprite = GetComponent<UISprite>();

			if( t_sprite != null ){
				t_sprite.width = (int)( m_designed_w * t_s ) + 6;
				
				t_sprite.height = (int)( m_designed_h * t_s ) + 4;
				
				return;
			}
		}

		Debug.LogError( "Error, no UITexture or UISprite found." );
	}

	#endregion
}
