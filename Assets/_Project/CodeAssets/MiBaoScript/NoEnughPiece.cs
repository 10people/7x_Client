using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class NoEnughPiece : MonoBehaviour {

[HideInInspector]public MibaoInfo  my_mibao ;  

	public GameObject ChapterBtns;
	public UILabel Dot_inLv;
	//public UILabel SuipianName;

	public void Init()
	{
		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(my_mibao.miBaoId);
		MiBaoSuipianXMltemp mMiBaoSuipianXMltemp = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid (mMiBaoXmlTemp.tempId);
		MiBaoDiaoLuoXmlTemp mMiBaoDiaoLuoXmlTemp = MiBaoDiaoLuoXmlTemp.getMiBaoDiaoLuoXmlTempBysuipian_id (mMiBaoSuipianXMltemp.id);
		string Pvelv = mMiBaoDiaoLuoXmlTemp.legendPveId;

//		Debug.Log("int.my_mibao.miBaoId(s[i])"+my_mibao.miBaoId);
//		Debug.Log("int.mMiBaoXmlTemp.tempId(s[i])"+mMiBaoXmlTemp.tempId);
//		Debug.Log("int.Pvelv(MiBaoSuipianXMltemp.id[i])"+mMiBaoSuipianXMltemp.id);
//		Debug.Log("int.Pvelv(s[i])"+Pvelv);
		//用，解析字符串。

		//SuipianName.text = NameIdTemplate.GetName_By_NameId (mMiBaoXmlTemp.nameId);
		string[] s = Pvelv.Split(new char[] { ',' });
		for(int i = 0;  i < s.Length; i++)
		{
			int j = 0 ;
			if(!int.TryParse(s[i], out j))
			{
				Dot_inLv.gameObject.SetActive(true);
				Dot_inLv.text = s[i];
				//Debug.Log("int.Parse(s[i])"+s[i]);
				return;
			}

			LegendPveTemplate mLg_PveTempTemplate = LegendPveTemplate.GetlegendPveTemplate_By_id(int.Parse(s[i]));
			NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mLg_PveTempTemplate.smaName);
			GameObject mChapterBtns = Instantiate(ChapterBtns)as GameObject;
			mChapterBtns.SetActive(true);
			mChapterBtns.transform.parent = ChapterBtns.transform.parent;
			mChapterBtns.transform.localPosition = new Vector3((212*i - (s.Length - 1)*106),8,0);
			mChapterBtns.transform.localScale = ChapterBtns.transform.localScale;
			ChapterBtn mChapterBtn = mChapterBtns.GetComponent<ChapterBtn>();
			mChapterBtn.Chapters = mLg_PveTempTemplate.bigId;
			mChapterBtn.lv_name = mNameIdTemplate.Name;
			mChapterBtn.m_mibao = my_mibao;
			mChapterBtn.Init();
		}

	}
	public void BackFun()
	{
		Destroy (this.gameObject);
	}
}
