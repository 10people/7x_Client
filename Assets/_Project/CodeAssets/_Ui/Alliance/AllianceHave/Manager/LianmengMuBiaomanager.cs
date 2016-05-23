using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class LianmengMuBiaomanager : MYNGUIPanel {

	public GameObject mItem;
	public UIEventListener mEventListener;

	public UILabel mTitel;

	void Awake()
	{ 
		mEventListener.onClick = Close;
	}

	void Start () {
	
	}

	void Update () {
	
	}
	public void Init(string mstr)
	{
		mTitel.text = mstr;
		InitItems ();
	}
	void InitItems()
	{
		int count = LMTargetTemplate.getLMTargetTemplateCOunt ();
		for(int i = 0 ; i < count; i ++)
		{
			GameObject m_mItem = Instantiate(mItem) as GameObject;
			
			m_mItem.SetActive(true);
			
			m_mItem.transform.parent = mItem.transform.parent;
			
			m_mItem.transform.localPosition = new Vector3(0,126-78*i,0);
			
			m_mItem.transform.localScale = mItem.transform.localScale;

			MuBiaoItem mMuBiaoItem = m_mItem.GetComponent<MuBiaoItem>();

			mMuBiaoItem.Id = i+1;
			mMuBiaoItem.Init();
		}
	}
	public void Close(GameObject mbutton)
	{
		MainCityUI.TryRemoveFromObjectList (this.gameObject);
		Destroy (this.gameObject);
	}
	#region fulfil my ngui panel
	
	/// <summary>
	/// my click in my ngui panel
	/// </summary>
	/// <param name="ui"></param>
	public override void MYClick(GameObject ui)
	{
		
	}
	
	public override void MYMouseOver(GameObject ui)
	{
		
	}
	
	public override void MYMouseOut(GameObject ui)
	{
		
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
		
	}
	
	public override void MYelease(GameObject ui)
	{
		
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
		
	}
	#endregion
}
