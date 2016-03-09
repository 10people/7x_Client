using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DramaActorLabel : DramaActor
{
	public UIAnchor.Side anchor;
	
	public string text;
	
	public Vector3 localPosition;
	
	public Vector3 localRotation;
	
	public int fontSize;

	public FontStyle fontStyle;

	public bool applyGradient;

	public Color gradientTop;

	public Color gradientBottom;

	public UILabel.Effect labelEffect;

	public Color effectColor;

	public Vector2 effectDistance;

	public Color labelColor;

	public Vector2 dimensions;

	public int depth;
	
	
	[HideInInspector] public List<DramaActorUIData> datas = new List<DramaActorUIData>();
	
	
	private GameObject labelGc;


	void Start()
	{
		actorType = ACTOR_TYPE.UILabel;
	}
	
	protected override float func ()
	{
		base.func ();
		
		labelGc = Instantiate (DramaControllor.Instance().labelTemple);
		
		if(anchor == UIAnchor.Side.Bottom)
		{
			labelGc.transform.parent = DramaControllor.Instance().anchorBottom.transform;
		}
		else if(anchor == UIAnchor.Side.BottomLeft)
		{
			labelGc.transform.parent = DramaControllor.Instance().anchorBottomLeft.transform;
		}
		else if(anchor == UIAnchor.Side.BottomRight)
		{
			labelGc.transform.parent = DramaControllor.Instance().anchorBottomRight.transform;
		}
		else if(anchor == UIAnchor.Side.Center)
		{
			labelGc.transform.parent = DramaControllor.Instance().anchorCenter.transform;
		}
		else if(anchor == UIAnchor.Side.Left)
		{
			labelGc.transform.parent = DramaControllor.Instance().anchorLeft.transform;
		}
		else if(anchor == UIAnchor.Side.Right)
		{
			labelGc.transform.parent = DramaControllor.Instance().anchorRight.transform;
		}
		else if(anchor == UIAnchor.Side.Top)
		{
			labelGc.transform.parent = DramaControllor.Instance().anchorTop.transform;
		}
		else if(anchor == UIAnchor.Side.TopLeft)
		{
			labelGc.transform.parent = DramaControllor.Instance().anchorTopLeft.transform;
		}
		else if(anchor == UIAnchor.Side.TopRight)
		{
			labelGc.transform.parent = DramaControllor.Instance().anchorTopRight.transform;
		}
		
		labelGc.transform.localPosition = localPosition;
		
		labelGc.transform.localScale = new Vector3 (1, 1, 1);
		
		labelGc.transform.localEulerAngles = localRotation;
		
		labelGc.SetActive (true);
		
		UILabel label = labelGc.GetComponent<UILabel>();
		
		label.text = text;

		label.fontSize = fontSize;

		label.SetDimensions ((int)dimensions.x, (int)dimensions.y);
		
		label.depth = depth;

		label.fontStyle = fontStyle;

		label.applyGradient = applyGradient;

		label.gradientBottom = gradientBottom;

		label.gradientTop = gradientTop;

		label.effectStyle = labelEffect;

		label.effectColor = effectColor;

		label.effectDistance = effectDistance;

		label.color = effectColor;

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
		yield return new WaitForSeconds (data.waittingTime);
		
		if(data.actionType == DramaActorUIData.UIActionType.actionAlpha)
		{
			TweenAlpha.Begin(labelGc, 0, data.startValue);
			
			TweenAlpha.Begin(labelGc, data.actionTime, data.endValue);
		}
		else if(data.actionType == DramaActorUIData.UIActionType.actionMove)
		{
			if(Vector3.Distance(data.startVector3, Vector3.zero) > .1f)
			{
				gameObject.transform.localPosition = data.startVector3;
			}
			
			iTween.MoveTo(labelGc, iTween.Hash(
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
			
			iTween.ScaleTo(labelGc, iTween.Hash(
				"scale", data.endVector3,
				"time", data.actionTime,
				"easetype", data.easeType
				));
		}
	}
	
	protected override bool funcDone ()
	{
		Destroy (labelGc);
		
		labelGc = null;
		
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
