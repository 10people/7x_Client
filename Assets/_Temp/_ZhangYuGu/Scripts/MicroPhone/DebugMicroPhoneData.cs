using UnityEngine;
using System.Collections;

public class DebugMicroPhoneData : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log( "DebugMicroPhoneData.Start()" );

		AudioClip m_clip = Microphone.Start("Built-in Microphone", true, 3, 8000);

//		int t_length = m_clip.channels * m_clip.samples;

		Debug.Log( m_clip.channels + ", " + m_clip.samples + ": " + m_clip.channels * m_clip.samples );

		Debug.Log( "DebugMicroPhoneData.Start() Done." );
	}
}
