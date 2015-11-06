using UnityEngine;
using System.Collections;
using qxmobile.protobuf;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
public class MiBaoNumAddFly : MonoBehaviour
{

    public List<Label> shuXingLabels = new List<Label>();

    public UILabel shengMingLabel;
    public UILabel gongJiLabel;
    public UILabel fangYuLabel;

    public UILabel gaoJi1;
    public UILabel gaoJi2;

    private string gaoJi1Str;
    private string gaoJi2Str;

    [HideInInspector]
    public List<string> Shuxing = new List<string>();
    [HideInInspector]
    public List<string> ShuxingData = new List<string>();

    private float time = 0.8f;

    public void GetMibao(int state, MibaoInfo mibaoInfo, Vector3 position)
    {
        string str1 = "攻击";
        string str2 = "武器暴击加深";
        string Gaoji;
        string GaojiData;

        Shuxing.Clear();
        ShuxingData.Clear();

//        if (mibaoInfo.wqSH > 0)
//        {
//            Gaoji = "武器伤害加深";
//            GaojiData = mibaoInfo.wqSH.ToString();
//
//            Shuxing.Add(Gaoji);
//            ShuxingData.Add(GaojiData);
//        }
//		if (mibaoInfo.wqJM > 0)
//        {
//            Gaoji = "武器伤害减免";
//            GaojiData = mibaoInfo.wqJM.ToString();
//            Shuxing.Add(Gaoji);
//            ShuxingData.Add(GaojiData);
//        }
//		if (mibaoInfo.wqBJ > 0)
//        {
//            Gaoji = "武器暴击加深";
//            GaojiData = mibaoInfo.wqBJ.ToString();
//            Shuxing.Add(Gaoji);
//            ShuxingData.Add(GaojiData);
//        }
//		if (mibaoInfo.wqRX > 0)
//        {
//            Gaoji = "武器暴击减免";
//            GaojiData = mibaoInfo.wqRX.ToString();
//            Shuxing.Add(Gaoji);
//            ShuxingData.Add(GaojiData);
//        }
//		if (mibaoInfo.jnSH > 0)
//        {
//            Gaoji = "技能伤害加深";
//            GaojiData = mibaoInfo.jnSH.ToString();
//            Shuxing.Add(Gaoji);
//            ShuxingData.Add(GaojiData);
//        }
//		if (mibaoInfo.jnJM > 0)
//        {
//            Gaoji = "技能伤害减免";
//            GaojiData = mibaoInfo.jnJM.ToString();
//            Shuxing.Add(Gaoji);
//            ShuxingData.Add(GaojiData);
//        }
//		if (mibaoInfo.jnBJ > 0)
//        {
//            Gaoji = "技能暴击加深";
//            GaojiData = mibaoInfo.jnBJ.ToString();
//            Shuxing.Add(Gaoji);
//            ShuxingData.Add(GaojiData);
//        }
//		if (mibaoInfo.jnRX > 0)
//        {
//            Gaoji = "技能暴击减免";
//            GaojiData = mibaoInfo.jnRX.ToString();
//            Shuxing.Add(Gaoji);
//            ShuxingData.Add(GaojiData);
//        }
//
        if (Shuxing.Count == 2)
        {
            //Debug.Log("Shuxing.count" +Shuxing.Count);
            gaoJi1Str = ShuxingData[0] + Shuxing[0];
            gaoJi2Str = ShuxingData[1] + Shuxing[1];
        }

        if (state == 1)
        {
            shengMingLabel.text = MyColorData.getColorString(4, "+" + mibaoInfo.shengMing.ToString() + "生命");
            gongJiLabel.text = MyColorData.getColorString(4, "+" + mibaoInfo.gongJi.ToString() + "攻击");
            fangYuLabel.text = MyColorData.getColorString(4, "+" + mibaoInfo.fangYu.ToString() + "防御");
            gaoJi1.text = gaoJi1Str == null ? "" : MyColorData.getColorString(4, "+" + gaoJi1Str);
            gaoJi2.text = gaoJi2Str == null ? "" : MyColorData.getColorString(4, "+" + gaoJi2Str);
        }

        else if (state == 2)
        {
            shengMingLabel.text = MyColorData.getColorString(5, "-" + mibaoInfo.shengMing.ToString() + "生命");
            gongJiLabel.text = MyColorData.getColorString(5, "-" + mibaoInfo.gongJi.ToString() + "攻击");
            fangYuLabel.text = MyColorData.getColorString(5, "-" + mibaoInfo.fangYu.ToString() + "防御");
            gaoJi1.text = gaoJi1Str == null ? "" : MyColorData.getColorString(5, "-" + gaoJi1Str);
            gaoJi2.text = gaoJi2Str == null ? "" : MyColorData.getColorString(5, "-" + gaoJi2Str);
        }

        FlyAnim(position);
    }

    void FlyAnim(Vector3 pos)
    {
        Hashtable fly = new Hashtable();

        fly.Add("time", time);
        fly.Add("position", pos);
        fly.Add("easetype", iTween.EaseType.easeOutQuart);
        fly.Add("islocal", true);
        fly.Add("oncomplete", "Complete");
        fly.Add("oncompletetarget", this.gameObject);

        iTween.MoveTo(this.gameObject, fly);
    }

    void Complete()
    {
//        if (UIYindao.m_UIYindao.m_isOpenYindao && GetPveTempID.CurLev == 100205)
//        {
//            MapData.mapinstance.GuidLevel = -1;
//            MapData.mapinstance.ShowPVEGuid();
//        }

        Destroy(this.gameObject);
    }
}
