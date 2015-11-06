using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class UnLoadManager
{
    public UnLoadManager(ClientMain main)
    {
        m_Main = main;
    }

    public void init()
    {
//        UnLoadManager.DownLoad(ResMgr.Inst().GetUrl(MYNGUIManager.m_SLoadPath + "UIInitSprite"), GameManager.m_AtlasAndUIFont.CurLoad);
//        UnLoadManager.DownLoad(ResMgr.Inst().GetUrl(MYNGUIManager.m_SLoadPath + "UILoadingSprite") , GameManager.m_AtlasAndUIFont.CurLoad);
//        UnLoadManager.DownLoad(ResMgr.Inst().GetUrl(MYNGUIManager.m_SLoadPath + "UILoadingBGSprite"), GameManager.m_AtlasAndUIFont.CurLoad);
    }

    public void UpData()
    {
        if (m_isSetLoad)
        {
            if (m_isLoad)
            {
                loadCurent();
                //m_UILoading.updataload(wwwDate.progress, m_iLoadCurentIndex + 1, m_iLoadCurentIndex + m_listDownLoadUrl.Count);
            }
        }
        else
        {
            loadCurent();
        }
    }
    public void loadCurent()
    {
        if (!m_isLoading)
        {
            if (m_listDownLoadUrl.Count != 0)
            {
                m_Main.StartCoroutine(load(m_listDownLoadUrl[0]));
            }
            else
            {

            }
        }
    }

    public IEnumerator load(string url)
    {
		Debug.LogError( "Never Shoud Be Here." );

        m_isLoading = true;
		if (ClientMain.M_BRESLOAD)
		{

		}
		else 
		{
			if (www.Contains(url))
			{
				wwwDate = www[url] as WWW;
			}
			else
			{
				//wwwDate = WWW.LoadFromCacheOrDownload(url, 1);
				wwwDate = new WWW(url);
				yield return wwwDate;
				www.Add(url, wwwDate);
			}
			m_listDownLoadOver[0](ref wwwDate, m_listDownLoadUrl[0], m_ResNull);

		}
		m_isLoading = false;
		m_listDownLoadUrl.RemoveAt(0);
		m_listDownLoadOver.RemoveAt(0);
		
		
		if (m_listDownLoadOver.Count == 0 && m_isSetLoad)
		{
			m_isSetLoad = false;
			m_iLoadCurentIndex = 0;
			
			//switch (m_ELoadState)
			{
				//                case ELoadState.E_Default:
				//                    break;
				//                case ELoadState.E_UpLoad:
				//                    m_UIInitLoad.uiDelete();
				//                    m_UIInitLoad = null;
				//                    break;
				//                case ELoadState.E_SceneLoad:
				//                    m_UILoading.uiDelete();
				//                    m_UILoading = null;
				//                    break;
				//                case ELoadState.E_TempLoad:
				//                    m_UIloadingTemp.uiDelete();
				//                    break;
			}

//			m_ELoadState = ELoadState.E_Default;
			
			if (m_SetLoadOver != null)
			{
				m_SetLoadOver();
			}
		}
		else if (m_isSetLoad)
		{
			m_iLoadCurentIndex++;
		}
		wwwDate = null;
		m_isLoading = false;
    }

    public static void setLoad(SetLoadOver setLoadOver, ELoadState state)
    {
        m_SetLoadOver = setLoadOver;
        m_isSetLoad = true;
        m_isLoad = false;

//        m_ELoadState = state;

//        switch (m_ELoadState)
//        {
//            case ELoadState.E_Default:
//                break;
//            case ELoadState.E_UpLoad:
//                m_UIInitLoad = new UIInitLoad();
//                break;
//            case ELoadState.E_SceneLoad:
//                m_UILoading = new UILoading();
//                break;
//            case ELoadState.E_TempLoad:
//                if (m_UIloadingTemp == null)
//                {
//                    m_UIloadingTemp = new UILoadingTemp();
//                }
//                else
//                {
//                    m_UIloadingTemp.uiInit();
//                }
//                break;
//        }
    }

    public static void startLoad()
    {
        m_isLoad = true;
    }

//    public static void DownLoad(string path, DownLoadOver downLoadOver)
	public static void DownLoad(string p_res_path, Bundle_Loader.LoadResourceDone downLoadOver, List<EventDelegate> p_callback_list = null, bool p_open_simulate = true ){
//        m_listDownLoadUrl.Add(path);
//        m_listDownLoadOver.Add(downLoadOver);

		Global.ResourcesDotLoad( p_res_path, downLoadOver, p_callback_list, p_open_simulate );
    }

    public void deleteWWW(string path){
        if(www.Contains(path))
        {
            if ((www[path] as WWW).assetBundle != null)
            {
                (www[path] as WWW).assetBundle.Unload(false);
            }
            
            (www[path] as WWW).Dispose();
            www.Remove(path);
        }
    }

	public delegate void DownLoadOver(ref WWW www, string path, Object obj);//加载个体委托设定
    public static List<DownLoadOver> m_listDownLoadOver = new List<DownLoadOver>();//加载个体委托回调容器
    public static List<string> m_listDownLoadUrl = new List<string>();//加载网络路径列表

    public delegate void SetLoadOver();//加载模块委托设定
    private static SetLoadOver m_SetLoadOver;//加载模块委托回调
    
//	private static ELoadState m_ELoadState = ELoadState.E_Default;

	private static bool m_isSetLoad = false;//是否加载模块
    private static bool m_isLoading = false;//是否加载中
    private static bool m_isLoad = false;//是否可以加载
    public static int m_iLoadCurentIndex = 0;//当前加载索引
    private static WWW wwwDate;
//    private static UILoading m_UILoading;
//    private static UILoadingTemp m_UIloadingTemp;
//    private static UIInitLoad m_UIInitLoad;
    private Hashtable www = new Hashtable();
    private ClientMain m_Main;
	public Object m_ResNull = null;
	public WWW m_WWWNull = null;
    public enum ELoadState
    {
        E_Default,
        E_UpLoad,
        E_SceneLoad,
        E_TempLoad,
    }
}