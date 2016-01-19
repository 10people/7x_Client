using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BattleReplayorWrite : MonoBehaviour, SocketProcessor
{
	public BattleInit battleDate;


	private static BattleReplayorWrite _instance;

	private List<ReplayNode> replayNodes = new List<ReplayNode>();


	void Awake() { _instance = this; }

	public static BattleReplayorWrite Instance() { return _instance; }

	void OnDestroy(){
		_instance = null;
	}

	public void initWritor()
	{
		for(int i = 0; i < BattleControlor.Instance().enemyNodes.Count; i++)
		{
			BaseAI node = BattleControlor.Instance().enemyNodes[i];

			ReplayNode replay = (ReplayNode)node.gameObject.AddComponent<ReplayNode>();

			replay.initDate(node);

			replay.heroOrSoldier = 0;

			replay.troopPosition = i + 10;

			replay.nodePosition = 0;

			replayNodes.Add(replay);
		}

		for(int i = 0; i < BattleControlor.Instance().selfNodes.Count; i++)
		{
			BaseAI node = BattleControlor.Instance().selfNodes[i];

			ReplayNode replay = (ReplayNode)node.gameObject.AddComponent<ReplayNode>();
			
			replay.initDate(node);

			replay.heroOrSoldier = 0;

			replay.troopPosition = i;
			
			replay.nodePosition = 0;

			replayNodes.Add(replay);
		}
	}

	public void addReplayNodeAutoFight(int nodeId)
	{
		foreach(ReplayNode re in replayNodes)
		{
			if(re.nodeId != nodeId) continue;

			re.addAutoFight();

			return;
		}
	}

	public void addReplayNodeAttacked(int nodeId, float hpValue)
	{
		foreach(ReplayNode re in replayNodes)
		{
			if(re.nodeId != nodeId) continue;

			re.addAttackedNode(hpValue);

			return;
		}
	}

	public void addReplayPlayerMove(int nodeId, Vector3 offset)
	{
		foreach(ReplayNode re in replayNodes)
		{
			if(re.nodeId != nodeId) continue;

			re.addKingMoveNode(offset);

			return;
		}
	}

	public void addReplayKingRevise(int nodeId, Vector3 position)
	{
		foreach(ReplayNode re in replayNodes)
		{
			if(re.nodeId != nodeId) continue;
			
			re.addKingReviseNode(position);
			
			return;
		}
	}

	public void addReplayKingForward(int nodeId, Vector3 forward)
	{
		foreach(ReplayNode re in replayNodes)
		{
			if(re.nodeId != nodeId) continue;
			
			re.addKingForwardNode(forward);
			
			return;
		}
	}

	public void addReplayKingAttack(int nodeId, Vector3 offset)
	{
		foreach(ReplayNode re in replayNodes)
		{
			if(re.nodeId != nodeId) continue;
			
			re.addKingAttackNode(offset);
			
			return;
		}
	}

	public void addReplayKingWeapon(int nodeId, KingControllor.WeaponType weapon)
	{
		foreach(ReplayNode re in replayNodes)
		{
			if(re.nodeId != nodeId) continue;

			re.addKingWeaponNode(weapon);
			
			return;
		}
	}

	public void addReplayKingInspire(int nodeId)
	{
		foreach(ReplayNode re in replayNodes)
		{
			if(re.nodeId != nodeId) continue;
			
			re.addKingInspireNode();
			
			return;
		}
	}

	public void addReplayKingRelif(int nodeId)
	{
		foreach(ReplayNode re in replayNodes)
		{
			if(re.nodeId != nodeId) continue;
			
			re.addKingReliefNode();
			
			return;
		}
	}

	public void addReplayKingSkill(int nodeId, int skillType)
	{
		foreach(ReplayNode re in replayNodes)
		{
			if(re.nodeId != nodeId) continue;

			re.addKingSkillNode(skillType);

			return;
		}
	}

	public List<ReplayNode> getReplayList()
	{
		return replayNodes;
	}

	public void sendReplay()
	{
//		BattleReplayData req = new BattleReplayData();
//		
//		req.battleId = 1;
//
//		req.selfs = battleDate.selfs;
//
//		req.enemys = battleDate.enemys;
//
//		req.nodes = new List<BattleNodeData>();
//
//		foreach(ReplayNode replayNode in replayNodes)
//		{
//			BattleNodeData data = new BattleNodeData();
//
//			data.nodeId = replayNode.nodeId;
//
//			data.nodes = new List<ReplayNodeData>();
//
//			foreach(BaseReplay br in replayNode.replayList)
//			{
//				ReplayNodeData nodeData = new ReplayNodeData();
//
//				if(br.replayorType == BaseReplay.ReplayorNodeType.AUTO_FIGHT)
//				{
//					ReplayAutoFight raf = (ReplayAutoFight)br;
//
//					nodeData.nodeType = ReplayorNodeTypeData.AUTO_FIGHT;
//
//					nodeData.delayTime = raf.delayTime;
//				}
//				else if(br.replayorType == BaseReplay.ReplayorNodeType.ATTACK)
//				{
//					ReplayAttacked ra = (ReplayAttacked)br;
//
//					nodeData.nodeType = ReplayorNodeTypeData.ATTACK;
//
//					nodeData.delayTime = ra.delayTime;
//
//					nodeData.hpValue = ra.hpValue;
//				}
//				else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_MOVE)
//				{
//					ReplayKingMove rkm = (ReplayKingMove)br;
//
//					nodeData.nodeType = ReplayorNodeTypeData.KING_MOVE;
//
//					nodeData.delayTime = rkm.delayTime;
//
//					nodeData.offsetX = rkm.offset.x;
//
//					nodeData.offsetY = rkm.offset.y;
//
//					nodeData.offsetZ = rkm.offset.z;
//				}
//				else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_REVISE)
//				{
//					ReplayKingRevise rkr = (ReplayKingRevise)br;
//
//					nodeData.nodeType = ReplayorNodeTypeData.KING_REVISE;
//
//					nodeData.delayTime = rkr.delayTime;
//
//					nodeData.offsetX = rkr.position.x;
//
//					nodeData.offsetY = rkr.position.y;
//
//					nodeData.offsetZ = rkr.position.z;
//				}
//				else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_FORWARD)
//				{
//					ReplayKingForward rkf = (ReplayKingForward)br;
//
//					nodeData.nodeType = ReplayorNodeTypeData.KING_FOWARD;
//
//					nodeData.delayTime = rkf.delayTime;
//					
//					nodeData.offsetX = rkf.forward.x;
//					
//					nodeData.offsetY = rkf.forward.y;
//					
//					nodeData.offsetZ = rkf.forward.z;
//				}
//				else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_ATTACK)
//				{
//					ReplayKingAttack rka = (ReplayKingAttack)br;
//
//					nodeData.nodeType = ReplayorNodeTypeData.KING_ATTACK;
//
//					nodeData.delayTime = rka.delayTime;
//
//					nodeData.offsetX = rka.offset.x;
//				
//					nodeData.offsetY = rka.offset.y;
//
//					nodeData.offsetZ = rka.offset.z;
//				}
//				else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_WEAPON)
//				{
//					ReplayKingWeapon rkw = (ReplayKingWeapon)br;
//
//					nodeData.nodeType = ReplayorNodeTypeData.KING_WEAPON;
//
//					nodeData.delayTime = rkw.delayTime;
//
//					nodeData.weapon = rkw.weapon;
//				}
//				else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_INSPIRE)
//				{
//					ReplayKingInspire rki = (ReplayKingInspire)br;
//
//					nodeData.nodeType = ReplayorNodeTypeData.KING_INSPIRE;
//
//					nodeData.delayTime = rki.delayTime;
//				}
//				else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_RELIF)
//				{
//					ReplayKingRelief rkr = (ReplayKingRelief)br;
//
//					nodeData.nodeType = ReplayorNodeTypeData.KING_RELIF;
//
//					nodeData.delayTime = rkr.delayTime;
//				}
//				else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_SKILL)
//				{
//					ReplayKingSkill rks = (ReplayKingSkill)br;
//
//					nodeData.nodeType = ReplayorNodeTypeData.KING_SKILL;
//
//					nodeData.delayTime = rks.delayTime;
//
//					nodeData.skillType = rks.skillType;
//				}
//
//				data.nodes.Add(nodeData);
//			}
//
//			req.nodes.Add(data);
//		}
//
//		MemoryStream tempStream = new MemoryStream();
//		
//		QiXiongSerializer t_qx = new QiXiongSerializer();
//		
//		t_qx.Serialize(tempStream, req);
//		
//		byte[] t_protof;
//		
//		t_protof = tempStream.ToArray();
//		
//		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_Report_battle_replay, ref t_protof);
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		return false;
	}

}
