using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
public class MiBaoShangZhenScrollView : MonoBehaviour
{
    public static List<ShangzhenMiBaoTemp> localMiBao = new List<ShangzhenMiBaoTemp>();

    public List<MibaoInfo> MibaoInfo_ZuHe1 = new List<MibaoInfo>();
    public List<MibaoInfo> MibaoInfo_ZuHe2 = new List<MibaoInfo>();
    public List<MibaoInfo> MibaoInfo_ZuHe3 = new List<MibaoInfo>();
    public List<MibaoInfo> MibaoInfo_ZuHe4 = new List<MibaoInfo>();
    public List<MibaoInfo> MibaoInfo_ZuHe5 = new List<MibaoInfo>();
    public List<MibaoInfo> MibaoInfo_ZuHe6 = new List<MibaoInfo>();
    public List<MibaoInfo> MibaoInfo_ZuHe7 = new List<MibaoInfo>();

    public List<List<MibaoInfo>> MibaoInfo_ZuHe = new List<List<MibaoInfo>>();

    [HideInInspector]
    public GameObject IconSamplePrefab;

    Transform MiBaoTemp;
    GameObject mSecret;
    private UIPanel scrollor;

    public static MiBaoShangZhenScrollView mMiBaoShangZhenScrollView;

    [HideInInspector]
    public int Secret_num;
    //public GameObject Secret_Temp;
    Transform MiBaoRoot;
    public GameObject RightBtn;
    public GameObject LeftBtn;
    void Start()
    {
        this.gameObject.transform.localPosition = new Vector3(-3, 72, 0);
        mMiBaoShangZhenScrollView = this;
        scrollor = this.gameObject.GetComponent<UIPanel>();
        LeftBtn.SetActive(false);
        RightBtn.SetActive(false);
    }

    //void LoadMiBaoshangzhenBack(ref WWW p_www,string p_path, Object p_object)
    //{
    //    mSecret = Instantiate( p_object ) as GameObject;

    //    mSecret.SetActive(true);

    //    MiBaoRoot = this.transform.FindChild("UIGrid1");

    //    mSecret.transform.parent = MiBaoRoot.transform;

    //    //mSecret.transform.localPosition = Secret_Temp.transform.localPosition;

    //    mSecret.transform.localPosition = new Vector3(-300+m_i*150,0,0);

    //    mSecret.transform.localScale = new Vector3(1,1,1);


    //}

    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (IconSamplePrefab == null)
        {
            IconSamplePrefab = p_object as GameObject;
        }

        for (int i = 0; i < MibaoInfo_ZuHe.Count; i++)
        {
            for (int j = 0; j < MibaoInfo_ZuHe[i].Count; j++)
            {
                GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
                iconSampleObject.transform.parent = transform.FindChild("UIGrid1");
                var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

                iconSampleObject.transform.localPosition = new Vector3(-300 + i * 150, 120 - j * 120, 0);

                ShangzhenMiBaoTemp mShangzhenMiBaoTemp = iconSampleObject.gameObject.AddComponent<ShangzhenMiBaoTemp>();

                mShangzhenMiBaoTemp.m_mibaoinfo = MibaoInfo_ZuHe[i][j];
                mShangzhenMiBaoTemp.MibaoInfo_ZH = MibaoInfo_ZuHe[i];
                mShangzhenMiBaoTemp.MiBaoZuHeId = i + 1;
                mShangzhenMiBaoTemp.m_IconSampleManager = iconSampleManager;
                mShangzhenMiBaoTemp.SetIcon();

                localMiBao.Add(mShangzhenMiBaoTemp);

                foreach (int item in MapData.mapinstance.Save_MibaoId)
                {
					//Debug.Log("SavaMiBaodbid 1 ---------  =  "+item);
					if (item == MibaoInfo_ZuHe[i][j].dbId&&item != -1)
                    {
						//Debug.Log("item   =  "+item);
                        ChoosedMiBaoManeger.mChoosedMiBaoManeger.mibaoin_bechoosed = MibaoInfo_ZuHe[i][j];
                        ChoosedMiBaoManeger.mChoosedMiBaoManeger.InstanceMiBao(new Vector3(-150, 15, 0), true);
                    }
                }
            }
        }
    }

    public void Init()
    {
        MibaoInfo_ZuHe.Clear();

        MibaoInfo_ZuHe.Add(MibaoInfo_ZuHe1);
        MibaoInfo_ZuHe.Add(MibaoInfo_ZuHe2);
        MibaoInfo_ZuHe.Add(MibaoInfo_ZuHe3);
        MibaoInfo_ZuHe.Add(MibaoInfo_ZuHe4);
        MibaoInfo_ZuHe.Add(MibaoInfo_ZuHe5);
        MibaoInfo_ZuHe.Add(MibaoInfo_ZuHe6);
        MibaoInfo_ZuHe.Add(MibaoInfo_ZuHe7);
        Secret_num = MibaoInfo_ZuHe.Count;

        localMiBao.Clear();
        ChoosedMiBaoManeger.mChoosedMiBaoManeger.ZhanLi = JunZhuData.Instance().m_junzhuInfo.zhanLi;

        if (IconSamplePrefab == null)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
        }
        else
        {
            WWW temp = null;
            OnIconSampleLoadCallBack(ref temp, null, IconSamplePrefab);
        }
    }

    void FixedUpdate()
    {

        scrollor.clipOffset = new Vector2(-scrollor.gameObject.transform.localPosition.x, 0);
        int Move_x = Secret_num * 150 - 750 + (int)scrollor.cachedGameObject.transform.localPosition.x;
        if (MibaoInfo_ZuHe.Count <= 5)
        {
            LeftBtn.SetActive(false);
            RightBtn.SetActive(false);
            return;
        }
        if (Move_x <= 10)
        {
            RightBtn.SetActive(false);
            LeftBtn.SetActive(true);
        }
        else if (scrollor.cachedGameObject.transform.localPosition.x >= -10)
        {
            RightBtn.SetActive(true);
            LeftBtn.SetActive(false);
        }
        else
        {
            RightBtn.SetActive(true);
            LeftBtn.SetActive(true);
        }

    }

    public void RightMove()
    {
        StartCoroutine(StartMove(1, Secret_num));
    }
    public void LeftMove()
    {
        StartCoroutine(StartMove(-1, Secret_num));
    }
    IEnumerator StartMove(int i, int j)
    {
        int moveX;
        float movey = 72.43f;
        if (i == 1)//向右移动
        {

            moveX = j * 150 - 750 + (int)scrollor.cachedGameObject.transform.localPosition.x;

            if (moveX > 750)
            {
                SpringPanel.Begin(scrollor.cachedGameObject,
                                   new Vector3(scrollor.cachedGameObject.transform.localPosition.x - 750, movey, 0f), 8f);
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
               // Debug.Log("moveX" + moveX);
               // Debug.Log("scrollor.cacx" + scrollor.cachedGameObject.transform.localPosition.x);
                SpringPanel.Begin(scrollor.cachedGameObject,
                                   new Vector3(scrollor.cachedGameObject.transform.localPosition.x - moveX, movey, 0f), 8f);
                yield return new WaitForSeconds(0.2f);
            }
        }
        else
        {
            moveX = (int)(-scrollor.cachedGameObject.transform.localPosition.x);
            if (moveX > 750)
            {
                SpringPanel.Begin(scrollor.cachedGameObject,
                                   new Vector3(scrollor.cachedGameObject.transform.localPosition.x + 750, movey, 0f), 8f);
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                SpringPanel.Begin(scrollor.cachedGameObject,
                                   new Vector3(scrollor.cachedGameObject.transform.localPosition.x + moveX, movey, 0f), 8f);
                yield return new WaitForSeconds(0.2f);
            }
        }


    }

}
