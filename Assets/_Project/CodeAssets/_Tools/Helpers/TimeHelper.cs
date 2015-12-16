﻿//#define DEBUG_TIMER_HELPER

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TimeHelper : Singleton<TimeHelper>{

	#region Current Time

	/// "yyyy-MM-dd hh:mm:ss"
	public static string GetCurrentTime_String(){
		return DateTime.Now.ToLocalTime().ToString( "MMdd-hhmmss" );
	}

	private static DateTime m_date_time = System.DateTime.Now;

	public static long GetCurrentTime_MilliSecond(){
		return (long) ( ( DateTime.Now - m_date_time ).TotalMilliseconds );
	}

	public static double GetCurrentTime_Second(){
		return GetCurrentTime_MilliSecond() / 1000.0;
	}

	#endregion



	#region Time String Format

	private static int m_temp_time_sec 		= 0;

	private static int m_temp_time_min 		= 0;

	private static int m_temp_time_hour		= 0;

	/// 1）如果没超过小时，则显示
	/// 分：秒
	/// 01：01
	/// 2）如果超过小时，则显示
	/// 时：分：秒
		/// 01：01：01
	/// 3）如果小时数超过两位数
	/// 时：分：秒
		/// 101：01:01
	/// 此为通用规则，游戏内时间的显示格式均遵循该规则
	public static string GetUniformedTimeString( int p_int_sec ){
		m_temp_time_sec = p_int_sec % 60;

		m_temp_time_min = p_int_sec / 60;

		m_temp_time_hour = m_temp_time_min / 60;

		m_temp_time_min = m_temp_time_min % 60;

		return m_temp_time_hour.ToString( "D2" ) + ":" + 
			m_temp_time_min.ToString( "D2" ) + ":" +
				m_temp_time_sec.ToString( "D2" );
	}

	/// 1）如果没超过小时，则显示
	/// 分：秒
	/// 01：01
	/// 2）如果超过小时，则显示
	/// 时：分：秒
	/// 01：01：01
	/// 3）如果小时数超过两位数
	/// 时：分：秒
	/// 101：01:01
	/// 此为通用规则，游戏内时间的显示格式均遵循该规则
	public static string GetUniformedTimeString( float p_float_sec ){
		return GetUniformedTimeString( (int)p_float_sec );
	}

	/// 1）如果没超过小时，则显示
	/// 分：秒
	/// 01：01
	/// 2）如果超过小时，则显示
	/// 时：分：秒
	/// 01：01：01
	/// 3）如果小时数超过两位数
	/// 时：分：秒
	/// 101：01:01
	/// 此为通用规则，游戏内时间的显示格式均遵循该规则
	public static string GetUniformedTimeString( int p_int_hour, int p_int_min, int p_int_sec ){
		return GetUniformedTimeString( p_int_hour * 3600 + p_int_min * 60 + p_int_sec );
	}

	#endregion



	#region ClockTime

    public struct ClockTime
    {
        public ClockTime(int l_hour, int l_minute, int l_second)
        {
            hour = l_hour;
            minute = l_minute;
            second = l_second;
        }

        public static ClockTime zero = new ClockTime(0, 0, 0);

        public int hour;
        public int minute;
        public int second;

        public static ClockTime Parse(string text)
        {
            try
            {
                var splited = text.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Select(item => int.Parse(item)).ToList();
                if (splited.Count == 0) throw new Exception("ClockTime length is 0, transfer fail.");
                if (splited.Count == 1) return new ClockTime(0, 0, splited[0]);
                if (splited.Count == 2) return new ClockTime(0, splited[1], splited[0]);
                if (splited.Count == 3) return new ClockTime(splited[2], splited[1], splited[0]);
                throw new Exception("ClockTime length larger than 3, transfer fail.");
            }
            catch (Exception e)
            {
                Debug.LogError("Transfer ClockTime fail, contact LiangXiao if you donot know how to solve this, exception:" + e.Source + "\nstackTrace:" + e.StackTrace);
                return zero;
            }
        }

        public override string ToString()
        {
            string hourStr = hour.ToString();
            while (hourStr.Length < 2)
            {
                hourStr = "0" + hourStr;
            }

            string minuteStr = minute.ToString();
            while (minuteStr.Length < 2)
            {
                minuteStr = "0" + minuteStr;
            }

            string secondStr = second.ToString();
            while (secondStr.Length < 2)
            {
                secondStr = "0" + secondStr;
            }

            if (hour == 0)
            {
                return minuteStr + ":" + secondStr;
            }
            else
            {
                return hourStr + ":" + minuteStr + ":" + secondStr;
            }
        }
    }

    public static ClockTime SecondToClockTime(int second)
    {
        if (second < 0)
        {
            return ClockTime.zero;
        }

        return new ClockTime(second / 3600, (second % 3600) / 60, second % 60);
    }

    public static int ClockTimeToSecond(ClockTime clockTime)
    {
        return clockTime.hour * 3600 + clockTime.minute * 60 + clockTime.second;
    }

	#endregion



    #region Time Calculate Module

    private class TimeCalc
    {
        public string key;

        public bool IsOverTime;
        public float StartTime;
        public float OverTimeDuration;
        public bool IsOneDelegate;
        public TimeCalcVoidDelegate m_TimeCalcVoidDelegate;
        public TimeCalcIntDelegate m_TimeCalcIntDelegate;

        public static bool ContainsKey(string key)
        {
            return m_timeCalcList.Any(item => item.key == key);
        }

        public static TimeCalc GetTimeCalc(string key)
        {
            if (!ContainsKey(key)) return null;

            return m_timeCalcList.Where(item => item.key == key).FirstOrDefault();
        }

        public static List<TimeCalc> m_timeCalcList = new List<TimeCalc>();
    }

    public delegate void TimeCalcVoidDelegate();
    public delegate void TimeCalcIntDelegate(int time);

    /// <summary>
    /// Add time calc dictionary item, delegate execute when time calc ends.
    /// </summary>
    /// <param name="key">item key</param>
    /// <param name="duration">time over duration</param>
    /// <param name="l_timeCalcVoidDelegate">delegate</param>
    /// <returns>is add succeed</returns>
    public bool AddOneDelegateToTimeCalc(string key, float duration, TimeCalcVoidDelegate l_timeCalcVoidDelegate = null)
    {
        if (TimeCalc.ContainsKey(key))
        {
            Debug.LogError("Cannot add key:" + key + " to time calc cause key already exist.");
            return false;
        }

        TimeCalc.m_timeCalcList.Add(new TimeCalc() { key = key, IsOverTime = true, StartTime = -1, OverTimeDuration = duration, m_TimeCalcVoidDelegate = l_timeCalcVoidDelegate, IsOneDelegate = true });

        StartCalc(key);
        return true;
    }

    /// <summary>
    /// Add time calc dictionary item, delegate execute every seconds.
    /// </summary>
    /// <param name="key">item key</param>
    /// <param name="duration">time over duration</param>
    /// <param name="l_timeCalcIntDelegate">delegate</param>
    /// <returns>is add succeed</returns>
    public bool AddEveryDelegateToTimeCalc(string key, float duration, TimeCalcIntDelegate l_timeCalcIntDelegate = null)
    {
        if (TimeCalc.ContainsKey(key))
        {
            Debug.LogError("Cannot add key:" + key + " to time calc cause key already exist.");
            return false;
        }

        TimeCalc.m_timeCalcList.Add(new TimeCalc() { key = key, IsOverTime = true, StartTime = -1, OverTimeDuration = duration, m_TimeCalcIntDelegate = l_timeCalcIntDelegate, IsOneDelegate = false });

        StartCalc(key);
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool RemoveFromTimeCalcWhenDisable(string key)
    {
        if ( GameObjectHelper.m_dont_destroy_on_load_gb != null)
        {
            return TimeHelper.Instance.RemoveFromTimeCalc(key);
        }
        else
        {
            Debug.LogWarning("Cancel remove time calc key:" + key + " cause singleton has been destroyed.");
            return false;
        }
    }

    /// <summary>
    /// Remove time calc dictionary item.
    /// </summary>
    /// <param name="key">item key</param>
    /// <returns>is remove succeed</returns>
    public bool RemoveFromTimeCalc(string key)
    {
        StopCalc(key);

        if (!TimeCalc.ContainsKey(key))
        {
            Debug.LogError("Cannot remove key:" + key + " to time calc cause key not exist.");
            return false;
        }

        TimeCalc.m_timeCalcList.Remove(TimeCalc.GetTimeCalc(key));
        return true;
    }

    /// <summary>
    /// IsTimeCalcKeyExist
    /// </summary>
    /// <param name="key">item key</param>
    /// <returns>is key exist</returns>
    public bool IsTimeCalcKeyExist(string key)
    {
        return TimeCalc.ContainsKey(key);
    }

    /// <summary>
    /// Start time calc
    /// </summary>
    /// <param name="key">item key</param>
    private void StartCalc(string key)
    {
        if (!TimeCalc.ContainsKey(key))
        {
            Debug.LogError("key:" + key + " not exist.");
            return;
        }

        TimeCalc.GetTimeCalc(key).IsOverTime = false;
        TimeCalc.GetTimeCalc(key).StartTime = Time.realtimeSinceStartup;
    }

    /// <summary>
    /// Stop time calc
    /// </summary>
    /// <param name="key">item key</param>
    private void StopCalc(string key)
    {
        if (!TimeCalc.ContainsKey(key))
        {
            Debug.LogError("key:" + key + " not exist.");
            return;
        }

        TimeCalc.GetTimeCalc(key).IsOverTime = true;
        TimeCalc.GetTimeCalc(key).StartTime = -1;
    }

    /// <summary>
    /// Is over time
    /// </summary>
    /// <param name="key">item key</param>
    /// <returns>is over</returns>
    public bool IsCalcTimeOver(string key)
    {
        if (!TimeCalc.ContainsKey(key))
        {
            Debug.LogError("key:" + key + " not exist.");
            return false;
        }

        if (TimeCalc.GetTimeCalc(key).StartTime < 0)
        {
            Debug.LogError("Time calc key:" + key + " stopped or never start.");
            return false;
        }

        return TimeCalc.GetTimeCalc(key).IsOverTime;
    }

    public float GetCalcTime(string key)
    {
        if (!TimeCalc.ContainsKey(key))
        {
            Debug.LogError("key:" + key + " not exist.");
            return -1;
        }

        if (TimeCalc.GetTimeCalc(key).StartTime < 0)
        {
            Debug.LogError("Time calc key:" + key + " stopped or never start.");
            return -1;
        }

        return Time.realtimeSinceStartup - TimeCalc.GetTimeCalc(key).StartTime;
    }

    private float TimeCalcLastTime;

    // Update is called once per frame
    void Update()
    {
        //One delegate.
        var tempList = TimeCalc.m_timeCalcList.Where(item => (item.IsOneDelegate) && (item.StartTime >= 0) && (!item.IsOverTime)).ToList();
        for (int i = 0; i < tempList.Count; i++)
        {
            if (Time.realtimeSinceStartup - tempList[i].StartTime > tempList[i].OverTimeDuration)
            {
                tempList[i].IsOverTime = true;

                if (tempList[i].m_TimeCalcVoidDelegate != null)
                {
                    tempList[i].m_TimeCalcVoidDelegate();
                }
            }
        }

        //Every delegate.
        if (Time.realtimeSinceStartup - TimeCalcLastTime > 1.0f)
        {
            var tempList2 = TimeCalc.m_timeCalcList.Where(item => (!item.IsOneDelegate) && (item.StartTime >= 0) && (!item.IsOverTime)).ToList();

            for (int i = 0; i < tempList2.Count; i++)
            {
                if (Time.realtimeSinceStartup - tempList2[i].StartTime > tempList2[i].OverTimeDuration)
                {
                    tempList2[i].IsOverTime = true;
                }

                if (tempList2[i].m_TimeCalcIntDelegate != null)
                {
                    tempList2[i].m_TimeCalcIntDelegate((int)GetCalcTime(tempList2[i].key));
                }
            }

            TimeCalcLastTime = Time.realtimeSinceStartup;
        }
    }

    #endregion


	
	#region Origin Utilities's time code

	/// <summary>
	/// Gets the current month day hour minute second.
	/// </summary>
	public static string GetCurMonthDayHourMinSec(){
		System.DateTime t_time = new System.DateTime();
		
		t_time = System.DateTime.Now;
		
		string t_time_str = t_time.ToString("MM-dd HH:mm:ss");
		
		#if DEBUG_TIMER_HELPER
		Debug.Log( "TimeHelper.GetCurMonthDayHourMinSec: " + t_time_str );
		#endif
		
		return t_time_str;
	}
	
	private static float m_signet_time = 0.0f;
	
	public static float SignetTime()
	{
		m_signet_time = Time.realtimeSinceStartup;

		return m_signet_time;
	}
	
	public static float GetDeltaTimeSinceSignet()
	{
		return (Time.realtimeSinceStartup - m_signet_time);
	}
	
	public static void LogDeltaTimeSinceSignet(string p_prefix)
	{
		float t_delta = GetDeltaTimeSinceSignet();
		
		Debug.Log( p_prefix + " : " + MathHelper.FloatPrecision( t_delta, 5 ) );
		
		SignetTime();
	}
	
	public class TimeInfo
	{
		public string m_time_name;
		
		private float m_total_time = 0.0f;
		
		private int m_updated_count = 0;
		
		private float m_tagged_time = 0.0f;
		
		public TimeInfo(string p_time_name, float p_duration = 0.0f)
		{
			m_time_name = p_time_name;
			
			m_total_time = p_duration;
			
			ResetTaggedTime();
		}
		
		public void Reset()
		{
			m_total_time = 0.0f;
			
			m_updated_count = 0;
			
			ResetTaggedTime();
		}
		
		public void UpdateTotalTime(float p_delta)
		{
			m_total_time += p_delta;
			
			m_updated_count++;
			
			ResetTaggedTime();
		}
		
		public int GetUpdatedCount()
		{
			return m_updated_count;
		}
		
		public float GetTotalTime()
		{
			return m_total_time;
		}
		
		public void ResetTaggedTime()
		{
			m_tagged_time = Time.realtimeSinceStartup;
		}
		
		public float GetDeltaTime()
		{
			return Time.realtimeSinceStartup - m_tagged_time;
		}
	}
	
	private static Dictionary<string, TimeInfo> m_time_info_dict = new Dictionary<string, TimeInfo>();
	
	public static int GetTimeInfoUpdatedCount(string p_time_name)
	{
		TimeInfo t_info = GetTimeInfo(p_time_name);
		
		int t_count = t_info.GetUpdatedCount();
		
		return t_count;
	}
	
	public static float GetTimeInfoTotalTime(string p_time_name)
	{
		TimeInfo t_info = GetTimeInfo(p_time_name);
		
		float t_duration = t_info.GetTotalTime();
		
		return t_duration;
	}
	
	public static void UpdateTimeInfo(string p_time_name)
	{
		TimeInfo t_info = GetTimeInfo(p_time_name);
		
		UpdateTimeInfo(p_time_name, t_info.GetDeltaTime());
	}
	
	public static float GetTimeInfoDeltaTime(string p_time_name)
	{
		TimeInfo t_info = GetTimeInfo(p_time_name);
		
		return t_info.GetDeltaTime();
	}
	
	public static void UpdateTimeInfo(string p_time_name, float p_delta)
	{
		TimeInfo t_info = GetTimeInfo(p_time_name);
		
		t_info.UpdateTotalTime(p_delta);
	}
	
	public static void ResetTimeInfo(string p_time_name)
	{
		TimeInfo t_info = GetTimeInfo(p_time_name);
		
		t_info.Reset();
	}
	
	public static void ResetTaggedTime(string p_time_name)
	{
		TimeInfo t_info = GetTimeInfo(p_time_name);
		
		t_info.ResetTaggedTime();
	}
	
	/** Desc:
     * Find or Create.
     */
	private static TimeInfo GetTimeInfo(string p_time_name)
	{
		TimeInfo t_info = null;
		
		m_time_info_dict.TryGetValue(p_time_name, out t_info);
		
		if (t_info == null)
		{
			t_info = new TimeInfo(p_time_name);
			
			m_time_info_dict.Add(p_time_name, t_info);
		}
		
		{
			return t_info;
		}
	}
	
	public static void LogTimeInfo(string p_desc, string p_time_info_name)
	{
		Debug.Log( TimeHelper.GetTimeInfoUpdatedCount(p_time_info_name) +
		          " " + p_desc + ": " +
		          GetTimeInfoDeltaTime(p_time_info_name).ToString("f8") + " - " +
		          GetTimeInfoTotalTime(p_time_info_name).ToString("f8"));
	}
	
	public const string CONST_TIME_INFO_CHECK_SHADER = "CheckShader";
	
	public const string CONST_TIME_INFO_NOT_FOUND_IN_BUNDLE = "BundleNotFound";
	
	public const string CONST_TIME_INFO_CREATE_NODE = "CreateNode";
	
	public const string CONST_TIME_INFO_CREATE_EFFECT = "CreateEffect";
	
	#endregion



}