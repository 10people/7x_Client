using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class MiBaoStarUpUI : MonoBehaviour {

	public MibaoInfo MiBao1;
	public MibaoInfo MiBao2;

	public GameObject EffRoot;

	public UILabel LifeLabel;

	public UILabel LifeAddLabel;

	public UILabel GongJiLabel;

	public UILabel GongJiAddLabel;

	public UILabel FangyuLabel;

	public UILabel FangyuAddLabel;

	public UISprite Star1;
	public UISprite Star2;

	public UITexture MiBaoicon1;
	public UITexture MiBaoicon2;

	public UILabel m_ShanSuoLabel;
	float m_color;

	int index;
	void Start () {
	
	}
	void Update()
	{
		if(m_color >= 1 )
		{
			index = -1;
		}
		if(m_color < 0.5f )
		{
			index = 1;
		}
		m_color += index*Time.deltaTime*0.90f;
		m_ShanSuoLabel.alpha = Mathf.Abs (m_color);
	}
	GameObject mCamer;
	public void Init()
	{
		Global.m_isZhanli = true;//当Global.m_isZhanli为true的时候  关闭播放战力提升

	    mCamer = GameObject.FindGameObjectWithTag ("MiBao");
//		mCamer.GetComponent<Camera> ().enabled = true;
//		UIBackgroundEffect mUIBackgroundEffect = mCamer.GetComponent<UIBackgroundEffect>();
//		if(mUIBackgroundEffect == null )
//		{
//			mCamer.AddComponent<UIBackgroundEffect>();
//		}

//		UI2DTool.Instance.AddTopUI (this.gameObject);

		EffectTool.SetUIBackgroundEffect (mCamer.gameObject, true);

//		LifeLabel.text =  MiBao2.shengMing.ToString ();
//		GongJiLabel.text =  MiBao2.gongJi.ToString ();
//		FangyuLabel.text =  MiBao2.fangYu.ToString ();

		LifeAddLabel.text = MiBao2.shengMing.ToString ()+"([-][10ff2b]"+(MiBao2.shengMing-MiBao1.shengMing).ToString () + "↑[-])";
		GongJiAddLabel.text = MiBao2.gongJi.ToString ()+"([-][10ff2b]"+(MiBao2.gongJi-MiBao1.gongJi).ToString () + "↑[-])";
		FangyuAddLabel.text = MiBao2.fangYu.ToString ()+"([-][10ff2b]"+(MiBao2.fangYu-MiBao1.fangYu).ToString () + "↑[-])";
		MiBaoXmlTemp mmibaoxml = MiBaoXmlTemp.getMiBaoXmlTempById (MiBao1.miBaoId);
		MiBaoicon1.mainTexture = (Texture)Resources.Load (Res2DTemplate.GetResPath (Res2DTemplate.Res.MIBAO_BIGICON) + mmibaoxml.icon.ToString ());
		MiBaoicon2.mainTexture = (Texture)Resources.Load (Res2DTemplate.GetResPath (Res2DTemplate.Res.MIBAO_BIGICON) + mmibaoxml.icon.ToString ());

		UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,EffRoot.gameObject,EffectIdTemplate.GetPathByeffectId(620214));
		ShowStar ();
		StartCoroutine ("OPenZhanliShow");
	}
	void ShowStar()
	{
		for(int i = 0 ; i < MiBao1.star; i ++)
		{
			GameObject StarTemp = Instantiate(Star1.gameObject) as GameObject;
			
			StarTemp.SetActive(true);
			
			StarTemp.transform.parent = Star1.gameObject.transform.parent;
			
			StarTemp.transform.localPosition = new Vector3(0,-23+13*i,0);
			
			StarTemp.transform.localScale = Star1.gameObject.transform.localScale;

		}
		for(int i = 0 ; i < MiBao2.star; i ++)
		{
			GameObject StarTemp = Instantiate(Star2.gameObject) as GameObject;
			
			StarTemp.SetActive(true);
			
			StarTemp.transform.parent = Star2.gameObject.transform.parent;
			
			StarTemp.transform.localPosition = new Vector3(0,-23+13*i,0);
			
			StarTemp.transform.localScale = Star2.gameObject.transform.localScale;

		}
	}
	IEnumerator OPenZhanliShow()
	{
		yield return new WaitForSeconds (1.5f);
		Debug.Log ("显示战力提升！");
		Global.m_isZhanli = false;
	}
	public void Close()
	{
		Global.m_isZhanli = false;
		EffectTool.SetUIBackgroundEffect (mCamer.gameObject, false);
		MainCityUI.TryRemoveFromObjectList (this.gameObject);
		Destroy (this.gameObject);
	}
}
