using UnityEngine;
using System.Collections;

public class GetDragData : MonoBehaviour {

	public UIScrollView mScrollView;
	void Start () {
	
		mScrollView.onDragFinished = OnDragFinished;

		if (mScrollView.horizontalScrollBar != null)
			mScrollView.horizontalScrollBar.onDragFinished = OnDragFinished;
		
		if (mScrollView.verticalScrollBar != null)
			mScrollView.verticalScrollBar.onDragFinished = OnDragFinished;
	}

	void OnDrag (Vector2 delta)
	{
		MembersManager.mInstance.IsDrag = true;
	}

	void OnDragFinished () { 
		if (enabled)
		{
			MembersManager.mInstance.IsDrag = false;
		}
		
	}
}
