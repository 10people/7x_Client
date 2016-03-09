using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EnterLoading : MonoBehaviour {

	public  List<long> MiBaoidList = new List<long>();
	public static EnterLoading m_EnterLoading;
	void Awake()
	{
		m_EnterLoading = this;
	}
    void OnClick()
	{
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
			
		}
		                       //参数为武将ID
		//EnterBattleField.EnterBattlePve(-100, 0, LevelType.LEVEL_NORMAL);
		if(CityGlobalData.PT_Or_CQ)
		{
			int  my_tempSection = MapData.mapinstance.myMapinfo.s_section;
			int a = Pve_Level_Info.CurLev;
			
			int my_tempLevel = MapData.mapinstance.Lv[a].guanQiaId%10;
			
			CityGlobalData.m_tempSection = my_tempSection;
			CityGlobalData.m_tempLevel = my_tempLevel;
			
			//		wujiangidlist = GeneralManeger.instManeger.BuzhenGrl;                                                                  //把武将ID添加到wujianglist中
			for (int x = MiBaoidList.Count; x < 3; x++)
			{
				
				MiBaoidList.Add(-1);
			}
			
			SendBuZhenMassege (MiBaoidList);   
			if(MapData.mapinstance.Lv[a].type == 1)
			{
				EnterBattleField.EnterBattlePve(my_tempSection, my_tempLevel, LevelType.LEVEL_ELITE);
			}
			else{
				EnterBattleField.EnterBattlePve(my_tempSection, my_tempLevel, LevelType.LEVEL_NORMAL);
			}

		}
		else{
			int  my_tempSection = MapData.mapinstance.myMapinfo.s_section;
			int a = Pve_Level_Info.CurLev;
			
			int my_tempLevel = a%10;
			
			CityGlobalData.m_tempSection = my_tempSection;
			CityGlobalData.m_tempLevel = my_tempLevel;
			
			//		wujiangidlist = GeneralManeger.instManeger.BuzhenGrl;                                                                  //把武将ID添加到wujianglist中
			for (int x = MiBaoidList.Count; x < 3; x++)
			{
				
				MiBaoidList.Add(-1);
			}
			
			SendBuZhenMassege (MiBaoidList);   
			EnterBattleField.EnterBattlePve(my_tempSection, my_tempLevel, LevelType.LEVEL_TALE);
		}


	}
	public void SendBuZhenMassege(List<long> wujiangId)
	{
		MibaoSelect Mibaoid = new MibaoSelect ();
		
		MemoryStream miBaoStream = new MemoryStream ();
		
		QiXiongSerializer MiBaoSer = new QiXiongSerializer ();
		Mibaoid.type = 1;
		Mibaoid.mibaoIds = MiBaoidList;
		foreach(int m_id in MiBaoidList)
		{
			Debug.Log("PVE Mibao Dbid = " +m_id);
		}
		MiBaoSer.Serialize (miBaoStream,Mibaoid);
		byte[] t_protof;
		t_protof = miBaoStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_MIBAO_SELECT,ref t_protof);
	}

}
