using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TreasureCityUITop : MonoBehaviour {

	public ErrorMessage errorMsg;

	public UILabel boxNum;

	public UILabel nextBox;

	/// <summary>
	/// Ins it top U.
	/// </summary>
	/// <param name="tempMsg">Temp message.</param>
	public void InItTopUI (ErrorMessage tempMsg)
	{
		errorMsg = tempMsg;

		boxNum.text = "宝箱波数：" + tempMsg.errorCode + "/" + tempMsg.cmd;

		nextBox.text = int.Parse (tempMsg.errorDesc) == 0 ? "" : "下一波宝箱：" + (int.Parse (tempMsg.errorDesc) > 10 ? TimeHelper.GetUniformedTimeString (int.Parse (tempMsg.errorDesc)) : tempMsg.errorDesc + "秒");
	}
}
