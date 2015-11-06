using UnityEngine;
using System.Collections;

using qxmobile.protobuf;

public class EnemyBar : MonoBehaviour
{
	public GameObject gc_barBoss;

	public GameObject gc_barPlayer;

	public UILabel labelBossName;

	public UILabel labelPlayerName;

	public UIProgressBar barBoss;

	public UIProgressBar barPlayer;

	public UISprite spriteAvatar;


	private BaseAI focusNode = null;

	private float targetValue;

	void Start ()
	{
		targetValue = 1;

		focusNode = null;

		gc_barBoss.SetActive (false);

		gc_barPlayer.SetActive (false);
	}

	public void setFocusNode(BaseAI _node)
	{
		if (_node.nodeId < 0) return;

		if (gc_barBoss.activeSelf == true) return;

		if (gc_barPlayer.activeSelf == true) return;

		if (_node.stance != BaseAI.Stance.STANCE_ENEMY) return;

		if (_node.isAlive == false) return;

		if (_node.nodeData.GetAttribute ((int)AIdata.AttributeType.ATTRTYPE_hp) < 0) return;

		NodeType nodeType = _node.nodeData.nodeType;

		gc_barBoss.SetActive (nodeType == NodeType.BOSS);

		gc_barPlayer.SetActive(nodeType == NodeType.PLAYER);

		labelBossName.text = _node.nodeData.nodeName;

		labelPlayerName.text = _node.nodeData.nodeName;

		if(nodeType == NodeType.BOSS || nodeType == NodeType.PLAYER)
		{
			focusNode = _node;
		}

		if(nodeType == NodeType.PLAYER)
		{
			spriteAvatar.spriteName = "PlayerIcon" + _node.nodeData.modleId;
		}
	}
	
	void FixedUpdate()
	{
		if (focusNode == null) return;

		targetValue = focusNode.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) / focusNode.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax );

		if(focusNode.isAlive == false || focusNode.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) < 0)
		{
			Start();
		}

		UpdateBar ();
	}

	void UpdateBar()
	{
		float step = .05f;

		float value = 0;

		if (barBoss.gameObject.activeSelf == true) value = barBoss.value;

		else if(barPlayer.gameObject.activeSelf == true) value = barPlayer.value;

		if(Mathf.Abs(value - targetValue) < step * 1.5f)
		{
			value = targetValue;
		}
		else if(value > targetValue)
		{
			value -= step;
		}
		else if(value < targetValue)
		{
			value += step;
		}

		barBoss.value = value;

		barPlayer.value = value;
	}

}
