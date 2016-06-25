using UnityEngine;
using System.Collections;

public interface IMSCListener
{
    /// <summary>
    /// 语音解析结果
    /// </summary>
    /// <param name="result"></param>
    void MSCResult(string tag, string result, string fileStr);

    void MSCStarted();

    void MSCEnded();

    /// <summary>
    /// Put error code to user.
    /// </summary>
    /// <param name="error">
    /// 
    /// 
    /// </param>
    void MSCError(string error);

    void MSCVolume(int vol);
}
