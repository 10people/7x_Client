using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EquipGrowthGemEquipItemManagerment : MonoBehaviour {

    public UILabel m_LabelLevel;
    public UISprite m_SpriteIcon;
    public UISprite m_SpritePinZhi;
    public UILabel m_LabelSuccess;
    public struct GemsEquip
    {
        public int _gong;
        public int _fang;
        public int _ming;

    };
    struct AttributeInfo
    {
        public int _type;
        public string _count;
    };
    private List<AttributeInfo> _listsInfo = new List<AttributeInfo>();
    public List<EquipAttributeManagerment> m_listAttribute;
    public Dictionary<int, UILabel> m_DicInfo = new Dictionary<int, UILabel>();
    private int _ExpId = 0;
 
    public void ShowInfo(EquipGrowthEquipInfoManagerment.EquipBaseInfo baseInfo)
    {
        m_SpriteIcon.spriteName = baseInfo._Icon.ToString();
        m_LabelLevel.text = "[b]Lv." + baseInfo._Level + "[/b]";
        if (FunctionWindowsCreateManagerment.SpecialSizeFit(baseInfo._PinZhi))
        {
            m_SpritePinZhi.width = m_SpritePinZhi.height = 96;
        }
        else
        {
            m_SpritePinZhi.width = m_SpritePinZhi.height = 86;
        }

        m_SpritePinZhi.spriteName = QualityIconSelected.SelectQuality(baseInfo._PinZhi);
        ShowAttribute(baseInfo);
    }
    private string GetName(int index)
    {
        switch (index)
        {
            case 0:
                {
                    return "攻击";
                }
                break;
            case 1:
                {
                    return "防御";
                }
                break;
            case 2:
                {
                    return "生命";
                }
                break;

        }
        return null;
    }
    void ShowAttribute(EquipGrowthEquipInfoManagerment.EquipBaseInfo baseInfo)
    {
        _listsInfo.Clear();
        m_DicInfo.Clear();
        if (baseInfo._gemGong > 0)
        {
            AttributeInfo info = new AttributeInfo();
         
            info._type = 0;
            info._count = baseInfo._gemGong.ToString();
            _listsInfo.Add(info);
        }

        if (baseInfo._gemFang > 0)
        {
            AttributeInfo info = new AttributeInfo();
            info._type = 1;
            info._count = baseInfo._gemFang.ToString();
            _listsInfo.Add(info);
        }
 

        if (baseInfo._gemMing > 0)
        {
            AttributeInfo info = new AttributeInfo();
            info._type = 2;
            info._count = baseInfo._gemMing.ToString();
            _listsInfo.Add(info);
        }
        for (int i = 0; i < 3; i++)
        {
            if (_listsInfo.Count > 0 && i < _listsInfo.Count)
            {
                m_listAttribute[i].m_LabName.text = GetName(_listsInfo[i]._type);
                if (int.Parse(_listsInfo[i]._count) > 0)
                {
                    m_listAttribute[i].m_LabCount.text = MyColorData.getColorString(4, "+" + _listsInfo[i]._count);
                }
                else
                {
                    m_listAttribute[i].m_LabCount.text =   "+" + _listsInfo[i]._count;
                }
                m_DicInfo.Add(_listsInfo[i]._type, m_listAttribute[i].m_LabCount);
            }
            else
            {
                m_listAttribute[i].m_LabName.text = "";
                m_listAttribute[i].m_LabCount.text = "";
            }
        }
       
    }

    void ShowChangeInfo(EquipGrowthInlayLayerManagerment.AddAttribute att_change)
    {
        
        if (Mathf.Abs(att_change._GongAdd) > 0 && m_DicInfo.ContainsKey(0))
        {
            CreateClone(m_DicInfo[0].gameObject, att_change._GongAdd);
        }

        if (Mathf.Abs(att_change._FangAdd) > 0 && m_DicInfo.ContainsKey(1))
        {
            CreateClone(m_DicInfo[1].gameObject, att_change._FangAdd);

        }

        if (Mathf.Abs(att_change._MingAdd) > 0 && m_DicInfo.ContainsKey(2))
        {
            CreateClone(m_DicInfo[2].gameObject, att_change._MingAdd);
        }
    }
    void CreateClone(GameObject move, int content)
    {
        GameObject clone = NGUITools.AddChild(move.transform.parent.gameObject, move);
        clone.transform.localPosition = move.transform.localPosition;
        clone.transform.localRotation = move.transform.localRotation;
        clone.transform.localScale = move.transform.localScale;
        clone.GetComponent<UILabel>().text = "";
        if (content < 0)
        {
            clone.GetComponent<UILabel>().text = MyColorData.getColorString(5, content.ToString());
        }
        else
        {
            clone.GetComponent<UILabel>().text = MyColorData.getColorString(4, "+" + content.ToString());
        }

        clone.AddComponent(typeof(TweenPosition));
        clone.AddComponent(typeof(TweenAlpha));
        clone.GetComponent<TweenPosition>().from = move.transform.localPosition;
        clone.GetComponent<TweenPosition>().to = move.transform.localPosition + Vector3.up * 40;
        clone.GetComponent<TweenPosition>().duration = 0.5f;
        clone.GetComponent<TweenAlpha>().from = 1.0f;
        clone.GetComponent<TweenAlpha>().to = 0;
        clone.GetComponent<TweenPosition>().duration = 0.8f;
        StartCoroutine(WatiForee(clone));
    }

    IEnumerator WatiForee(GameObject obj)
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(obj);
    }
}
 
