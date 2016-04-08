﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class FriendOperationLayerManagerment : MonoBehaviour , SocketListener
{
 
    public List<EventIndexHandle> m_listEvent;
    public List<EventIndexHandle> m_listItemEvent;
    public GameObject m_Parent;
    public GameObject m_ParentForbid;
    public GameObject m_MainParent;
    public List<UIScrollView> m_listScrollView;
    public List<UILabel> m_listLabel;
    public UIScrollView m_ScrollView;
    public GameObject m_Durable_UI;
    public UIScrollView m_ScrollView2;

    public GameObject m_ObjItemMesh;
    public GameObject m_ObjItemMesh2;
    public List<GameObject> listGameobject;

    public List<GameObject> listTitle;
    public int height_index = -20;
    public GameObject m_LoadingObj;

    public GameObject m_HiddenObject;

    public UILabel m_labelFriedsTag;
    public UILabel m_labelForbidTag;
    public GameObject m_ObjTopLeft;
    private bool isget = false;
    private Dictionary<int, GameObject> friendItenDic = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> friendForbidItenDic = new Dictionary<int, GameObject>();
    private bool IsNoFirst = false;
    //int pageIndex = 1;
	int pageIndex = 0;
    string deleName = "";
    string _ForbidName = "";
    private List<BlackJunzhuInfo> listForbidInfor = new List<BlackJunzhuInfo>();
    private int friendCountMax = 0;
    void Awake()
    {
        SocketTool.RegisterSocketListener(this);
    }
	void Start () 
    {
        MainCityUI.setGlobalTitle(m_ObjTopLeft, "好友", 0, 0);
        MainCityUI.setGlobalBelongings(m_Durable_UI, 0, 0);
        listInfo.Clear();
        friendItenDic.Clear();
        index = 0;
        m_listEvent.ForEach(p => p.m_Handle += TouchEvent);
        m_listItemEvent.ForEach(p => p.m_Handle += ItemTouchEvent);
        m_listEvent[1].GetComponent<ButtonColorAndDepthManagerment>().ButtonsControl(false);
        //  pageIndex = 1;
        BlockedData.Instance().RequestBlockedInfo();

        RequestData(pageIndex);
	}
    private int _index_Touch = 0;

    void ItemTouchEvent(int index)
    {
        switch (index)
        {
            case 0:
                {
                    CallBackEmail(_OwnerID);
                }
                break;
            case 1:
                {
                    CallBackAddForbid(_OwnerID);
                }
                break;
            case 2:
                {
                    CallBackDeleteFriend(_OwnerID, _ForbidName);
                }
                break;

        }
    }
    void TouchEvent(int index)
    {
        if (_index_Touch != index)
        {
            _index_Touch = index;
            switch (index)
            {
                case 0:
                    {
                        foreach (EventIndexHandle item in m_listEvent)
                        {
                            if (item.m_SendIndex == index)
                            {
                                item.GetComponent<ButtonColorAndDepthManagerment>().ButtonsControl(true);
                            }
                            else
                            {
                                item.GetComponent<ButtonColorAndDepthManagerment>().ButtonsControl(false);
                            }
                        }
                        listForbidInfor.Clear();
                        listGameobject[0].SetActive(false);
                        listGameobject[1].SetActive(true);
                        RefreshFriendsInfo();
                    }
                    break;
                case 1:
                    {
                        foreach (EventIndexHandle item in m_listEvent)
                        {
                            if (item.m_SendIndex == index)
                            {
                                item.GetComponent<ButtonColorAndDepthManagerment>().ButtonsControl(true);
                            }
                            else
                            {
                                item.GetComponent<ButtonColorAndDepthManagerment>().ButtonsControl(false);
                            }
                        }

                 
                        foreach (KeyValuePair<int, GameObject> item in friendItenDic)
                        {
                            item.Value.GetComponent<UIDragScrollView>().enabled = true;
                            item.Value.GetComponent<FriendOperationItemManagerment>().m_GaoLiangKuang.SetActive(false);
                        }

                        listForbidInfor.Clear();
                        listGameobject[0].SetActive(true);
                        listGameobject[1].SetActive(false);
                        m_ObjItemMesh.SetActive(false);
                        ShowForbidInfo();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    void Update()
    {
        //if (listInfo.Count > 0)
        //{
        //    if (m_listScrollView[0].GetComponent<SpringPanel>() != null)
        //    {
        //        isget = true;
        //        m_listScrollView[0].GetComponent<SpringPanel>().onFinished = CallBack;
        //    }
        //}
        if (FriendOperationData.Instance.m_FriendInfoGet)
        {
            FriendOperationData.Instance.m_FriendInfoGet = false;
            FriendInfoShow(FriendOperationData.Instance.m_FriendListInfo);
        }
    }
    public void RequestData(int index)
    {
        FriendOperationData.Instance.m_FriendInfoRequest = true;
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        GetFriendListReq friend = new GetFriendListReq();
        friend.pageNo = index;
       // friend.pageSize = 4;
		friend.pageSize = 0;
        t_qx.Serialize(t_tream, friend);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_FRIEND_REQ, ref t_protof);
        
    }

    private List<FriendJunzhuInfo> listInfo = new List<FriendJunzhuInfo>();

    void FriendInfoShow(GetFriendListResp ReponseInfo)
    {
        friendCountMax = ReponseInfo.friendMax;
        m_listLabel[0].text = ReponseInfo.friendCount + " / " + ReponseInfo.friendMax;
        if  ( ReponseInfo.friendCount > 0 &&  ReponseInfo.friends != null)
        {
            for (int i = 0; i < ReponseInfo.friends.Count; i++)
            {
                listInfo.Add(ReponseInfo.friends[i]);
            }
        }

        for (int j = 0; j < listInfo.Count; j++)
        {
            for (int i = 0; i < listInfo.Count - 1 - j; i++)
            {
                if (listInfo[i].offlineTime > listInfo[i + 1].offlineTime)
                {
                    FriendJunzhuInfo t = listInfo[i];
                    listInfo[i] = listInfo[i + 1];
                    listInfo[i + 1] = t;
                }
            }
        }

        if (ReponseInfo.friendCount > 0)
        {
            listTitle[0].SetActive(false);
        }
        else
        {
            listTitle[0].SetActive(true);
            m_labelFriedsTag.text = LanguageTemplate.GetText(LanguageTemplate.Text.NO_FRIENDS_1);
        }

        m_HiddenObject.SetActive(true);
        listGameobject[1].SetActive(true);
        m_LoadingObj.SetActive(false);
        RefreshFriendsInfo();
        m_listEvent[0].transform.GetComponent<Collider>().enabled = true;
        m_listEvent[1].transform.GetComponent<Collider>().enabled = true;
    }
    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                //case ProtoIndexes.S_FRIEND_RESP://返回好友信息
                //    {
                //        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                //        QiXiongSerializer t_qx = new QiXiongSerializer();

                //        GetFriendListResp ReponseInfo = new GetFriendListResp();
                //        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());
                //        friendCountMax = ReponseInfo.friendCount;
                     
                //       // Debug.Log("ReponseInfo.friendCountReponseInfo.friendCountReponseInfo.friendCount ::" + ReponseInfo.friendCount);
                //        m_listLabel[0].text = ReponseInfo.friendCount + " / " + ReponseInfo.friendMax;
                  
                //        if (listInfo.Count < ReponseInfo.friendCount)
                //        {
                //            //Debug.Log("ReponseInfo.friends.Count" + ReponseInfo.friends.Count);
                //            for (int i = 0; i < ReponseInfo.friends.Count; i++)
                //            {
                //                listInfo.Add(ReponseInfo.friends[i]);
                //            }
                //        }
                //       // if (ReponseInfo.friends != null)
                //        {
                //          //  Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
                //            if (ReponseInfo.friendCount > 0)
                //            {
                //                listTitle[0].SetActive(false);
                //            }
                //            else
                //            {
                //                listTitle[0].SetActive(true);
                //                m_labelFriedsTag.text = LanguageTemplate.GetText(LanguageTemplate.Text.NO_FRIENDS_1);
                //            }
                         
                //            m_HiddenObject.SetActive(true);
                //            listGameobject[1].SetActive(true);
                //            m_LoadingObj.SetActive(false);
                //            RefreshFriendsInfo();
                //        }
 
                //        return true;
                //    }
                //    break;
                case ProtoIndexes.S_DELEFRIEND_RESP://返回删除信息 
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        FriendResp ReponseInfo = new FriendResp();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());

                        if (ReponseInfo.result == 0)
                        {
                            Destroy(friendItenDic[int.Parse(deleName)]);
                            friendItenDic.Remove(int.Parse(deleName));
                            foreach (KeyValuePair<int, GameObject> item in friendItenDic)
                            {
                                if (int.Parse(item.Value.name) > int.Parse(deleName))
                                {
                                    Vector3 vec = new Vector3();
                                    vec = item.Value.transform.localPosition;
                                    vec.y += 104.0f;
                                    item.Value.transform.localPosition = vec;
                                }
                            }
                       
							FriendOperationData.Instance.friendIdList.Remove (ReponseInfo.junzhuId);
							//add by liuleilei ,refresh friend info
							foreach (FriendJunzhuInfo friend in FriendOperationData.Instance.m_FriendListInfo.friends)
							{
								if (friend.ownerid == ReponseInfo.junzhuId)
								{
									FriendOperationData.Instance.m_FriendListInfo.friends.Remove (friend);
									break;
								}
							}

                        }
                        for (int i = 0; i < listInfo.Count; i++)
                        {
                            if (listInfo[i].ownerid == ReponseInfo.junzhuId)
                            {
                                listInfo.RemoveAt(i);
                                break;
                            }
                        
                        }
                        m_listLabel[0].text = listInfo.Count + " / " + friendCountMax.ToString();
                        if (listInfo.Count == 0)
                        {
                            listTitle[0].SetActive(true);
                            m_labelFriedsTag.text = LanguageTemplate.GetText(LanguageTemplate.Text.NO_FRIENDS_1);
                        }
                        m_ObjItemMesh.SetActive(false);
                        _isTouchOn = false;
                        return true;
                    }
                    break;
                case ProtoIndexes.S_Join_BlackList_Resp://返回加入黑名单信息 
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        BlacklistResp ReponseInfo = new BlacklistResp();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());
                       // Debug.Log("tempResponse.junzhuIdtempResponse.junzhuId ::" + ReponseInfo.junzhuId);
                        if (ReponseInfo.result == 0)
                        {
                            BlockedData.Instance().m_BlockedInfoDic.Add(ReponseInfo.junzhuId, ReponseInfo.junzhuInfo);
                            Destroy(friendItenDic[int.Parse(_ForbidName)]);
                            friendItenDic.Remove(int.Parse(_ForbidName));
                            foreach (KeyValuePair<int, GameObject> item in friendItenDic)
                            {
                                if (int.Parse(item.Value.name) > int.Parse(_ForbidName))
                                {
                                    Vector3 vec = new Vector3();
                                    vec = item.Value.transform.localPosition;
                                    vec.y += 104.0f;
                                    item.Value.transform.localPosition = vec;
                                }
                            }
                        }
                        else if (ReponseInfo.result == 103)
                        {
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadCallbackForbidFull);
                        }

                        for (int i = 0; i < listInfo.Count; i++)
                        {
                            if (listInfo[i].ownerid == ReponseInfo.junzhuId)
                            {
                                listInfo.RemoveAt(i);
                                break;
                            }

                        }
                        m_listLabel[0].text = listInfo.Count + " / " + friendCountMax.ToString();
                        if (listInfo.Count == 0)
                        {
                            listTitle[0].SetActive(true);
                            m_labelFriedsTag.text = LanguageTemplate.GetText(LanguageTemplate.Text.NO_FRIENDS_1);
                        }
                        m_ObjItemMesh.SetActive(false);
                        _isTouchOn = false;
                        return true;
                    }
                    break;
                case ProtoIndexes.S_CANCEL_BLACK://返回解除屏蔽
                    {
                       // Debug.Log("RRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRS");
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        BlacklistResp tempResponse = new BlacklistResp();

                        t_qx.Deserialize(t_stream, tempResponse, tempResponse.GetType());

                       // Debug.Log("tempResponse.junzhuIdtempResponse.junzhuId ::" + tempResponse.junzhuId);
                        if (tempResponse.result == 0)
                        {
                            need_Name = BlockedData.Instance().m_BlockedInfoDic[tempResponse.junzhuId].name;
                            BlockedData.Instance().m_BlockedInfoDic.Remove(tempResponse.junzhuId);
                            Destroy(friendForbidItenDic[int.Parse(disForbideName)]);
                            friendForbidItenDic.Remove(int.Parse(disForbideName));
                            foreach (KeyValuePair<int, GameObject> item in friendForbidItenDic)
                            {
                                if (int.Parse(item.Value.name) > int.Parse(disForbideName))
                                {
                                    Vector3 vec = new Vector3();
                                    vec = item.Value.transform.localPosition;
                                    vec.y += 104.0f;
                                    item.Value.transform.localPosition = vec;
                                }
                                else
                                { 
                                
                                }
                            }
                            if (BlockedData.Instance().m_BlockedInfoDic.Count == 0)
                            {
                                m_labelForbidTag.text = LanguageTemplate.GetText(LanguageTemplate.Text.NO_PERSON_IN_LINE_1);
                                listTitle[1].SetActive(true);
                               
                            }
                            else
                            {
                                listTitle[1].SetActive(false);
                            }
                        
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),UIBoxLoadCallbackDisForbidSuccss);
                   
                        }
                        m_listLabel[1].text = BlockedData.Instance().m_BlockedInfoDic.Count.ToString() + " / " + BlockedData.Instance().m_ForbidFriendsMax.ToString();
                        return true;
                    }
                    break;
                //case ProtoIndexes.S_ADDRIEND_RESP://返回添加好友信息
                //    {
                  
                //        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                //        QiXiongSerializer t_qx = new QiXiongSerializer();

                //        FriendResp tempResponse = new FriendResp();

                //        t_qx.Deserialize(t_stream, tempResponse, tempResponse.GetType());
                //       // Debug.Log("tempResponse.resulttempResponse.resulttempResponse.result ::" + tempResponse.result);
                //        if (tempResponse.result == 0)
                //        {
                //            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadCallbackAddFriendSuccess);
                //        }
                //        else if (tempResponse.result == 103)
                //        {
                //            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadCallbackFriendFull);
                //        }
                //        return true;
                //    }
                //    break;
                default:
                    break;
            }
        }
        return false;
    }
    string need_Name = "";
    public void UIBoxLoadCallbackAddFriendSuccess(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_14) + need_Name + LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_15); 
 
       // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, confirmStr, null, null);
 
    }

    public void UIBoxLoadCallbackFriendFull(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_2);
        string str2 = LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_3);

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), MyColorData.getColorString(1, str2), null, confirmStr, null, null);
    }

    public void UIBoxLoadCallbackForbidFull(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_2);
        string str2 = LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_3);
        string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), MyColorData.getColorString(1, str2), null, confirmStr, null, null);
    }
    public void UIBoxLoadCallbackDisForbidSuccss(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_10);
     //   string str2 = LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_11);
       // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, confirmStr, null, null);
 
      //  uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str),MyColorData.getColorString(1, str2) ,null , concelr, confirmStr, AddFriend, titleFont, titleFont, btn1Font);
    }

    void AddFriend(int index)
    {
        if (index == 2)
        {
            MemoryStream tempStream = new MemoryStream();
            QiXiongSerializer t_serializer = new QiXiongSerializer();
            CancelBlack temp = new CancelBlack();
            temp.junzhuId = save_FriendId;
            t_serializer.Serialize(tempStream, temp);

            byte[] t_protof = tempStream.ToArray();

            t_protof = tempStream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ADDFRIEND_REQ, ref t_protof);
        }
    }
  
    public void RefreshFriendsInfo()
    {
        int all_child = m_Parent.transform.childCount;
        int size = listInfo.Count;
        if (size > all_child)
        {
   
            //if (friendCountMax < 4)
            //{
            //    m_ScrollView.enabled = false;
            //}
            //else
            //{
            //    m_ScrollView.enabled = true;
            //}
 
            for (int i = index; i < size; i++)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.FRIEND_OPERATION_ITEM),
                                                LoadCallback);
            }
        }
        else
        {
            //foreach (KeyValuePair<int, GameObject> item in friendItenDic)
            //{
            //    if (int.Parse(item.Value.name) > int.Parse(deleName))
            //    {
            //        Vector3 vec = new Vector3();
            //        vec = item.Value.transform.localPosition;
            //        vec.y += 104.0f;
            //        item.Value.transform.localPosition = vec;
            //    }
            //}
        
        }
 
    }

    int index = 0;
    public void LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
        tempObject.transform.parent = m_Parent.transform;
        tempObject.name = index.ToString();
        tempObject.transform.localScale = Vector3.one;
        tempObject.transform.localPosition = new Vector3(0,-104*index,0);
        tempObject.transform.GetComponent<FriendOperationItemManagerment>().ShowInfo(listInfo[index],ShowHideMesh/*, ShowMesh*/);
        friendItenDic.Add(index, tempObject);
        if (index < listInfo.Count - 1)
        {
            index++;
        }
        else
        {
            m_ScrollView.disableDragIfFits = true;
            index = listInfo.Count; 
        }
      
    }
    void ShowForbidInfo()
    {
        listForbidInfor.Clear();
        int childCount = m_ParentForbid.transform.childCount;
        if (childCount > 0)
        {
            index_NUm = 0;
            friendForbidItenDic.Clear();
        }
        for (int i = 0; i < childCount; i++)
        {
            Destroy(m_ParentForbid.transform.GetChild(i).gameObject);
            m_ParentForbid.GetComponent<UIGrid>().repositionNow =true;
        }
        m_listLabel[1].text = BlockedData.Instance().m_BlockedInfoDic.Count.ToString() + " / " + BlockedData.Instance().m_ForbidFriendsMax.ToString();
       if (BlockedData.Instance().m_BlockedInfoDic.Count > 0)
        {
            listTitle[1].SetActive(false);
 
        }
        else
        {
            m_labelForbidTag.text = LanguageTemplate.GetText(LanguageTemplate.Text.NO_PERSON_IN_LINE_1);
            listTitle[1].SetActive(true);
        }

        foreach (KeyValuePair<long, BlackJunzhuInfo> item in BlockedData.Instance().m_BlockedInfoDic)
        {
            listForbidInfor.Add(item.Value);
        }

        int size = listForbidInfor.Count;
        if (size > 4)
        {
            m_ScrollView2.enabled = true;
        }
        else
        {
            m_ScrollView2.enabled = false;
        }
        for (int i = size -1 ; i >= 0; i--)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SETTINGS_UP_BLOCKED_ITEM),
                                                 LoadForbidCallback);
        }
    }
    int index_NUm = 0;
    public void LoadForbidCallback(ref WWW p_www, string p_path, Object p_object)
    {

        GameObject tempObject = Instantiate(p_object) as GameObject;
        tempObject.transform.parent = m_ParentForbid.transform;

        tempObject.name = index_NUm.ToString();
        tempObject.transform.localPosition = Vector3.zero;
        tempObject.transform.localScale = Vector3.one;
        friendForbidItenDic.Add(index_NUm, tempObject);
        tempObject.GetComponent<SettingBlockedItemManagerment>().ShowInfo(listForbidInfor[index_NUm], CallBackAddDisForbid);
        if (index_NUm < listForbidInfor.Count - 1)
        {
            index_NUm++;
        }
        m_ParentForbid.GetComponent<UIGrid>().repositionNow =true;
    }

    private long _OwnerID = 0;
    void CallBackEmail(long index)
    {
        int size = listInfo.Count;
        string name = "";
        for(int i = 0;i < size;i++)
        {
            if (listInfo[i].ownerid == index)
            {
                name = listInfo[i].name;
            }
        }
        if (!string.IsNullOrEmpty(name))
        {
//			EmailData.Instance.ReplyLetter (name);
			NewEmailData.Instance().OpenEmail (NewEmailData.EmailOpenType.EMAIL_REPLY_PAGE,name);
        	Destroy(m_MainParent);
        }       
    }
    string forbid_FriendName = "";
    private GameObject _OblNull = null;
    void CallBackAddForbid(long id)
    {
        for (int i = 0; i < listInfo.Count; i++)
        {
            if (listInfo[i].ownerid == id)
            {
                forbid_FriendName = listInfo[i].name;
            }
        }
        save_FriendId = id;
        if (_OblNull == null)
        {
            _OblNull = gameObject;
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                UIBoxLoadCallbackForbidTag);
        }
    }
    public void UIBoxLoadCallbackForbidTag(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        _OblNull = boxObj;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_5) + forbid_FriendName + LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_6);
        string str2 = LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_7) + "\n" + LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_8);
        string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), MyColorData.getColorString(1, str2), null, concelr, confirmStr, AddForbid);

    }

    void AddForbid(int index)
    {
        if (index == 2)
        {
            MemoryStream t_tream = new MemoryStream();
            QiXiongSerializer t_qx = new QiXiongSerializer();
            JoinToBlacklist forbid = new JoinToBlacklist();
            forbid.junzhuId = save_FriendId;
            t_qx.Serialize(t_tream, forbid);
            byte[] t_protof;
            t_protof = t_tream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_Join_BlackList, ref t_protof);
        }
    }

    

    void DeleteFriend(int index)
    {
        if (index == 2)
        {
            MemoryStream t_tream = new MemoryStream();
            QiXiongSerializer t_qx = new QiXiongSerializer();
            RemoveFriendReq friend = new RemoveFriendReq();
            friend.junzhuId = save_FriendId;

            t_qx.Serialize(t_tream, friend);
            byte[] t_protof;
            t_protof = t_tream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_DELEFRIEND_REQ, ref t_protof);
        }
    }
    long save_FriendId = 0;
    private GameObject _objDelNull = null;
    void CallBackDeleteFriend(long id, string name)
    {
        save_FriendId = id;
        deleName = name;
        if (_objDelNull == null)
        {
            _objDelNull = gameObject;
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                UIBoxLoadCallbackDeleTag);
        }
    }

    public void UIBoxLoadCallbackDeleTag(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        _objDelNull = boxObj;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str = LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_4);
        string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), null, null, concelr, confirmStr, DeleteFriend);
    }
    string disForbideName = "";
    private GameObject _objNull = null;
    void CallBackAddDisForbid(long id, string name)
    {
        save_FriendId = id;
        disForbideName = name;
        if (_objNull == null)
        {
            _objNull = gameObject;
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                         UIBoxLoadCallbackDisForbidTag);
        }
    }

    public void UIBoxLoadCallbackDisForbidTag(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        _objNull = boxObj;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_9);
        string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), null, null, concelr, confirmStr, DisForbid);
    }
    void DisForbid(int index)
    {
        if (index == 2)
        {
            MemoryStream tempStream = new MemoryStream();
            QiXiongSerializer t_serializer = new QiXiongSerializer();
            CancelBlack temp = new CancelBlack();
            temp.junzhuId = save_FriendId;
            t_serializer.Serialize(tempStream, temp);

            byte[] t_protof = tempStream.ToArray();

            t_protof = tempStream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_CANCEL_BLACK, ref t_protof);
        }
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);
    }
    bool isOn = false;

    void CallBack()
    {
 
            //Debug.Log("m_listScrollView[0].transform.localPosition.y" +m_listScrollView[0].transform.localPosition.y);
            //Debug.Log("listInfo.Count * height_index" +listInfo.Count * height_index);
            //if (m_listScrollView[0].transform.localPosition.y > -13 + listInfo.Count * height_index)
            //{
                //int child_Count =  m_Parent.transform.childCount;
                //if (friendCountMax > child_Count)
                //{
                //    m_LoadingObj.SetActive(true);
                //    pageIndex++;
                //   // Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSS :" + pageIndex);
                //    RequestData(pageIndex);
                //}
                //else
                //{
                //    Debug.Log("FULLFULLFULLFULLFULLFULLFULLFULLFULLFULLFULLFULLFULLFULL");
                //}
         //   }
            //else
            //{

            //    Debug.Log("DOWNDOWNDOWNDOWNDOWNDOWNDOWNDOWNDOWNDOWNDOWNDOWNDOWNDOWNDOWNDOWN");
            //}
        
    }
    void ShowMesh()
    {

        for (int i = 0; i < m_listEvent.Count; i++)
        {
            m_listEvent[i].transform.GetComponent<Collider>().enabled = false;
        }
    }

    private bool _isTouchOn = false;
    void ShowHideMesh(long id,float pos_y,string name)
    {
        GameObject obj = new GameObject();
        obj.transform.parent = friendItenDic[int.Parse(name)].transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.parent = m_ObjItemMesh.transform;
        if (obj.transform.localPosition.y < -137)
        {
            obj.transform.localPosition = -137 * Vector3.one;
        }
        else if (obj.transform.localPosition.y > 122)
        {
            obj.transform.localPosition = 122 * Vector3.one;
        }
        m_ObjItemMesh2.transform.localPosition = new Vector3(293, obj.transform.localPosition.y, 0);
        Destroy(obj);

        if (_isTouchOn && _OwnerID != id)
        {
            friendItenDic[int.Parse(_ForbidName)].GetComponent<FriendOperationItemManagerment>().m_GaoLiangKuang.SetActive(false);
            _OwnerID = id;
            _ForbidName = name;
        }
        else if (_isTouchOn && _OwnerID == id)
        {
            foreach (KeyValuePair<int, GameObject> item in friendItenDic)
            {
                item.Value.GetComponent<UIDragScrollView>().enabled = true;
            }
            _OwnerID = id;
            _ForbidName = name;
            _isTouchOn = false;
            friendItenDic[int.Parse(name)].GetComponent<FriendOperationItemManagerment>().m_GaoLiangKuang.SetActive(false);
            m_ObjItemMesh.SetActive(false);

        }
        else if (!_isTouchOn)
        {
            foreach (KeyValuePair<int, GameObject> item in friendItenDic)
            {
                item.Value.GetComponent<UIDragScrollView>().enabled = false;
            }
            _OwnerID = id;
            _ForbidName = name;
            _isTouchOn = true;
            m_ObjItemMesh.SetActive(true);
         //   m_ScrollView.enabled = false;
        }
     

    }

    public static void AddFriends(int FriendId)
    {

        MemoryStream tempStream = new MemoryStream();
        QiXiongSerializer t_serializer = new QiXiongSerializer();
        CancelBlack temp = new CancelBlack();
        temp.junzhuId = FriendId;
        t_serializer.Serialize(tempStream, temp);

        byte[] t_protof = tempStream.ToArray();

        t_protof = tempStream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ADDFRIEND_REQ, ref t_protof);
    }


    private bool WetherIsMin(int index)
    {
        List<GameObject> list = new List<GameObject>();
        foreach (KeyValuePair<int, GameObject> item in friendItenDic)
        {
            list.Add(item.Value);
        }

        for (int j = 0; j < list.Count; j++)
        {
            for (int i = 0; i < list.Count - 1 - j; i++)
            {
                if ( int.Parse(list[i].name) > int.Parse( list[i + 1].name))
                {
                    GameObject t = null;

                    t = list[i];

                    list[i] = list[i + 1];

                    list[i + 1] = t;
                }
            }
        }

        if (int.Parse(list[0].name) == index)
        {
            return true;
        }
        return false;
    }


    private bool WetherIsMax(int index)
    {
        List<GameObject> list = new List<GameObject>();
        foreach (KeyValuePair<int, GameObject> item in friendItenDic)
        {
            list.Add(item.Value);
        }

        for (int j = 0; j < list.Count; j++)
        {
            for (int i = 0; i < list.Count - 1 - j; i++)
            {
                if (int.Parse(list[i].name) < int.Parse(list[i + 1].name))
                {
                    GameObject t = null;

                    t = list[i];

                    list[i] = list[i + 1];

                    list[i + 1] = t;
                }
            }
        }

        if (int.Parse(list[0].name) == index)
        {
            return true;
        }
        return false;
        return false;
    }

}
