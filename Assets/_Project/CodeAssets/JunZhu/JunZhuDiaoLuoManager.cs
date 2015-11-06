using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class JunZhuDiaoLuoManager : MonoBehaviour ,SocketProcessor
{
    struct LevelInFo
    {
        public bool isShow;
        //  public int guanQiaId;
        public int stars;
        public int id;
        public string bigId;
        public string smaName;
    }
  public GameObject m_objMain;
  public UILabel m_LabTag;
   List<LevelInFo> listLevelInfo = new List<LevelInFo> ();
   List<LevelInFo> listLevelInfo2 = new List<LevelInFo>();
  List<int> listDiaoLuoId = new List<int>();
  List<int> listMinId = new List<int>();
  public UIGrid m_Grid;
  public EventHandler m_TouchEvent;
  public Section  MapCurrentInfo = new Section();

  private int _MaxLevelId = 0;
  private int currentSection = 0;
  private bool isQuery;
  private int _DiaoLuoId = 0;
  int index = 0;
  public int m_DiaoLuoId;

  void Awake()
  {
	//SocketTool.RegisterMessageProcessor(this);
  
  }
  void Start()
  {
	m_TouchEvent.m_handler += EventBack;
   // JunZhuDiaoLuoManager.RequestMapInfo(-1);
  }

  void OnEnable()
  {
  SocketTool.RegisterMessageProcessor(this);
   index = 0;
   index_DiaoLuoNum = 0;
  }
  public void ShowDiaoLuoLevel(int diaoLuoId) 
  {
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            if (FreshGuide.Instance().IsActive(100260) && TaskData.Instance.m_TaskInfoDic[100260].progress >= 0)
            {
             //   FunctionWindowsCreateManagerment.SetSelectEquipInfo(0, 0);
                TaskData.Instance.m_iCurMissionIndex = 100260;

                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                tempTaskData.m_iCurIndex = 4;
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
        }
        index_DiaoLuoNum = 0;
      _DiaoLuoId = diaoLuoId;
      listDiaoLuoId.Clear();
      SocketTool.Instance().SendSocketMessage(ProtoIndexes.PVE_MAX_ID_REQ);
  }

  private void QuerySection()//查询掉落关卡是否为当前章节关卡
  {
      listLevelInfo.Clear();

      for (int i = 0; i < DiaoLuoTemplate.templates.Count; i++)
      {
          if (DiaoLuoTemplate.templates[i].itemId == _DiaoLuoId)
          {
              if (DiaoLuoTemplate.templates[i].PveIds.IndexOf(",") == -1)
              {
                  int k = 0;
                  if (int.TryParse(DiaoLuoTemplate.templates[i].PveIds, out k))
                  {
                      listDiaoLuoId.Add(int.Parse(DiaoLuoTemplate.templates[i].PveIds));
                      m_LabTag.gameObject.SetActive(false);
                  }
                  else
                  {
                      m_LabTag.text = DiaoLuoTemplate.templates[i].PveIds;
                      m_LabTag.gameObject.SetActive(true);
                  }
              }
              else
              {
                  //  Debug.Log(" DiaoLuoTemplate.templates[i].PveIds DiaoLuoTemplate.templates[i].PveIds ::" + DiaoLuoTemplate.templates[i].PveIds);
                  string[] data = DiaoLuoTemplate.templates[i].PveIds.Split(',');
                  for (int j = 0; j < data.Length; j++)
                  {
                      listDiaoLuoId.Add(int.Parse(data[j]));
                  }
              }

              break;
          }
      }

      int size = listDiaoLuoId.Count;
      for (int i = 0; i < size; i++)
      {
          for (int j = 0; j < PveTempTemplate.templates.Count; j++)
          {

              if (listDiaoLuoId[i] == PveTempTemplate.templates[j].id)
              {
                  LevelInFo Info = new LevelInFo();
                  if (PveTempTemplate.templates[j].id <= _MaxLevelId)
                  {
                      Info.isShow = true;
                  }
                  else
                  {
                      Info.isShow = false;
                  }
                  Info.bigId = PveTempTemplate.templates[j].bigId.ToString();
                  Info.smaName = PveTempTemplate.templates[j].smaName.ToString();
                  Info.id = PveTempTemplate.templates[j].id;
                  //   Info.stars = MapCurrentInfo.s_allLevel[k].s_starNum;
                  listLevelInfo.Add(Info);
              }

          }
      }
      CreateItems();
  }
	
  public bool OnProcessSocketMessage(QXBuffer p_message)
  {
	if (p_message != null)
	{
	  switch (p_message.m_protocol_index)
	  {
		case ProtoIndexes.PVE_PAGE_RET:
		{
			MemoryStream t_stream = new MemoryStream (p_message.m_protocol_message, 0, p_message.position);
			QiXiongSerializer t_qx = new QiXiongSerializer ();
			Section tempInfo = new Section ();
			t_qx.Deserialize (t_stream, tempInfo, tempInfo.GetType ());
	        MapCurrentInfo = tempInfo;
			 
        }
		 return true;
        case ProtoIndexes.PVE_MAX_ID_RESP:
         {
             MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

             QiXiongSerializer t_qx = new QiXiongSerializer();

             GuanQiaMaxId tempInfo = new GuanQiaMaxId();

             t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

             _MaxLevelId = tempInfo.commonId;
             QuerySection();
         }
         return true;
	    default: return false;
	  }
	}
	 return false;
	
   }

	/// 获取关卡当前完成信息
   public static void RequestMapInfo( int p_section )
   {
	 	PvePageReq mapinfo = new PvePageReq ();
	
		MemoryStream mapStream = new MemoryStream ();
	 
		QiXiongSerializer maper = new QiXiongSerializer ();
	
		mapinfo.s_section = p_section;
	
		maper.Serialize (mapStream,mapinfo);
	 
		byte[] t_protof;
	
		t_protof = mapStream.ToArray();
 
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.PVE_PAGE_REQ, ref t_protof);
	}

	 
 
	void CreateItems()
    {
        int sizea = listLevelInfo.Count;
        for (int i = 0; i < sizea; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.JUN_ZHU_DROP_ITEM),
                                        ResourcesLoadCallBack);
        }
	}

    int index_DiaoLuoNum = 0;
    public void ResourcesLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject DiaoluoShow = Instantiate(p_object) as GameObject;
        DiaoluoShow.transform.parent = m_Grid.transform;

        DiaoluoShow.transform.localPosition = Vector2.zero;

        DiaoluoShow.transform.localScale = Vector3.one;
       // DiaoluoShow.transform.GetComponent<JunZhuDiaoLuoItem>().ShowInfo(listLevelInfo[index_DiaoLuoNum].id,listLevelInfo[index_DiaoLuoNum].isShow, listLevelInfo[index_DiaoLuoNum].bigId.ToString(), listLevelInfo[index_DiaoLuoNum].smaName.ToString(), GoTo);
        if (index_DiaoLuoNum < listLevelInfo.Count - 1)
        {
            index_DiaoLuoNum++;
        }
        m_Grid.repositionNow = true;
      //  DiaoluoShow.transform.GetComponent<JunZhuDiaoLuoItem>().ShowInfo(diaoluoInfo.isShow, diaoluoInfo.bigId.ToString(), diaoluoInfo.smaName.ToString(), diaoluoInfo.icon.ToString(), diaoluoInfo.stars);
    }

   
    
	void EventBack(GameObject obj)
    {
		index = 0;
	
		for(int i = 0; i < m_Grid.transform.childCount;i++)
        {
	  		Destroy(m_Grid.transform.GetChild(i).gameObject);
	 	}

	 this.gameObject.SetActive (false);
   }

    void OnDisable()
    {
 
	  SocketTool.UnRegisterMessageProcessor(this);
	}

	void OnDestroy()
    {

	}

    void GoTo(int id)
    {
        MainCityUI.TryRemoveFromObjectList(m_objMain);

      //  WindowBackShowController.SaveWindowInfo(m_objMain.name, Res2DTemplate.Res.INTENSIFY_EQUIP_GROWTH_AMEND);
        EnterGuoGuanmap.Instance().ShouldOpen_id = id;

        Destroy(m_objMain);
    
    }
}
