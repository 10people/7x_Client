using UnityEngine;
using System.Collections;

public class CrossDis : MonoBehaviour {

	public UILabel wanSheng;

	public UILabel shengLi;

	public UILabel xianSheng;

	private string ws;

	private string sl;

	private string xs;

	public EventHandler sureBtn;

	public GameObject window;

	private Vector3 m_pos1;
	private float m_time;

	void Start ()
	{
		ShowDis ();
		
		sureBtn.m_handler += ClickBtn;
	}

//	public void GetPingJiaInFo (Vector3 pos1,Vector3 pos2,float time)
//	{
//		m_pos1 = pos1;
//		m_time = time;
//		Hashtable move = new Hashtable ();
//		move.Add ("position",pos2);
//		move.Add ("islocal",true);
//		move.Add ("time",time);
//		move.Add ("easetype",iTween.EaseType.easeOutQuad);
//		iTween.MoveTo (window,move);
//		
//		Hashtable scale = new Hashtable ();
//		scale.Add ("time",time);
//		scale.Add ("scale",Vector3.one);
//		scale.Add ("easetype",iTween.EaseType.easeOutQuad);
//		iTween.ScaleTo (window,scale);
//	}

	void ShowDis ()
	{
		ws = DescIdTemplate.GetDescriptionById (201);
		sl = DescIdTemplate.GetDescriptionById (202);
		xs = DescIdTemplate.GetDescriptionById (203);
	}

	void ClickBtn (GameObject tempObj)
	{
//		Hashtable move = new Hashtable ();
//		move.Add ("position",m_pos1);
//		move.Add ("islocal",true);
//		move.Add ("time",m_time);
//		move.Add ("easetype",iTween.EaseType.linear);
//		iTween.MoveTo (window,move);
//		
//		Hashtable scale = new Hashtable ();
//		scale.Add ("time",m_time);
//		scale.Add ("scale",Vector3.zero);
//		scale.Add ("easetype",iTween.EaseType.linear);
//		iTween.ScaleTo (window,scale);
		Destroy (this.gameObject);
	}

	void Update ()
	{
		wanSheng.text = ws;
		shengLi.text = sl;
		xianSheng.text = xs;
	}
}
