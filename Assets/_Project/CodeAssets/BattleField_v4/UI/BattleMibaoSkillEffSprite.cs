using UnityEngine;
using System.Collections;

public class BattleMibaoSkillEffSprite : MonoBehaviour 
{
	private UISprite sprite;


	void Start()
	{
		sprite = gameObject.GetComponent<UISprite>();
	}

	private void iSetAlpha(float _alpha)
	{
		sprite.alpha = _alpha;
	}

	private void uSetAlpha(float _alpha)
	{
		sprite.alpha = _alpha;
	}

	private void setScale(float _scale)
	{
		sprite.transform.localScale = new Vector3(_scale, _scale, 1f);
	}

}
