using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class PveStarsMissionManagerment : MonoBehaviour, SocketProcessor 
{
    public List<PveStarsMissionItemManagerment> ListItemInfo = new List<PveStarsMissionItemManagerment>();
	private int SaveLeveId = 0;
	private int SaveIndex = 0;
    private int GetIndex = 0;
    public EventHandler m_Event;

    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
        m_Event.m_handler += CloseThisLayer;
    }
    void Start () 
    {
     
	
	}

    //private void ShowStarsRewardInfo(List<int> listId, List<int> listHuode, List<int> listIsLingQu)
    //{
    //    for (int i = 0; i < ListItemInfo.Count; i++)
    //    {
    //        if (listHuode[i] > 0)
    //        {
    //        //    Debug.Log("listHuode[i]listHuode[i]listHuode[i] :::" + listHuode[i]);
    //            ListItemInfo[i].m_DesContent.text = DescIdTemplate.GetDescriptionById(PveStarTemplate.getPveStarTemplateByStarId(listId[i]).desc);
    //            ListItemInfo[i].m_Complete.gameObject.SetActive(listIsLingQu[i] > 0 ? false : true);
    //            ListItemInfo[i].m_Gou.gameObject.SetActive(listIsLingQu[i] > 0 ? true : false);
    //            if (listIsLingQu[i] > 0)
    //            {
    //                ListItemInfo[i].transform.collider.enabled = false;
    //                ListItemInfo[i].m_Back.spriteName = "backGround_Common_big";
    //            }
    //            else
    //            {
    //                ListItemInfo[i].m_Back.spriteName = "backGround_Common_big";
    //            }
    //            ListItemInfo[i].m_LevelId = SaveLeveId;
    //            ListItemInfo[i].m_StarIndex = SaveIndex;
    //            ListItemInfo[i].LingQu = GetTouchIndex;
    //            ListItemInfo[i].ShowReward(PveStarTemplate.GetAwardInfo(listId[i]));
    //        }
    //        else
    //        {
    //            if (PveStarTemplate.getPveStarTemplateByStarId(listId[i]) != null)
    //            {
    //                ListItemInfo[i].m_DesContent.text = DescIdTemplate.GetDescriptionById(PveStarTemplate.getPveStarTemplateByStarId(listId[i]).desc);
    //                ListItemInfo[i].m_Complete.gameObject.SetActive(false);
    //                ListItemInfo[i].m_Complete.gameObject.SetActive(false);
    //                ListItemInfo[i].m_Gou.gameObject.SetActive(false);
    //                ListItemInfo[i].m_Back.spriteName = "bg1";
    //                ListItemInfo[i].transform.collider.enabled = false;
    //                ListItemInfo[i].ShowReward(PveStarTemplate.GetAwardInfo(listId[i]));
    //            }
    //        }
    //    }
    //}
    //private void ShowStarsRewardInfo(List<int> listId,  List<StarInfo> listinfo)
    //{
    //    for (int i = 0; i < ListItemInfo.Count; i++)
    //    {
    //        if (listinfo[i].finished)
    //        {

    //            ListItemInfo[i].m_DesContent.text = DescIdTemplate.GetDescriptionById(PveStarTemplate.getPveStarTemplateByStarId(listId[i]).desc);
    //            ListItemInfo[i].m_Complete.gameObject.SetActive(listIsLingQu[i] > 0 ? false : true);
    //            ListItemInfo[i].m_Gou.gameObject.SetActive(listIsLingQu[i] > 0 ? true : false);
    //            if (listIsLingQu[i] > 0)
    //            {
    //                ListItemInfo[i].transform.collider.enabled = false;
    //                ListItemInfo[i].m_Back.spriteName = "backGround_Common_big";
    //            }
    //            else
    //            {
    //                ListItemInfo[i].m_Back.spriteName = "backGround_Common_big";
    //            }
    //            ListItemInfo[i].m_LevelId = SaveLeveId;
    //            ListItemInfo[i].m_StarIndex = SaveIndex;
    //            ListItemInfo[i].LingQu = GetTouchIndex;
    //            ListItemInfo[i].ShowReward(PveStarTemplate.GetAwardInfo(listId[i]));
    //        }
    //        else
    //        {
    //            if (PveStarTemplate.getPveStarTemplateByStarId(listId[i]) != null)
    //            {
    //                ListItemInfo[i].m_DesContent.text = DescIdTemplate.GetDescriptionById(PveStarTemplate.getPveStarTemplateByStarId(listId[i]).desc);
    //                ListItemInfo[i].m_Complete.gameObject.SetActive(false);
    //                ListItemInfo[i].m_Complete.gameObject.SetActive(false);
    //                ListItemInfo[i].m_Gou.gameObject.SetActive(false);
    //                ListItemInfo[i].m_Back.spriteName = "bg1";
    //                ListItemInfo[i].transform.collider.enabled = false;
    //                ListItemInfo[i].ShowReward(PveStarTemplate.GetAwardInfo(listId[i]));
    //            }
    //        }
    //    }
    //}
 
    public void DataTidy(int levelid,List<StarInfo> starinfo)
    {

      //  ShowStarsRewardInfo(PveTempTemplate.GetPveStarsIdByLevelId(levelid), List<StarInfo> starinfo);
 
    }
    //public void DataTidy(int levelid, int huode, int lingqu)
    //{
    //    Debug.Log("huodehuodehuodehuode : " + huode + "lingqulingqulingqu :" + lingqu);
    //    SaveLeveId = levelid;
      
    //    if (SpliteIntNum(huode) != null && SpliteIntNum(lingqu) != null && PveTempTemplate.GetPveStarsIdByLevelId(levelid) != null)
    //    {
    //        ShowStarsRewardInfo(PveTempTemplate.GetPveStarsIdByLevelId(levelid), SpliteIntNum(huode), SpliteIntNum(lingqu));
    //    }
    //}
   
    private List<int> SpliteIntNum(int index)
    {
		List<int> listMid = new List<int>();
        if (index < 100)
        {
            if (index > 0)
            {
                listMid.Add(index / 10);
                listMid.Add(index % 10);
                return listMid;
            }
            else
            {
                listMid.Add(0);
                listMid.Add(0);
                return listMid;
            }
        }
        else
        {
                listMid.Add(index /100);
                listMid.Add((index -100) / 10);
                return listMid;
        }
    }

    void GetTouchIndex(int index)
    {
        GetIndex = index;
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.PVE_STAR_REWARD_GET_RET:
                    {
			           	Debug.Log("棰嗗彇濂栧姳杩斿洖");
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        PveStarGetSuccess tempInfo = new PveStarGetSuccess();

                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
                        ListItemInfo[GetIndex].m_Complete.gameObject.SetActive(false);
                        ListItemInfo[GetIndex].m_Back.spriteName = "backGround_Common_finish";
                        ListItemInfo[GetIndex].m_Gou.gameObject.SetActive(true);
				        ListItemInfo[GetIndex].transform.GetComponent<Collider>().enabled = false;
                        ListItemInfo[GetIndex].transform.GetComponent<Collider>().enabled = false;
//                        if (UIYindao.m_UIYindao.m_isOpenYindao)
//                        {
//                            if (FreshGuide.Instance().IsActive(100007))
//                            {
//                                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100007];
//                                TaskData.Instance.m_iCurMissionIndex = 100007;
//                                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
//                            }
//                          
					if (tempInfo.s_result)
					{
						//MapDataChange();
					PveLevelUImaneger.mPveLevelUImaneger.sendLevelDrop(SaveLeveId);

					}
//				if(Pve_Level_Info.CurLev == 100203)
//				{
//					MapData.mapinstance.GuidLevel = 21;
//					MapData.mapinstance.IsCloseGuid = false;
//					MapData.mapinstance.ShowPVEGuid ();
//				}
                        return true;
               }

                default: return false;
            }
        }
        return false;
    }

    void MapDataChange()
    { 
     for(int i = 0; i < MapData.mapinstance.myMapinfo.s_allLevel.Count;i++)
     {
         if( MapData.mapinstance.myMapinfo.s_allLevel[i].guanQiaId == SaveLeveId)
         {
                 if (GetIndex == 0)
                 {
                     //MapData.mapinstance.myMapinfo.s_allLevel[i].acheiveRewardState += 100;
                 }
                 else
                 {
                   //  MapData.mapinstance.myMapinfo.s_allLevel[i].acheiveRewardState += 10;
                 }
         }
     }
    
    }
    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
    void CloseThisLayer(GameObject obj)
    {

//		if (Pve_Level_Info.CurLev == 100203) {
//
//			MapData.mapinstance.GuidLevel = 22;  
//			MapData.mapinstance.IsCloseGuid = false;
//			MapData.mapinstance.ShowPVEGuid ();
//		}

        Destroy(this.gameObject);
    }
}
