using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

public class MathHelper {

	#region float Convert
	
	/** Params:
     * p_float:		origin float
     * p_precision:	precision count
 	 *
     * Example:
     * FloatPrecision( 0.123456f, 2 ) -> 0.12
     */
	public static float FloatPrecision( float p_float, int p_precision ){
		if (p_precision < 0)
		{
			return p_float;
		}
		
		int t_count = 1;
		
		for (int i = 0; i < p_precision; i++)
		{
			t_count *= 10;
		}
		
		int t_time_int = (int)(p_float * t_count);
		
		float t_time = t_time_int * 1.0f / t_count;
		
		return t_time;
	}

	#endregion



	#region Bezier
	
	public class SegmentInFoldLine
	{
		public Vector2 StartPoint;
		public Vector2 EndPoint;
		public float Distance;
		public float PerviousPercentInTotal;
		public float Percent;
	}
	
	public static Vector2 GetPointFromFoldLine(float precent, List<Vector2> positionList)
	{
		List<SegmentInFoldLine> temp = GetSegmentListFromFoldLine(positionList);
		if (temp == null)
		{
			return Vector2.zero;
		}
		
		return GetPointFromSegmentLine(precent, temp);
	}
	
	public static List<SegmentInFoldLine> GetSegmentListFromFoldLine(List<Vector2> positionList)
	{
		if (positionList == null || positionList.Count < 2)
		{
			Debug.LogError("Cannot get point cause position number less than 2.");
			return null;
		}
		
		List<SegmentInFoldLine> temp = new List<SegmentInFoldLine>();
		for (int i = 0; i < positionList.Count - 1; i++)
		{
			temp.Add(new SegmentInFoldLine()
			         {
				StartPoint = positionList[i],
				EndPoint = positionList[i + 1],
			});
		}
		
		temp.ForEach(item => item.Distance = Vector2.Distance(item.StartPoint, item.EndPoint));
		float TotalDistance = temp.Select(item => item.Distance).Sum();
		temp.ForEach(item => item.Percent = item.Distance / TotalDistance);
		
		for (int i = 0; i < temp.Count; i++)
		{
			temp[i].PerviousPercentInTotal = i > 0 ? (temp[i - 1].Percent + temp[i - 1].PerviousPercentInTotal) : 0;
		}
		
		return temp;
	}
	
	public static Vector2 GetPointFromSegmentLine(float precent, List<SegmentInFoldLine> segmentList)
	{
		SegmentInFoldLine tempLine = segmentList.Where(item => item.PerviousPercentInTotal <= precent).OrderBy(item2 => item2.PerviousPercentInTotal).Last();
		
		//Debug.LogWarning(precent + "," + (precent - tempLine.PerviousPercentInTotal)/tempLine.Percent + "," + tempLine.StartPoint);
		return Vector2.Lerp(tempLine.StartPoint, tempLine.EndPoint, (precent - tempLine.PerviousPercentInTotal) / tempLine.Percent);
	}
	
	public static Vector2 GetDirectionFromFoldLine(float precent, List<Vector2> positionList)
	{
		List<SegmentInFoldLine> temp = GetSegmentListFromFoldLine(positionList);
		if (temp == null)
		{
			return Vector2.zero;
		}
		
		return GetDirectionFromSegmentLine(precent, temp);
	}
	
	public static Vector2 GetDirectionFromSegmentLine(float precent, List<SegmentInFoldLine> segmentList)
	{
		SegmentInFoldLine tempLine = segmentList.Where(item => item.PerviousPercentInTotal <= precent).OrderBy(item2 => item2.PerviousPercentInTotal).Last();
		return tempLine.EndPoint - tempLine.StartPoint;
	}
	
	#endregion
}
