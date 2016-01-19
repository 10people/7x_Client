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

	public UIProgressBar barBoss_2;

	public UIProgressBar barPlayer;

	public UISprite spriteAvatar;

	public UILabel LabelBloodNum;


	private BaseAI focusNode = null;

	private float targetValue;

	private float curValue;


	void Start ()
	{
		targetValue = 1;

		focusNode = null;

		curValue = 1;

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

			if(nodeType == NodeType.BOSS)
			{
				barBoss_2.gameObject.SetActive(focusNode.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hpNum) > 1);
			}
		}

		if(nodeType == NodeType.PLAYER)
		{
			spriteAvatar.spriteName = "PlayerIcon" + _node.nodeData.modleId;
		}
	}
	
	void FixedUpdate()
	{
		if (focusNode == null) return;

		targetValue = focusNode.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) / focusNode.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMaxReal );

		if(focusNode.isAlive == false || focusNode.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) < 0)
		{
			Start();
		}
		else
		{
			UpdateBarPlayer ();
			
			UpdateBarBoss ();
		}
	}

	void UpdateBarPlayer()
	{
		if (barPlayer.gameObject.activeSelf == false) return;

		float step = .05f;

		float value = barPlayer.value;

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

		barPlayer.value = value;
	}

	void UpdateBarBoss()
	{
		if (focusNode.nodeData.nodeType != NodeType.BOSS) return;

		int indexMax = (int)focusNode.nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hpNum);

		float step = (1 / (indexMax * .3f)) / 30f;

		float unity = 1f / indexMax;

		int curIndex = (int)(curValue / unity);

		if(targetValue % unity == 0)
		{
			curIndex --;
		}

		float _curValue = (targetValue - (curIndex * unity)) / unity;

		if(curValue - step > targetValue)
		{
			//float step = .05f;
			
			if(Mathf.Abs(curValue - _curValue) > step * 1.5f)
			{
				_curValue = curValue - step;
			}
		}
		else if(curValue + step < targetValue)
		{
			if(Mathf.Abs(curValue - _curValue) > step * 1.5f)
			{
				_curValue = curValue + step;
			}
		}
		else
		{
			_curValue = targetValue;
		}

		curValue = _curValue;

		float curIndexValue = (_curValue - (curIndex * unity)) / unity;

		if(curIndex < 1)
		{
			barBoss_2.value = 0f;

			barBoss.value = curIndexValue;
		}
		else
		{
			barBoss_2.value = curIndexValue;

			barBoss.value = 1f;
		}

		string[] strTempNums = LabelBloodNum.text.Split ('x');

		string strTempNum = strTempNums [1];

		int tempNum = int.Parse(strTempNum);

		if(tempNum != curIndex + 1)
		{
			LabelBloodNum.transform.localScale = new Vector3(3, 3, 0);

			TweenScale.Begin(LabelBloodNum.gameObject, .3f, new Vector3(1, 1, 1));
		}

		LabelBloodNum.text = "x" + (curIndex + 1);
	}

}
