using UnityEngine;
using System.Collections;

public class UILoadingFx : MonoBehaviour {

	public UISlider m_target_slider = null;

	public int m_relative_pos 		= 370;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		UpdatePos();
	}

	public void UpdatePos(){
		if( m_target_slider == null ){
			return;
		}

		gameObject.transform.localPosition = new Vector3( m_target_slider.value * m_relative_pos * 2 - m_relative_pos, 0, 0 );
	}
}
