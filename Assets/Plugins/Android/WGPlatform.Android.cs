#if UNITY_EDITOR_WIN || UNITY_ANDROID
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using LitJson;
using com.tencent.bugly.unity3d;

namespace Msdk
{
	/// <summary>
	/// 封装msdk的api 
	/// </summary>
    public partial class WGPlatform
	{	  
	    // The instance of billing script.
		private static readonly string className = "com.tencent.msdk.u3d.UnityWGPlatform";
		private AndroidJavaClass unityWGPlatform;
	    private static WGPlatform _instance;
	    public static WGPlatform Instance
	    {
	        get
	        {
	            if(_instance==null){
	                _instance = new WGPlatform();
	            }
	            return _instance;
	        }
	    }

		private AndroidJavaClass GetUnityWGPlatform()
		{
			if (unityWGPlatform == null) 
			{
				unityWGPlatform = new AndroidJavaClass(className);
			}
			return unityWGPlatform;
		}

		// MSDK的C#层初始化，目前主要是Bugly的初始化
		public void Init() {
			InitBugly ();
		}

		// 初始化Bugly的C#插件
		private void InitBugly() {
			// 设置开启Bugly的调式信息开关，发布时请设置为false
			Bugly.EnableLog (false);
			// 设置上报C#异常后是否退出应用, 默认情况下, C#异常不会导致应用崩溃, 但为避免C#异常引发后续一系列诡异问题, 你可以设置捕获C#异常后退出应用
			Bugly.SetAutoQuitApplication (false);
			// 设置自动上报的c#错误信息的类型，可设置为Exception,Error等
			// 启动C#堆栈捕获上报, 并注册日志监听回调
			Bugly.RegisterExceptionHandler (LogSeverity.Exception, null);
		}

		public void WGLogin(int platform) 
		{
			GetUnityWGPlatform().CallStatic("Login", platform);
		}

		public LoginRet WGGetLoginRecord()
		{
			string loginRetStr = GetUnityWGPlatform().CallStatic<string>("WGGetLoginRecord");
			if (loginRetStr != null)
			{
				return LoginRet.ParseJson (loginRetStr);
			}
			else
			{
				return null;
			}
		}
	   
		//登录QQ,回调是loginNotify
		public void WGLoginQQ()
	    {
			GetUnityWGPlatform().CallStatic("LoginQQ");	        
	    }

		//登录微信,回调是loginNotify
		public void WGLoginWX()
		{
			GetUnityWGPlatform().CallStatic("LoginWX");		
		}

		/// <summary>
		/// 查询QQ的个人信息,回调是relationNotify
		/// </summary>
		public bool WGQueryMyQQInfo()
		{
			return GetUnityWGPlatform().CallStatic<bool>("QueryMyQQInfo");
		}

		/// <summary>
		/// 查询QQ的同玩好友信息,回调是relationNotify
		/// </summary>
		public bool WGQueryQQGameFriendsInfo()
		{
			return GetUnityWGPlatform().CallStatic<bool>("QueryQQGameFriendsInfo");
		}

		/// <summary>
		/// 结构化消息分享(拉起手Q分享到空间),通过游戏设置的全局回调的shareNotify回调返回数据给游戏
		/// </summary>
		/// <param name="title">结构化消息的标题.</param>
		/// <param name="desc">结构化消息的概要信息</param>
		/// <param name="url">内容的跳转url，填游戏对应游戏中心详情页，游戏被分享消息拉起时, MSDK会给游戏wakeupNotify回调</param>
		/// <param name="imgUrl">分享消息缩略图URL.</param>
		/// <param name="imgUrlLen">分享消息缩略图URL长度.</param>
		public void WGSendToQQ_Qzone(string title, string desc, string url, string imgUrl, int imgUrlLen)
		{
			GetUnityWGPlatform().CallStatic("SendToQQ_Qzone", title, desc, url, imgUrl, imgUrlLen);
		}

		/// <summary>
		/// 结构化消息分享(拉起手Q分享给好友会话),通过游戏设置的全局回调的shareNotify回调返回数据给游戏
		/// </summary>
		/// <param name="title">结构化消息的标题.</param>
		/// <param name="desc">结构化消息的概要信息</param>
		/// <param name="url">内容的跳转url，填游戏对应游戏中心详情页，游戏被分享消息拉起时, MSDK会给游戏wakeupNotify回调</param>
		/// <param name="imgUrl">分享消息缩略图URL.</param>
		/// <param name="imgUrlLen">分享消息缩略图URL长度.</param>
		public void WGSendToQQ_Session(string title, string desc, string url, string imgUrl, int imgUrlLen)
		{
			GetUnityWGPlatform().CallStatic("SendToQQ_Session", title, desc, url, imgUrl, imgUrlLen);
		}

		/// <summary>
		/// 将音乐消息分享到手Q会话
		/// </summary>
		/// <param name="title">结构化消息的标题</param>
		/// <param name="desc">结构化消息的概要信息.</param>
		/// <param name="musicUrl">点击消息后跳转的URL.</param>
		/// <param name="musicDataUrl">音乐数据URL（例如http:// ***.mp3）.</param>
		/// <param name="imgUrl">分享消息缩略图URL.</param>
		public void WGSendToQQWithMusic_Session(string title, string desc, string musicUrl, string musicDataUrl, string imgUrl)
		{
			GetUnityWGPlatform().CallStatic("SendToQQWithMusic_Session", title, desc, musicUrl, musicDataUrl, imgUrl);		
		}

		/// <summary>
		/// 将音乐消息分享到空间
		/// </summary>
		/// <param name="title">结构化消息的标题</param>
		/// <param name="desc">结构化消息的概要信息.</param>
		/// <param name="musicUrl">点击消息后跳转的URL.</param>
		/// <param name="musicDataUrl">音乐数据URL（例如http:// ***.mp3）.</param>
		/// <param name="imgUrl">分享消息缩略图URL.</param>
		public void WGSendToQQWithMusic_Qzone(string title, string desc, string musicUrl, string musicDataUrl, string imgUrl)
		{
			GetUnityWGPlatform().CallStatic("SendToQQWithMusic_Qzone", title, desc, musicUrl, musicDataUrl, imgUrl);
		}

		/// <summary>
		/// 后端分享给好友，无需拉起手Q，但是只能分享给游戏内好友.
		/// </summary>
		/// <param name="act">好友点击分享消息拉起页面还是直接拉起游戏, 传入 1 拉起游戏, 传入 0, 拉起targetUrl</param>
		/// <param name="friendOpenId">好友的openId</param>
		/// <param name="title">分享的标题.</param>
		/// <param name="summary">分享的简介</param>
		/// <param name="targetUrl">内容的跳转url，填游戏对应游戏中心详情页，游戏被分享消息拉起时, MSDK会给游戏WakeupNotify回调, 返回的json串中extInfo中会以key-value的方式带回所有的自定义参数..</param>
		/// <param name="imageUrl">分享缩略图URL.</param>
		/// <param name="previewText">可选, 预览文字</param>
		/// <param name="gameTag"> gameTag 可选, 此参数必须填入如下值的其中一个
		///											MSG_INVITE                //邀请
		///											MSG_FRIEND_EXCEED       //超越炫耀
		///											MSG_HEART_SEND          //送心
		///											MSG_SHARE_FRIEND_PVP    //PVP对战
		/// </param>
		public bool WGSendToQQGameFriend(int act, string friendOpenId, string title, string summary, string targetUrl, string imageUrl, string previewText, string gameTag)
		{
				return GetUnityWGPlatform().CallStatic<bool>("SendToQQGameFriend", act, friendOpenId, title, summary,  targetUrl,  imageUrl,  previewText, gameTag);
		}

		/// <summary>
		/// 大图消息分享到手Q会话，要完成此种消息分享需要唤起手Q, 需要用户参与才能完成整个分享过程,可以分享给游戏内和游戏外好友, 通常用来炫耀成绩或者其他需要详图的功能.
		/// 消息分享出去以后, 消息接收者点击消息不能唤起游戏.
		/// </summary>
		/// <param name="imgFilePath">需要分享图片的本地文件路径, 图片需放在sd卡。每次分享的图片路径不能相同，相同会导致图片显示有问题，游戏需要自行保证每次分享图片的地址不相同.</param>
		public void WGSendToQQWithPhoto_Session(string imgFilePath)
		{
			GetUnityWGPlatform().CallStatic("SendToQQWithPhoto_Session", imgFilePath);
		}

		/// <summary>
		/// 大图消息分享到空间，要完成此种消息分享需要唤起手Q, 需要用户参与才能完成整个分享过程,可以分享给游戏内和游戏外好友, 通常用来炫耀成绩或者其他需要详图的功能.
		/// 消息分享出去以后, 消息接收者点击消息不能唤起游戏.
		/// </summary>
		/// <param name="imgFilePath">需要分享图片的本地文件路径, 图片需放在sd卡。每次分享的图片路径不能相同，相同会导致图片显示有问题，游戏需要自行保证每次分享图片的地址不相同.</param>
		public void WGSendToQQWithPhoto_Qzone(string imgFilePath)
		{
			GetUnityWGPlatform().CallStatic("SendToQQWithPhoto_Qzone", imgFilePath);
		}

		/// <summary>
		/// 玩家在游戏中直接其他游戏玩家为QQ好友。目前该接口游戏需要在主线程调用。
		/// </summary>
		/// <param name="fopenid">需要添加好友的openid.</param>
		/// <param name="desc">要添加好友的备注.</param>
		/// <param name="message">添加好友时发送的验证信息</param>
		public void WGAddGameFriendToQQ(string fopenid, string desc, string message)
		{
			GetUnityWGPlatform().CallStatic("AddGameFriendToQQ", fopenid, desc, message);
		}
		/// <summary>
		/// 游戏群绑定：游戏公会/联盟内，公会会长可通过点击“绑定”按钮，拉取会长自己创建的群，绑定某个群作为该公会的公会群	
		/// 目前该接口游戏需要在主线程调用
		/// </summary>
		/// <param name="unionid">公会ID，opensdk限制只能填数字，字符可能会导致绑定失败 </param>
		/// <param name="union_name">公会名称</param>
		/// <param name="zoneid">大区ID，opensdk限制只能填数字，字符可能会导致绑定失败 .</param>
		/// <param name="signature">游戏盟主身份验证签名，生成算法为玩家openid_游戏appid_游戏appkey_公会id_区id 做md5</param>
		public void WGBindQQGroup(string unionid, string union_name, string zoneid,  string signature)
		{
			GetUnityWGPlatform().CallStatic("BindQQGroup", unionid, union_name, zoneid, signature);
		}

		/// <summary>
		/// 游戏内加群,公会成功绑定qq群后，公会成员可通过点击“加群”按钮，加入该公会群。目前该接口游戏需要在主线程调用
		/// </summary>
		/// <param name="qqGroupKey">需要添加的QQ群对应的key，游戏server可通过调用openAPI的接口获取.</param>
		public void WGJoinQQGroup(string qqGroupKey)
		{
			GetUnityWGPlatform().CallStatic("JoinQQGroup", qqGroupKey);
		}

		/**
	 * 解绑公会当前绑定的QQ群，结果会通过WGQQGroupObserver的OnUnbindGroupNotify回调给游戏
	 * @param cGroupOpenid 公会绑定的群的群openid
	 * @param cUnionid 公会ID
	 */
		
		public void WGUnbindQQGroup(string cGroupOpenid, string cUnionid)
		{
			GetUnityWGPlatform().CallStatic("WGUnbindQQGroup", cGroupOpenid, cUnionid);
		}

		/**
	 * 查询公会绑定的群的信息，查询结果会通过WGQQGroupObserver的OnQueryGroupInfoNotify回调给游戏
	 * @param unionid 公会ID
	 * @param zoneid 大区ID
	 */
		public void WGQueryQQGroupInfo(string cUnionid, string cZoneid) 
		{
			GetUnityWGPlatform().CallStatic("WGQueryQQGroupInfo", cUnionid, cZoneid);
		}

		/**
   	 * 查询公会绑定群加群时的GroupKey的信息，查询结果会通过WGQQGroupObserver的OnQueryGroupKeyNotify回调给游戏
   	 * @param cGroupOpenid 群openID
   	 */
		public void WGQueryQQGroupKey(string cGroupOpenid)
		{
			GetUnityWGPlatform().CallStatic("WGQueryQQGroupKey", cGroupOpenid);
		}


		/// <summary>
		/// 打开内置浏览器,此内置Webview从安全性, 性能各方面优于系统内置Webview, 
		/// 如果手机上安装了QQ浏览器则会使用QQ浏览器的内核, 性能更优. 同时还提供了在内置浏览器中分享到QQ和微信的功能.
		/// </summary>
		/// <param name="url">URL.</param>
		public  void WGOpenUrl(string url) 
		{
			GetUnityWGPlatform().CallStatic("OpenUrl", url);
		}							

		public void WGReportEvent(string name, Dictionary<string, string> data, bool isRealTime) 
		{
			JsonData json = new JsonData ();
			foreach (string key in data.Keys) {
				json[key] = data[key];
			}
			string jsonData = json.ToJson ();
			Debug.Log ("json=" + jsonData);
			GetUnityWGPlatform().CallStatic("ReportEvent", name, jsonData, isRealTime);
		}

		/// <summary>
		/// 查询微信个人信息，通过RelationNotify回调
		/// </summary>
		/// <returns><c>true</c>, if my WX info was queryed, <c>false</c> otherwise.</returns>
		public  bool WGQueryMyWXInfo()
		{
			return GetUnityWGPlatform().CallStatic<bool>("QueryMyWXInfo");			
		}

		/// <summary>
		/// 获取微信好友信息, 回调在RelationNotify中
		/// </summary>
		/// <returns><c>true</c>, if WX game friends info was queryed, <c>false</c> otherwise.</returns>
		public bool WGQueryWXGameFriendsInfo()
		{
			return GetUnityWGPlatform().CallStatic<bool>("QueryWXGameFriendsInfo");			
		}

		/// <summary>
		/// 结构化消息分享,此种消息分享需要唤起微信客户端, 需要用户参与才能完成整个分享过程, 可以分享给游戏内和游戏外好友, 通常用来邀请游戏外好友.
		/// 消息分享出去以后, 消息接收者点击消息可以拉起游戏。回调是ShareNotify,其中的flag字段表示返回状态, 可能值及说明如下:
		///     eFlag_Succ: 分享成功
		///     eFlag_Error: 分享失败
		/// </summary>
		/// <param name="title">结构化消息的标题</param>
		/// <param name="desc">结构化消息的概要信息.</param>
		/// <param name="mediaTagName">请根据实际情况填入下列值的一个, 此值会传到微信供统计用, 在分享返回时也会带回此值, 可以用于区分分享来源
		///														"MSG_INVITE";                   // 邀请
		///														"MSG_SHARE_MOMENT_HIGH_SCORE";    //分享本周最高到朋友圈
		///														"MSG_SHARE_MOMENT_BEST_SCORE";    //分享历史最高到朋友圈
		///														"MSG_SHARE_MOMENT_CROWN";         //分享金冠到朋友圈
		///														"MSG_SHARE_FRIEND_HIGH_SCORE";     //分享本周最高给好友
		///														"MSG_SHARE_FRIEND_BEST_SCORE";     //分享历史最高给好友
		///														"MSG_SHARE_FRIEND_CROWN";          //分享金冠给好友
		///														"MSG_friend_exceed"         // 超越炫耀
		///														"MSG_heart_send"            // 送心</param>
		/// <param name="thumbData">结构化消息的缩略图</param>
		/// <param name="thumbDataLen">结构化消息的缩略图数据长度</param>
		/// <param name="messageExt">游戏分享时传入拓展字符串，通过此消息拉起游戏会通过 WakeUpNotify 中messageExt字段回传给游戏</param>
		public void WGSendToWeixin_Session(string title, string desc, string mediaTagName, 
		                                        byte[] thumbData, int thumbDataLen,string messageExt) 
		{
			GetUnityWGPlatform().CallStatic("SendToWeixin_Session", title, desc, mediaTagName, 
				                      thumbData, thumbDataLen, messageExt);
		}

		/// <summary>
		/// 大图消息分享到微信好友。此种消息分享需要唤起微信, 需要用户参与才能完成整个分享过程, 可以分享给游戏内和游戏外好友, 
		/// 通常用来炫耀成绩或者其他需要详图的功能. 此种消息可以分享到会话(好友)或者朋友圈, 微信4.0及以上支持分享到会话, 
		/// 微信4.2及以上支持分享到朋友圈. 图片大小不能大于10M, 分享的图片微信会做相应的压缩处理. 
		/// </summary>
		/// <param name="mediaTagName">请根据实际情况填入下列值的一个, 此值会传到微信供统计用, 在分享返回时也会带回此值, 可以用于区分分享来源
		///														"MSG_INVITE";                   // 邀请
		///														"MSG_SHARE_MOMENT_HIGH_SCORE";    //分享本周最高到朋友圈
		///														"MSG_SHARE_MOMENT_BEST_SCORE";    //分享历史最高到朋友圈
		///														"MSG_SHARE_MOMENT_CROWN";         //分享金冠到朋友圈
		///														"MSG_SHARE_FRIEND_HIGH_SCORE";     //分享本周最高给好友
		///														"MSG_SHARE_FRIEND_BEST_SCORE";     //分享历史最高给好友
		///														"MSG_SHARE_FRIEND_CROWN";          //分享金冠给好友
		///														"MSG_friend_exceed"         // 超越炫耀
		///														"MSG_heart_send"            // 送心	
		/// </param>
		/// <param name="imgData">原图文件数据</param>
		/// <param name="imgDataLen">原图文件数据长度(图片大小不能超过10M)</param>
		/// <param name="messageExt">游戏分享是传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify()中WakeupRet的messageExt回传给游戏</param>
		/// <param name="mediaAction">该参数在分享到微信朋友圈的情况下才起作用</param>
		public void WGSendToWeixinWithPhoto_Session(string mediaTagName, byte[] imgData,
		                                                     int imgDataLen, string messageExt,string mediaAction)
		{
			GetUnityWGPlatform().CallStatic("SendToWeixinWithPhoto_Session", mediaTagName, imgData, imgDataLen, messageExt, mediaAction);
		}

		/// <summary>
		/// 大图消息分享到微信朋友圈。此种消息分享需要唤起微信, 需要用户参与才能完成整个分享过程, 可以分享给游戏内和游戏外好友, 
		/// 通常用来炫耀成绩或者其他需要详图的功能. 此种消息可以分享到会话(好友)或者朋友圈, 微信4.0及以上支持分享到会话, 
		/// 微信4.2及以上支持分享到朋友圈. 图片大小不能大于10M, 分享的图片微信会做相应的压缩处理. 
		/// </summary>
		/// <param name="mediaTagName">请根据实际情况填入下列值的一个, 此值会传到微信供统计用, 在分享返回时也会带回此值, 可以用于区分分享来源
		///														"MSG_INVITE";                   // 邀请
		///														"MSG_SHARE_MOMENT_HIGH_SCORE";    //分享本周最高到朋友圈
		///														"MSG_SHARE_MOMENT_BEST_SCORE";    //分享历史最高到朋友圈
		///														"MSG_SHARE_MOMENT_CROWN";         //分享金冠到朋友圈
		///														"MSG_SHARE_FRIEND_HIGH_SCORE";     //分享本周最高给好友
		///														"MSG_SHARE_FRIEND_BEST_SCORE";     //分享历史最高给好友
		///														"MSG_SHARE_FRIEND_CROWN";          //分享金冠给好友
		///														"MSG_friend_exceed"         // 超越炫耀
		///														"MSG_heart_send"            // 送心	
		/// </param>
		/// <param name="imgData">原图文件数据</param>
		/// <param name="imgDataLen">原图文件数据长度(图片大小不能超过10M)</param>
		/// <param name="messageExt">游戏分享时传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify()中WakeupRet的messageExt回传给游戏</param>
		/// <param name="mediaAction">可选参数：WECHAT_SNS_JUMP_SHOWRANK （跳排行），WECHAT_SNS_JUMP_URL（跳链接）
		/// 							WECHAT_SNS_JUMP_APP （跳APP）
		/// </param>
		public void WGSendToWeixinWithPhoto_Moment(string mediaTagName, byte[] imgData,
		                                                     int imgDataLen, string messageExt,string mediaAction)
		{
			GetUnityWGPlatform().CallStatic("SendToWeixinWithPhoto_Moment", mediaTagName, imgData, imgDataLen, messageExt, mediaAction);		
		}

    /**
     * 分享本地图片到微信
	 * @param scene 指定分享到朋友圈, 或者微信会话, 可能值和作用如下:
	 *   WechatScene_Session: 分享到微信会话
	 *   WechatScene_Timeline: 分享到微信朋友圈
	 * @param mediaTagName (必填)请根据实际情况填入下列值的一个, 此值会传到微信供统计用, 在分享返回时也会带回此值, 可以用于区分分享来源
		 "MSG_INVITE";                   // 邀请
		 "MSG_SHARE_MOMENT_HIGH_SCORE";    //分享本周最高到朋友圈
		 "MSG_SHARE_MOMENT_BEST_SCORE";    //分享历史最高到朋友圈
		 "MSG_SHARE_MOMENT_CROWN";         //分享金冠到朋友圈
		 "MSG_SHARE_FRIEND_HIGH_SCORE";     //分享本周最高给好友
		 "MSG_SHARE_FRIEND_BEST_SCORE";     //分享历史最高给好友
		 "MSG_SHARE_FRIEND_CROWN";          //分享金冠给好友
		 "MSG_friend_exceed"         // 超越炫耀
		 "MSG_heart_send"            // 送心
	 * @param imgPath 本地图片的路径(建议图片小于3MB)
	 * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
	 * @param messageAction scene为1(分享到微信朋友圈)的情况下才起作用
	 *   WECHAT_SNS_JUMP_SHOWRANK       跳排行
	 *   WECHAT_SNS_JUMP_URL            跳链接
	 *   WECHAT_SNS_JUMP_APP           跳APP
	 * @return void
	 *   通过游戏设置的全局回调的OnShareNotify(ShareRet& shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
	 *     eFlag_Succ: 分享成功
	 *     eFlag_Error: 分享失败
	 */
		public void WGSendToWeixinWithPhotoPath(int scene, string mediaTagName,string imgPath, string messageExt, string messageAction)
		{
			GetUnityWGPlatform().CallStatic ("WGSendToWeixinWithPhotoPath", scene, mediaTagName, imgPath, messageExt, messageAction);
		}

		/// <summary>
		/// 把音乐消息分享给微信好友		
		/// </summary>
		/// <param name="title">音乐消息的标题.</param>
		/// <param name="desc">音乐消息的概要信息</param>
		/// <param name="musicUrl">音乐消息的目标URL</param>
		/// <param name="musicDataUrl">音乐消息的数据URL.</param>
		/// <param name="mediaTagName"> 请根据实际情况填入下列值的一个, 此值会传到微信供统计用, 在分享返回时也会带回此值, 可以用于区分分享来源
		///														"MSG_INVITE";                   // 邀请
		///														"MSG_SHARE_MOMENT_HIGH_SCORE";    //分享本周最高到朋友圈
		///														"MSG_SHARE_MOMENT_BEST_SCORE";    //分享历史最高到朋友圈
		///														"MSG_SHARE_MOMENT_CROWN";         //分享金冠到朋友圈
		///														"MSG_SHARE_FRIEND_HIGH_SCORE";     //分享本周最高给好友
		///														"MSG_SHARE_FRIEND_BEST_SCORE";     //分享历史最高给好友
		///														"MSG_SHARE_FRIEND_CROWN";          //分享金冠给好友
		///														"MSG_friend_exceed"         // 超越炫耀
		///														"MSG_heart_send"            // 送心
		/// </param>
		/// <param name="imgData">原图文件数据.</param>
		/// <param name="imgDataLen">原图文件数据长度(图片大小不能超过10M).</param>
		/// <param name="messageExt">游戏分享时传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify()中WakeupRet的messageExt回传给游戏</param>
		/// <param name="messageAction">该参数在分享到微信朋友圈的情况下才起作用.</param>
		public void WGSendToWeixinWithMusic_Session(string  title, string  desc, string musicUrl, string musicDataUrl,
		                                                 string mediaTagName, byte[] imgData, int imgDataLen, string messageExt, string messageAction)
		{
			GetUnityWGPlatform().CallStatic("SendToWeixinWithMusic_Session", title,  desc, musicUrl, musicDataUrl,
				                       mediaTagName, imgData, imgDataLen, messageExt, messageAction);
		}

		/// <summary>
		/// 把音乐消息分享给微信朋友圈
		/// </summary>
		/// <param name="title">音乐消息的标题.</param>
		/// <param name="desc">音乐消息的概要信息</param>
		/// <param name="musicUrl">音乐消息的目标URL</param>
		/// <param name="musicDataUrl">音乐消息的数据URL.</param>
		/// <param name="mediaTagName"> 请根据实际情况填入下列值的一个, 此值会传到微信供统计用, 在分享返回时也会带回此值, 可以用于区分分享来源
		///														"MSG_INVITE";                   // 邀请
		///														"MSG_SHARE_MOMENT_HIGH_SCORE";    //分享本周最高到朋友圈
		///														"MSG_SHARE_MOMENT_BEST_SCORE";    //分享历史最高到朋友圈
		///														"MSG_SHARE_MOMENT_CROWN";         //分享金冠到朋友圈
		///														"MSG_SHARE_FRIEND_HIGH_SCORE";     //分享本周最高给好友
		///														"MSG_SHARE_FRIEND_BEST_SCORE";     //分享历史最高给好友
		///														"MSG_SHARE_FRIEND_CROWN";          //分享金冠给好友
		///														"MSG_friend_exceed"         // 超越炫耀
		///														"MSG_heart_send"            // 送心
		/// </param>
		/// <param name="imgData">原图文件数据.</param>
		/// <param name="imgDataLen">原图文件数据长度(图片大小不能超过10M).</param>
		/// <param name="messageExt">游戏分享时传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify()中WakeupRet的messageExt回传给游戏</param>
		/// <param name="messageAction">可选参数：WECHAT_SNS_JUMP_SHOWRANK （跳排行），WECHAT_SNS_JUMP_URL（跳链接）
		/// 							WECHAT_SNS_JUMP_APP （跳APP）
		/// </param>
		public void WGSendToWeixinWithMusic_Moment(string  title, string  desc, string musicUrl, string musicDataUrl,
		                                                 string mediaTagName, byte[] imgData, int imgDataLen, string messageExt, string messageAction)
		{
			GetUnityWGPlatform().CallStatic("SendToWeixinWithMusic_Moment", title,  desc, musicUrl, musicDataUrl,
				                       mediaTagName, imgData, imgDataLen, messageExt, messageAction);
		}

		/// <summary>
		///  此接口类似SendToQQGameFriend, 此接口用于分享消息到微信好友, 分享必须指定好友openid
		/// </summary>
		/// <returns><c>true</c>, if send to WX game friend was WGed, <c>false</c> otherwise.</returns>
		/// <param name="fopenid">好友的openid</param>
		/// <param name="title">分享标题</param>
		/// <param name="description">分享描述.</param>
		/// <param name="messageExt">游戏分享是传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify()中WakeupRet messageExt回传给游戏</param>
		/// <param name="mediaTagName">Media tag name.</param>
		/// <param name="thumbMediaId">图片的id 通过uploadToWX接口获取.</param>
		/// <param name="msdkExtInfo">游戏自定义透传字段，通过分享结果shareRet.extInfo返回给游戏，游戏可以用extInfo区分request</param>
		public  bool WGSendToWXGameFriend(string fopenid, string title, string description,
		                                           string messageExt, string mediaTagName, string thumbMediaId, string msdkExtInfo)
		{
			return GetUnityWGPlatform().CallStatic<bool>("SendToWXGameFriend",  fopenid,  title,  description,
				                              messageExt, mediaTagName,  thumbMediaId,  msdkExtInfo);
		}

		/// <summary>
		/// 刷新微信票据
		/// </summary>
		public void WGRefreshWXToken()
		{
			GetUnityWGPlatform().CallStatic("RefreshWXToken");			
		}

		/// <summary>
		/// 	展示对应类型指定公告栏下的公告
		/// </summary>
		/// <param name="scene">公告栏ID，不能为空, 这个参数和公告管理端的“公告栏”设置对应</param>
		public void WGShowNotice(string scene)
		{
			GetUnityWGPlatform().CallStatic("ShowNotice", scene);			
		}

		/// <summary>
		/// 从本地数据库读取指定scene下指定type的当前有效公告
		/// </summary>
		/// <returns>返回NoticeInfo的Json串.</returns>
		/// <param name="scene">这个参数和公告管理端的“公告栏”对应</param>
//		public string WGGetNoticeData(string scene)
//		{
//			return GetUnityWGPlatform().CallStatic<string>("GetNoticeData", scene);			
//		}

		public List<NoticeInfo> WGGetNoticeData(string scene)
		{
			string noticeDataStr = GetUnityWGPlatform().CallStatic<string>("GetNoticeData", scene);	

			if (noticeDataStr != null) 
			{
				return NoticeInfo.ParseJson(noticeDataStr);
			} 
			else 
			{
				return null;
			}
		}

		/// <summary>
		/// 隐藏正在展示的滚动公告.
		/// </summary>
		public void WGHideScrollNotice()
		{
			GetUnityWGPlatform().CallStatic("HideScrollNotice");			
		}

		/// <summary>
		/// 获取附近人的信息，回调到OnLocationNotify
		/// </summary>
		public void WGGetNearbyPersonInfo()
		{
			GetUnityWGPlatform().CallStatic("GetNearbyPersonInfo");			
		}

		/// <summary>
		/// 清除个人位置信息,回调到OnLocationNotify
		/// </summary>
		/// <returns><c>true</c>, if location was cleaned, <c>false</c> otherwise.</returns>
		public bool WGCleanLocation()
		{
			return GetUnityWGPlatform().CallStatic<bool>("CleanLocation");
		}

		/// <summary>
		/// 获取当前玩家位置信息,返回给游戏的同时上报到MSDK后台。
		/// </summary>
		/// <returns>true则说明客户端侧未发生错误，false则说明客户端侧发生错误
		///   通过游戏设置的全局回调的OnLocationGotNotify()方法返回数据给游戏
		/// 	LocationRet rr
		///     rr.platform表示当前的授权平台, 值类型为ePlatform, 可能值为ePlatform_QQ, ePlatform_Weixin
		///     rr.flag值表示返回状态, 可能值(eFlag枚举)如下：
		///          eFlag_Succ: 获取成功
		///          eFlag_Error: 获取失败
		///     rr.longitude 玩家位置经度，double类型
		///     rr.latitude 玩家位置纬度，double类型
		/// </returns>
		public bool WGGetLocationInfo()
		{
			return GetUnityWGPlatform().CallStatic<bool>("GetLocationInfo");
		}

		/// <summary>
		/// 检查手Q/微信是否安装
		/// </summary>
		/// <returns><c>true</c> if this instance is platform installed the specified platform; otherwise, <c>false</c>.</returns>
		/// <param name="platform">游戏传入的平台类型, 可能值为：微信=1， QQ=2</param>
		public bool WGIsPlatformInstalled(int platform)
		{
			return GetUnityWGPlatform().CallStatic<bool>("IsPlatformInstalled", platform);
		}

		/// <summary>
		///  获取手Q/微信版本
		/// </summary>
		/// <returns>The platform APP version.</returns>
		/// <param name="platform">游戏传入的平台类型, 可能值为：微信=1， QQ=2</param>
		public string WGGetPlatformAPPVersion(int platform)
		{
			return GetUnityWGPlatform().CallStatic<string>("GetPlatformAPPVersion", platform);
		}

		/// <summary>
		/// 检查接口在用户安装手Q/微信上是否支持，目前只支持查询4个API接口
		/// </summary>
		/// <returns><c>true</c>, if API support was checked, <c>false</c> otherwise.</returns>
		/// <param name="api">SendToQQWithPhoto = 0， JoinQQGroup = 1， AddGameFriendToQQ = 2， BindQQGroup = 3</param>
		public bool WGCheckApiSupport(int api)
		{
			return GetUnityWGPlatform().CallStatic<bool>("CheckApiSupport", api);	
		}

		/// <summary>
		/// 获取MSDK版本
		/// </summary>
		/// <returns>The version.</returns>
		public string WGGetVersion()
		{
			return GetUnityWGPlatform().CallStatic<string>("GetVersion");
		}

		/// <summary>
		/// 获取安装渠道
		/// </summary>
		/// <returns>The channel identifier.</returns>
		public string WGGetChannelId() {
				return GetUnityWGPlatform().CallStatic<string>("GetChannelId");
		}

		/// <summary>
		/// 获取注册渠道
		/// </summary>
		/// <returns>The register channel identifier.</returns>
		public string WGGetRegisterChannelId() {
				return GetUnityWGPlatform().CallStatic<string>("GetRegisterChannelId");
		}

		/// <summary>
		///    通过外部拉起的URL登陆。该接口用于异帐号场景发生时，用户选择使用外部拉起帐号时调用。
		///	登陆成功后通过onLoginNotify回调
		/// </summary>
		/// <returns><c>true</c>, if user was switched, <c>false</c> otherwise.</returns>
		/// <param name="switchToLaunchUser">为true时表示用户需要切换到拉起帐号，此时该接口会使用上一次保存的拉起帐号登陆数据登陆。登陆成功后通过onLoginNotify回调；
		///															如果没有票据，或票据无效函数将会返回NO，不会发生onLoginNotify回调。
		///															为false时表示用户继续使用原帐号，此时删除保存的拉起帐号数据，避免产生混淆。
		/// </param>
		public  bool WGSwitchUser(bool switchToLaunchUser)
		{
			return GetUnityWGPlatform().CallStatic<bool>("SwitchUser", switchToLaunchUser);			
		}

		/// <summary>
		/// 登出MSDK。返回值已弃用, 全都返回true
		/// </summary>
		public bool WGLogout()
		{
			return GetUnityWGPlatform().CallStatic<bool>("Logout");			
		}

		/// <summary>
		/// Enables the crash report.
		/// </summary>
		/// <param name="bRdmEnable">是否开启RDM的crash异常捕获上报</param>
		/// <param name="bMtaEnable">是否开启MTA的crash异常捕获上报</param>
		public void WGEnableCrashReport(bool bRdmEnable, bool bMtaEnable)
		{
			GetUnityWGPlatform().CallStatic("EnableCrashReport", bRdmEnable, bMtaEnable);			
		}

		/// <summary>
		/// 获取pf, 用于支付, 和pfKey配对使用
		/// </summary>
		/// <returns>The pf.</returns>
		/// <param name="gameCustomInfo">默认可以填空, 部分游戏经分有特殊需求可以通过此自定义字段传入特殊需求数据</param>
		public string WGGetPf(string gameCustomInfo) 
		{
			return GetUnityWGPlatform().CallStatic<string>("GetPf", gameCustomInfo);			
		}

		/// <summary>
		/// 获取pfkey，pfKey由msdk 服务器加密生成，支付过程校验
		/// </summary>
		/// <returns>返回当前pf加密后对应fpKey字符串.</returns>
		public string WGGetPfKey()
		{
			return GetUnityWGPlatform().CallStatic<string>("GetPfKey");		
		}

		/// <summary>
		///  此接口用于已经登录过的游戏, 在用户再次进入游戏时使用, 游戏启动时先调用此接口, 此接口会尝试到后台验证票据
		///  此接口会通过OnLoginNotify将结果回调给游戏, 本接口只会返回两种flag, eFlag_Local_Invalid和eFlag_Succ,
		///  如果本地没有票据或者本地票据验证失败返回的flag为eFlag_Local_Invalid, 游戏接到此flag则引导用户到授权页面授权即可.
		///	如果本地有票据并且验证成功, 则flag为eFlag_Succ, 游戏接到此flag则可以直接使用sdk提供的票据, 无需再次验证.
		///  验证结果通过OnLoginNotify返回
		/// </summary>
		public void WGLoginWithLocalInfo()
		{
			GetUnityWGPlatform().CallStatic("LoginWithLocalInfo");
		}

		/// <summary>
		///  打开AMS营销活动中心
		/// </summary>
		/// <returns><c>true</c>, if ams center was opened, <c>false</c> otherwise.</returns>
		/// <param name="param">可传入附加在URL后的参数，长度限制256.格式为"key1=***&key2=***",注意特殊字符需要urlencode。
		///               不能和MSDK将附加在URL的固定参数重复，列表如下：
		///                参数名            说明	        值
		///                timestamp	       请求的时间戳
		///                appid 	       游戏ID
		///               algorithm	       加密算法标识	v1
		///               msdkEncodeParam  密文
		///                version	       MSDK版本号	    例如1.6.2i
		///                sig	           请求本身的签名
		///               encode	       编码参数	    1
		/// </param>
		public bool WGOpenAmsCenter(string param)
		{
			return GetUnityWGPlatform().CallStatic<bool>("OpenAmsCenter", param);			
		}

		/// <summary>
		/// 用户反馈接口, 反馈内容查看http://mcloud.ied.com/queryLogSystem/ceQuery.html?token=07899ab75c30e499d5b33181c2d8ddc7&gameid=0&projectid=ce
		///  通过OnFeedbackNotify回调反馈接口调用结果
		/// </summary>
		/// <param name="game">游戏名称, 游戏使用自己app的名称即可</param>
		/// <param name="txt">反馈内容</param>
		public bool WGFeedback(string game, string txt)
		{
			return GetUnityWGPlatform().CallStatic<bool>("Feedback", game, txt);			
		}

		/// <summary>
		///  用户反馈接口, 反馈内容查看http://mcloud.ied.com/queryLogSystem/ceQuery.html?token=07899ab75c30e499d5b33181c2d8ddc7&gameid=0&projectid=ce
		///  通过OnFeedbackNotify回调反馈接口调用结果
		/// </summary>
		/// <param name="body">反馈的内容, 内容由游戏自己定义格式, SDK对此没有限制</param>
		public void WGFeedback(string body)
		{
			GetUnityWGPlatform().CallStatic("Feedback", body);			
		}

		/* 应用宝省流量更新 */

		/// <summary>
		/// 设置应用宝省流量更新的全局回调方法,回调的结果通过json字符串表示
		/// </summary>
		/// <param name="OnCheckNeedUpdateInfo">检查应用宝后台是否有更新包的回调, 如果有更新包，则返回新包大小、增量包大小, 游戏结合此接口返回结果和自身业务场景确定是否弹窗提示用户更新</param>
		/// <param name="OnDownloadAppProgressChanged">此回调分为两种情况, 省流量更新(StartSaveUpdate)和普通更新(StartCommonUpdate)
		///	 1. 省流量更新时, 此回调表示应用在应用宝下载页面的的下载进度
		/// 	 2. 普通更新时, 此回调表示下载调用方新包或者patch包的进度
		/// </param>
		/// <param name="OnDownloadAppStateChanged">此回调分为两种情况, 省流量更新(StartSaveUpdate)和普通更新(StartCommonUpdate)
		///	 1. 省流量更新时, 此回调表示应用在应用宝下载页面的的下载状态
		/// 	 2. 普通更新时, 此回调表示下载调用方新包或者patch包的状态.
		///  </param>
		/// <param name="OnDownloadYYBProgressChanged">省流量更新(StartSaveUpdate)，当没有安装应用宝时，会先下载应用宝, 此为下载应用宝包的进度回调
		///  (可选, 游戏可以自行确定是否需要显示下载应用宝的进度, 应用宝下载完成以后会自动拉起系统安装界面)
		/// </param>
		/// <param name="OnDownloadYYBStateChanged">省流量更新(StartSaveUpdate)，当没有安装应用宝时，会先下载应用宝, 此为下载应用宝包的状态变化回调
		/// (可选, 游戏可以自行确定是否需要显示下载应用宝的状态, 应用宝下载完成以后会自动拉起系统安装界面)
		/// </param>

		/// <summary>
		/// 查询结果回调到由SetSaveUpdateObserver接口设置的回调对象的OnCheckNeedUpdateInfo方法
		/// </summary>
		public void WGCheckNeedUpdate()
		{
			GetUnityWGPlatform().CallStatic("CheckNeedUpdate");			
		}

		/// <summary>
		/// 检查应用宝是否安装
		/// return 0:已安装应用宝  1：未安装应用宝  2：安装了低版本的应用宝
		/// </summary>
		public int WGCheckYYBInstalled()
		{
			return GetUnityWGPlatform().CallStatic<int>("CheckYYBInstalled");			
		}

		/// <summary>
		/// 开始省流量更新,下载进度和状态变化会通过OnDownloadAppProgressChanged和OnDownloadAppStateChanged回调给游戏
		/// @para  isUseYYB:是否拉起应用宝更新游戏.
		/// 如果选false，会直接在游戏内完成更新;如果选true,会拉起应用宝更新(此时未安装应用宝则会先下载应用宝);
		/// </summary>
		public void WGStartSaveUpdate(bool isUseYYB)
		{
			GetUnityWGPlatform().CallStatic("StartSaveUpdate", isUseYYB);			
		}

		/// <summary>
		/// 打开暂停位和退出位广告,adType的值为枚举eADType。点击广告按钮会通过OnADNotify回调给游戏,在广告界面按返回键会通过OnADBackPressedNotify回调给游戏
		/// </summary>
		public void WGShowAD(int adType)
		{
			GetUnityWGPlatform().CallStatic("ShowAD", adType);		
		}

		/**
	 * 将微信卡券插入到微信卡包
	 * @param cardId 卡券ID
	 * @param timestamp 计算签名的时间戳
	 * @param sign 签名
	 */
		public void WGAddCardToWXCardPackage(string cardId, string timestamp, string sign)
		{
			GetUnityWGPlatform().CallStatic("WGAddCardToWXCardPackage", cardId, timestamp, sign);		
		}	

		/**
     * 打开微信deeplink（deeplink功能的开通和配置请联系微信侧）
	 * @param link 具体跳转deeplink，可填写为：
	 *             INDEX：跳转微信游戏中心首页
     *             DETAIL：跳转微信游戏中心详情页
     *             LIBRARY：跳转微信游戏中心游戏库
     *             具体跳转的url （需要在微信侧先配置好此url）
     */
		public void WGOpenWeiXinDeeplink(string link) 
		{
			GetUnityWGPlatform().CallStatic("WGOpenWeiXinDeeplink", link);
		}
	}
}
#endif