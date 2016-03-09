using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class RequestTest : MonoBehaviour {

    void Start()
    {
        Dictionary<string,string> tempUrl = new Dictionary<string,string>();
        tempUrl.Add("name","li");
        tempUrl.Add("pwd","zhangwen");
        HttpRequest.Instance().Connect(CityGlobalData.LoginURL,tempUrl,RequestSuccess, RequestFail);
    }

    void RequestSuccess(string tempResponse)
    {
//        Debug.Log("数据返回：" + tempResponse);

        JSONNode tempNode = JSON.Parse(tempResponse);

        if (tempNode["lastLogin"] != null)
        {
 
        }

        JSONArray loginList = tempNode["loginList"].AsArray;

//        foreach (JSONNode tempItemNode in loginList)
//        {
//            Debug.Log(tempItemNode["id"].AsInt);
//        }
    }

    void RequestFail(string tempResponse)
    {
        Debug.Log(tempResponse);
    }
}
