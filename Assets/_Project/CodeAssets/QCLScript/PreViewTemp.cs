using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PreViewTemp : MonoBehaviour {

	List<int> First_t_items = new List<int>();
	List<int> t_items = new List<int>();

	List<int> First__itemnunber1 = new List<int>();

	List<int> t_itemnunber2 = new List<int>();

	[HideInInspector]
	public GameObject IconSamplePrefab;

	public UILabel LayerNunber;

	[HideInInspector]public int Layer;

	private int numPara;

	public GameObject AwardRoot;
	void Start () {
	
	}

	void Update () {
	
	}
	public void Init()
	{
		LayerNunber.text = "第"+Layer.ToString()+"层";

		ChonglouPveTemplate mCL = ChonglouPveTemplate.Get_QCL_PVETemplate_By_Layer (Layer);
		
		string m_Award = mCL.awardShow;

		if(Layer == 15)
		{
			Debug.Log("m_Award = "+m_Award);
		}
		char[] t_items_delimiter = { '#' };
		
		char[] t_item_id_delimiter = { ':' };
		
		string[] t_item_strings = m_Award.Split(t_items_delimiter);
		
		for (int i = 0; i < t_item_strings.Length; i++)
		{
			string t_item = t_item_strings[i];
			
			string[] t_finals = t_item.Split(t_item_id_delimiter);
			
			if(t_finals[1] != "" && !t_items.Contains(int.Parse(t_finals[1])))
			{
				t_items.Add(int.Parse(t_finals[1]));
				t_itemnunber2.Add(int.Parse(t_finals[2]));
			}
		}
		
		string[] First_item_strings =m_Award.Split(t_items_delimiter);
		
		for (int i = 0; i < First_item_strings.Length; i++)
		{
			string t_item = First_item_strings[i];
			
			string[] t_finals = t_item.Split(t_item_id_delimiter);
			
			if(t_finals[1] != "" && !First_t_items.Contains(int.Parse(t_finals[1])))
			{
				First_t_items.Add(int.Parse(t_finals[1]));
				First__itemnunber1.Add(int.Parse(t_finals[2]));
			}
		}
		numPara = t_items.Count + First_t_items.Count;

		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleCallBack);
		}
		else
		{
			WWW temp = null;
			OnIconSampleCallBack(ref temp, null, IconSamplePrefab);
		}
	}
	private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		AwardRoot.GetComponent<UIGrid> ().enabled = false;
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
		List<AwardTemp> mAwardTemp = new List<AwardTemp> ();

//		Debug.Log ("numPara = "+numPara);
//		for (int n = 0; n < First_t_items.Count; n++) {
//			Debug.Log("First_t_items[n] = "+First_t_items[n]);
//		}
//		for (int n = 0; n < t_items.Count; n++) {
//			Debug.Log("t_items[n] = "+First_t_items[n]);
//		}
		int lenght = numPara;
		if(lenght > 4) lenght = 4;

		for (int n = 0; n < lenght; n++)
		{
	
			int nuber = 0;
			int number = 0;
			if(n < First_t_items.Count)
			{
				nuber = First__itemnunber1[n] ;
				number = First_t_items[n];
			}
			else
			{
				nuber = t_itemnunber2[n-First_t_items.Count];
				number = t_items[n-First_t_items.Count];
			}
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			
			iconSampleObject.SetActive(true);
			
			iconSampleObject.transform.parent = AwardRoot.transform;

			//FirstAwardPos = iconSampleObject.transform.localPosition;
			
			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			var iconSpriteName = "";
			
			CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(number);
			
			iconSpriteName = mItemTemp.icon.ToString();
			
			iconSampleManager.SetIconType(IconSampleManager.IconType.item);
			
			NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(number);
			
			string mdesc = DescIdTemplate.GetDescriptionById(number);
			
			var popTitle = mNameIdTemplate.Name;
			
			var popDesc = mdesc;
			iconSampleObject.transform.localPosition = new Vector3(n * 70 - (lenght - 1) * 35, 0, 0);

			//Debug.Log("iconSampleObject.transform.localPosition.x = "+iconSampleObject.transform.localPosition.x);
			iconSampleManager.SetIconByID(mItemTemp.id, nuber.ToString(), 3);
			iconSampleManager.SetIconPopText(number, popTitle, popDesc, 1);
			iconSampleObject.transform.localScale = new Vector3(0.6f,0.6f,1);
			if(n < First_t_items.Count)
				iconSampleManager.FirstWinSpr.gameObject.SetActive(true);
		}
		
	}
}
