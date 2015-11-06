using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;

public class FirstTanBao : Singleton<FirstTanBao>
{
    public bool isGet;

    //第一次探宝得强化材料
    public bool GetTanBaoState1()
    {
        for (int i = 0; i < BagData.Instance().m_bagItemList.Count; i++)
        {
            if (BagData.Instance().m_bagItemList[i].itemId == 920001 &&
                BagData.Instance().m_bagItemList[i].cnt > 0)
            {
                return true;
            }
        }
        return false;
    }

    //第二次探宝的秘宝
    public bool GetTanBaoState2()
    {
        return CityGlobalData.isTanBaoGet;
    }
}
