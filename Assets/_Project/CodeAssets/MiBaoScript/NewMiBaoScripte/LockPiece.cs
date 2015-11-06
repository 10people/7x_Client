using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class LockPiece : MonoBehaviour ,SocketProcessor{

	[HideInInspector]public MibaoInfo  my_Diaoluomibao ;  

	public UILabel Dot_inLv;

	public UILabel Tips;

	public GameObject ChapterBtns;

	public GameObject ScrollPanle;

	public UILabel MiBaoName;

	public GameObject ChildparentGB;

	void Awake()
	{
		SocketTool.RegisterMessageProcessor(this);
		
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start () {
	
	}
	

	void Update () {
	
	}
    public void Init()
	{
		MiBaoXmlTemp mMiBao = MiBaoXmlTemp.getMiBaoXmlTempById (my_Diaoluomibao.miBaoId);

		string  mName = NameIdTemplate.GetName_By_NameId (mMiBao.nameId);

		MiBaoName.text = mName;

		MiBaoScrollView.IsOPenPath = true;

		MiBaoScrollView.OpenMiBaoId = my_Diaoluomibao.miBaoId;

		SocketTool.Instance().SendSocketMessage(ProtoIndexes.PVE_MAX_ID_REQ);
	}

	public GuanQiaMaxId  MapCurrentInfo = new GuanQiaMaxId();


	public bool OnProcessSocketMessage(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.PVE_MAX_ID_RESP:
			{
				MemoryStream t_stream = new MemoryStream (p_message.m_protocol_message, 0, p_message.position);

				QiXiongSerializer t_qx = new QiXiongSerializer ();

				GuanQiaMaxId tempInfo = new GuanQiaMaxId ();

				t_qx.Deserialize (t_stream, tempInfo, tempInfo.GetType ());

				MapCurrentInfo = tempInfo;

				Debug.Log("MapCurrentInfo.chuanQiId  = " +MapCurrentInfo.chuanQiId );
			    QuerySection();
			}
				return true;

			default: return false;
			}
		}
		return false;
		
	}

	public void QuerySection()
	{
		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(my_Diaoluomibao.miBaoId);
		
		MiBaoSuipianXMltemp mMiBaoSuipianXMltemp = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid (mMiBaoXmlTemp.tempId);
		
		MiBaoDiaoLuoXmlTemp mMiBaoDiaoLuoXmlTemp = MiBaoDiaoLuoXmlTemp.getMiBaoDiaoLuoXmlTempBysuipian_id (mMiBaoSuipianXMltemp.id);
		
		string Pvelv = mMiBaoDiaoLuoXmlTemp.legendPveId;
		
		string[] s = Pvelv.Split(new char[] { ',' });
		
		for (int i = 0; i < s.Length; i++) 
		{
			
			int j = 0;
			
			if (!int.TryParse (s [i], out j)) 
			{
				ScrollPanle.SetActive(false);
				
				Tips.gameObject.SetActive(false);
				
				Dot_inLv.gameObject.SetActive(true);
				
				Dot_inLv.text = s[i];
				
				return;
				
			}
			
			LegendPveTemplate mLg_PveTempTemplate = LegendPveTemplate.GetlegendPveTemplate_By_id(int.Parse(s[i]));
			
			NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mLg_PveTempTemplate.smaName);
			
			GameObject mChapterBtns = Instantiate(ChapterBtns)as GameObject;
			
			mChapterBtns.SetActive(true);
			
			mChapterBtns.transform.parent = ChapterBtns.transform.parent;
			
			//mChapterBtns.transform.localPosition = new Vector3((212*i - (s.Length - 1)*106),8,0);
			
			mChapterBtns.transform.localScale = ChapterBtns.transform.localScale;
			
			ChapteName mChapterBtn = mChapterBtns.GetComponent<ChapteName>();
			
			mChapterBtn.ChapterNum = mLg_PveTempTemplate.bigId;


			mChapterBtn.Chapter_name = mNameIdTemplate.Name;
			
			mChapterBtn.Levelid = int.Parse(s[i]);

			if(mChapterBtn.Levelid <= MapCurrentInfo.chuanQiId && FunctionOpenTemp.GetWhetherContainID(109))
			{
				mChapterBtn.IsOPen = true;
			}
			else{
				mChapterBtn.IsOPen = false;
			}

			mChapterBtn.Init();
		}
		
		ChildparentGB.GetComponent<UIGrid> ().repositionNow = true;
	}

	public void Close()
	{
		MiBaoScrollView.OpenMiBaoId = 0;
		MiBaoScrollView.IsOPenPath = false;
		Destroy (this.gameObject);

	}
}
