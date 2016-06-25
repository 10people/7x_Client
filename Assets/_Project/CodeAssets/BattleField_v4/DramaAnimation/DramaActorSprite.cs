using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DramaActorSprite : DramaActor
{
	public UIAnchor.Side anchor;

	public string spriteName;

	public Vector3 localPosition;

	public Vector3 localRotation;

	public Vector2 dimensions;

	public int depth;


	[HideInInspector] public List<DramaActorUIData> datas = new List<DramaActorUIData>();


	private GameObject spriteGc;


	void Start()
	{
		actorType = ACTOR_TYPE.UISprite;
	}

	protected override void funcStart()
	{
		foreach(DramaActorUIData data in datas)
		{
			if(data.actionType == DramaActorUIData.UIActionType.actionAnimator)
			{
				Global.ResourcesDotLoad(data.animatorPath, LoadCallback );
			}
		}
	}

	protected override float func ()
	{
		base.func ();

		spriteGc = Instantiate (DramaControllor.Instance().spriteTemple);

		if(anchor == UIAnchor.Side.Bottom)
		{
			spriteGc.transform.parent = DramaControllor.Instance().anchorBottom.transform;
		}
		else if(anchor == UIAnchor.Side.BottomLeft)
		{
			spriteGc.transform.parent = DramaControllor.Instance().anchorBottomLeft.transform;
		}
		else if(anchor == UIAnchor.Side.BottomRight)
		{
			spriteGc.transform.parent = DramaControllor.Instance().anchorBottomRight.transform;
		}
		else if(anchor == UIAnchor.Side.Center)
		{
			spriteGc.transform.parent = DramaControllor.Instance().anchorCenter.transform;
		}
		else if(anchor == UIAnchor.Side.Left)
		{
			spriteGc.transform.parent = DramaControllor.Instance().anchorLeft.transform;
		}
		else if(anchor == UIAnchor.Side.Right)
		{
			spriteGc.transform.parent = DramaControllor.Instance().anchorRight.transform;
		}
		else if(anchor == UIAnchor.Side.Top)
		{
			spriteGc.transform.parent = DramaControllor.Instance().anchorTop.transform;
		}
		else if(anchor == UIAnchor.Side.TopLeft)
		{
			spriteGc.transform.parent = DramaControllor.Instance().anchorTopLeft.transform;
		}
		else if(anchor == UIAnchor.Side.TopRight)
		{
			spriteGc.transform.parent = DramaControllor.Instance().anchorTopRight.transform;
		}

		spriteGc.transform.localPosition = localPosition;

		spriteGc.transform.localScale = new Vector3 (1, 1, 1);

		spriteGc.transform.localEulerAngles = localRotation;

		spriteGc.SetActive (true);

		UISprite sprite = spriteGc.GetComponent<UISprite>();

		sprite.spriteName = spriteName;

		sprite.SetDimensions ((int)dimensions.x, (int)dimensions.y);

		sprite.depth = depth;

		float time = 0;

		foreach(DramaActorUIData data in datas)
		{
			StartCoroutine(playAnim(data));

			if(data.actionTime + data.waittingTime > time)
			{
				time = data.actionTime + data.waittingTime;
			}
		}

		return time;
	}

	IEnumerator playAnim(DramaActorUIData data)
	{
		if(data.waittingTime > .1f) yield return new WaitForSeconds (data.waittingTime);

		if(data.actionType == DramaActorUIData.UIActionType.actionAlpha)
		{
			TweenAlpha.Begin(spriteGc, 0, data.startValue);

			TweenAlpha.Begin(spriteGc, data.actionTime, data.endValue);
		}
		else if(data.actionType == DramaActorUIData.UIActionType.actionMove)
		{
			if(Vector3.Distance(data.startVector3, Vector3.zero) > .1f)
			{
				gameObject.transform.localPosition = data.startVector3;
			}

			iTween.MoveTo(spriteGc, iTween.Hash(
				"position", data.endVector3,
				"islocal", true,
				"time", data.actionTime,
				"easetype", data.easeType
				));
		}
		else if(data.actionType == DramaActorUIData.UIActionType.actionSale)
		{
			if(Vector3.Distance(data.startVector3, Vector3.zero) > .1f) 
			{
				gameObject.transform.localScale = data.startVector3;
			}

			iTween.ScaleTo(spriteGc, iTween.Hash(
				"scale", data.endVector3,
				"time", data.actionTime,
				"easetype", data.easeType
				));
		}
		else if(data.actionType == DramaActorUIData.UIActionType.actionAnimator)
		{
			if(data.runTimeAnimatorController != null)
			{
				Animator ani = spriteGc.AddComponent<Animator>();

				ani.runtimeAnimatorController = data.runTimeAnimatorController;
			}
		}
	}

	private void LoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		foreach(DramaActorUIData data in datas)
		{
			if(data.actionType == DramaActorUIData.UIActionType.actionAnimator && data.animatorPath.Equals(p_path))
			{
				data.runTimeAnimatorController = p_object as RuntimeAnimatorController;
			}
		}
	}

	protected override void funcForcedEnd ()
	{
		funcDone ();
	}

	protected override bool funcDone ()
	{
		Destroy (spriteGc);

		spriteGc = null;

		return true;
	}

	public void refreshDatasWhenCheckout()
	{
		datas.Clear ();

		DramaActorUIData[] dataArray = GetComponents<DramaActorUIData>();

		foreach(DramaActorUIData tempData in dataArray)
		{
			datas.Add(tempData);
		}
	}

}
