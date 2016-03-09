using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class EnemyBar : MonoBehaviour
{
	public GameObject gc_barBoss;

	public GameObject gc_barPlayer;

	public UILabel labelBossName;

	public UILabel labelPlayerName;

	public UIProgressBar barBossTemple;

	public UIProgressBar barBoss_white;

	public UIProgressBar barPlayer;

	public UISprite spriteAvatar;

	public UILabel LabelBloodNum;

	public UISprite barNuqi;


	private BaseAI focusNode = null;

	private KingControllor focusKing = null;

	private float targetValue;

	private float curValue;

	private float tempTargetValue;

	private float whiteRefreshTime;

	private float preTime = 1f;

	private float whiteRate = .5f;

	private float boosStep;

	private List<UIProgressBar> bosBarList = new List<UIProgressBar>();


	void Start ()
	{
		targetValue = 1;

		tempTargetValue = 1;

		focusNode = null;

		curValue = 1;

		boosStep = 0;

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
				barBossTemple.gameObject.SetActive(false);

				bosBarList.Clear();

				for(int i = 0; i < focusNode.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hpNum); i++)
				{
					GameObject barObject = Instantiate(barBossTemple.gameObject) as GameObject;

					barObject.transform.parent = barBossTemple.transform.parent;

					barObject.transform.localScale = barBossTemple.transform.localScale;

					barObject.transform.localPosition = barBossTemple.transform.localPosition;

					barObject.SetActive(true);

					UIProgressBar bar = barObject.GetComponent<UIProgressBar>();

					bar.foregroundWidget.depth = 307 + i * 2;

					UISprite sprite = bar.foregroundWidget as UISprite;

					sprite.spriteName = "battleBarBoss_" + (i % 5);

					bosBarList.Add(bar);
				}

				//barBoss_2.gameObject.SetActive(focusNode.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hpNum) > 1);
			}
		}

		if(nodeType == NodeType.PLAYER)
		{
			spriteAvatar.spriteName = "PlayerIcon" + _node.nodeData.modleId;
		}
		else if(nodeType == NodeType.BOSS)
		{
			boosStep = 0;
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

		tempTargetValue = targetValue;
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

		if (barNuqi.gameObject.activeSelf == false) return;

		if(focusKing == null) focusKing = focusNode as KingControllor;

		if(focusKing == null || focusKing.kingSkillMibao == null || focusKing.kingSkillMibao.Count == 0)
		{
			barNuqi.gameObject.SetActive(false);
		}

		float nuqiMax = (float)CanshuTemplate.GetValueByKey (CanshuTemplate.NUQI_MAX);
		
		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan && CityGlobalData.m_tempSection == 0 && CityGlobalData.m_tempLevel == 1)
		{
			nuqiMax = (float)CanshuTemplate.GetValueByKey (CanshuTemplate.NUQI_MAX_0);
		}
		
		float nuqi = focusNode.nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_NUQI);

		barNuqi.fillAmount = nuqi / nuqiMax;
	}

	void UpdateBarBoss()
	{
		if (focusNode.nodeData.nodeType != NodeType.BOSS) return;

		int indexMax = (int)focusNode.nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hpNum);

		float step = (1 / (indexMax * preTime)) / 30f;

		float unity = 1f / indexMax;

		int curIndex = (int)(curValue / unity);

		if(targetValue % unity == 0)
		{
			curIndex --;
		}

		float _curValue = (targetValue - (curIndex * unity)) / unity;

		if(boosStep == 0) boosStep = step;

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

		updataBarWhitBoss (boosStep, curIndex);

		for(int i = 0; i < bosBarList.Count; i++)
		{
			if( i == curIndex)
			{
				bosBarList[i].value = curIndexValue;
			}
			else if(i > curIndex)
			{
				bosBarList[i].value = 0;
			}
			else
			{
				bosBarList[i].value = 1;
			}
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

	private void updataBarWhitBoss(float step, int curIndex)
	{
		barBoss_white.foregroundWidget.depth = 306 + curIndex * 2;

		if(tempTargetValue != targetValue)
		{
			whiteRefreshTime = Time.realtimeSinceStartup;

			if(curIndex < bosBarList.Count) barBoss_white.value = bosBarList[curIndex].value;
		}

		if(Time.realtimeSinceStartup - whiteRefreshTime < preTime * whiteRate)
		{
			return;
		}

		if(barBoss_white.value > bosBarList[curIndex].value )
		{
			barBoss_white.value -= step;
		}
		else
		{
			barBoss_white.value = bosBarList[curIndex].value;
		}

	}
	
}
