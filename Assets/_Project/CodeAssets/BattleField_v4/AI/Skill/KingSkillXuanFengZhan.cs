using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
*  无敌斩
*/
public class KingSkillXuanFengZhan : MonoBehaviour
{
	private KingControllor king;

	private int count;

	private int curCount;

	void Start()
	{
		SkillTemplate skill = SkillTemplate.getSkillTemplateById (200011);

		count = (int)skill.value2;

		king = gameObject.GetComponent<KingControllor> ();

		curCount = 0;
	}

	public void resetXuanFengCount()
	{
		curCount = 0;

		ResetXuanFengCountCallback( king.mAnim );
	}

	public static void ResetXuanFengCountCallback( Animator p_animator ){
		p_animator.SetBool("XuanFengOver", false);
	}

	public void addXuanFengCount()
	{
		curCount++;

		if(curCount >= count)
		{
			AddXuanFengCountCallback( king.mAnim );
		}
	}

	public static void AddXuanFengCountCallback( Animator p_animator ){
		p_animator.SetBool("XuanFengOver", true);
	}

}
