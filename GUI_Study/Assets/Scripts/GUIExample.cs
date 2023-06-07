using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIExample : MonoBehaviour
{
    public GUIStyle style;
    public GUISkin skin;
    private string _textFieldString = "text field";

    private string _textAreaString = "text area";

    private bool _toggleBool = true;

    private int _toolBarIndex = 0;
    private string[] _toolBarNames = { "A", "B", "C" };

    private int _selectionGridIndex = 0;
    private string[] _selectionGridNames = { "A", "B", "C", "D" };

    private float _sliderValue = 0.0f;

    private Vector2 _scrollViewVector = Vector2.zero;
    private string _innerText = "i am inside the scroll view";

    private Rect _windowRect = new Rect(125, 250, 100, 100);

    private void OnGUI()
    {
        // label 仅显示信息
        GUI.Label(new Rect(25, 0, 100, 30), "Label");

        // button 按钮交互
        if (GUI.Button(new Rect(25, 50, 100, 30), "Button"))
        {
            // 点击按钮的逻辑
        }

        // textfield 单行文本输入框
        _textFieldString = GUI.TextField(new Rect(25, 100, 100, 30), _textFieldString);

        // textarea 多行文本输入框
        _textAreaString = GUI.TextArea(new Rect(25, 150, 100, 30), _textAreaString);

        // toggle 具有开/关状态的复选框
        if (GUI.Toggle(new Rect(25, 200, 100, 30), _toggleBool, "Toggle") != _toggleBool)
        {
            _toggleBool = !_toggleBool;
            Debug.Log($"toggle: {_toggleBool}");
        }

        // toolbar 一行button，每次只有一个button处于激活中
        _toolBarIndex = GUI.Toolbar(new Rect(25, 250, 100, 30), _toolBarIndex, _toolBarNames);

        // selection grid 多行toolbar
        _selectionGridIndex = GUI.SelectionGrid(new Rect(25, 300, 100, 50), _selectionGridIndex, _selectionGridNames, 2);

        // horizontal slider 水平滑钮
        _sliderValue = GUI.HorizontalSlider(new Rect(125, 0, 100, 30), _sliderValue, 0, 10f);

        // vertical slider 垂直滑钮
        _sliderValue = GUI.VerticalSlider(new Rect(175, 50, 30, 50), _sliderValue, 0, 10f);

        // scroll view 包含更大控件集合的可视区域, 第一个rect是位置，第二个rect是包含的空间大小
        _scrollViewVector = GUI.BeginScrollView(new Rect(125, 125, 100, 100), _scrollViewVector, new Rect(0, 0, 400, 400));
        _innerText = GUI.TextArea(new Rect(0, 0, 400, 400), _innerText);
        GUI.EndScrollView();

        // window 可拖动的控件容器，可获得和失去焦点
        _windowRect = GUI.Window(0, _windowRect, WindowFunc, "Window");

        // 更改GUI字体大小
        GUIStyle style = GUI.skin.GetStyle("label");
        style.fontSize = (int)(20.0f + 10.0f * Mathf.Sin(Time.time));
        GUI.Label(new Rect(225, 25, 100, 30), "Hello World!", style);

        // 自动布局
        GUILayout.Button("this is button");

        // 创建一个组
        GUI.BeginGroup(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 100));
        GUI.Box(new Rect(0, 0, 100, 100), "Group is here");
        GUI.Button(new Rect(10, 40, 80, 30), "Click me");
        GUI.EndGroup();

        // 包裹在指定区域
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 50, 100, 100));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Click me"))
        {

        }
        GUILayout.Box("Value");
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        // 当用户在GUI中执行了任何操作(点击按钮，拖动滑动条等)，相当于监听操作, 放在逻辑最后进行判断
        if (GUI.changed)
        {
            Debug.Log("changed!!!");
        }
    }

    void WindowFunc(int id)
    {
        // 窗口中逻辑处理
    }
}
