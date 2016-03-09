using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class JunZhuTaoZhuangEffect : MonoBehaviour
{
    public UIGrid m_gridLeft;
    public UIGrid m_gridRight;
    public UIScrollView m_ScrollViewLeft;
    public UIScrollView m_ScrollViewRight;
    public GameObject m_Back;
    public struct TaoZHuang
    {
        public string _Title;
        public string _Des;
        public bool _IsMax;
        public int _Type;
    };
    private List<TaoZHuang> _listInfo = new List<TaoZHuang>();
     
	void Start ()
	{
 
	}
	void OnEnable()
	{
        m_Back.SetActive(true);
       ShowInfo ();
	}
    void OnDisable()
    {
        m_Back.SetActive(false);
        int size = m_gridLeft.transform.childCount;
        for (int i = 0; i < size; i++)
        {
            Destroy(m_gridLeft.transform.GetChild(i).gameObject);
        }
        int size2 = m_gridRight.transform.childCount;
        for (int i = 0; i < size2; i++)
        {
            Destroy(m_gridRight.transform.GetChild(i).gameObject);
        }
    }

    void ShowInfo()
    {
        _listInfo.Clear();
        for (int i = 0; i < TaoZhuangTemplate.templates.Count; i++)
        {
            TaoZHuang tz = new TaoZHuang();
            tz._Type = TaoZhuangTemplate.templates[i].type;
            if (TaoZhuangTemplate.templates[i].type == 1)
            {
                if (EquipsOfBody.Instance().GetEquipCountByQuality(TaoZhuangTemplate.templates[i].condition) < TaoZhuangTemplate.templates[i].neededNum)
                {
                    tz._Title = NameIdTemplate.GetName_By_NameId(TaoZhuangTemplate.templates[i].targetShow)
                        + "  (" + EquipsOfBody.Instance().GetEquipCountByQuality(TaoZhuangTemplate.templates[i].condition).ToString() + "/" + TaoZhuangTemplate.templates[i].neededNum + ")";
                    tz._Des = ShuXingTemp.GetShuXingName(TaoZhuangTemplate.templates[i].shuxing1) + TaoZhuangTemplate.templates[i].num1 + "、" + ShuXingTemp.GetShuXingName(TaoZhuangTemplate.templates[i].shuxing2) + TaoZhuangTemplate.templates[i].num2 + "、" + ShuXingTemp.GetShuXingName(TaoZhuangTemplate.templates[i].shuxing3) + TaoZhuangTemplate.templates[i].num3;

                }
                else if (EquipsOfBody.Instance().GetEquipCountByQuality(TaoZhuangTemplate.templates[i].condition) >= TaoZhuangTemplate.templates[i].neededNum)
                {
                    tz._Title = MyColorData.getColorString(4, NameIdTemplate.GetName_By_NameId(TaoZhuangTemplate.templates[i].targetShow)
                       + "  (" + TaoZhuangTemplate.templates[i].neededNum.ToString() + "/" + TaoZhuangTemplate.templates[i].neededNum.ToString() + ")");
                    tz._Des = MyColorData.getColorString(4, ShuXingTemp.GetShuXingName(TaoZhuangTemplate.templates[i].shuxing1)
                        + TaoZhuangTemplate.templates[i].num1 + "、" + ShuXingTemp.GetShuXingName(TaoZhuangTemplate.templates[i].shuxing2)
                        + TaoZhuangTemplate.templates[i].num2 + "、" + ShuXingTemp.GetShuXingName(TaoZhuangTemplate.templates[i].shuxing3)
                        + TaoZhuangTemplate.templates[i].num3);
                    tz._IsMax = true;

                }
                else
                {
                    tz._Title = MyColorData.getColorString(4, NameIdTemplate.GetName_By_NameId(TaoZhuangTemplate.templates[i].targetShow)
                        + "  (" + EquipsOfBody.Instance().GetEquipCountByQuality(TaoZhuangTemplate.templates[i].condition).ToString() + "/" + TaoZhuangTemplate.templates[i].neededNum + ")");
                    tz._Des = MyColorData.getColorString(4, ShuXingTemp.GetShuXingName(TaoZhuangTemplate.templates[i].shuxing1)
                        + TaoZhuangTemplate.templates[i].num1 + "、" + ShuXingTemp.GetShuXingName(TaoZhuangTemplate.templates[i].shuxing2)
                        + TaoZhuangTemplate.templates[i].num2 + "、" + ShuXingTemp.GetShuXingName(TaoZhuangTemplate.templates[i].shuxing3)
                        + TaoZhuangTemplate.templates[i].num3);
                    tz._IsMax = true;
                }
            }
            else if (TaoZhuangTemplate.templates[i].type == 2)
            {
                if (EquipsOfBody.Instance().GetEquipCountByStrengthLevel(TaoZhuangTemplate.templates[i].condition) < TaoZhuangTemplate.templates[i].neededNum)
                {
                    tz._Title = NameIdTemplate.GetName_By_NameId(TaoZhuangTemplate.templates[i].targetShow)
                        + "  (" + EquipsOfBody.Instance().GetEquipCountByStrengthLevel(TaoZhuangTemplate.templates[i].condition).ToString() + "/" + TaoZhuangTemplate.templates[i] .neededNum+ ")";
                    tz._Des = ShuXingTemp.GetShuXingName(TaoZhuangTemplate.templates[i].shuxing1) 
                        + TaoZhuangTemplate.templates[i].num1 + "、" 
                        + ShuXingTemp.GetShuXingName(TaoZhuangTemplate.templates[i].shuxing2) 
                        + TaoZhuangTemplate.templates[i].num2 + "、" 
                        + ShuXingTemp.GetShuXingName(TaoZhuangTemplate.templates[i].shuxing3) 
                        + TaoZhuangTemplate.templates[i].num3;

                }
                else
                {
                    tz._Title = MyColorData.getColorString(4, NameIdTemplate.GetName_By_NameId(TaoZhuangTemplate.templates[i].targetShow)
                        + "  (" + EquipsOfBody.Instance().GetEquipCountByStrengthLevel(TaoZhuangTemplate.templates[i].condition).ToString() + "/" + TaoZhuangTemplate.templates[i].neededNum + ")");
                    tz._Des = MyColorData.getColorString(4, ShuXingTemp.GetShuXingName(TaoZhuangTemplate.templates[i].shuxing1)
                        + TaoZhuangTemplate.templates[i].num1 + "、"
                        + ShuXingTemp.GetShuXingName(TaoZhuangTemplate.templates[i].shuxing2)
                        + TaoZhuangTemplate.templates[i].num2 + "、" 
                        + ShuXingTemp.GetShuXingName(TaoZhuangTemplate.templates[i].shuxing3)
                        + TaoZhuangTemplate.templates[i].num3);
                    tz._IsMax = true;
                }
            }
            _listInfo.Add(tz);
        }

        Create();
    }


    void Create()
    {
        index_num = 0;
        int size = _listInfo.Count;
        int Count_Type_1 = 0;
        for (int i = 0; i < size; i++)
        {
            if (_listInfo[i]._Type == 1)
            {
                Count_Type_1++;
            }
        }

        int Max_Num_1 = 0;
        int Max_Num_2 = 0;
        for (int i = 0; i < size; i++)
        {
            if (_listInfo[i]._Type == 1 && _listInfo[i]._IsMax)
            {
                Max_Num_1 = i;
            }
            else if (_listInfo[i]._Type == 2 && _listInfo[i]._IsMax)
            {
                Max_Num_2 = i - Count_Type_1;
            }
        }
       
        //if (Max_Num_2 > 0)
        //{
        //    if (Max_Num_2 % 5 == 0)
        //    {
        //        m_ScrollViewRight.GetComponent<UIPanel>().clipOffset = new Vector2(0, -128 - (Max_Num_2 / 5 - 1) * 300);
        //    }
        //    else
        //    {
        //        m_ScrollViewRight.GetComponent<UIPanel>().clipOffset = new Vector2(0, -128 - (Max_Num_2 / 5) * 300);
        //    }
        //}
        for (int i = 0; i < size; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TAO_ZHUANG_ITEM), ResourcesLoadCallBack);
        }

        //if (Max_Num_1 > 0)
        //{
        //    if (Max_Num_1 % 5 == 0)
        //    {
        //        m_ScrollViewLeft.MoveAbsolute(new Vector3(0, (Max_Num_1 / 5 -1) * 300, 0));
            
        //        //m_ScrollViewLeft.GetComponent<UIPanel>().clipOffset = new Vector2(0, -128 - (Max_Num_1 / 5 - 1) * 300);
        //        //m_ScrollViewLeft.transform.localPosition = new Vector3(-14, (Max_Num_1 / 5 - 1) * 300, 0);
        //    }
        //    else
        //    {
        //        m_ScrollViewLeft.currentMomentum = new Vector3(0,(Max_Num_1 / 5) * 300,0);
        //        //m_ScrollViewLeft.GetComponent<UIPanel>().clipOffset = new Vector2(0, -128 - (Max_Num_1 / 5) * 300);
        //        //m_ScrollViewLeft.transform.localPosition = new Vector3(-14, (Max_Num_1 / 5) * 300, 0);
        //    }
        //}
    }
   private int index_num = 0;
    public void ResourcesLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_gridLeft != null)
        {
            GameObject tempObject = Instantiate(p_object) as GameObject;

            if (_listInfo[index_num]._Type == 1)
            {
                tempObject.transform.parent = m_gridLeft.transform;
            }
            else
            {
                tempObject.transform.parent = m_gridRight.transform;
            }
            tempObject.transform.localScale = Vector3.one;
            tempObject.transform.localPosition = Vector3.zero;
            tempObject.GetComponent<JunZhuTaoZhuangItemManagerment>().ShowInfo(_listInfo[index_num]);

            if (_listInfo[index_num]._Type == 1)
            {
                m_gridLeft.repositionNow = true;
            }
            else
            {
                m_gridRight.repositionNow = true;
            }
            if (index_num < _listInfo.Count - 1)
            {
                index_num++;
            }
            else
            {
                m_ScrollViewLeft.UpdateScrollbars(true);
                m_ScrollViewRight.UpdateScrollbars(true);
            }

        }
        else
        {
            p_object = null;
        }
    }

}
