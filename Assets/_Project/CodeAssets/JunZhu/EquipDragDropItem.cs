using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EquipDragDropItem : UIDragDropItem { //拖拽玩家身上或背包中的装备 进行穿脱操作

	public bool m_equipOfBody;

	public int m_bagIndex;

	[HideInInspector]
	public BagItem m_bagItem;
	

	public void SetData(BagItem tempBagItem,int tempBagIndex)
	{
		m_bagItem = tempBagItem;
		m_bagIndex = tempBagIndex;
	}

	protected override void OnDragDropRelease (GameObject surface)
	{
		if (surface != null)
		{
			if(m_equipOfBody) //通过拖拽后放手时碰撞到的gameobject来决定是拖装备 还是穿装备
			{
                if (NGUITools.FindInParents<UIDragScrollView>(surface)) //脱下装备 通过UIDragScrollView知道现在碰撞物体是scovlliew的item 进而知道现在是进行脱装备操作
				{
                    int tempNum = 0;
                    for (int i = 3; i < 6; i++)
                    {
                        if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(i))
                        {
                            ++tempNum;
                        }
                    }
                    if (tempNum == 1) //身上只有一件装备
                    {
                        PopupWindowManager.m_manager.m_equipError.SetActive(true);
                    }
                    else
                    {
                        EquipRemoveReq tempRemoveReq = new EquipRemoveReq();
                        tempRemoveReq.gridIndex = m_bagIndex;
//                        Debug.Log("tempBagIndex" + m_bagIndex);
                        MemoryStream tempStream = new MemoryStream();
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        t_qx.Serialize(tempStream, tempRemoveReq);

                        byte[] t_protof;
                        t_protof = tempStream.ToArray();
                        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EquipRemove, ref t_protof);

                        NGUITools.Destroy(gameObject);
                    }
				}
			}
			else if(surface.name.Equals("EquipOfBodyCollider")) //穿上装备
			{
				EquipAddReq tempAddReq = new EquipAddReq();

				tempAddReq.bagDBId = m_bagIndex;
                
				MemoryStream tempStream = new MemoryStream();

				QiXiongSerializer t_qx = new QiXiongSerializer();

				t_qx.Serialize(tempStream, tempAddReq);
				
				byte[] t_protof;
				t_protof = tempStream.ToArray();
			//	SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EquipAdd, ref t_protof);

				NGUITools.Destroy(gameObject);
			}
		}
		base.OnDragDropRelease(surface);
	}
	
}
