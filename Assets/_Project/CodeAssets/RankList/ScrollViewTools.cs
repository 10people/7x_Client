using UnityEngine;  
using System.Collections;  
using System.Collections.Generic; 

public class ScrollViewTools : MonoBehaviour {  

	public string perfix = string.Empty;  

	private List<UIWidget> itemList = new List<UIWidget>(); 

	private Vector4 posParam;  

	private Transform cachedTransform; 

	public UIScrollView scrollView;
	
	public float cellWidth;
	public float cellHeight;
	
	void Awake()  
	{  
		cachedTransform = this.transform;   
		
		posParam = new Vector4 (cellWidth, cellHeight,   
		                        scrollView.movement == UIScrollView.Movement.Horizontal ? 1 : 0,  
		                        scrollView.movement == UIScrollView.Movement.Vertical ? 1 : 0);  
	} 

	public void GetChildList ()
	{
		Debug.Log ("ChildCount:" + cachedTransform.childCount);
		itemList.Clear ();
		for (int i=0; i < cachedTransform.childCount; i++)  
		{  
			Transform t = cachedTransform.GetChild(i);  
			UIWidget uiw = t.GetComponent<UIWidget>();  
			uiw.name = string.Format("{0}_{1:D3}", perfix, itemList.Count);  
			itemList.Add(uiw);  
		}
	}

	void LateUpdate()  
	{  
		if (itemList.Count > 1)  
		{  
			int sourceIndex = -1;  
			int targetIndex = -1;  
			int sign = 0;  
			bool firstVislable = itemList[0].isVisible;  
			bool lastVisiable = itemList[itemList.Count-1].isVisible;  
			// if first and last both visiable or invisiable then return       
			if (firstVislable == lastVisiable)  
			{  
				return;  
			}  
			if (firstVislable)  
			{  
				// move last to first one   
				sourceIndex = itemList.Count-1;  
				targetIndex = 0;  
				sign = -1;  
			}  
			if (lastVisiable)  
			{  
				// move first to last one   
				sourceIndex = 0;  
				targetIndex = itemList.Count-1;  
				sign = 1;  
			}  
			if (sourceIndex > -1)  
			{  
				UIWidget movedWidget = itemList[sourceIndex];  
				Vector3 offset = new Vector3(sign*posParam.x * posParam.z, sign*posParam.y * posParam.w, 0);  
				movedWidget.cachedTransform.localPosition = itemList[targetIndex].cachedTransform.localPosition + offset;  
				// change item str   
				itemList.RemoveAt(sourceIndex);  
				itemList.Insert(targetIndex, movedWidget);  
			}  
		}  
	}  
}  