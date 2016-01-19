using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ActivityAwardItem : MonoBehaviour {

	public List<UITexture> m_awardIconList = new List<UITexture>();

	public List<UILabel> m_labelList = new List<UILabel>();

	public UILabel m_indexLabel;

	public UILabel m_stateLabel;
    int index = 0;

	void OnDestroy(){
		m_awardIconList.Clear();
	}

	public void InitWith(DailyAwardArr tempAwardArr)
	{
		int tempCount = tempAwardArr.items.Count;

		for(int i = 0;i < tempCount;i ++)
		{
            index = i;
            Global.ResourcesDotLoad( Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_ICON_PREFIX) + tempAwardArr.items[i].awardIconId, 
			                        ResourcesLoadCallBack );
			 
			m_labelList[i].text = tempAwardArr.items[i].awardName + "\nX" + tempAwardArr.items[i].cnt;
		}
		for(int i = tempCount;i < m_awardIconList.Count;i ++)
		{
			m_labelList[i].gameObject.SetActive(false);

			m_awardIconList[i].gameObject.SetActive(false);
		}

		m_indexLabel.text = tempAwardArr.name;

		m_stateLabel.text = (tempAwardArr.yiLing > 0)?("已领取"):("未领取");
	}

    public void ResourcesLoadCallBack( ref WWW p_www, string p_path, Object p_object )
    {
		m_awardIconList[index].mainTexture = (Texture)p_object;
 
    }
}