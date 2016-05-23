using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class JiaPianUIManegerment : MonoBehaviour {

	public UILabel JiapianNumber;

	public GameObject Fuwenitem;

	private List<int> mFuwenids = new List<int> ();

	public UIGrid mgrid;

	private List<GameObject> gameObjectLists = new List<GameObject> ();

	public List<UIEventListener> BtnList = new List<UIEventListener>(); 

	private int JiaPian  = -1;
	void Awake()
	{
		AddEventListener ();
		
	}
	public void AddEventListener()
	{
		BtnList.ForEach (item => SetBtnMoth(item));
	}
	void OnDestroy()
	{

	}
	void SetBtnMoth(UIEventListener mUIEventListener)
	{
		mUIEventListener.onClick = BtnManagerMent;
	}
	public void BtnManagerMent(GameObject mbutton)
	{
		int id = int.Parse(mbutton.name.Substring(7, mbutton.name.Length - 7));
		
		Debug.Log ("id.id = "+id);
		
		if(id < 1000)
		{
			DuiHuanBtn(id);
		}
		else
		{
			Close();
		}
	}
	void Start () {
	
		Init ();
	}

	void Update () {
	
		if(JiaPian != GetHuFuNum())
		{
			JiapianNumber.text = "甲片数量："+GetHuFuNum().ToString();
			JiaPian = GetHuFuNum();
		}

	}
	int GetHuFuNum ()
	{
		int jiapian = 0;
		for (int i = 0;i < BagData.Instance().m_bagItemList.Count;i ++)
		{
			if (BagData.Instance().m_bagItemList[i].itemId == 910003 && BagData.Instance().m_bagItemList[i].cnt > 0)
			{
				jiapian += BagData.Instance().m_bagItemList[i].cnt;
			}
		}
		return jiapian;
	}
	public void Init()
	{
		mFuwenids.Clear ();

		mFuwenids = FuWenDuiHuanTemplate.GetFuWenDuiHuanTemplate_IDs ();

		gameObjectLists = QXComData.CreateGameObjectList (Fuwenitem,mgrid,mFuwenids.Count,gameObjectLists);

		for(int i = 0 ; i < gameObjectLists.Count; i ++)
		{
			DuiHuanFuWenItem mDuiHuanFuWenItem = gameObjectLists[i].GetComponent<DuiHuanFuWenItem>();

			mDuiHuanFuWenItem.AwardId = mFuwenids[i];

			mDuiHuanFuWenItem.ButtonName.name = "Button_"+i.ToString();

			mDuiHuanFuWenItem.init();

			BtnList.Add(mDuiHuanFuWenItem.mEventListener);
		}
		AddEventListener ();
	}
	void DuiHuanBtn(int index)
	{
		DuiHuanFuWenItem mDuiHuanFuWenItem = gameObjectLists[index].GetComponent<DuiHuanFuWenItem>();
	
		mDuiHuanFuWenItem.MCost = JiaPian;
		mDuiHuanFuWenItem.DuiHuan ();
	}
	void Close()
	{
		Destroy (this.gameObject);
	}
}
