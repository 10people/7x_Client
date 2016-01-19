using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using qxmobile;
using qxmobile.protobuf;

public class ZhuangBei: XmlLoadManager
{
    public int id;

	public string m_name;

    public string funDesc;

    public int buWei;

    public int pinZhi;

    public string dengji;

    public List<float> xishu;

    public string gongji;

    public string fangyu;

    public string shengming;

	public int wqSH;
	
	public int wqJM;
	
	public int wqBJ;
	
	public int wqRX;
 
	public int jnSH;
	
	public int jnJM;
	
	public int jnBJ;
	
	public int jnRX;
 
    public int gongjiType;

    public int qianghuaMaxLv;

    public int xilianMaxLv;

    public int exp;

    public int expId;

    public string qianghuaId;

    public string icon;

    public int jinjieLv;

    public string jinjieItem;

    public string jinjieNum;

    public int jiejieId;

    public string jinjieIcon;

    public int color;
    public string skill;
    public int modelId;
    public int wqBJL;
    public int jnBJL;
    public int wqMBL;
    public int jnMBL;
    public int sxJiaCheng;
	public static List<ZhuangBei> templates = new List<ZhuangBei>();


    public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "ZhuangBei.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
    }

    public static void CurLoad(ref WWW www, string path, Object obj)
    {
        {
            templates.Clear();
        }

        XmlReader t_reader = null;

        if (obj != null)
        {
            TextAsset t_text_asset = obj as TextAsset;

            t_reader = XmlReader.Create(new StringReader(t_text_asset.text));

            //			Debug.Log( "Text: " + t_text_asset.text );
        }
        else
        {
            t_reader = XmlReader.Create(new StringReader(www.text));
        }

        bool t_has_items = true;

        do
        {
            t_has_items = t_reader.ReadToFollowing("ZhuangBei");

            if (!t_has_items)
            {
                break;
            }

            ZhuangBei t_template = new ZhuangBei();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.m_name = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.funDesc = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.buWei = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.pinZhi = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.dengji = t_reader.Value;

                t_reader.MoveToNextAttribute();

                string[] temp = t_reader.Value.Split(',');
                t_template.xishu = new List<float>();
                for (int i = 0; i < temp.Length; i++)
                {
                    t_template.xishu.Add(float.Parse(temp[i]));
                }
                t_reader.MoveToNextAttribute();
                t_template.gongji = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.fangyu = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.shengming = t_reader.Value;

                //				_template.tongli = _element.GetAttribute("tongli");

                //				_template.wuli = _element.GetAttribute("wuli");

                //				_template.mouli = _element.GetAttribute("mouli");

                t_reader.MoveToNextAttribute();
                t_template.wqSH = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.wqJM = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.wqBJ = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.wqRX = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.jnSH = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.jnJM = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.jnBJ = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.jnRX = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.gongjiType = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.qianghuaMaxLv = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.xilianMaxLv = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.exp = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.expId = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.qianghuaId = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.icon = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.jinjieLv = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.jinjieItem = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.jinjieNum = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.jiejieId = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.jinjieIcon = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.color = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.skill = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.modelId = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.wqBJL = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.jnBJL = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.wqMBL = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.jnMBL = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.sxJiaCheng = int.Parse(t_reader.Value);

            }

            //			t_template.Log();

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static ZhuangBei getZhuangBeiById(int id)
    {
        foreach (ZhuangBei template in templates)
        {
            if (template.id == id)
            {
                return template;
            }
        }

        Debug.LogWarning("XML ERROR: Can't get ZhuangBei with id " + id);

        return null;
    }
    public static int GetItem_Bu_Wei(BagItem p_item)
    {
        return GetItem_Bu_Wei(p_item.itemId);
    }


    public static int GetItem_Bu_Wei(int p_id)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            ZhuangBei t_item = templates[i];

            if (t_item.id == p_id)
            {
                return t_item.buWei;
            }
        }

        Debug.LogError("Not Exist In ZhuangBei: " + p_id);

        return 0;
    }

    public static bool AreAllWeaponOrAllArmor(int p_0_bu_wei, int p_1_bu_wei)
    {
        if (IsWeapon(p_0_bu_wei) && IsWeapon(p_1_bu_wei) ||
           IsArmor(p_0_bu_wei) && IsArmor(p_1_bu_wei))
        {
            return true;
        }

        return false;
    }

    public static bool IsEquip(BagItem p_item)
    {
        return IsWeapon(p_item) || IsArmor(p_item);
    }

    public static bool IsWeapon(BagItem p_item)
    {
        return IsWeapon(GetItem_Bu_Wei(p_item));
    }

    public static bool IsArmor(BagItem p_item)
    {
        return IsArmor(GetItem_Bu_Wei(p_item));
    }

    private static bool IsWeapon(int p_bu_wei)
    {
        if (p_bu_wei >= 1 && p_bu_wei <= 3)
        {
            return true;
        }

        return false;
    }

    private static bool IsArmor(int p_bu_wei)
    {
        if (p_bu_wei >= 11 && p_bu_wei <= 16)
        {
            return true;
        }

        return false;
    }

    public static int GetNameId(int p_id)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            ZhuangBei t_item = templates[i];

            if (t_item.id == p_id)
            {
				return int.Parse(t_item.m_name);
            }
        }

        Debug.LogError("Not Exist In ZhuangBei: " + p_id);

        return -1;
    }
    public static ZhuangBei GetItemByID(int p_id)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            ZhuangBei t_item = templates[i];

            if (t_item.id == p_id)
            {
                return t_item;
            }
        }

        Debug.LogError("Not Exist In ZhuangBei: " + p_id);

        return null;
    }

    public static int GetExpIdBy_EquipId(int equip_id)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].id == equip_id)
            {
                return templates[i].expId;
            }
        }
        return 0;
    }

    public static string GetIcon_ByEquipId(int equip_id)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].id == equip_id)
            {
                return templates[i].icon;
            }
        }
        return "";
    }
    public static int GetMaxLevelByEquipId(int equip_id)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].id == equip_id)
            {
                return templates[i].qianghuaMaxLv;
            }
        }
        return 0;
    }

    public static string GetQiangHuaIdByEquipID(int equip_id)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].id == equip_id)
            {
                return templates[i].qianghuaId;
            }
        }
        return null;
    }

    public static int GetColorByEquipID(int equip_id)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].id == equip_id)
            {
                return templates[i].color;
            }
        }
        return 0;
    }

    public static int GetBuWeiByItemId(int materialId)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (int.Parse(templates[i].jinjieItem) == materialId)
            {
                return templates[i].buWei;
            }
        }
        return 0;
    }
}
