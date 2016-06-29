using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class QCLPreViewManager : MonoBehaviour {

	public List<UIEventListener> BtnList = new List<UIEventListener>(); 

	[HideInInspector]
	public GameObject IconSamplePrefab;

	public GameObject AwardRoot;

	public GameObject AwardTemp;

	public UILabel mAddIntruction;
	void Awake()
	{
		BtnList.ForEach (item => SetBtnMoth(item));
		
	}
	void SetBtnMoth(UIEventListener mUIEventListener)
	{
		mUIEventListener.onClick = BtnManagerMent;
	}
	
	void Start () {
		
	}
	
	public void Init()
	{
		List <int > NendShowLayerNumber = ChonglouPveTemplate.Get_Key_Layer ();
	
		int count = NendShowLayerNumber.Count > 14 ? 14:NendShowLayerNumber.Count;
	
		for (int n = 0; n < count; n++)
		{
			GameObject LayerAwardTemp = Instantiate(AwardTemp) as GameObject;
			LayerAwardTemp.SetActive(true);
			LayerAwardTemp.transform.parent = AwardRoot.transform;
			LayerAwardTemp.GetComponent<PreViewTemp>().Layer = NendShowLayerNumber[n];
			LayerAwardTemp.GetComponent<PreViewTemp>().Init ();
		}
		AwardRoot.GetComponent<UIGrid> ().repositionNow = true;

		VipTemplate mvip = VipTemplate.GetVipInfoByLevel (5);

		int x = (mvip.ExpAdd * 100 - 100);

		mAddIntruction.text = "V5及以上特权可享受"+x.ToString()+"%经验加成";
	}
	
	public void BtnManagerMent(GameObject mbutton)
	{
		Close ();
	}
	
	void Close()
	{
		Destroy (this.gameObject);
	}
}
