//#define DEBUG_TRANSFORM_HELPER



using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using qxmobile.protobuf;



public class TransformHelper : MonoBehaviour {

	#region Rotation

	public static void SetLocalRotation( GameObject p_gb, Vector3 p_local_rot ){
		if ( p_gb == null ) {
			Debug.Log( "p_gb = null." );

			return;
		}

		p_gb.transform.localRotation = Quaternion.Euler( p_local_rot );
	}

    public static Vector3 Get2DTrackRotation(Vector3 sourcePos, Vector3 targetPos)
    {
        double angleTemp = Math.Atan2(targetPos.x - sourcePos.x, targetPos.z - sourcePos.z)/Math.PI*180;

        return new Vector3(0, (float) angleTemp, 0);
    }

    public static Vector3 Get2DTrackPosition(Vector3 sourcePos, Vector3 rotation, float distance)
    {
        return new Vector3((float) Math.Sin(rotation.y/180*Math.PI)*distance, 0, (float) Math.Cos(rotation.y/180*Math.PI)*distance) + sourcePos;
    }

    #endregion



    #region Save&Load

    private static Quaternion m_quaternion;
	
	private static Vector3 m_position;
	
	private static Vector3 m_local_scale;
	
	
	
	public static void StoreTransform(GameObject p_gb)
	{
		m_position = p_gb.transform.position;
		
		m_local_scale = p_gb.transform.localScale;
		
		m_quaternion = p_gb.transform.rotation;
	}
	
	public static void RestoreTransform(GameObject p_gb)
	{
		p_gb.transform.position = m_position;
		
		p_gb.transform.localScale = m_local_scale;
		
		p_gb.transform.rotation = m_quaternion;
	}

	#endregion



	#region Transform

    public static readonly List<string> SpecificGridItemName = new List<string>() {"a_", "z_"};

    /// <summary>
    /// Set parent's child num to specific num, standardize automaticlly.
    /// </summary>
    /// <param name="parentTransform">parent</param>
    /// <param name="prefabObject">child prefab</param>
    /// <param name="num">specific num</param>
    public static void AddOrDelItem(Transform parentTransform, GameObject prefabObject, int num, float offsetY = 0)
    {
        if (num < 0)
        {
            Debug.LogError("Num should not be nagative, num:" + num);
            return;
        }

        if (parentTransform.childCount > num)
        {
            while (parentTransform.childCount != num)
            {
                var child = parentTransform.GetChild(parentTransform.childCount - 1);
                child.parent = null;
                Destroy(child.gameObject);
            }
        }
        else if (parentTransform.childCount < num)
        {
            while (parentTransform.childCount != num)
            {
                var child = Instantiate(prefabObject) as GameObject;

                if (child == null)
                {
                    Debug.LogError("Fail to instantiate prefab, abort.");
                    return;
                }

                ActiveWithStandardize(parentTransform, child.transform);
                child.transform.localPosition = new Vector3(0, -offsetY*(parentTransform.childCount - 1), 0);
            }
        }
    }

    /// <summary>
	/// Set parent's child num to specific num, using pool manager, standardize automaticlly.
	/// </summary>
	/// <param name="parentTransform">parent</param>
	/// <param name="num">specific num</param>
	/// <param name="poolList">pool list</param>
	/// <param name="poolPrefabKey">which pool prefab to use</param>
	public static void AddOrDelItemUsingPool(Transform parentTransform, int num, PoolManagerListController poolList, string poolPrefabKey)
	{
		if (num < 0)
		{
			Debug.LogError("Num should not be nagative, num:" + num);
			return;
		}
		
		if (parentTransform.childCount > num)
		{
			while (parentTransform.childCount != num)
			{
				var child = parentTransform.GetChild(0);
				child.parent = null;
				poolList.ReturnItem(poolPrefabKey, child.gameObject);
			}
		}
		else if (parentTransform.childCount < num)
		{
			while (parentTransform.childCount != num)
			{
				var child = poolList.TakeItem(poolPrefabKey);
				
				if (child == null)
				{
					Debug.LogError("Fail to instantiate prefab, abort.");
					return;
				}
				
				ActiveWithStandardize(parentTransform, child.transform);
			}
		}
	}
	
	/// <summary>
	/// Set default transform and active.
	/// </summary>
	/// <param name="p_parent">parent transform</param>
	/// <param name="p_targetChild">transform standardized</param>
	public static void ActiveWithStandardize(Transform p_parent, Transform p_targetChild,float p_localScale=1f)
	{
		p_targetChild.transform.parent = p_parent;
		p_targetChild.transform.localPosition = Vector3.zero;
		p_targetChild.transform.localEulerAngles = Vector3.zero;
	    p_targetChild.transform.localScale = Vector3.one*p_localScale;
		p_targetChild.gameObject.SetActive(true);
	}
	
	#endregion



	#region Relations

	public static bool IsAncestor( GameObject p_parent_gb, GameObject p_child_gb ){
		if( p_child_gb.transform.parent == null ){
			return false;
		}

		GameObject t_temp = p_child_gb.transform.parent.gameObject;

		while( t_temp != null ){
			if( t_temp == p_parent_gb ){
				return true;
			}

			if( t_temp.transform.parent != null ){
				t_temp = t_temp.transform.parent.gameObject;	
			}
			else{
				t_temp = null;	
			}
		}

		return false;
	}

	public static bool IsParentOrChild( GameObject p_gb_1, GameObject p_gb_2 ){
		if( p_gb_1 == p_gb_2 ){
			return true;
		}

		if( IsAncestor( p_gb_1, p_gb_2 ) ){
			return true;
		}

		if( IsAncestor( p_gb_2, p_gb_1 ) ){
			return true;
		}

		return false;
	}

	#endregion



	#region Logs

	public static void LogPosition( GameObject p_gb, string p_prefex = "" ){
		if( p_gb == null ){
			Debug.Log( "gameobject is null." );

			return;
		}

		Debug.Log( p_prefex + " local.pos: " + p_gb.transform.localPosition );

		Debug.Log( p_prefex + " global.pos: " + p_gb.transform.localPosition );
	}

	
	
	public static void LogTransform( GameObject p_gb, string p_prefex = "" ){
		if( p_gb == null ){
			Debug.Log( "Object is null." );
			
			return;
		}
		
		Transform t_tran = p_gb.transform;

		Debug.Log( p_prefex + ": " + GameObjectHelper.GetGameObjectHierarchy( p_gb ) );
		
		// global
		{
			Debug.Log( "Scale: " + t_tran.lossyScale );
			
			Debug.Log( "Pos: " + t_tran.position );
			
			Debug.Log( "Rot: " + t_tran.rotation );
		}
		
		// local
		{
			Debug.Log( "Local.Scale: " + t_tran.localScale );
			
			Debug.Log( "Local.Pos: " + t_tran.localPosition );
			
			Debug.Log( "Local.Rot: " + t_tran.localRotation );
		}
	}

	#endregion



	#region Utilities

	/// Set localPosition and localRotation to Zero.
	public static void ResetLocalPosAndLocalRot( GameObject p_gb ){
		if (p_gb == null){
			return;
		}
		
		p_gb.transform.localPosition = Vector3.zero;
		
		p_gb.transform.localRotation = Quaternion.identity;
	}
	
	/// Set localPosition and localRotation and localScale to Zero.
	public static void ResetLocalPosAndLocalRotAndLocalScale( GameObject p_gb ){
		if (p_gb == null)
		{
			return;
		}
		
		ResetLocalPosAndLocalRot(p_gb);
		
		p_gb.transform.localScale = Vector3.one;
	}
	
	public static Vector3 GetLocalPositionInUIRoot( GameObject p_ngui_gb ){
		if (p_ngui_gb == null){
			Debug.LogError("Error, ngui gb = null.");
			
			return Vector3.zero;
		}
		
		Vector3 t_local_pos = p_ngui_gb.transform.localPosition;
		
		//		Debug.Log( p_ngui_gb.name + ": " + p_ngui_gb.transform.localPosition + " - " + p_ngui_gb.transform.position );
		
		Transform t_parent = p_ngui_gb.transform.parent;
		
		while ( t_parent != null ){
			if (t_parent.gameObject.GetComponent<UIRoot>() != null){
				break;
			}

			if( t_parent.gameObject.GetComponent<UICamera>() != null ){
				break;
			}
			
			t_local_pos = t_local_pos + t_parent.localPosition;
			
			//			Debug.Log( t_parent.name + ": " + t_parent.localPosition );
			
			t_parent = t_parent.parent;
			
			if (t_parent == null){
				Debug.LogError("Error, UIRoot Not Founded.");
				
				return Vector3.zero;
			}
		}
		
		return t_local_pos;
	}
	
	public static Vector3 GetLocalScaleInUIRoot(GameObject p_ngui_gb)
	{
		if (p_ngui_gb == null)
		{
			Debug.LogError("Error, ngui gb = null.");
			
			return Vector3.one;
		}
		
		Vector3 t_local_scale = p_ngui_gb.transform.localScale;
		
		//		Debug.Log( p_ngui_gb.name + ": " + p_ngui_gb.transform.localPosition + " - " + p_ngui_gb.transform.position );
		
		Transform t_parent = p_ngui_gb.transform.parent;
		
		while (t_parent != null)
		{
			if (t_parent.gameObject.GetComponent<UIRoot>() != null)
			{
				break;
			}
			
			t_local_scale.x = t_local_scale.x * t_parent.localScale.x;
			t_local_scale.y = t_local_scale.y * t_parent.localScale.y;
			t_local_scale.z = t_local_scale.z * t_parent.localScale.z;
			
			//			Debug.Log( t_parent.name + ": " + t_parent.localPosition );
			
			t_parent = t_parent.parent;
			
			if (t_parent == null)
			{
				Debug.LogError("Error, UIRoot Not Founded.");
				
				return Vector3.one;
			}
		}
		
		return t_local_scale;
	}
	
	public static void CopyTransform( GameObject p_source, GameObject p_destination ){
		if( p_source == null ){
			Debug.LogError("CopyTransform.Source = null");
			
			return;
		}
		
		if( p_destination == null ){
			Debug.LogError("CopyTransform.Des = null");
			
			return;
		}
		
		p_destination.transform.localPosition = p_source.transform.localPosition;
		
		p_destination.transform.localScale = p_source.transform.localScale;
		
		p_destination.transform.localRotation = p_source.transform.localRotation;
		
	}
	
	/// <summary>
	/// Ergodic parent's all children
	/// </summary>
	/// <param name="parent">parent</param>
	/// <returns>all children</returns>
	public static List<Transform> ErgodicChilds(Transform parent)
	{
		List<Transform> returnTransforms = new List<Transform>();
		for (int i = 0; i < parent.childCount; i++)
		{
			returnTransforms.Add(parent.GetChild(i));
		}
		
		foreach (var item in returnTransforms)
		{
			returnTransforms = returnTransforms.Concat(ErgodicChilds(item)).ToList();
		}
		
		return returnTransforms;
	}
	
	/// <summary>
	/// Ergodic child's all parents
	/// </summary>
	/// <param name="child">child</param>
	/// <returns>all parents</returns>
	public static List<Transform> ErgodicParents(Transform child)
	{
		if (child == null)
		{
			return null;
		}
		
		List<Transform> returnTransforms = new List<Transform>();
		Transform targetTransform = child.parent;
		while (targetTransform != null)
		{
			returnTransforms.Add(targetTransform);
			targetTransform = targetTransform.parent;
		}
		
		return returnTransforms;
	}
	
	/// <summary>
	/// Find the first child transform with special name. 
	/// </summary>
	/// <param name="parent">The parent tranfrom of the child which will be found.</param>
	/// <param name="objName">The name of the child transfrom.</param>
	/// <returns>The transfrom to be found, null if not found.</returns>
	public static Transform FindChild(Transform parent, string objName)
	{
		if (parent.name == objName)
		{
			return parent;
		}
		return (from Transform item in parent select FindChild(item, objName)).FirstOrDefault(child => child != null);
	}
	
	/// <summary>
	/// Find the first parent transform with special name. 
	/// </summary>
	/// <param name="child">The child tranfrom of the parent which will be found.</param>
	/// <param name="objName">The name of the child transfrom.</param>
	/// <returns>The transfrom to be found, null if not found.</returns>
	public static Transform FindParent(Transform child, string objName)
	{
		if (child == null)
		{
			return null;
		}
		return child.name == objName ? child : FindParent(child.parent.transform, objName);
	}
	
	/// <summary>
	/// Get the first parent specific component, for unity elder version in used, don't use GameObject.GetComponentInParent().
	/// </summary>
	/// <typeparam name="T">generic variable which inherited from monobehaviour</typeparam>
	/// <param name="child">The child tranfrom.</param>
	/// <returns>The component to be found, null if not found.</returns>
	public static T GetComponentInParent<T>(Transform child) where T : MonoBehaviour
	{
		if (child == null)
		{
			return null;
		}
		return child.GetComponent<T>() ?? GetComponentInParent<T>(child.parent.transform);
	}

    public static bool RayCastXToFirstCollider(Vector3 originalPos, out float firstPos, int direction = 2, bool isMax = true)
    {
        RaycastHit[] tempHits = new RaycastHit[] {};
        switch (direction)
        {
            case 1:
                tempHits = Physics.RaycastAll(new Ray(new Vector3((isMax ? 1 : -1)*Mathf.Infinity, originalPos.y, originalPos.z), new Vector3((isMax ? -1 : 1), 0, 0)), Mathf.Infinity);
                break;
            case 2:
                tempHits = Physics.RaycastAll(new Ray(new Vector3(originalPos.x, (isMax ? 1 : -1)*Mathf.Infinity, originalPos.z), new Vector3(0, (isMax ? -1 : 1), 0)), Mathf.Infinity);
                break;
            case 3:
                tempHits = Physics.RaycastAll(new Ray(new Vector3(originalPos.x, originalPos.y, (isMax ? 1 : -1)*Mathf.Infinity), new Vector3(0, 0, (isMax ? -1 : 1))), Mathf.Infinity);
                break;
            default:
                Debug.LogError("Input direction is wrong");
                firstPos = Mathf.Infinity;
                return false;
        }

        if (tempHits.Any())
        {
            switch (direction)
            {
                case 1:
                    firstPos = isMax ? tempHits.Max(item => item.point.x) : tempHits.Min(item => item.point.x);
                    break;
                case 2:
                    firstPos = isMax ? tempHits.Max(item => item.point.y) : tempHits.Min(item => item.point.y);
                    break;
                case 3:
                    firstPos = isMax ? tempHits.Max(item => item.point.z) : tempHits.Min(item => item.point.z);
                    break;
                default:
                    Debug.LogError("Input direction is wrong");
                    firstPos = Mathf.Infinity;
                    return false;
            }

            return true;
        }
        else
        {
            firstPos = Mathf.Infinity;
            return false;
        }
    }

    #endregion

}
