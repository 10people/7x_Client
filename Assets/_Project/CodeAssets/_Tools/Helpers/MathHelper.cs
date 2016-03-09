using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Random = UnityEngine.Random;

public class MathHelper{
	
	#region Random

	public static float GetRandom(float p_min_inc, float p_max_inc){
		return (p_max_inc - p_min_inc) * Random.value + p_min_inc;
	}

	public static int GetRandom( int p_min_inc, int p_max_inc ){
		return (int)( ( p_max_inc - p_min_inc ) * Random.value + p_min_inc );
	}

	#endregion


    #region Basic Math

    public static void SwapValue(ref float a, ref float b)
    {
        var temp = a;
        a = b;
        b = temp;
    }

    #endregion



    #region float Convert

    /** Params:
     * p_float:		origin float
     * p_precision:	precision count
 	 *
     * Example:
     * FloatPrecision( 0.123456f, 2 ) -> 0.12
     */
    public static float FloatPrecision( float p_float, int p_precision ){
        if ( p_precision < 0 ){
            return p_float;
        }

        int t_count = 1;

        for( int i = 0; i < p_precision; i++ ){
            t_count *= 10;
        }

        int t_time_int = (int)( p_float * t_count );

        float t_time = t_time_int * 1.0f / t_count;

        return t_time;
    }

	/** Params:
     * p_float:		origin float
     * p_precision:	precision count
 	 *
     * Example:
     * FloatPrecision( 0.123456f, 2 ) -> 0.12
     */
	public static string FloatToString( float p_float, int p_precision ){
		return p_float.ToString( "f" + p_precision );
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



    #region Equation Solver

    public static Vector2 GetPointAtCircle(Vector2 center, float radius, int totalPointNum, int pointIndex)
    {
        var degree = 2 * Mathf.PI / totalPointNum * pointIndex;

        return new Vector2(center.x + radius * Mathf.Cos(degree), center.y + radius * Mathf.Sin(degree));
    }

    #endregion



	#region Vector3

	public static Vector3 m_cached_vector3 = new Vector3( 0, 0, 0 );

	public static void Lerp( ref Vector3 from, ref Vector3 to, float t, ref Vector3 p_target ){
		t = Mathf.Clamp01 (t);

		p_target.x = from.x + (to.x - from.x) * t;

		p_target.y = from.y + (to.y - from.y) * t;

		p_target.z = from.z + (to.z - from.z) * t;
	}

	#endregion



	#region Vector4

	public static void LogVec4( Vector4 p_vec4, string p_prefix = "" ){
		if( !string.IsNullOrEmpty( p_prefix ) ){
			Debug.Log( "------ " + p_prefix + " ------" );
		}

		Debug.Log( "x: " + p_vec4.x );

		Debug.Log( "y: " + p_vec4.y );

		Debug.Log( "z: " + p_vec4.z );

		Debug.Log( "w: " + p_vec4.w );
	}

	#endregion



	#region Color

	/// Only parse RGB now.
	public static void ParseHexString( string p_string, out Color p_color, Color p_default_color ){
		if( p_string.Length < 6 ){
//			Debug.Log( "p_string len error: " + p_string );

			p_color = p_default_color;

			return;
		}
		
		p_string = p_string.Replace ("0x", "");

		p_string = p_string.Replace ("#", "");
        
		string t_r = p_string.Substring( 0, 2 );

		string t_g = p_string.Substring( 2, 2 );

		string t_b = p_string.Substring( 4, 2 );

		string t_a = "";

		if( p_string.Length == 8 ){
			t_a = p_string.Substring( 6, 2 );
		}

		float t_r_f = 0.0f;

		float t_g_f = 0.0f;

		float t_b_f = 0.0f;

		float t_a_f = 1.0f;

		try{
			t_r_f = int.Parse( t_r, System.Globalization.NumberStyles.HexNumber ) / 255.0f;
			
			t_g_f = int.Parse( t_g, System.Globalization.NumberStyles.HexNumber ) / 255.0f;
			
			t_b_f = int.Parse( t_b, System.Globalization.NumberStyles.HexNumber ) / 255.0f;

			if( p_string.Length == 8 ){
				t_a_f = int.Parse( t_a, System.Globalization.NumberStyles.HexNumber ) / 255.0f;
			}
		}
		catch( Exception e ){
			Debug.Log( "Parse error: " + e );

			p_color = p_default_color;

			return;
		}

		p_color = new Color( t_r_f, t_g_f, t_b_f, t_a_f );
	}

	#endregion
}
