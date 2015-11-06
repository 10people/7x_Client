using UnityEngine;
using System.Collections;

public class Common  {
	
	/**
 * @brief 直接支付错误码
 */
public  enum PPPayResultCode{
    /**
     * 购买成功
     */
    PPPayResultCodeSucceed	= 0,

	/**
     * 进入充值并兑换流程
     */
	PPPayResultCodePayAndExchange = 1,
    
    /**
     * 禁止访问
     */
    PPPayResultCodeForbidden = 1001,
    
    /**
     * 该用户不存在
     */
    PPPayResultCodeUserNotExist = 1002,
    
    /**
     * 必选参数丢失
     */
    PPPayResultCodeParamLost = 1003,
    
    /**
     * PP币余额不足
     */
    PPPayResultCodeNotSufficientFunds = 1004,
    
    /**
     * 该游戏数据不存在
     */
    PPPayResultCodeGameDataNotExist = 1005,
    
    /**
     * 开发者数据不存在
     */
    PPPayResultCodeDeveloperNotExist = 1006,
    
    /**
     * 该区数据不存在
     */
    PPPayResultCodeZoneNotExist = 1007,
    
    /**
     * 系统错误
     */
    PPPayResultCodeSystemError = 1008,
    
    /**
     * 购买失败
     */
    PPPayResultCodeFail = 1009,
    
    /**
     * 与开发商服务器通信失败，如果长时间未收到商品请联系PP客服：电话：020-38276673　 QQ：800055602
     */
    PPPayResultCodeCommunicationFail = 1010,
    
    /**
     * 开发商服务器未成功处理该订单，如果长时间未收到商品请联系PP客服：电话：020-38276673　 QQ：800055602
     */
    PPPayResultCodeUntreatedBillNo = 1011,
    
	/**
     * 用户中途取消
     */
	PPPayResultCodeCancel = 1012,
		
	/**
     * 您的账户存在异常，PP币余额无法正常使用，建议您联系PP客服，电话：020-38276673，QQ：800055602
     */
	PPPayResultCodeAccountIsLocked = 1013,
	
    /**
     * 非法访问，可能用户已经下线
     */
    PPPayResultCodeUserOffLine = 999,    
};

}
