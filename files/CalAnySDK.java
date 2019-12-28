package com.reign.sdk;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;

import org.cocos2dx.lib.Cocos2dxActivity;
import org.json.JSONObject;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.util.Log;
import android.widget.Toast;

import com.game.framework.IAPWrapper;
import com.game.framework.PluginWrapper;
import com.game.framework.PushWrapper;
import com.game.framework.UserWrapper;
import com.game.framework.java.Game;
import com.game.framework.java.GameIAP;
import com.game.framework.java.GameListener;
import com.game.framework.java.GameParam;
import com.game.framework.java.GamePush;
import com.game.framework.java.GameUser;
import com.game.framework.java.ToolBarPlaceEnum;
import com.game.framework.java.Game;
import com.reign.channel.*;

import com.reign.common.IChannelSdk;
import com.reign.channel.ChannelManager;
//import com.reyun.sdk.ReYun;
//import com.reyun.sdk.ReYun.Gender;
//import com.reyun.sdk.ReYun.PaymentType;

import android.os.Bundle;


public class CalAnySDK implements IChannelSdk {

	private static final String TAG = "[CAL_ANYSDK]";

	private Context context;
	private Activity activity;
	private String payStr;
	private String _reYunOrderId;
	private int _reYunMoney;
	private String _reYunGoodsName;
	private String _reYunLevel;
	private String pushToken;
	/**渠道 参数*
	 * 用常量 不可修改
	 * 如果有回调地址,要特别注意,最终给包的时候要切回正式
	 * 是否开启DEBUG,是否是DEBUG的初始化方法,都写成常量,避免后面出错
	 */
	private final String app_key = "F8B0F0CA-E7C8-B3CB-5DC6-77358A4D1705";
	private final String app_secret = "6a0299e979f775c42d7fd1c795bf3caa";
	private final String private_key = "CF22DD895316330EA51815E12EFD3F6B";
	private final String oauthLoginServer = "http://proxytest.hlw.aoshitang.com/root/anysdkLoginCheck.action";
	/** 是否已初始化 */
	private boolean isInited;
	/** 是否已登陆 这里的登陆指的是SDK登陆,而不是登陆中控 */
	private boolean isLogined;
    private boolean loginCalled = false;
	
	/**
	 * 初始化
	 */
	@Override
	public void initSDK(Activity a, Context c) {
		activity = a;
		context = c;
		activity.runOnUiThread(new Runnable(){
			@Override
			public void run(){
				if(isInited == true){
					Log.i(TAG,"SDK无需重复初始化,跳过初始化");
					return;
				}
				Log.i(TAG,"初始化ANYSDK");
				Game.getInstance().init(activity, app_key, app_secret, private_key, oauthLoginServer);
				//设置用户回调
				initListener();
				GamePush.getInstance().startPush();
			}
		});
	}

	//设置用户回调
	public void initListener(){
		Log.i(TAG,"用户注册事件监听"); 
		GameUser.getInstance().setListener(new GameListener() {
		    @Override
		    public void onCallBack(int arg0, String arg1) {
		    	Log.i(TAG,"~~~~" + arg0 + arg1);
		        switch(arg0)
		        {
		        case UserWrapper.ACTION_RET_INIT_SUCCESS://初始化SDK成功回调
		        	isInited = true;
                    if(loginCalled == true){
                        userLogin();
                    }
		        	
		            break;
		        case UserWrapper.ACTION_RET_INIT_FAIL://初始化SDK失败回调
		        	sendMsgToLua("init",-1);
		            break;
		        case UserWrapper.ACTION_RET_LOGIN_SUCCESS://登陆成功回调
		    		ChannelManager.nativeMessageBegin();
		    		ChannelChannelManager
					Manager.nativeAddString("action", "login");
		    		ChannelManager.nativeAddString("pushToken", pushToken);
		    		ChannelManager.nativeAddString("token", arg1);
		    		ChannelManager.nativeAddInt("state", 1);
		    		ChannelManager.nativeAddString("channelFlag", getChannel());
		    		ChannelManager.nativeMessageEnd();
		    		if(getChannel().equals("and_pengyouwan")){
		    			Log.i(TAG,"弹出悬浮框");
		    			GameParam param = new GameParam(ToolBarPlaceEnum.kToolBarTopRight.getPlace());
		    			GameUser.getInstance().callFunction("showToolBar",param);
		        	}
		    		setLogined(true);
		            break;
		        case UserWrapper.ACTION_RET_LOGIN_TIMEOUT://登陆超时回调
		        	sendMsgToLua("login",-1);
		            break;
		        case UserWrapper.ACTION_RET_LOGIN_NO_NEED://无需登陆回调
		        	Log.i(TAG,"无需登陆回调");
		            break;
		        case UserWrapper.ACTION_RET_LOGIN_CANCEL://登陆取消回调
		        	sendMsgToLua("login",-1);	
		        	
		            break;
		        case UserWrapper.ACTION_RET_LOGIN_FAIL://登陆失败回调
		        	sendMsgToLua("login",-1);
		        	setLogined(false);
		            break;
		        case UserWrapper.ACTION_RET_LOGOUT_SUCCESS://登出成功回调
		        	sendMsgToLua("logout",1);
		        	setLogined(false);
		            break;
		        case UserWrapper.ACTION_RET_LOGOUT_FAIL://登出失败回调
		            break;
		        case UserWrapper.ACTION_RET_PLATFORM_ENTER://平台中心进入回调
		            break;
		        case UserWrapper.ACTION_RET_PLATFORM_BACK://平台中心退出回调
		            break;
		        case UserWrapper.ACTION_RET_PAUSE_PAGE://暂停界面回调
		            break;
		        case UserWrapper.ACTION_RET_EXIT_PAGE://退出游戏回调
		        	activity.finish();
		        	android.os.Process.killProcess(android.os.Process.myPid());
		            break;
		        case UserWrapper.ACTION_RET_ANTIADDICTIONQUERY://防沉迷查询回调
		            break;
		        case UserWrapper.ACTION_RET_REALNAMEREGISTER://实名注册回调
		            break;
		        case UserWrapper.ACTION_RET_ACCOUNTSWITCH_SUCCESS://切换账号成功回调
		            sendMsgToLua("switchSuccss",1);
		    		ChannelManager.nativeMessageBegin();
		    		ChannelManager.nativeAddString("action", "login");
		    		ChannelManager.nativeAddString("pushToken", pushToken);
		    		ChannelManager.nativeAddString("token", arg1);
		    		ChannelManager.nativeAddInt("state", 1);
		    		ChannelManager.nativeAddString("channelFlag", getChannel());
		    		ChannelManager.nativeMessageEnd();
		    		setLogined(true);
		            break;
		        case UserWrapper.ACTION_RET_ACCOUNTSWITCH_FAIL://切换账号失败回调
		            break;
		        case UserWrapper.ACTION_RET_OPENSHOP://应用汇特有回调，接受到该回调调出游戏商店界面
		            break;
		        default:
		            break;
		        }
		    }
		});
		Log.i(TAG,"支付注册事件监听");
		//支付通知
		GameIAP.getInstance().setListener(new GameListener() {
			@Override
			public void onCallBack(int arg0, String arg1) {
				switch (arg0) {
				case IAPWrapper.PAYRESULT_SUCCESS:// 支付成功回调
					System.out.println("支付成功：   " + arg1);
//                	Log.i(TAG,"通知热云支付成功"+_reYunOrderId+PaymentType.APPLE.name()+"CNY"+_reYunMoney/100+ _reYunMoney+_reYunGoodsName+1+Integer.valueOf(_reYunLevel));
//                	ReYun.setPayment (_reYunOrderId, PaymentType.APPLE.name(), "CNY", _reYunMoney/100, _reYunMoney, _reYunGoodsName, 1,Integer.valueOf(_reYunLevel));
					break;
				case IAPWrapper.PAYRESULT_FAIL:// 支付失败回调
					System.out.println("支付失败：   " + arg1);
					break;
				case IAPWrapper.PAYRESULT_CANCEL:// 支付取消回调
					System.out.println("支付取消：   " + arg1);
					break;
				case IAPWrapper.PAYRESULT_NETWORK_ERROR:// 支付超时回调
					System.out.println("支付超时：   " + arg1);
					break;
				case IAPWrapper.PAYRESULT_PRODUCTIONINFOR_INCOMPLETE:// 支付信息提供不完全回调
					System.out.println("支付信息提供不完全：   " + arg1);
					break;
				/**
				 * 新增加:正在进行中回调 支付过程中若SDK没有回调结果，就认为支付正在进行中
				 * 游戏开发商可让玩家去判断是否需要等待，若不等待则进行下一次的支付
				 */
				case IAPWrapper.PAYRESULT_NOW_PAYING:
					System.out.println("支付正在进行：   " + arg1);
					Log.i("充值","重置充值状态");
					GameIAP.getInstance().resetPayState();
					break;
				default:
					break;
				}
			}
		});
		Log.i(TAG,"信鸽注册事件监听");
		//推送通知
		GamePush.getInstance().setListener(new GameListener() {
		    @Override
		    public void onCallBack(int arg0, String arg1) {
		        Log.i("信鸽收到推送",String.valueOf(arg0)+ "***" + arg1);
		        switch (arg0) {
		        case PushWrapper.ACTION_RET_RECEIVEMESSAGE:
		        	Log.i(TAG,"信鸽收到推送回调"); 
		        	break;
		        case PushWrapper.ACTION_RET_PUSHEXTENSION + 1:
		        	Log.i(TAG,"信鸽pushToken" + arg1);
		        	pushToken = arg1;
		        	GamePush.getInstance().callFunction("startPush", new GameParam(arg1));

		        	break;
		        default:
		        	Log.i(TAG,"不对" + arg0);
		        break;
		        }
		    }
		});
	}
	
	public String getChannel(){
		return Game.getInstance().getCustomParam();
	}
	/**
	 * 登陆
	 */
	@Override
	public void userLogin() {
		activity.runOnUiThread(new Runnable() {
			public void run() {
					Log.i(TAG, "SDK开始执行登陆");
                loginCalled = true;
                if(isInited == true){
                    GameUser.getInstance().login();
                }
			}
		});
	}

	/**
	 * 切换账号
	 */
	@Override
	public void userLogout() {
		activity.runOnUiThread(new Runnable(){
			@Override
			public void run() {
				Log.i(TAG, "调用执行注销");
				// 调用SDK执行登陆操作
				sendMsgToLua("logout",1);
				setLogined(false);

			}
			
		});
	}
	
	/**
	 * 隐藏悬浮窗
	 * @param visible
	 */
	public void setFloatMenuVisible(boolean visible) {
		final boolean _visible = visible;
		activity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				Log.i(TAG, "调用显示悬浮框" + String.valueOf(_visible));
				// 调用SDK显示/隐藏悬浮框操作
			}
		});

	}

	@Override
	public void guestLogin() {

	}
	public void sendFlag(Activity a){
		a.runOnUiThread(new Runnable(){
			@Override
			public void run() {
				sendMsgToLua("sendFlag",1);
			}
		});
	}
	/**
	 * 支付
	 */
	@Override
	public void pay(final String userId, final String playerId, final String playerName,
			final String serverId, final String args) {
				activity.runOnUiThread(new Runnable(){
					@Override
					public void run() {
						Log.i(TAG, "调用支付");
						try{
							JSONObject jsonArgs;
							jsonArgs = new JSONObject(args);
		                    String level = jsonArgs.getString("playerLv");
		                    String orderId = jsonArgs.getString("orderId");
		                    String itemId = jsonArgs.getString("itemId");
                            String productId = "";
                            String goodsName = jsonArgs.getString("goodsName");
//                            if(getChannel().equals("and_efun")){
//                                if(goodsName.equals("元宝")){
//                                    productId = String.format("%sCNY",String.valueOf(jsonArgs.getInt("money")/100));
//                                }else if(goodsName.equals("季卡")){
//                                    productId = "seasoncard";
//                                }else if(goodsName.equals("月卡")){
//                                    productId = "monthlycard";
//                                }
//                            }
                            
		                    int money = jsonArgs.getInt("money");
		                    String extraData = String.format("%s-%s-%s-%s-%s", serverId, playerId, orderId, itemId,"anysdk");
		                    //需要什么特定的参数需要去内网ChannelManager代码里面单独传
//							String vip = jsonArgs.getString("vip");
		                    String balance = jsonArgs.getString("balance");

		                	Map<String, String> mProductionInfo = new HashMap<String, String>();
							mProductionInfo.put("Product_Price",
									String.valueOf(money/100));
                            if(getChannel().equals("and_efun") || getChannel().equals("and_zhuan_wanmi")){
                                mProductionInfo.put("Product_Id", jsonArgs.getString("product_str"));
                                
                            }else{
                                mProductionInfo.put("Product_Id", itemId);

                            }
                            if(getChannel().equals("and_meizu")){
                                if(goodsName.equals("元宝")){
                                    mProductionInfo.put("Product_Name", String.valueOf(money/10)+goodsName);
                                }else{
                                    mProductionInfo.put("Product_Name", goodsName);
                                }
                            }else{
                                mProductionInfo.put("Product_Name", goodsName);
                            }
                            if(getChannel().equals("and_dangle")){
                                mProductionInfo.put("Server_Name","001");
                            }
							mProductionInfo.put("Server_Id", serverId);
                                mProductionInfo.put("Product_Count",
                                                    String.valueOf(1));
                            mProductionInfo.put("Coin_Rate","10");
                            if(getChannel().equals("and_51")){ //货币名称，用作描述
                                mProductionInfo.put("Coin_Name",goodsName);
                            }
							mProductionInfo.put("Role_Id", playerId);
							mProductionInfo.put("Role_Name", playerName);
							mProductionInfo.put("Role_Grade", level);
							mProductionInfo.put("Role_Balance", balance);
							mProductionInfo.put("EXT", extraData);
							//购买成功
                        	//通知热云,用户成功支付
                        	//前端报送支付有掉单的风险,收入数据会出现误差,如果要支付数据 准确,可以使用服务器报送支付
                        	//第二个参数是支付类型

							_reYunOrderId = orderId;
						    _reYunMoney = money;
						    _reYunGoodsName = goodsName;
						    _reYunLevel = level;
							Iterator<String> iter = mProductionInfo.keySet().iterator();
							while (iter.hasNext()) {
								String key = iter.next();
								String val = mProductionInfo.get(key);
								System.out.println("hashmap value: " + key + "   "
										+ val);
							}

							ArrayList<String> idArrayList = GameIAP.getInstance()
									.getPluginId();
							if (idArrayList.size() == 1) {
								System.out.println("sdk start pay");
								GameIAP.getInstance().payForProduct(
										idArrayList.get(0), mProductionInfo);
							} else {
								Toast.makeText(activity, "不支持充值",
										Toast.LENGTH_LONG).show();
							}
		                }
		                catch(Exception e){
		                    e.printStackTrace();
		                }
					}
					
				});
	}

	/**
	 * 传参给lua
	 * 
	 * @param action 如"login" "logout"
	 * @param status 非1会进入错误处理
	 */
	public void sendMsgToLua(String action, int status) {
		ChannelManager.nativeMessageBegin();
		ChannelManager.nativeAddString("action", action);
		ChannelManager.nativeAddInt("state", status);
		ChannelManager.nativeAddString("channelFlag", getChannel());
		ChannelManager.nativeMessageEnd();
	}

	@Override
	public Context getContext() {
		// TODO Auto-generated method stub
		return context;
	}
	@Override
	public Activity getActivity() {
		// TODO Auto-generated method stub
		return activity;
	}
	
	@Override
	public void onDestroy() {
		if(activity == null){
			Log.e(TAG, "ACTIVITY == NULL,destroy");
			return;
		}
		activity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				Log.i(TAG, "应用销毁");
				// 应用销毁操作
				 PluginWrapper.onDestroy();
				 Game.getInstance().release();
			}
		});
	}
	@Override
	public void onPause() {
		if(activity == null){
			Log.e(TAG, "ACTIVITY == NULL,pause");
			return;
		}
		activity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				Log.i(TAG, "应用Pause");
				// 应用Pause操作
				PluginWrapper.onPause();
			}
		});
		
	}
	@Override
	public void onRestart() {
		if(activity == null){
			Log.e(TAG, "ACTIVITY == NULL,restart");
			return;
		}
		activity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				Log.i(TAG, "应用Restart操作");
				// 应用Restart操作
				PluginWrapper.onRestart();
			}
		});
		
	}
	@Override
	public void onResume() {
		if(activity == null){
			Log.e(TAG, "ACTIVITY == NULL,resume");
			return;
		}
		if(activity == null){
			Log.e(TAG, "ACTIVITY == NULL,Resume");
		}
		activity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				Log.i(TAG, "应用Resume");
				// 应用Resume操作
				PluginWrapper.onResume();

			}
		});
		
	}
	@Override
	public void onStop() {
		if(activity == null){
			Log.e(TAG, "ACTIVITY == NULL,stop");
			return;
		}
		activity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				Log.i(TAG, "应用Stop");
				// 应用Stop操作
				PluginWrapper.onStop();
			}
		});
		
	}

	public boolean isLogined() {
		return isLogined;
	}

	public void setLogined(boolean _isLogined) {
		Log.i("设置登陆状态",String.valueOf(_isLogined));
		isLogined = _isLogined;
	}

	public void onActivityResult(int requestCode, int resultCode, Intent data) {
	    PluginWrapper.onActivityResult(requestCode, resultCode, data);
	}
	
	public void onNewIntent(Intent intent) {
	    PluginWrapper.onNewIntent(intent);
	}

	@Override
	public void collectInfo(final int scene_id,final String args,final String event){
		activity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				Log.i(TAG,args);
				Log.i(TAG,"准备发送额外信息给热云SDK");
				try{
					JSONObject jsonArgs;
					
					jsonArgs = new JSONObject(args);
					String playerId = jsonArgs.getString("playerId");
					String playerName = jsonArgs.getString("playerName");
		            String level = jsonArgs.getString("level");
		            String serverId = jsonArgs.getString("serverId");
		            String serverName = jsonArgs.getString("serverName");
		            int balance = jsonArgs.getInt("balance");
		            int vip = jsonArgs.getInt("vip");
		            String partyName = jsonArgs.getString("partyName");
		            Log.e("收集数据",""+ playerId+playerName+level+serverId+serverName+balance+vip+partyName);
		            Map<String, String> ppsArgs = new HashMap<String, String>();
		            Map<String, String> papaArgs = new HashMap<String, String>();
		           
					if(event.equals("login")){
						if(getChannel().equals("and_pps")){
							Log.e("pps数据接口","pps");
							 ppsArgs.put("dataType", "1");
							 ppsArgs.put("zoneId",serverId);
							  GameParam param = new GameParam(ppsArgs);
							 GameUser.getInstance().callFunction("submitLoginGameRole",param);
                        }else if(getChannel().equals("and_papa")||getChannel().equals("and_wandoujia")){
                            papaArgs.put("dataType", "1");
                            papaArgs.put("zoneId",serverId);
                            papaArgs.put("roleName",playerName);
                            papaArgs.put("zoneName",serverName);
                            papaArgs.put("roleLevel",level);
                            papaArgs.put("roleId",playerId);
                            GameParam param = new GameParam(papaArgs);
                            GameUser.getInstance().callFunction("submitLoginGameRole",param);
                            Log.i(TAG,"调用数据统计接口"+event);
                        }else if(getChannel().equals("and_57k")){
                            papaArgs.put("dataType", "1");
                            papaArgs.put("zoneId",serverId);
                            papaArgs.put("roleName",playerName);
                            papaArgs.put("zoneName",serverName);
                            papaArgs.put("roleLevel",level);
                            papaArgs.put("roleId",playerId);
                            papaArgs.put("balance", String.valueOf(balance));
                            papaArgs.put("partyName", partyName);
                            papaArgs.put("vipLevel", String.valueOf(vip));
                            papaArgs.put("roleCTime", "-1");
                            papaArgs.put("roleLevelMTime", "-1");
                            GameParam param = new GameParam(papaArgs);
                            GameUser.getInstance().callFunction("submitLoginGameRole",param);
                            Log.i(TAG,"调用数据统计接口"+event);
                        }else if(getChannel().equals("and_51")){
                        	if(!playerName.equals("葫芦娃")){ //初始角色不统计
	                            papaArgs.put("dataType", "1");
	                            papaArgs.put("zoneId",serverId);
	                            papaArgs.put("roleName",playerName);
	                            papaArgs.put("zoneName",serverName);
	                            papaArgs.put("roleLevel",level);
	                            papaArgs.put("roleId",playerId);
	                            GameParam param = new GameParam(papaArgs);
	                            GameUser.getInstance().callFunction("submitLoginGameRole",param);
	                            Log.i(TAG,"调用数据统计接口"+event);
                        	}
                        }
					}else if(event.equals("register")){
						if(getChannel().equals("and_pps")){
							ppsArgs.put("dataType", "2");
							ppsArgs.put("zoneId",serverId);
							 GameParam param = new GameParam(ppsArgs);
							 GameUser.getInstance().callFunction("submitLoginGameRole",param);
							Log.e("pps数据接口","pps2");
                        }else if(getChannel().equals("and_papa")||getChannel().equals("and_wandoujia")){
                            papaArgs.put("dataType", "2");
                            papaArgs.put("zoneId",serverId);
                            papaArgs.put("roleName",playerName);
                            papaArgs.put("zoneName",serverName);
                            papaArgs.put("roleLevel",level);
                            papaArgs.put("roleId",playerId);
                            GameParam param = new GameParam(papaArgs);
                            GameUser.getInstance().callFunction("submitLoginGameRole",param);
                            	Log.e("papa数据接口","papa2");
                            
                        }
					}
		        }
		        catch(Exception e){
		            e.printStackTrace();
		        }
			}
		});
		
	}
	
	public boolean doQuit(){
        GameUser.getInstance().callFunction("exit");
        return true;
	}

    public boolean canDoQuit(){
        return GameUser.getInstance().isFunctionSupported("exit");
    }
}
