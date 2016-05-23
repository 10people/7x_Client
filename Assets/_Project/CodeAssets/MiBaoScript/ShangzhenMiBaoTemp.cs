using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class ShangzhenMiBaoTemp : MonoBehaviour {
	[HideInInspector]public MibaoInfo  m_mibaoinfo ;
	[HideInInspector]public List<MibaoInfo> MibaoInfo_ZH = new List<MibaoInfo> ();
	[HideInInspector]public int  MiBaoZuHeId;

    [HideInInspector]
    private List<GameObject> createdStarList = new List<GameObject>();

    [HideInInspector]
    public bool ischoosed;
	private GameObject mibaoUIroot;
    public IconSampleManager m_IconSampleManager;

    public void SetIcon()
    {
        bool isShowDimmer = m_mibaoinfo.level <= 0;
        int startCount = m_mibaoinfo.star;
        MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(m_mibaoinfo.miBaoId);
        string qualityFrameSpriteName = IconSampleManager.QualityPrefix + (mMiBaoXmlTemp.pinzhi - 1);
        string fgSpriteName = "250101";
        string rightButtomSpriteName = "flag_finish";
        string buttomSpriteName = "xingxing1";

        m_IconSampleManager.SetIconType(IconSampleManager.IconType.OldOldMiBao);
        m_IconSampleManager.SetIconBasic(20, fgSpriteName, "", qualityFrameSpriteName,isShowDimmer);
        m_IconSampleManager.SetIconBasicDelegate(false,true,OnChoosed);
        m_IconSampleManager.SetIconDecoSprite("",rightButtomSpriteName, buttomSpriteName);
        m_IconSampleManager.ButtomSprite.gameObject.SetActive(false);

        CreateStars(startCount);
    }

	void Start () 
    {
		mibaoUIroot = GameObject.Find ("ChoosemiBao");
	}
	
	void Update () 
    {
		if(ChoosedMiBaoManeger.mChoosedMiBaoManeger.bechooseMiBao.Contains(m_mibaoinfo))
		{
			m_IconSampleManager.RightButtomCornorSprite.gameObject.SetActive(true);
			ischoosed = true;
		}
		else
		{
            m_IconSampleManager.RightButtomCornorSprite.gameObject.SetActive(false);
			ischoosed = false;
		}
	}

	private void LoadRemindUIBack(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject Remind = Instantiate( p_object ) as GameObject;
		GameObject sup = GameObject.Find("Mapss");
		
		Remind.transform.parent = sup.transform;
		Remind.transform.localScale = new Vector3(1,1,1);
		Remind.transform.localPosition = new Vector3(0,0,0);
	}

	public void OnChoosed(GameObject go)
	{
		if(ischoosed)
		{
			return;
		}
		else if(ChoosedMiBaoManeger.mChoosedMiBaoManeger.MiBaochoose.Count>=3)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_REMIND_MI_BAO ),LoadRemindUIBack);
			return;
		}
		else
        {
			//计算坐标作为参数传递

            float mx = transform.localPosition.x + transform.parent.localPosition.x + transform.parent.parent.localPosition.x + 152;
            float my = transform.localPosition.y + transform.parent.localPosition.y + transform.parent.parent.localPosition.y + 215;
            Vector3 pos = new Vector3(mx, my, 0);
            //Vector3 pos = transform.localPosition;
			if(mibaoUIroot)
			{
				ChoosedMiBaoManeger mChoosedMiBaoManeger = mibaoUIroot.GetComponent<ChoosedMiBaoManeger>();
				mChoosedMiBaoManeger.mibaoin_bechoosed = m_mibaoinfo;
				mChoosedMiBaoManeger.InstanceMiBao(pos,false);
				ischoosed = true;
			}
		}
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
		for(int i = 0; i < num; i++)
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
