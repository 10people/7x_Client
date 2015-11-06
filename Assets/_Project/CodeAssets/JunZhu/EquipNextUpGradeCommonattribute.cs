using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class EquipNextUpGradeCommonattribute : MonoBehaviour
{
    public struct EquipUpgradeInfo
    {
        public int _gongJiAfter;
        public int _fangYuAfter;
        public int _shengMingAfter;
    };
  
   public static EquipUpgradeInfo CommomAttribute(BagItem item, int zhuangbeiid)
    {
        EquipUpgradeInfo equipupgrade = new EquipUpgradeInfo();
        int qiangHuaLv = item.qiangHuaLv;
        int qiangHuaExp = item.qiangHuaExp;
        int jinJieZbId = int.Parse(ZhuangBei.getZhuangBeiById(zhuangbeiid).jiejieId);
        List<ExpTempTemp> expTempList = ExpTempTemp.GetEquipUpgradeInfo(ZhuangBei.GetExpIdBy_EquipId(item.itemId));
        if (expTempList == null)
        {
            return equipupgrade;
        }

        foreach (ExpTempTemp temp in expTempList)
        {
            if (temp.level < qiangHuaLv)
            {
                qiangHuaExp += temp.needExp;
            }
        }

        // 进阶后的装备信息
        ZhuangBei jinJieZb = ZhuangBei.getZhuangBeiById(jinJieZbId);
        if (jinJieZb == null)
        {
            return equipupgrade;
        }

        List<ExpTempTemp> expTemps = ExpTempTemp.GetEquipUpgradeInfo(jinJieZb.expId);
        for (int j = 0; j < expTemps.Count; j++)
        {
            for (int i = 0; i < expTemps.Count - 1 - j; i++)
            {
                if (expTemps[i].level > expTemps[i + 1].level)
                {
                    ExpTempTemp equip = new ExpTempTemp();
                    equip = expTemps[i];
                    expTemps[i] = expTemps[i + 1];
                    expTemps[i + 1] = equip;
                }

            }
        }
        int afterLevel = 0; // 初始等級為0
        int afterExp = qiangHuaExp;
        foreach (ExpTempTemp temp in expTemps)
        {
            if (temp.needExp == -1)
            {// 表示满级
                break;
            }
            if (afterExp >= temp.needExp)
            {
                afterLevel += 1;
                afterExp -= temp.needExp;
            }
        }
        QiangHuaTemplate qiangHua = QiangHuaTemplate.GetTemplateByItemId(int.Parse(jinJieZb.qianghuaId), afterLevel);
        if (qiangHua == null)
        {
            return equipupgrade;
        }
        equipupgrade._gongJiAfter = int.Parse(jinJieZb.gongji) + qiangHua.gongji;
        equipupgrade._fangYuAfter = int.Parse(jinJieZb.fangyu) + qiangHua.fangyu;
        equipupgrade._shengMingAfter = int.Parse(jinJieZb.shengming) + qiangHua.shengming;
        return equipupgrade;
    }
}
