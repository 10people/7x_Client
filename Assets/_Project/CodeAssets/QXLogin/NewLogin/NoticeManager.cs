using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NoticeManager : MonoBehaviour {

	public UILabel m_labelContent;
    public UIGrid m_TableObject;
	public GameObject loginObj1;
	public GameObject loginObj2;

    public GameObject m_AnnounceItem;
    public GameObject m_Parent;
    public UIScrollView m_ScrollView;

    public UIScrollView m_ScrollViewDes;
    public GameObject m_ObjectContent;
    private List<GameObject> listItem = new List<GameObject>();
    public struct AnnounceInfo
    {
        public string title;
        public string content;
        public int aligment;
        public int _New;
    };

    private List<AnnounceInfo> listInfo = new List<AnnounceInfo>();
    public void GetNoticeStr(string tempStr)
    {
       ////Debug.Log("tempStrtempStrtempStrtempStr ::" + tempStr);
        //        #$$*$$#|*1*封测公告#1#[272727]
        //◇ 欢迎来到《[FF0000]
        //    七雄无双[-]》游戏世界！


        if (!string.IsNullOrEmpty(tempStr) && tempStr.IndexOf('|') > -1)
        {
            listInfo.Clear();

            try
            {
                string[] ss = tempStr.Split('|');
                if (ss[0].Equals("#$$*$$#"))
                {

                    int size = ss.Length;
                    for (int i = 1; i < size; i++)
                    {
                        if (!string.IsNullOrEmpty(ss[i]))
                        {
                            if (ss[i].IndexOf('*') > -1)
                            {
                                string[] tempInfo = ss[i].Split('*');
                                if (tempInfo[2].IndexOf('#') > -1)
                                {
                                    int tt = 0;
                                    string[] tempInfo2 = tempInfo[2].Split('#');
                                    AnnounceInfo info = new AnnounceInfo();
                                    if (int.TryParse(tempInfo[1], out tt))
                                    {
                                        info.aligment = int.Parse(tempInfo2[1]);
                                    }
                                    info.title = tempInfo2[0];
                                    info.content = tempInfo2[2];
                                    if (int.TryParse(tempInfo[1], out tt))
                                    {
                                        info._New = int.Parse(tempInfo[1]);
                                    }
                                    listInfo.Add(info);
                                }
                            }
                        }

                    }
                    WWW p_www = null;
                    //}
                    int sizeAll = listInfo.Count;
                    if (sizeAll <= 4)
                    {

                        m_ScrollView.enabled = false;
                    }
                    else
                    {
                        m_ScrollView.enabled = true; ;
                    }

                    for (int i = 0; i < sizeAll; i++)
                    {
                        ResourcesLoadCallBack(ref p_www, i.ToString(), m_AnnounceItem);
                    }

                }
            }
            catch (Exception e)
            {
				Debug.LogError( "Notice Exception: " + e );
            }
        }
    }

    int indexNum = 0;
    void ResourcesLoadCallBack(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
        tempObject.name = p_path;
        listItem.Add(tempObject);
        if (indexNum == 0)
        {
            tempObject.GetComponent<AnnounceItemManagerment>().m_SpriteGuang.gameObject.SetActive(true);
            tempObject.GetComponent<AnnounceItemManagerment>().m_SpriteBack.transform.localScale = new Vector3(1.05f, 1.05f, 1);
           // m_labelContent.text = listInfo[int.Parse(p_path)].content;
            WWW _www = null;
            ResourcesLoadCallBackContent(ref _www,  "0" , m_ObjectContent);
        }
        tempObject.transform.parent = m_Parent.transform;
        tempObject.transform.localPosition = Vector3.zero;
        tempObject.transform.localScale = Vector3.one;
        tempObject.transform.GetComponent<AnnounceItemManagerment>().ContentShow(listInfo[indexNum], ShowInfo);
        m_Parent.GetComponent<UIGrid>().repositionNow = true;
        indexNum++;
    }

    private int _TouchNumSave = 0;

    void ShowInfo(int index){
        if (_TouchNumSave != index){
            m_ScrollViewDes.transform.localPosition = new Vector3(107, 380, 0);
            m_ScrollViewDes.GetComponent<UIPanel>().clipOffset = new Vector2(0, -379);
            m_ScrollViewDes.UpdatePosition();
            m_TableObject.transform.localPosition = new Vector3(0,-227,0);
            listItem[index].GetComponent<AnnounceItemManagerment>().m_SpriteBack.transform.localScale = new Vector3(1.05f, 1.05f, 1);
            listItem[index].GetComponent<AnnounceItemManagerment>().m_SpriteGuang.gameObject.SetActive(true);
            listItem[_TouchNumSave].GetComponent<AnnounceItemManagerment>().m_SpriteGuang.gameObject.SetActive(false);
            listItem[_TouchNumSave].GetComponent<AnnounceItemManagerment>().m_SpriteBack.transform.localScale = Vector3.one;
            _TouchNumSave = index;

            if (_ContentTemp != null){
				
                Destroy(_ContentTemp);

                WWW _www = null;

                ResourcesLoadCallBackContent(ref _www, index.ToString(), m_ObjectContent);
            }
        }
    }
	
	public void CloseBtn (){
		if ( ThirdPlatform.IsThirdPlatform () ) {
			if( ThirdPlatform.Instance() != null ){
				if( !ThirdPlatform.IsMyAppAndroidPlatform() ){
					if( ThirdPlatform.GetPlatformStatus() != ThirdPlatform.PlatformStatus.LogIn ){
						ThirdPlatform.CheckLoginToShowSDK();

						Debug.Log( "Not In Login Status, CheckSDK and return" );

						return;
					}
				}
			}
			else{
				Debug.LogError( "Error, instance not exist." );
			}
		}
		else{
			// on 3rd platform, check is in update state
			// if not, check here
			if( PrepareBundleHelper.IsDeviceCheckOpen() ){
				if( !DeviceHelper.CheckIsDeviceSupported() ){
					return;
				}
			}
		}

		if( !ClientMain.m_is_templates_loaded ){
			Debug.Log( "Data Not Loaded." );

			return;
		}

//		Debug.Log( Time.realtimeSinceStartup +  "NoticeManager.CloseBtn()" );
//		Debug.Log ("ThirdPlatform.IsMyAppAndroidPlatform():" + ThirdPlatform.IsMyAppAndroidPlatform());

//		loginObj1.SetActive (ThirdPlatform.IsMyAppAndroidPlatform());
//		loginObj2.SetActive (!ThirdPlatform.IsMyAppAndroidPlatform());

		AccountRequest.account.SetActiveLoginObj (ThirdPlatform.IsMyAppAndroidPlatform());

		Destroy (this.gameObject);

		if( ThirdPlatform.IsThirdPlatform() ){
			if( ThirdPlatform.IsMyAppAndroidPlatform() ){
				if( ThirdPlatform.GetPlatformStatus() != ThirdPlatform.PlatformStatus.LogIn ){
					Debug.Log( "1st Time Enter Game, Show Login Type UI." );
				}
				else{
					Debug.Log( "Enter Game when already logged in, Direct Goto Server Select." );

					AccountRequest.account.DengLuRequestSuccess( ThirdPlatform.Instance().GetLoginInfo() );		
				}

				// temporary use the same code
				// MyApp have two entrance, 1st is here, 2nd is after 1st time login success
//				if( string.IsNullOrEmpty( ThirdPlatform.Instance().GetLoginInfo() ) ){
//
//					Debug.Log( "1st time enter game, show login type UI." );
//				}
//				else{
//					Debug.Log( "enter game when already logged in, direct goto server select." );
//
//					AccountRequest.account.DengLuRequestSuccess( ThirdPlatform.Instance().GetLoginInfo() );		
//				}
			}
			else{
				AccountRequest.account.DengLuRequestSuccess( ThirdPlatform.Instance().GetLoginInfo() );	
			}
		}
	}

    private GameObject _ContentTemp = null;
    void ResourcesLoadCallBackContent(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        _ContentTemp = Instantiate(p_object) as GameObject;
        _ContentTemp.transform.parent = m_TableObject.transform;
        _ContentTemp.GetComponent<NoticeContentInfo>().ShowContent(listInfo[int.Parse(p_path)]);
        _ContentTemp.transform.localPosition = Vector3.zero;
        _ContentTemp.transform.localScale = Vector3.one;

        m_TableObject.repositionNow = true;
    }
}
