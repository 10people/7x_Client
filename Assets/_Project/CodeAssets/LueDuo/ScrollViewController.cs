using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Loop循环ScrollView
/// Add Component To Grid
/// UIGrid下放置部分Item可实现循环Item
/// </summary>
[RequireComponent(typeof(UIGrid))]
public class ScrollViewController : MonoBehaviour {

	#region Members
	/// <summary>
	/// 存储物件列表
	/// </summary>
	private List<UIWidget> itemWidgetList = new List<UIWidget>();
	
	private Vector4 posParam;
	private Transform cachedTrans;

	/// <summary>
	/// 起始下标
	/// </summary>
	private int startIndex;
	/// <summary>
	/// 最大长度
	/// </summary>
	private int maxCount;
	/// <summary>
	/// 物件刷新委托
	/// </summary>
	public delegate void OnItemInfoInIt (GameObject go);
	private OnItemInfoInIt onItemInfoInItCallBack;
	/// <summary>
	/// 点击事件委托
	/// </summary>
	public delegate void OnItemClick (GameObject go,int i);
	private OnItemClick onItemClickCallBack;
	/// <summary>
	/// 父scrollview
	/// </summary>
	private UIScrollView scrollView;

	#endregion

	void Awake ()
	{
		cachedTrans = this.transform;

		scrollView = cachedTrans.transform.parent.GetComponent<UIScrollView> ();

		scrollView.GetComponent<UIPanel> ().cullWhileDragging = true;
		
		UIGrid grid = this.GetComponent<UIGrid> ();
		float cellWidth = grid.cellWidth;
		float cellHight = grid.cellHeight;
		posParam = new Vector4 (cellWidth,cellHight,
		                        grid.arrangement == UIGrid.Arrangement.Horizontal?1:0,
		                        grid.arrangement == UIGrid.Arrangement.Vertical?1:0);
	}

	/// <summary>
	/// 初始化item
	/// </summary>
	public void InIt (bool isClick)
	{
		itemWidgetList.Clear ();
		
		for (int i = 0;i < cachedTrans.childCount; ++i)
		{
			Transform t = cachedTrans.GetChild (i);
			UIWidget widget = t.GetComponent<UIWidget> ();
			if (widget == null)
			{
				widget = t.gameObject.AddComponent<UIWidget> ();
			}
			widget.name = itemWidgetList.Count.ToString ();
			
			itemWidgetList.Add (widget);

			//可点击
			if (isClick)
			{
				BoxCollider boxCol = widget.GetComponent<BoxCollider> ();

				if (boxCol == null)
				{
					boxCol = widget.gameObject.AddComponent<BoxCollider> ();
					widget.autoResizeBoxCollider = true;
				}

				//事件接收
				UIEventListener listener = widget.GetComponent<UIEventListener> ();

				if (listener == null)
				{
					listener = widget.gameObject.AddComponent<UIEventListener> ();
				}

				listener.onClick = OnClickListItem;
			}
		}
	}
	
	/// <summary>
	/// 更新表单
	/// </summary>
	/// <param name="count">数量</param>
	public void UpdateListItem (int count)
	{
		startIndex = 0;
		maxCount = count;
		
		for (int i = 0;i < itemWidgetList.Count;i ++)
		{
			UIWidget widget = itemWidgetList[i];
			widget.name = i.ToString ();
			widget.Invalidate (true);
			
			NGUITools.SetActive (widget.gameObject,i < count);
		}
	}
	
	public List<UIWidget> GetItemList ()
	{
		return itemWidgetList;
	}
	
	public List<T> GetItemListInChildren<T> () where T : Component
	{
		List<T> list = new List<T> ();
		
		foreach (UIWidget widget in itemWidgetList)
		{
			list.Add (widget.GetComponentInParent<T> ());
		}
		
		return list;
	}
	
	void LateUpdate()  
	{  
		if (itemWidgetList.Count <= 1)  
		{  
			return;  
		}  
		
		int _sourceIndex = -1;  
		int _targetIndex = -1;  
		int _sign = 0;  
		
		bool firstVislable = itemWidgetList[0].isVisible;  
		bool lastVisiable = itemWidgetList[itemWidgetList.Count - 1].isVisible;  
		
		// 如果都显示,那么返回  
		if (firstVislable == lastVisiable)  
		{  
			return;  
		}  
		
		// 得到需要替换的源和目标  
		if (firstVislable)  
		{  
			_sourceIndex = itemWidgetList.Count - 1;  
			_targetIndex = 0;  
			_sign = 1;  
		}  
		else if (lastVisiable)  
		{  
			_sourceIndex = 0;  
			_targetIndex = itemWidgetList.Count - 1;  
			_sign = -1;  
		}  
		
		// 如果小于真正的初始索引或大于真正的结束索引,返回  
		int realSourceIndex = int.Parse(itemWidgetList[_sourceIndex].gameObject.name);  
		int realTargetIndex = int.Parse(itemWidgetList[_targetIndex].gameObject.name);  
		
		if (realTargetIndex <= startIndex || realTargetIndex >= (maxCount - 1))  
		{  
			scrollView.restrictWithinPanel = true;  
			return;  
		}  
		
		scrollView.restrictWithinPanel = false; 
		
		UIWidget movedWidget = itemWidgetList[_sourceIndex];  
		
		Vector3 _offset = new Vector2(_sign * posParam.x * posParam.z,_sign * posParam.y * posParam.w);  
		
		movedWidget.cachedTransform.localPosition = itemWidgetList[_targetIndex].cachedTransform.localPosition + _offset;  
		itemWidgetList.RemoveAt(_sourceIndex);  
		itemWidgetList.Insert(_targetIndex, movedWidget);  
		movedWidget.name = (realSourceIndex > realTargetIndex ? (realTargetIndex - 1) : (realTargetIndex + 1)).ToString();

		OnInItItemInfo (movedWidget.gameObject);
	}

	/// <summary>
	/// 设置代理
	/// </summary>
	/// <param name="onItemInIt">On item in it.</param>
	/// <param name="onItemClick">On item click.</param>
	public void SetDelegate (OnItemInfoInIt onItemInIt,OnItemClick onItemClick)
	{
		onItemInfoInItCallBack = onItemInIt;

		if (onItemClick != null)
		{
			onItemClickCallBack = onItemClick;
		}
	}

	void OnInItItemInfo (GameObject go)
	{
		if (onItemInfoInItCallBack != null)
		{
			onItemInfoInItCallBack (go);
		}
	}

	void OnClickListItem (GameObject go)
	{
		int number = int.Parse (go.name);

		if (onItemClickCallBack != null)
		{
			onItemClickCallBack (go,number);
		}
	}
}
