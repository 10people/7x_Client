using UnityEngine;
using System.Collections;

public class createPveUI : MonoBehaviour {

	float mScale;
	Vector3 scale1;
	Vector3 scale2;
	float time = 0.5f;
	public GameObject sup;
	void Start () {
	
		mScale = 0.0f;
		scale1 = new Vector3 (1f,1f,1);
		scale2 = new Vector3 (0f,0f,0f);
		this.transform.localScale = new Vector3(mScale,mScale,mScale);
		Create ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void delet()
	{
		CityGlobalData.PveLevel_UI_is_OPen = false;
		EnterGuoGuanmap.Instance().ShouldOpen_id = 0;
		//MapData.mapinstance.OpenEffect();
		PassLevelBtn.Instance().OPenEffect ();
		MapData.mapinstance.GuidLevel = 0;
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
		MapData.mapinstance.ShowYinDao = true;

		iTween.ScaleTo (this.gameObject,iTween.Hash("scale",scale2,"time",time));
		MapData.mapinstance.UI_exsit = false;
		Destroy (sup,time);
	}
	public void Create()
	{
		iTween.ScaleTo (this.gameObject,iTween.Hash("scale",scale1,"time",time));
	}
}
