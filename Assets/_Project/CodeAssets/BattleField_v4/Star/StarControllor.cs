using UnityEngine;
using System.Collections;

public class StarControllor : MonoBehaviour
{
	public enum CompleteFalg
	{
		Unsetted,          //未设定
		Done,              //手动完成
		Undone             //手动未完成
	}


	private int condition;

	private CompleteFalg completeFalg;//是否手动判断完成？

	private float baseHp;

	private float finalHp;


	public bool achieveCondition()
	{
		if (completeFalg == CompleteFalg.Done) return true;

		if (completeFalg == CompleteFalg.Undone) return false;

		if(condition == 1)//剩余血量大于等于总血量的20%
		{
			return finalHp >= baseHp * .2f;
		}
		else if(condition == 2)//剩余血量大于等于总血量的50%
		{
			return finalHp >= baseHp * .5f;
		}
		else if(condition == 3)//剩余血量大于等于总血量的80%
		{
			return finalHp >= baseHp * .8f;
		}

		Debug.Log ("THERE IS NO STARTEMP " + condition);

		return false;
	}

	public void BattleStart(int _condition)
	{
		condition = _condition;

		completeFalg = CompleteFalg.Unsetted;

		if(condition == 1 //剩余血量大于等于总血量的20%
		   || condition == 2 //剩余血量大于等于总血量的50%
		   || condition == 3 //剩余血量大于等于总血量的80%
		   ) 
		{
			baseHp = BattleControlor.Instance().getKing().nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax );
		}
		else if(condition == 4)//带着秘宝上阵
		{
			setAchieveComplete(CompleteFalg.Done);
		}
	}

	public void BattleEnd()
	{
		if(condition == 1 //剩余血量大于等于总血量的20%
		   || condition == 2 //剩余血量大于等于总血量的50%
		   || condition == 3 //剩余血量大于等于总血量的80%
		   ) 
		{
			finalHp = BattleControlor.Instance().getKing().nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp );
		}
	}

	public void setAchieveComplete(CompleteFalg _completeFalg)
	{
		completeFalg = _completeFalg;
	}

}
