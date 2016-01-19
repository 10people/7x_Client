using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using LitJson;
using Msdk;

namespace  Msdk
{
	public class ePlatform
	{
		public const int ePlatform_None = 0; 
		public const int ePlatform_Weixin = 1 ; 
		public const int ePlatform_QQ = 2; 
		public const int ePlatform_WTLogin = 3;
		public const int ePlatform_QQHall = 4;
		public const int ePlatform_Guest = 5;
	}

	public class eFlag
	{
		public const int eFlag_Succ = 0;
		/** QQ&QZone login fail and can't get accesstoken */
		public const int eFlag_QQ_NoAcessToken = 1000; 
		/** QQ&QZone user has cancelled login process */
		public const int eFlag_QQ_UserCancel = 1001; 
		
		/** QQ&QZone login fail (tencentDidNotLogin) */
		public const int eFlag_QQ_LoginFail = 1002; 
		/** QQ&QZone networkErr */
		public const int eFlag_QQ_NetworkErr = 1003; 
		/** QQ is not install */
		public const int eFlag_QQ_NotInstall = 1004;
		/** QQ don't support open api */
		public const int eFlag_QQ_NotSupportApi = 1005; 
		/** QQ&QZone networkErr */
		public const int eFlag_QQ_AccessTokenExpired = 1006; 
		/** pay token 过期 时间  */
		public const int eFlag_QQ_PayTokenExpired = 1007; 
		
		/** Weixin is not installed */
		public const int eFlag_WX_NotInstall = 2000; 
		/** Weixin don't support api */
		public const int eFlag_WX_NotSupportApi = 2001; 
		/** Weixin user has cancelled */
		public const int eFlag_WX_UserCancel = 2002; 
		/** Weixin User has denys */
		public const int eFlag_WX_UserDeny = 2003; 
		public const int eFlag_WX_LoginFail = 2004; 
		/** Weixin 刷新票据成功 */
		public const int eFlag_WX_RefreshTokenSucc = 2005; 
		/** Weixin 刷新票据失败 */
		public const int eFlag_WX_RefreshTokenFail = 2006; 
		/** Weixin accessToken过期, 尝试用refreshtoken刷新票据中 */
		public const int eFlag_WX_AccessTokenExpired = 2007; 
		/** Weixin refresh也过期 */
		public const int eFlag_WX_RefreshTokenExpired = 2008; 
		
		public const int eFlag_Error = -1;
		
		/** 自动登录失败, 需要重新授权, 包含本地票据过期, 刷新失败登所有错误 */
		public const int eFlag_Local_Invalid = -2; 
		
		/** 不在白名单 */
		public const int eFlag_NotInWhiteList = -3; 
		/** 需要引导用户开启定位服务 */
		public const int eFlag_LbsNeedOpenLocationService = -4; 
		/** 定位失败	 */
		public const int eFlag_LbsLocateFail = -5;
		
		/* 快速登陆相关返回值 */
		/**需要进入登陆页 */
		public const int eFlag_NeedLogin = 3001;
		/**使用URL登陆成功 */
		public const int eFlag_UrlLogin = 3002;
		/**需要弹出异帐号提示 */
		public const int eFlag_NeedSelectAccount = 3003; 
		/**通过URL将票据刷新 */
		public const int eFlag_AccountRefresh = 3004; 

		public const int eFlag_Checking_Token = 5001; //添加正在检查token的逻辑

		public const int eFlag_InvalidOnGuest = -7; //该功能在Guest模式下不可使用
		public const int eFlag_Guest_AccessTokenInvalid = 4001; //Guest的票据失效
		public const int eFlag_Guest_LoginFailed = 4002;  //Guest模式登录失败
		public const int eFlag_Guest_RegisterFailed = 4003; //Guest模式注册失败
	}	

	public abstract class CallbackRet
	{
		public int flag = -1;
		public string desc = "";
		public int platform = 0;
		
		/* 以下方法是为了能使用LitJson的自动将Json映射为Object方法 */
		public int _flag 
		{ 
			get{return flag;} 
			set{  flag = value;}
		}
		public string _desc 
		{ 
			get{return desc;} 
			set{  desc = value;}
		}
		public int _platform 
		{ 
			get{return platform;} 
			set{  platform = value;}
		}
		
		public CallbackRet(int platform, int flag, string desc) {
			this.platform = platform;
			this.flag = flag;
			this.desc = desc;
		}
		public CallbackRet ()
		{
		}
		
		public override string ToString(){
			string str = "flag: " + flag + ";";
			str += "desc: " + desc + ";";
			str += "platform: " + platform + ";";
			return str;
		}
	}

	public class eTokenType
	{
		public const int eToken_QQ_Access = 1;
		public const int eToken_QQ_Pay = 2;
		public const int eToken_WX_Access = 3;
		public const int eToken_WX_Code = 4;
		public const int eToken_WX_Refresh = 5;
	}

	public class ePermission {
		public const int eOPEN_NONE = 0;
		public const int eOPEN_PERMISSION_GET_USER_INFO = 0x2;
		public const int eOPEN_PERMISSION_GET_SIMPLE_USER_INFO = 0x4;
		public const int eOPEN_PERMISSION_ADD_ALBUM = 0x8;
		public const int eOPEN_PERMISSION_ADD_IDOL = 0x10;
		public const int eOPEN_PERMISSION_ADD_ONE_BLOG = 0x20;
		public const int eOPEN_PERMISSION_ADD_PIC_T = 0x40;
		public const int eOPEN_PERMISSION_ADD_SHARE = 0x80;
		public const int eOPEN_PERMISSION_ADD_TOPIC = 0x100;
		public const int eOPEN_PERMISSION_CHECK_PAGE_FANS = 0x200;
		public const int eOPEN_PERMISSION_DEL_IDOL = 0x400;
		public const int eOPEN_PERMISSION_DEL_T = 0x800;
		public const int eOPEN_PERMISSION_GET_FANSLIST = 0x1000;
		public const int eOPEN_PERMISSION_GET_IDOLLIST = 0x2000;
		public const int eOPEN_PERMISSION_GET_INFO = 0x4000;
		public const int eOPEN_PERMISSION_GET_OTHER_INFO = 0x8000;
		public const int eOPEN_PERMISSION_GET_REPOST_LIST = 0x10000;
		public const int eOPEN_PERMISSION_LIST_ALBUM = 0x20000;
		public const int eOPEN_PERMISSION_UPLOAD_PIC = 0x40000;
		public const int eOPEN_PERMISSION_GET_VIP_INFO = 0x80000;
		public const int eOPEN_PERMISSION_GET_VIP_RICH_INFO = 0x100000;
		public const int eOPEN_PERMISSION_GET_INTIMATE_FRIENDS_WEIBO = 0x200000;
		public const int eOPEN_PERMISSION_MATCH_NICK_TIPS_WEIBO = 0x400000;
		public const int eOPEN_PERMISSION_GET_APP_FRIENDS = 0x800000;
		public const int eOPEN_ALL = 0xffffff;
	}

	public class eWechatScene
	{
		public const int WechatScene_Session = 0;
		public const int WechatScene_Timeline = 1;
	}

	public class eQQScene
	{
		public const int QQScene_None = 0;
		public const int QQScene_QZone = 1;
		public const int QQScene_Session = 2;
	}

	public class eADType
	{
		public const int Type_Pause = 1; 
		public const int Type_Stop = 2;
	}

	public class eMSG_NOTICETYPE
	{
		public const int eMSG_NOTICETYPE_ALERT = 0; 
		public const int eMSG_NOTICETYPE_SCROLL = 1 ; 
		public const int eMSG_NOTICETYPE_ALL = 2; 				
	}

	public class eMSG_CONTENTTYPE
	{
		public const int eMSG_CONTENTTYPE_TEXT = 0; 	//文本公告
		public const int eMSG_CONTENTTYPE_IMAGE = 1 ; 	//图片公告
		public const int eMSG_CONTENTTYPE_WEB = 2; 		//网页公告		
	}

	public class eMSDK_SCREENDIR
	{
		public const int eMSDK_SCREENDIR_SENSOR = 0; 	//横竖屏 
		public const int eMSDK_SCREENDIR_PORTRAIT = 1 ; 	//竖屏
		public const int eMSDK_SCREENDIR_LANDSCAPE = 2; 		//横屏		
	}

	public class eApiName
	{
		public const int eApiName_WGSendToQQWithPhoto = 0;
		public const int eApiName_WGJoinQQGroup = 1 ;
		public const int eApiName_WGAddGameFriendToQQ = 2;	
		public const int eApiName_WGBindQQGroup = 3;
	}

	public class TMSelfUpdateSDKUpdateInfo
	{
		public const int STATUS_OK = 0; 
		public const int STATUS_CHECKUPDATE_FAILURE = 1; 
		public const int STATUS_CHECKUPDATE_RESPONSE_IS_NULL = 2;
		
		public const int UpdateMethod_NoUpdate = 0; 
		public const int UpdateMethod_Normal = 1; 
		public const int UpdateMethod_ByPatch = 2; 
	}
	
	public class TMAssistantDownloadSDKTaskState
	{
		public const int DownloadSDKTaskState_WAITING = 1;
		public const int DownloadSDKTaskState_DOWNLOADING = 2;
		public const int DownloadSDKTaskState_SUCCEED = 4;
		public const int DownloadSDKTaskState_PAUSED = 3;
		public const int DownloadSDKTaskState_FAILED = 5;
		public const int DownloadSDKTaskState_DELETE = 6;
	}

	public class TMSelfUpdateTaskState {
		public const int SelfUpdateSDKTaskState_SUCCESS  = 0;
		public const int SelfUpdateSDKTaskState_DOWNLOADING  = 1;
		public const int SelfUpdateSDKTaskState_FAILURE  = 2;
	}

	public class TMYYBInstallState {
		public const int LOWWER_VERSION_INSTALLED    = 2;
		public const int UN_INSTALLED    = 1;
		public const int ALREADY_INSTALLED   = 0;
	}

	public class TMAssistantDownloadErrorCode {
		public const int DownloadSDKErrorCode_CLIENT_PROTOCOL_EXCEPTION  = 732;
		public const int DownloadSDKErrorCode_CONNECT_TIMEOUT    = 601;
		public const int DownloadSDKErrorCode_HTTP_LOCATION_HEADER_IS_NULL   = 702;
		public const int DownloadSDKErrorCode_INTERRUPTED    = 600;
		public const int DownloadSDKErrorCode_IO_EXCEPTION   = 606;
		public const int DownloadSDKErrorCode_NONE   = 0;
		public const int DownloadSDKErrorCode_PARSER_CONTENT_FAILED  = 704;
		public const int DownloadSDKErrorCode_RANGE_NOT_MATCH    = 706;
		public const int DownloadSDKErrorCode_REDIRECT_TOO_MANY_TIMES    = 709;
		public const int DownloadSDKErrorCode_RESPONSE_IS_NULL   = 701;
		public const int DownloadSDKErrorCode_SET_RANGE_FAILED   = 707;
		public const int DownloadSDKErrorCode_SOCKET_EXCEPTION   = 605;
		public const int DownloadSDKErrorCode_SOCKET_TIMEOUT = 602;
		public const int DownloadSDKErrorCode_SPACE_NOT_ENOUGH   = 730;
		public const int DownloadSDKErrorCode_TOTAL_SIZE_NOT_SAME    = 705;
		public const int DownloadSDKErrorCode_UNKNOWN_EXCEPTION  = 604;
		public const int DownloadSDKErrorCode_UNKNOWN_HOST   = 603;
		public const int DownloadSDKErrorCode_URL_EMPTY  = 731;
		public const int DownloadSDKErrorCode_URL_HOOK   = 708;
		public const int DownloadSDKErrorCode_URL_INVALID    = 700;
		public const int DownloadSDKErrorCode_WRITE_FILE_FAILED  = 703;
		public const int DownloadSDKErrorCode_WRITE_FILE_NO_ENOUGH_SPACE = 710;
		public const int DownloadSDKErrorCode_WRITE_FILE_SDCARD_EXCEPTION    = 711;
	}

	public class TokenRet
	{
		public int type = 0;
		public string value = "";
		public long expiration = 0;
		
		public int _type 
		{ 
			get{return type;} 
			set{ type = value;}
		}
		public string _value 
		{ 
			get{return value;} 
			set{  this.value = value;}
		}
		public string _expiration
		{ 
			get{return expiration.ToString();} 
			set{  expiration = long.Parse(value);}
		}
		public TokenRet() {
			
		}
		
		public TokenRet(int type, string value, long expiration) : base(){				
			this.type = type;
			this.value = value;
			this.expiration = expiration;
		}
	}

	public class LoginRet: CallbackRet
	{
		public string open_id = "";                       
		public string user_id = "";
		public string pf = "";
		public string pf_key = "";
		public List<TokenRet> token = new List<TokenRet>();
		
		public string _open_id 
		{ 
			get{return open_id;} 
			set{  open_id = value;}
		}
		public string _user_id
		{ 
			get{return user_id;} 
			set{  user_id = value;}
		}
		public string _pf 
		{ 
			get{return pf;} 
			set{  pf = value;}
		}
		public string _pf_key 
		{ 
			get{return pf_key;} 
			set{  pf_key = value;}
		}            
		                        
		public List<TokenRet> _token
		{ 
			get{return token;} 
			set{  token = value;}
		}   

		public override System.String ToString ()
		{
			return ToLog ();
		}

		public string ToLog() 
		{
			StringBuilder msg = new StringBuilder();
			msg.AppendLine("***********************LoginInfo***********************");
			msg.AppendLine("desc: " + this.desc);
			msg.AppendLine("open_id: " + this.open_id);
			msg.AppendLine("pf: " + this.pf);
			msg.AppendLine("pf_key: " + this.pf_key);
			msg.AppendLine("user_id: " + this.user_id);
			msg.AppendLine("flag: " + "" + this.flag);
			msg.AppendLine("platform: " + "" + this.platform);
			for (int i = 0; i < this.token.Count; i++) {
				string type = "";
				switch (this.token[i].type) {
				case eTokenType.eToken_QQ_Access:
					type = "eToken_QQ_Access";
					break;
				case eTokenType.eToken_QQ_Pay:
					type = "eToken_QQ_Pay";
					break;
				case eTokenType.eToken_WX_Access:
					type = "eToken_WX_Access";
					break;
				case eTokenType.eToken_WX_Code:
					type = "eToken_WX_Code";
					break;
				case eTokenType.eToken_WX_Refresh:
					type = "eToken_WX_Refresh";
					break;
				default:
					type = "errorType";
					break;
				}
				msg.AppendLine(type + ":" + this.token[i].value);
				msg.AppendLine(type + ":" + this.token[i].expiration);
			}
			msg.AppendLine("***********************LoginInfo***********************");
			Debug.Log(msg.ToString());
			return msg.ToString ();
		}
		
		public string GetTokenByType(int type) {
			for (int i = 0; i < this.token.Count; i++) {
				if (this.token[i].type == type) {
					return this.token[i].value;
				}
			}
			return "";
		}
		
		public string GetAccessToken() {
			int plat = this.platform;
			string accessToken = "";
			if (plat == ePlatform.ePlatform_QQ || plat == ePlatform.ePlatform_QQHall) {
				accessToken = this.GetTokenByType(eTokenType.eToken_QQ_Access);
			} else if (plat == ePlatform.ePlatform_Weixin) {
				accessToken = this.GetTokenByType(eTokenType.eToken_WX_Access);
			}
			return accessToken;
			
		}
		public long GetTokenExpireByType(int type) {
			for (int i = 0; i < this.token.Count; i++) {
				if (this.token[i].type == type) {
					return this.token[i].expiration;
				}
			}
			return 0;
		}
		/// <summary>
		/// 解析json串，返回一个LoginRet，若解析失败返回null.
		/// </summary>
		/// <returns>The json.</returns>
		/// <param name="json">Json.</param>
		public static LoginRet ParseJson(string json){                
			try{                                                   
				LoginRet ret = JsonMapper.ToObject<LoginRet>(json);                    
				return ret;                                  
			}catch (Exception ex)
			{
				Debug.Log("errro:"+ex.Message+"\n"+ex.StackTrace);
			}
			return null;
			
		}    
		
		public LoginRet ()
		{
		}		
	}

	public class KVPair
	{
		public string key = "";
		public string value = "";
		
		public string _key
		{ 
			get{return key;} 
			set{ key = value;}
		}
		public string _value
		{ 
			get{return value;} 
			set{ this.value = value;}
		}
	}

	public class WakeupRet: CallbackRet
	{       
		/** 传递的openid */
		public string open_id = ""; 
		/** 对应微信消息中的mediaTagName */
		public string media_tag_name = ""; 
		/** 扩展消息,唤醒游戏时带给游戏 */
		public string messageExt = "";  
		/** 语言     目前只有微信5.1以上用，手Q不用 */
		public string lang = "";            
		/** 国家     目前只有微信5.1以上用，手Q不用 */
		public string country = "";     
		public List<KVPair> extInfo = new List<KVPair>();
		
		public string _open_Id
		{ 
			get{return open_id;} 
			set{ open_id = value;}
		}
		public string _media_tag_name 
		{ 
			get{return media_tag_name ;} 
			set{ media_tag_name  = value;}
		}
		public string _messageExt
		{ 
			get{return messageExt;} 
			set{ messageExt = value;}
		}
		public string _lang
		{ 
			get{return lang;} 
			set{ lang = value;}
		}
		public string _country
		{ 
			get{return country;} 
			set{ country = value;}
		}                               
		public override string ToString()
		{
			string str = base.ToString();
			str += "open_id: " + open_id + ";";
			str += "media_tag_name: " + media_tag_name + ";";
			return str;
		}              

		public List<KVPair> _extInfo
		{ 
			get{return extInfo;} 
			set{  extInfo = value;}
		}			
		
		public WakeupRet ()
		{
		}
		
		/// <summary>
		/// 解析json串，返回一个 WakeupRet，若解析失败返回null.
		/// </summary>
		/// <returns>The json.</returns>
		/// <param name="json">Json.</param>
		public static  WakeupRet ParseJson(string json)
		{                
			try{                                
				WakeupRet ret = JsonMapper.ToObject<WakeupRet>(json);                    
				return ret;  
			}catch (Exception ex)
			{
				Debug.Log("errro:"+ex.Message+"\n"+ex.StackTrace);
			}
			return null;
		}  				
	}

	public class ShareRet: CallbackRet
	{
		public string extInfo = "";

		public string _extInfo
		{ 
			get{return extInfo;} 
			set{ extInfo = value;}
		}
		public ShareRet ()
		{
		}

		public static ShareRet ParseJson(string json){                
			try{                   
				ShareRet ret = JsonMapper.ToObject<ShareRet>(json);                    
				return ret;                    
			}catch (Exception ex)
			{
				Debug.Log("errro:"+ex.Message+"\n"+ex.StackTrace);
			}
			return null;
		}            

		public override string ToString()
		{
			return base.ToString() 
				+ "; extInfo: " + extInfo;
		}
	}

	public class CardRet: CallbackRet
	{
		public String open_id = ""; // 传递的openid
		public String wx_card_list = ""; // 对应微信消息中的mediaTagName
		public List<KVPair> extInfo = new List<KVPair>();// 存放所有平台过来信息。

		public string _open_Id
		{ 
			get{return open_id;} 
			set{ open_id = value;}
		}
		public string _wx_card_list
		{ 
			get{return wx_card_list;} 
			set{ wx_card_list = value;}
		}
		public List<KVPair> _extInfo
		{ 
			get{return extInfo;} 
			set{  extInfo = value;}
		}

		public static CardRet ParseJson(string json){                
			try{                   
				CardRet ret = JsonMapper.ToObject<CardRet>(json);                    
				return ret;    
			}catch (Exception ex)
			{
				Debug.Log("errro:"+ex.Message+"\n"+ex.StackTrace);
			}
			return null;
		} 
		
		public override string ToString() {
			String str = "";
			if (this != null) {
				str = base.ToString();
				str = str + "open_id: " + this.open_id + ";";
				str = str + "wx_card_list: " + this.wx_card_list + ";";
				
				if (this.extInfo != null) {
					foreach (KVPair e in extInfo) {
						str = str + "key:" + e.key + "value:" +e.value;
					}
				} else {
					str = "no GroupInfo";
				}
			} 
			
			return str;
		}
	}

	public class ADRet
	{
		public string viewTag = "";
		public int scene;
		
		public ADRet ()
		{
		}
		
		public string _viewTag 
		{ 
			get{return viewTag;} 
			set{  viewTag = value;}
		}
		
		public int _scene 
		{ 
			get{return scene;} 
			set{  scene = value;}
		}
		
		public override System.String ToString ()
		{
			StringBuilder msg = new StringBuilder();
			msg.AppendLine("***********************LoginInfo***********************");
			msg.AppendLine("viewTag: " + this.viewTag);
			msg.AppendLine("scene: " + this.scene);
			msg.AppendLine("***********************LoginInfo***********************");
			Debug.Log(msg.ToString());
			return msg.ToString ();
		}
		
		/// <summary>
		/// 解析json串，返回一个ADRet，若解析失败返回null.
		/// </summary>
		/// <returns>The json.</returns>
		/// <param name="json">Json.</param>
		public static ADRet ParseJson(string json){                
			try{                                                   
				ADRet ret = JsonMapper.ToObject<ADRet>(json);                    
				return ret;                                  
			}catch (Exception ex)
			{
				Debug.Log("errro:"+ex.Message+"\n"+ex.StackTrace);
			}
			return null;		
		}  
		
	}

	public class PersonInfo
	{
		public string nickName = "";
		public string openId = "";
		public string gender = "";
		public string pictureSmall = "";
		public string pictureMiddle = "";
		public string pictureLarge = "";
		public string province = "";
		public string city = "";
		public string gpsCity = "";
		
		public float distance = 0;
		public bool isFriend = false;
		public long timestamp = 0;
		
		public string lang = "";
		public string country = "";
		
		public string _nickName
		{ 
			get{return nickName;} 
			set{  nickName = value;}
		}
		public string _openId
		{ 
			get{return openId;} 
			set{  openId = value;}
		}
		public string _gender
		{ 
			get{return gender;} 
			set{  gender = value;}
		}
		public string _pictureSmall
		{ 
			get{return pictureSmall;} 
			set{  pictureSmall = value;}
		}
		public string _pictureMiddle
		{ 
			get{return pictureMiddle;} 
			set{  pictureMiddle = value;}
		}
		public string _pictureLarge
		{ 
			get{return pictureLarge;} 
			set{  pictureLarge = value;}
		}
		public string _province
		{ 
			get{return province;} 
			set{  province = value;}
		}
		public string _city
		{ 
			get{return city;} 
			set{  city = value;}
		}
		public string _gpsCity
		{ 
			get{return gpsCity;} 
			set{  gpsCity = value;}
		}
		public string _distance
		{ 
			get{return distance.ToString();} 
			set{  distance = float.Parse(value);}
		}
		public bool _isFriend
		{ 
			get{return isFriend;} 
			set{  isFriend = value;}
		}
		public string _timestamp
		{ 
			get{return timestamp.ToString();} 
			set{  timestamp = long.Parse(value);}
		}
		public string _lang
		{ 
			get{return lang;} 
			set{  lang = value;}
		}
		public string _country
		{ 
			get{return country;} 
			set{  country = value;}
		}
		public PersonInfo() {
			
		}
		
		public PersonInfo(string nickName, string openId, string gender,
		                  string pictureSmall, string pictureMiddle, string pictureLarge,
		                  string provice, string city): base()
		{
			this.nickName = nickName;
			this.openId = openId;
			this.gender = gender;
			this.pictureSmall = pictureSmall;
			this.pictureMiddle = pictureMiddle;
			this.pictureLarge = pictureLarge;
			this.province = provice;
			this.city = city;
		}
		
		public PersonInfo(string nickName, string openId, string gender,
		                  string pictureSmall, string pictureMiddle, string pictureLarge,
		                  string provice, string city, string gpsCity, string lang, string country) : base()
		{
			this.nickName = nickName;
			this.openId = openId;
			this.gender = gender;
			this.pictureSmall = pictureSmall;
			this.pictureMiddle = pictureMiddle;
			this.pictureLarge = pictureLarge;
			this.province = provice;
			this.city = city;
			this.gpsCity = gpsCity;
			this.lang = lang;
			this.country = country;
		}
		
		public PersonInfo(string nickName, string openId, string gender,
		                  string pictureSmall, string pictureMiddle, string pictureLarge,
		                  string province, string city, float distance, bool isFriend,
		                  long timestamp) : base()
		{
			this.nickName = nickName;
			this.openId = openId;
			this.gender = gender;
			this.pictureSmall = pictureSmall;
			this.pictureMiddle = pictureMiddle;
			this.pictureLarge = pictureLarge;
			this.province = province;
			this.city = city;
			this.distance = distance;
			this.isFriend = isFriend;
			this.timestamp = timestamp;
		}
		
		public PersonInfo(string nickName, string openId, string gender,
		                  string pictureSmall, string pictureMiddle, string pictureLarge,
		                  string province, string city, string lang, string country):base(){		
			this.nickName = nickName;
			this.openId = openId;
			this.gender = gender;
			this.pictureSmall = pictureSmall;
			this.pictureMiddle = pictureMiddle;
			this.pictureLarge = pictureLarge;
			this.province = province;
			this.city = city;
			this.lang = lang;
			this.country = country;
		}
		
		public override string ToString() {
			string str = "";
			str += "nickName: " + nickName + "; ";
			str += "openId: " + openId + "; ";
			str += "gender: " + gender + "; ";
			str += "pictureSmall: " + pictureSmall + "; ";
			str += "pictureMiddle: " + pictureMiddle + "; ";
			str += "pictureLarge: " + pictureLarge + "; ";
			str += "provice: " + province + "; ";
			str += "city: " + city + "; ";
			str += "distance: " + distance + "; ";
			str += "isFriend: " + isFriend + "; ";
			str += "timestamp: " + timestamp + "; ";
			str += "lang: " + lang + "; ";
			str += "country: " + country + "; ";
			return str;
		}
	}

	public class RelationRet : CallbackRet
	{
		public List<PersonInfo> persons = new List<PersonInfo>();
		public List<PersonInfo> _persons { 
			get{return persons;} 
			set{ persons = value;}
		}
		/// <summary>
		/// 解析json串，返回一个RelationRet，若解析失败返回null.
		/// </summary>
		/// <returns>The json.</returns>
		/// <param name="json">Json.</param>
		public static RelationRet ParseJson(string json){                
			try{                   
				RelationRet ret = JsonMapper.ToObject<RelationRet>(json);                    
				return ret;    
			}catch (Exception ex)
			{
				Debug.Log("errro:"+ex.Message+"\n"+ex.StackTrace);
			}
			return null;
		}               
		public override string ToString() {
			String str = "";
			if (this != null && this.persons != null) {
				str = base.ToString();
				str = str + "friends(num): " + this.persons.Count + ";";
			} else {
				str = "friends(num): 0;";
			}
			return str;
		}
		
		public RelationRet(){
		}				
	}

	public class LocationRet : CallbackRet
	{
		public double longitude;
		public double latitude;
		
		public double _longitude
		{ 
			get{return longitude;} 
			set{  longitude = value;}
		}
		public double _latitude
		{ 
			get{return latitude;} 
			set{ latitude = value;}
		}
		public LocationRet ()
		{
		}

		public static LocationRet ParseJson(string json){                
			try{                   
				LocationRet ret = JsonMapper.ToObject<LocationRet>(json);                    
				return ret;                    
			}catch (Exception ex)
			{
				Debug.Log("errro:"+ex.Message+"\n"+ex.StackTrace);
			}
			return null;
		}                           

		public override string ToString()
		{
			return base.ToString() 
				+ "; longitude: " + longitude
					+ "; latitude: " + latitude;
		}
	}

	public class PicInfo
	{
		public string picPath;
		public int screenDir;
		public string hashValue;	
		
		public string _picPath 
		{ 
			get{return picPath;} 
			set{picPath = value;}
		}
		
		public string _hashValue 
		{ 
			get{return hashValue;} 
			set{hashValue = value;}
		}
		
		public int _screenDir
		{ 
			get{return screenDir;} 
			set{screenDir = value;}
		}
		
		public PicInfo ()
		{
			screenDir = eMSDK_SCREENDIR.eMSDK_SCREENDIR_SENSOR;
		}
		
		public override string ToString()
		{
			string screen = "";
			if (screenDir == eMSDK_SCREENDIR.eMSDK_SCREENDIR_LANDSCAPE) {
				screen = "LANDSCAPE";
			} else if (screenDir == eMSDK_SCREENDIR.eMSDK_SCREENDIR_PORTRAIT) {
				screen = "PORTRAIT";
			} else {
				screen = "SENSOR";
			}
			return "PicUrl:" + picPath
				+ "; ScreenDir: " + screen
					+ "; PicHash: " + hashValue;
		}
	}

	public class NoticeInfo
	{									
		public string msg_id; // 公告id           
		//public string mAppId ; // appid           
		public string open_id; // 用户open_id           
		public string msg_url ; // 公告跳转链接            
		public int msg_type ; // 公告展示类型，滚动弹出           
		public string msg_scene ; // 公告展示的场景，管理端后台配置           
		public string start_time; // 公告有效期开始时间           
		public string end_time ; // 公告有效期结束时间           
		public string update_time ; // 公告更新时间            
		public int content_type;// 公告内容类型，eMSG_NOTICETYPE            
		
		//文本公告特殊字段
		public string msg_title ; // 公告标题            
		public string msg_content;// 公告内容           
		
		//网页公告特殊字段
		public string content_url; 
		
		public List<PicInfo> picArray = new List<PicInfo>();
		
		/* 以下方法是为了能使用LitJson的自动将Json映射为Object方法 */
		public string _update_time 
		{ 
			get{return update_time;} 
			set{  update_time = value;}
		}
		public string _msg_title
		{ 
			get{return msg_title;} 
			set{ msg_title = value;}
		}
		public int _content_type
		{ 
			get{return content_type;} 
			set{ content_type = value;}
		}
		public string _end_time
		{ 
			get{return end_time;} 
			set{  end_time = value;}
		}
		public string _start_time 
		{ 
			get{return start_time;} 
			set{  start_time = value;}
		}
		public string _msg_scene
		{ 
			get{return msg_scene;} 
			set{msg_scene = value;}
		}
		public int _msg_type 
		{ 
			get{return msg_type;} 
			set{ msg_type = value;}
		}
		public string _msg_url 
		{ 
			get{return msg_url;} 
			set{msg_url = value;}
		}
		public string _content_url
		{ 
			get{return content_url;} 
			set{ content_url = value;}
		}
		public string _msg_id 
		{ 
			get{return msg_id;} 
			set{msg_id = value;}
		}
		public string _open_id
		{ 
			get{return open_id;} 
			set{open_id = value;}
		}
		public string _msg_content
		{ 
			get{return msg_content;} 
			set{ msg_content = value;}
		}
		public List<PicInfo> _picArray
		{ 
			get{return picArray;} 
			set{ picArray = value;}
		}
		
		public NoticeInfo list{ get; set;}
		
		
		public NoticeInfo ()
		{
		}
		
		public override string ToString()
		{
			string noticePics = "";
			if (picArray.Count > 0) {
				for(int i = 0; i < picArray.Count; i++){
					noticePics += picArray[i].ToString() + ";";
				}
			}
			return "NoticeId:" + msg_id
				+ "NoticeContentType:" + content_type
					+ "; NoticeTitle: " + msg_title
					+ "; NoticeContent: " + msg_content
					+ "; NoticePic: " + noticePics;
			
		}
		
		
		public static  List<NoticeInfo> ParseJson(string json){
			Debug.Log ("len:"+json.Length+"NoticeInfo:" + json);
			try{
				JsonData datas = JsonMapper.ToObject(json); 
				if(datas["_notice_list"].IsArray){
					NoticeInfoList noticeList = JsonMapper.ToObject<NoticeInfoList>(json);
					return noticeList.list;
				}
			}catch (Exception ex)
			{
				Debug.Log("errro:"+ex.Message+"\n"+ex.StackTrace);
			}
			return null;
		}               		
	}

	/// <summary>
	/// 方便解析json的类
	/// </summary>
	public class NoticeInfoList
	{
		public List<NoticeInfo> _notice_list
		{
			get{return list;} 
			set{list = value;}
		}

		public List<NoticeInfo> list = new List<NoticeInfo>();

		public NoticeInfoList ()
		{
		}
	}

	public class QQGroupInfo
	{
		public string groupName = ""; //群名称//
		public string fingerMemo = "";//群的相关简介//
		public string memberNum = ""; //群成员数//
		public string maxNum = ""; //该群可容纳的最多成员数//
		public string ownerOpenid = ""; //群主openid//
		public string unionid = ""; //与该QQ群绑定的公会ID//
		public string zoneid = ""; //大区ID//
		public string adminOpenids = ""; //管理员openid。如果管理员有多个的话，用“,”隔开，例如0000000000000000000000002329FBEF,0000000000000000000000002329FAFF//
		
		public string groupOpenid = "";  //和游戏公会ID绑定的QQ群的groupOpenid//
		public string groupKey = "";   //需要添加的QQ群对应的key//

		public string _groupName
		{ 
			get{return groupName;} 
			set{ groupName = value;}
		}
		public string _fingerMemo
		{ 
			get{return fingerMemo;} 
			set{ fingerMemo = value;}
		}
		public string _memberNum
		{ 
			get{return memberNum;} 
			set{ memberNum = value;}
		}
		public string _maxNum
		{ 
			get{return maxNum;} 
			set{ maxNum = value;}
		}
		public string _ownerOpenid
		{ 
			get{return ownerOpenid;} 
			set{ ownerOpenid = value;}
		}
		public string _unionid
		{ 
			get{return unionid;} 
			set{ unionid = value;}
		}
		public string _adminOpenids
		{ 
			get{return adminOpenids;} 
			set{ adminOpenids = value;}
		}
		public string _groupOpenid
		{ 
			get{return groupOpenid;} 
			set{ groupOpenid = value;}
		}
		public string _groupKey
		{ 
			get{return groupKey;} 
			set{ groupKey = value;}
		}

		public QQGroupInfo() {
			
		}
		
		public QQGroupInfo( string groupName, string fingerMemo, string memberNum,
		                   string maxNum, string ownerOpenid, string unionid, string zoneid, string adminOpenids, 
		                   string groupOpenid, string groupKey):base(){			
			this.groupName = groupName;
			this.fingerMemo = fingerMemo;
			this.memberNum = memberNum;
			this.maxNum = maxNum;
			this.ownerOpenid = ownerOpenid;
			this.unionid = unionid;
			this.zoneid = zoneid;
			this.adminOpenids = adminOpenids;
			this.groupOpenid = groupOpenid;
			this.groupKey = groupKey;
		}
		
		public override string ToString() {
			string str = "";
			str += "groupName: " + groupName + "; ";
			str += "fingerMemo: " + fingerMemo + "; ";
			str += "memberNum: " + memberNum + "; ";
			str += "maxNum: " + maxNum + "; ";
			str += "ownerOpenid: " + ownerOpenid + "; ";
			str += "unionid: " + unionid + "; ";
			str += "zoneid: " + zoneid + "; ";
			str += "adminOpenids: " + adminOpenids + "; ";
			str += "groupOpenid: " + groupOpenid + "; ";
			str += "groupKey: " + groupKey + "; ";
			
			return str;
		}
	}

	public class GroupRet: CallbackRet
	{
		public int errorCode = 0;
		public QQGroupInfo mGroupInfo = new QQGroupInfo();

		public int _errorCode
		{
			get{return errorCode;}
			set{errorCode = value;}
		}
		public QQGroupInfo _mGroupInfo
		{ 
			get{return mGroupInfo;} 
			set{ mGroupInfo = value;}
		}
		
		public static GroupRet ParseJson(string json){                
			try{                   
				GroupRet ret = JsonMapper.ToObject<GroupRet>(json);                    
				return ret;    
			}catch (Exception ex)
			{
				Debug.Log("errro:"+ex.Message+"\n"+ex.StackTrace);
			}
			return null;
		} 
		
		public override string ToString() {
			String str = "";
			if (this != null) {
				str = base.ToString();
				str = str + "errorCode: " + this.errorCode + ";";
				
				if (this.mGroupInfo != null) {
					str = str + "mGroupInfo: " + this.mGroupInfo.ToString() + ";";
				} else {
					str = "no GroupInfo";
				}
			} 
			
			return str;
		}
	}

}
