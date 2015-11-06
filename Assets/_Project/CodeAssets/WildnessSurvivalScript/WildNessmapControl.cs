using UnityEngine;
using System.Collections;

public class WildNessmapControl : MonoBehaviour {

	protected UIRoot mRoot;

	Transform mTrans;
	void Start () {
	
		mTrans = this.transform;
		Debug.Log(mTrans.name);
    	Transform mParent = mTrans.parent;
		mRoot = NGUITools.FindInParents<UIRoot>(mParent);
	}
	
	// Update is called once per frame
	void Update () {
	


	}
	void OnDrag(Vector2 delta)
	{
		OnDragDropMove((Vector3)delta * mRoot.pixelSizeAdjustment);
	}
	void OnPress()
	{
//		int m_touchCount = UICamera.touchCount;
//		if(m_touchCount > 2 ){
//
//			m_touchCount = 2;
//		}
//		for(int i = 0; i < m_touchCount; i++)
//		{
//			Vector3 p = Input.GetTouch(i).position;
//		}
//		UICamera.MouseOrTouch m_MouseOrTouch = UICamera.GetTouch(UICamera.currentTouchID);
//		Debug.Log ("m_touchCount = " + m_touchCount);
//			Debug.Log("m_MouseOrTouch = " +m_MouseOrTouch);
	}
	protected virtual void OnDragDropMove (Vector3 delta)
	{
//		if(mTrans.localPosition.x < 270&&mTrans.localPosition.x > -270&&mTrans.localPosition.y < 400&&mTrans.localPosition.y > -400)
//		{
//			mTrans.localPosition += delta;
//		}

		if(mTrans.localPosition.x > 270)
		{
			if(delta.x > 0)
			{
				return;
			}
		}
		if(mTrans.localPosition.x < -270)
		{
			if(delta.x < 0)
			{
				return;
			}
		}
		if(mTrans.localPosition.y > 400)
		{
			if(delta.y > 0)
			{
				return;
			}
		}
		if(mTrans.localPosition.y < -400)
		{
			if(delta.y < 0)
			{
				return;
			}
		}
		mTrans.localPosition += delta;
//		else
//		{
//			if(mTrans.localPosition.x >= 270)
//			{
//				Vector3 pos = new Vector3(260,mTrans.localPosition.y,0);
//				iTween.MoveTo(this.gameObject, iTween.Hash("position", pos, "time",1.0f,"islocal",true));
//				mTrans.localPosition += delta;
//				//mTrans.localPosition = new Vector3(260,mTrans.localPosition.y,0);
//			}
//			else if(mTrans.localPosition.x <= -270)
//			{
//				Vector3 pos = new Vector3(-260,mTrans.localPosition.y,0);
//				iTween.MoveTo(this.gameObject, iTween.Hash("position", pos, "time",1.0f,"islocal",true));
//				mTrans.localPosition += delta;
//				//mTrans.localPosition = new Vector3(-260,mTrans.localPosition.y,0);
//			}
//			else if(mTrans.localPosition.y <= -400)
//			{
//				Vector3 pos = new Vector3(mTrans.localPosition.x,-390,0);
//				iTween.MoveTo(this.gameObject, iTween.Hash("position", pos, "time",1.0f,"islocal",true));
//				mTrans.localPosition += delta;
//			//	mTrans.localPosition = new Vector3(mTrans.localPosition.x,-390,0);
//			}
//			else if(mTrans.localPosition.y >= 400)
//			{
//				Vector3 pos = new Vector3(mTrans.localPosition.x,390,0);
//				iTween.MoveTo(this.gameObject, iTween.Hash("position",pos, "time",1.0f,"islocal",true));
//				mTrans.localPosition += delta;
//				//mTrans.localPosition = new Vector3(mTrans.localPosition.x,390,0);
//			}
//		}
			


	}
}
