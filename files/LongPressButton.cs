using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using LuaInterface;
using UnityEngine.UI;

public class LongPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler 
{
    private bool isDown = false;
    private bool isUp = false;
    private bool isShowTips = false;
    private float lastUpBtnTime = 0f;
    private float duration = 0.3f;
    private LuaFunction clickFunc;
    private LuaFunction longPressFunc;
    private LuaFunction endFunc;

    private LuaFunction longUpdateFunc;
	public void OnPointerDown (PointerEventData pointerEventData) {
        //Debug.Log("onPointerDown1111");
        // 按下了按钮
        isDown = true;
	}
	
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        //Debug.Log("onPointerUp1111");
        // 抬起了按钮
        //isUp = true;
    }

    public void AddCallback(GameObject go, LuaFunction func1, LuaFunction func2, LuaFunction func3, LuaFunction longUpdateFunc)
    {
        // 按钮点击
        clickFunc = func1;
        // 按钮长按
        longPressFunc = func2;
        // 按钮长按取消
        endFunc = func3;

        this.longUpdateFunc = longUpdateFunc;
    }

    void Update()
    {
        if (isDown)
        {
            if (Input.GetMouseButtonUp(0)){
                isUp = true;
            }
            //Debug.Log("LongPressButton Update");
            if (isUp && Time.time - lastUpBtnTime <= duration)
            {
                //Debug.Log("点击了按钮" + isUp + (Time.time - lastUpBtnTime));
                clickFunc.Call();
                isDown = false;
                isUp = false;
            }
            else if (isShowTips == false && Time.time - lastUpBtnTime > duration)
            {
                isShowTips = true;
                //Debug.Log("长按了按钮" + (Time.time - lastUpBtnTime));
                longPressFunc.Call();
            }
            else
            {
               
                //Debug.Log("按着按钮没松开" + (Time.time - lastUpBtnTime));
                if (null != longUpdateFunc)
                {
                    if (Time.time - lastUpBtnTime > duration * 2)
                    {
                        longUpdateFunc.Call();   
                    }
                   
                }
            }

            if (isShowTips && isUp)
            {
                //Debug.Log("松开了长按按钮" + (Time.time - lastUpBtnTime));
                endFunc.Call();
                isDown = false;
                isUp = false;
                isShowTips = false;
            }
        }
        else
        {
            lastUpBtnTime = Time.time;
        }
    }
}
