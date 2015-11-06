using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RewardMovingEffectManagerment : MonoBehaviour
{
    public  GameObject m_Object;
    public  GameObject m_Grid;
    public  int m_PosOffset = 80;

    public  int m_DefaltPosX = -50;
    private Vector3 _DefaltPos;
    public struct AwardInfo
    {
        public int _id;
        public int _Type;
        public string _Icon;
        public int _Count;
    };

    private List<AwardInfo> _ListInfo = new List<AwardInfo>();

    void OnEnable()
    {
        AwardsInfoTidy("0:900001:2000#0:900006:300#0:900015:10");
    }
    public void AwardsInfoTidy(string award)
    {
        _ListInfo.Clear();
        Vector3 poss = m_Grid.transform.localPosition;
        poss.y += m_PosOffset;
        m_Grid.transform.localPosition = poss;

        _IndexLeft = 0;
        _IndexRight = 0;
        //"0:900001:2000#0:900006:300#0:900015:10"
        if (award.IndexOf('#') > -1)
        {
            string[] awardInfo = award.Split('#');
            for (int i = 0; i < awardInfo.Length; i++)
            {
                string[] aw = awardInfo[i].Split(':');
                AwardInfo adi = new AwardInfo();
                adi._id = int.Parse(aw[1]);
                adi._Type = int.Parse(aw[0]);
                adi._Icon = aw[1];
                adi._Count = int.Parse(aw[2]);
                _ListInfo.Add(adi);
            }
        }
        else
        {
            string[] aw = award.Split(':');
            AwardInfo adi = new AwardInfo();
            adi._id = int.Parse(aw[1]);
            adi._Type = int.Parse(aw[0]);
            adi._Icon = aw[1];
            adi._Count = int.Parse(aw[2]);
            _ListInfo.Add(adi);
        }
        int size = _ListInfo.Count;
        if (size % 2 == 0)
        {
            _isNoAtCenter = true;
            _DefaltPos = new Vector3(m_DefaltPosX, 0, 0);
        }
        else
        {
            _DefaltPos = Vector3.zero;
            _isNoAtCenter = false;
        }
        for (int i = 0; i < size; i++)
        {
            CreateItem();
        }
    }
 
    private bool _isNoAtCenter = false;

    private int _IndexLeft = 0;
    private int _IndexRight = 0;
    private void CreateItem()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
                           ResLoaded);
    }
    private  int _indexNum = 0;
    private  void ResLoaded(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        if (m_Grid != null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            tempObject.name = _ListInfo[_indexNum]._id.ToString();
            tempObject.transform.parent = m_Grid.transform;
            tempObject.transform.localScale = Vector3.one;

            if (_indexNum != 0)
            {
                if (_indexNum % 2 == 0)
                {
                    _IndexLeft++;
                    _DefaltPos.x = -1 * _IndexLeft * 100;
                }
                else
                {
                    _IndexRight++;
                    _DefaltPos.x = _IndexRight * 100;
                }
            }
            tempObject.transform.localPosition = _DefaltPos;
            tempObject.GetComponent<UIWidget>().depth = 8;
            tempObject.GetComponent<UIWidget>().SetDimensions(80, 80);
            IconSampleManager iconSampleManager = tempObject.GetComponent<IconSampleManager>();
            iconSampleManager.SetIconByID(_ListInfo[_indexNum]._id,"",10);

			iconSampleManager.SetIconPopText(_ListInfo[_indexNum]._id, NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(_ListInfo[_indexNum]._id).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(_ListInfo[_indexNum]._id).descId));

            tempObject.transform.localScale = Vector3.one*0.64f;

            tempObject.AddComponent<TweenPosition>();
            tempObject.GetComponent<TweenPosition>().to = tempObject.transform.localPosition + Vector3.up * m_PosOffset;
            tempObject.GetComponent<TweenPosition>().duration = 0.8f;

            if (_indexNum < _ListInfo.Count - 1)
            {
                _indexNum++;
            }
            else
            {
             //   Destroy(m_Object);
            }
         //   m_Grid.repositionNow = true;
        }
        else
        {
          p_object = null;
        }

    }
}
