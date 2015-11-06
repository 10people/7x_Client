using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class PveStarsMissionItemManagerment : MonoBehaviour
{
    public PveStarsMissionManagerment m_Managerment;
    public EventSuoHandle m_LingQu;
    public UISprite m_Complete;
    public UISprite m_Gou;
    public UISprite m_Back;
    public UILabel m_DesContent;
	public int m_LevelId = 0;
	public int m_StarIndex = 0;
	private int saveIndex = 0;
    public UIGrid m_awardIconGrid;


    public delegate void LingQuSendInfo(int index);
    public  LingQuSendInfo LingQu;


    struct RewardInfo
    {
        public string type;
        public string count;
        public string icon;
    }
    private RewardInfo rewardShowInfo;
	void Start ()
    {
        m_LingQu.m_Handle += GetAward;
	}

     public  void ShowReward(string [] awardInfo)
    {
        rewardShowInfo.type = awardInfo[0];
        rewardShowInfo.icon = awardInfo[1];
        rewardShowInfo.count = awardInfo[2];
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_AWARD_ITEM_SMALL), ResourcesLoadCallBack);
    }

    void GetAward(int index,GameObject obj)
    {
        LingQu(index);
		if(m_StarIndex > 0)
		{
		 saveIndex = index;
        // m_Managerment.ListItemInfo[saveIndex].m_RewardIcon.collider.enabled = false;
		 index = m_StarIndex -1;
		}
		else 
		{
			saveIndex = index;
        //    m_Managerment.ListItemInfo[saveIndex].m_RewardIcon.collider.enabled = false;
		}
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        GetPveStarAward award = new GetPveStarAward();
		award.s_starNum = m_LevelId*10+(index+1);

        t_qx.Serialize(t_tream, award);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.PVE_STAR_REWARD_GET, ref t_protof);

     
    }
    public void ResourcesLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject rewardShow = Instantiate(p_object) as GameObject;
        rewardShow.GetComponent<Collider>().enabled = false;
        rewardShow.transform.parent = m_awardIconGrid.transform;
        rewardShow.transform.localPosition = Vector3.zero;
        rewardShow.transform.localScale = Vector3.one;
        rewardShow.transform.GetComponent<TaskAwardItemAmend>().Show(rewardShowInfo.icon, rewardShowInfo.count, int.Parse(rewardShowInfo.type));
    }
}
