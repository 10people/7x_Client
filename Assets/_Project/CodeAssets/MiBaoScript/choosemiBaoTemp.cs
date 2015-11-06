using UnityEngine;
using System.Collections;
using qxmobile.protobuf;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;

public class choosemiBaoTemp : MonoBehaviour
{
    [HideInInspector]
    public IconSampleManager m_IconSampleManager;
    [HideInInspector]
    public MibaoInfo mibao_bechoosed;
    [HideInInspector]
    private List<GameObject> createdStarList = new List<GameObject>();

    //public UISprite spriteStar;
    //public UISprite Icon;
    //public UISprite PinZhi;
    //public UISprite backgruond;

    [HideInInspector]
    public GameObject numAddObj;//数字

    public void SetIcon(Vector3 pos, bool isSave)
    {
        MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(mibao_bechoosed.miBaoId);

        int startCount = mibao_bechoosed.star;
        string qualityFrameSpriteName = IconSampleManager.QualityPrefix + (mMiBaoXmlTemp.pinzhi - 1);
        string fgSpriteName = "250101";
        string rightButtomSpriteName = "flag_finish";
        string buttomSpriteName = "xingxing1";

        transform.localPosition = pos;

        m_IconSampleManager.SetIconType(IconSampleManager.IconType.oldMiBao);
        m_IconSampleManager.SetIconBasic(20, fgSpriteName, "", qualityFrameSpriteName);
        m_IconSampleManager.SetIconBasicDelegate(false, true, OnDeleteMibao);
        m_IconSampleManager.SetIconDecoSprite("", rightButtomSpriteName, buttomSpriteName);
        m_IconSampleManager.ButtomSprite.gameObject.SetActive(false);

        CreateStars(startCount);
    }

    public void OnDeleteMibao(GameObject go)
    {
        if (ChoosedMiBaoManeger.mChoosedMiBaoManeger.bechooseMiBao.Contains(mibao_bechoosed))
        {
            Transform parenttranf = transform.parent.parent.transform;
            Transform UIScrollViewTrans = parenttranf.transform.FindChild("MiBaoScrollView");
            Vector3 UIScrollViewTranspos = UIScrollViewTrans.transform.localPosition;

            foreach (ShangzhenMiBaoTemp item in MiBaoShangZhenScrollView.localMiBao)
            {
                if (item.m_mibaoinfo == mibao_bechoosed)
                {
//                    if (EnterLoading.m_EnterLoading.MiBaoidList.Contains(mibao_bechoosed.dbId))
//                    {
//                        EnterLoading.m_EnterLoading.MiBaoidList.Remove(mibao_bechoosed.dbId);
//                    }

                    item.ischoosed = false;
                    Vector3 temppos = item.gameObject.transform.localPosition;
                    Vector3 tempposparent = item.gameObject.transform.parent.localPosition;
                    var m_x = UIScrollViewTranspos.x + temppos.x + tempposparent.x + 152;
                    var m_y = UIScrollViewTranspos.y + temppos.y + tempposparent.y + 215f;
                    Vector3 endpos = new Vector3(m_x, m_y, 0);
                    float dis = Vector3.Distance(transform.localPosition, endpos);
                    float mtime = (dis / 500.0f);
                    iTween.MoveTo(gameObject, iTween.Hash("position", endpos, "time", mtime, "islocal", true));
                    
                    //Deactive this temporary.
                    //StartCoroutine(display(mtime));
                    ChoosedMiBaoManeger.mChoosedMiBaoManeger.MiBaochoose.Remove(gameObject);
                    ChoosedMiBaoManeger.mChoosedMiBaoManeger.bechooseMiBao.Remove(mibao_bechoosed);
                    ChoosedMiBaoManeger.mChoosedMiBaoManeger.SortMiBao();

                    JunZhuInfoRet mJunzu_Data = MiBaoShangZhen.mMiBaoShangzhen.GJunzu_Data;
                    MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(mibao_bechoosed.miBaoId);
//                    if (mMiBaoXmlTemp.shengming < 0)
//                    {
//                        mMiBaoXmlTemp.shengming = 0;
//                    }
//                    if (mMiBaoXmlTemp.gongji < 0)
//                    {
//                        mMiBaoXmlTemp.gongji = 0;
//                    }
//                    if (mMiBaoXmlTemp.fangyu < 0)
//                    {
//                        mMiBaoXmlTemp.fangyu = 0;
//                    }
//                    if (mMiBaoXmlTemp.wqSH < 0)
//                    {
//                        mMiBaoXmlTemp.wqSH = 0;
//                    }
//                    if (mMiBaoXmlTemp.wqJM < 0)
//                    {
//                        mMiBaoXmlTemp.wqJM = 0;
//                    }
//                    if (mMiBaoXmlTemp.wqBJ < 0)
//                    {
//                        mMiBaoXmlTemp.wqBJ = 0;
//                    }
//                    if (mMiBaoXmlTemp.wqRX < 0)
//                    {
//                        mMiBaoXmlTemp.wqRX = 0;
//                    }
//                    if (mMiBaoXmlTemp.jnSH < 0)
//                    {
//                        mMiBaoXmlTemp.jnSH = 0;
//                    }
//                    if (mMiBaoXmlTemp.jnJM < 0)
//                    {
//                        mMiBaoXmlTemp.jnJM = 0;
//                    }
//                    if (mMiBaoXmlTemp.jnBJ < 0)
//                    {
//                        mMiBaoXmlTemp.jnBJ = 0;
//                    }
//                    if (mMiBaoXmlTemp.jnRX < 0)
//                    {
//                        mMiBaoXmlTemp.jnRX = 0;
//                    }
//					mJunzu_Data.shengMing -= mibao_bechoosed.shengMing;
//					mJunzu_Data.gongJi -= mibao_bechoosed.gongJi;
//					mJunzu_Data.fangYu -= mibao_bechoosed.fangYu;
//					mJunzu_Data.wqSH -= mibao_bechoosed.wqSH;
//					mJunzu_Data.wqJM -= mibao_bechoosed.wqJM;
//					mJunzu_Data.wqBJ -= mibao_bechoosed.wqBJ;
//					mJunzu_Data.wqRX -= mibao_bechoosed.wqRX;
//					mJunzu_Data.jnSH -= mibao_bechoosed.jnSH;
//					mJunzu_Data.jnJM -= mibao_bechoosed.jnJM;
//					mJunzu_Data.jnBJ -= mibao_bechoosed.jnBJ;
//					mJunzu_Data.jnRX -= mibao_bechoosed.jnRX;

                    ChoosedMiBaoManeger.mChoosedMiBaoManeger.ZhanLi = Global.getZhanli(mJunzu_Data);

                    //实例化数字弹出
                    GameObject numObj = (GameObject)Instantiate(numAddObj);

                    numObj.SetActive(true);
                    numObj.transform.parent = transform.parent;

                    numObj.transform.localPosition = transform.localPosition + new Vector3(0, 50, 0);
                    numObj.transform.localScale = numAddObj.transform.localScale;

                    Vector3 targetPos = transform.localPosition + new Vector3(0, 100, 0);

                    MiBaoNumAddFly numFly = numObj.GetComponent<MiBaoNumAddFly>();
                    numFly.GetMibao(2, mibao_bechoosed, targetPos);

                    Destroy(gameObject, mtime * 2 / 3);
                }
            }
        }
    }

    private IEnumerator display(float m_t)
    {
        yield return new WaitForSeconds(0.01f);

        //TweenAlpha micon = Icon.GetComponent<TweenAlpha>();
        //micon.enabled = true;
        //TweenAlpha mPinZhi = PinZhi.GetComponent<TweenAlpha>();
        //mPinZhi.enabled = true;
        //TweenAlpha mbackgruond = backgruond.GetComponent<TweenAlpha>();
        //mbackgruond.enabled = true;
        //if(createdStarList.Count>0)
        //{
        //    foreach (var item in createdStarList)
        //    {
        //        TweenAlpha msprite = item.GetComponent<TweenAlpha>();
        //        msprite.enabled = true;
        //    }
        //}
    }

    private void CreateStars(int num)
    {
        if (num <= 0)
        {
            return;
        }

        //Clear created star list.
        foreach (var star in createdStarList)
        {
            Destroy(star.gameObject);
        }
        createdStarList.Clear();

        //Create star list.
        for (int i = 0; i < num; i++)
        {
            var prefab = m_IconSampleManager.ButtomSprite.gameObject;

            GameObject spriteObject = (GameObject)Instantiate(prefab);
            spriteObject.SetActive(true);

            spriteObject.transform.parent = prefab.transform.parent;
            spriteObject.transform.localScale = prefab.transform.localScale;
            spriteObject.transform.localPosition = prefab.transform.localPosition + new Vector3(i * 20 - (num - 1) * 10, 0, 0);

            createdStarList.Add(spriteObject);
        }
        m_IconSampleManager.ButtomSprite.gameObject.SetActive(false);
    }
}
