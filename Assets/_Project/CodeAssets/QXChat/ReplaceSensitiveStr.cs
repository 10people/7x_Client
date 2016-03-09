using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ReplaceSensitiveStr {

	/// <summary>
	/// 内存词典
	/// </summary>
	private static WordGroup[] MEMORYLEXICON = new WordGroup[(int)char.MaxValue];

	/// <summary>
	/// 检测源游标
	/// </summary>
	private static int cursor = 0;

	/// <summary>
	/// 匹配成功后偏移量
	/// </summary>
	private static int wordlenght = 0;
	
	/// <summary>
	/// 检测词游标
	/// </summary>
	private static int nextCursor = 0;

	private static string targetStr;

	/// <summary>
	/// 判断是否是中文
	/// </summary>
	/// <param name="character"></param>
	/// <returns></returns>
	private static bool isCHS(char character)
	{
		//  中文表意字符的范围 4E00-9FA5
		int charVal = (int)character;
		return (charVal >= 0x4e00 && charVal <= 0x9fa5);
	}
	
	/// <summary>
	/// 判断是否是数字
	/// </summary>
	/// <param name="character"></param>
	/// <returns></returns>
	private static bool isNum(char character)
	{
		int charVal = (int)character;
		return (charVal >= 48 && charVal <= 57);
	}
	
	/// <summary>
	/// 判断是否是字母
	/// </summary>
	/// <param name="character"></param>
	/// <returns></returns>
	private static bool isAlphabet(char character)
	{
		int charVal = (int)character;
		return ((charVal >= 97 && charVal <= 122) || (charVal >= 65 && charVal <= 90));
	}
	
	
	/// <summary>
	/// 转半角小写的函数(DBC case)
	/// </summary>
	/// <param name="input">任意字符串</param>
	/// <returns>半角字符串</returns>
	///<remarks>
	///全角空格为12288，半角空格为32
	///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
	///</remarks>
	private static string ToDBC(string input)
	{
		char[] c = input.ToCharArray();
		for (int i = 0; i < c.Length; i++)
		{
			if (c[i] == 12288)
			{
				c[i] = (char)32;
				continue;
			}
			if (c[i] > 65280 && c[i] < 65375)
				c[i] = (char)(c[i] - 65248);
		}
		return new string(c).ToLower();
	}

	/// <summary>
	/// 加载内存词库
	/// </summary>
	public static void LoadSensitiveWord (List<SysparaTemplate> tempLates)
	{
		List<string> wordList = new List<string>();
		Array.Clear (MEMORYLEXICON, 0, MEMORYLEXICON.Length);

		foreach (SysparaTemplate syspara in tempLates)
		{
			string key = ToDBC (syspara.word);
			wordList.Add (key);
		}
		Comparison<string> cmp = delegate (string key1, string key2)
		{
			return key1.CompareTo (key2);
		};
		wordList.Sort (cmp);
		for (int i = wordList.Count - 1; i > 0; i--)
		{
			if (wordList[i].ToString () == wordList[i - 1].ToString ())
			{
				wordList.RemoveAt (i);
			}
		}
		foreach (var word in wordList)
		{
			WordGroup group = MEMORYLEXICON[(int)word[0]];
			if (group == null)
			{
				group = new WordGroup();
				MEMORYLEXICON[(int)word[0]] = group;
				
			}
			group.Add (word.Substring(1));
		}
	}

	/// <summary>
	/// 检测
	/// </summary>
	/// <param name="blackWord"></param>
	/// <returns></returns>
	private static bool Check (string blackWord)
	{
		wordlenght = 0;
		//检测源下一位游标
		nextCursor = cursor + 1;
		bool found = false;
		//遍历词的每一位做匹配
		for (int i = 0; i < blackWord.Length; i++)
		{
			//特殊字符偏移游标
			int offset = 0;
			if (nextCursor >= targetStr.Length)
			{
				break;
			}
			else
			{
				//检测下位字符如果不是汉字 数字 字符 偏移量加1
				for (int y = nextCursor; y < targetStr.Length; y++)
				{
					
					if (!isCHS(targetStr[y]) && !isNum(targetStr[y]) && !isAlphabet(targetStr[y]))
					{
//						offset++;
						//避让特殊字符，下位游标如果>=字符串长度 跳出
						if (nextCursor + offset >= targetStr.Length) break;
						wordlenght++;
					}
					else break;
				}
				
				if ((int)blackWord[i] == (int)targetStr[nextCursor + offset])
				{
					found = true;
				}
				else
				{
					found = false;
					break;
				}
			}
			nextCursor = nextCursor + 1 + offset;
			wordlenght++;
		}
		return found;
	}
	
	/// <summary>
	/// 查找并替换
	/// </summary>
	/// <param name="replaceChar"></param>
	public static string Filter (string tempStr)
	{
		cursor = 0;

		targetStr = tempStr;

		char[] tempString = tempStr.ToCharArray (); ;

		for (int i = 0; i < tempStr.Length; i++)
		{
			//查询以该字为首字符的词组
			WordGroup group = MEMORYLEXICON[(int)ToDBC (tempStr)[i]];

			if (group != null)
			{
				for (int z = 0; z < group.Count(); z++)
				{
					string word = group.GetWord (z);
					if (word.Length == 0 || Check (word))
					{
						string blackword = string.Empty;
						for (int pos = 0; pos < wordlenght + 1; pos++)
						{
							blackword += tempString[pos + cursor].ToString ();
							tempString[pos + cursor] = '*';
						}

						cursor = cursor + wordlenght;
						i = i + wordlenght;
					}
				}
			}
			cursor++;
		}
		return new string(tempString);
	}
}

/// <summary>
/// 具有相同首字符的词组集合
/// </summary>
class WordGroup
{
	/// <summary>
	/// 集合
	/// </summary>
	private List<string> groupList;
	
	public WordGroup ()
	{
		groupList = new List<string>();
	}
	
	/// <summary>
	/// 添加词
	/// </summary>
	/// <param name="word"></param>
	public void Add (string word)
	{
		groupList.Add (word);
	}
	
	/// <summary>
	/// 获取总数
	/// </summary>
	/// <returns></returns>
	public int Count ()
	{
		return groupList.Count;
	}
	
	/// <summary>
	/// 根据下标获取词
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public string GetWord (int index)
	{
		return groupList[index];
	}
}
