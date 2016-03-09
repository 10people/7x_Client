using UnityEngine;
using System.Collections;

public class BoxChangeScale : MonoBehaviour {

	float m_scale = 0.8f;
	bool ChangeBig = true;
	void Start () {
	
		this.transform.localScale = new Vector3(m_scale,m_scale,m_scale);
	}
	
	// Update is called once per frame
	void Update () {
	
	
	
	}
	void OnEnable()
	{
//		Debug.Log("=========1");
//		Debug.Log("enabled="+enabled);
		StartCoroutine ("BtnShake");
	}
	void OnDisable()
	{
//		Debug.Log("=========2");
//		Debug.Log("enabled="+enabled);
		StopCoroutine("BtnShake");
		this.transform.localScale = Vector3.one;
		this.transform.localEulerAngles = Vector3.zero;
	}
	IEnumerator BtnShake()
	{
		while(m_scale <= 1.1f)
		{
			m_scale += Time.deltaTime*0.4f;

			this.transform.localScale = new Vector3(m_scale,m_scale,m_scale);

			yield return new WaitForSeconds (0.001f);
			
			//iTween.ShakePosition(this.gameObject,new Vector3(0.02f,0.001f,0),1);

		}
		while(m_scale > 1.1f)
		{
			//iTween.ShakePosition(this.gameObject,new Vector3(0.02f,0.001f,0),1);
			iTween.ShakeRotation(this.gameObject,new Vector3(0,0,20f),0.8f);
			yield return new WaitForSeconds (2.5f);
		}
	}
}
