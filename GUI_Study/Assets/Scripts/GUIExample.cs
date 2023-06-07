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
        // label ����ʾ��Ϣ
        GUI.Label(new Rect(25, 0, 100, 30), "Label");

        // button ��ť����
        if (GUI.Button(new Rect(25, 50, 100, 30), "Button"))
        {
            // �����ť���߼�
        }

        // textfield �����ı������
        _textFieldString = GUI.TextField(new Rect(25, 100, 100, 30), _textFieldString);

        // textarea �����ı������
        _textAreaString = GUI.TextArea(new Rect(25, 150, 100, 30), _textAreaString);

        // toggle ���п�/��״̬�ĸ�ѡ��
        if (GUI.Toggle(new Rect(25, 200, 100, 30), _toggleBool, "Toggle") != _toggleBool)
        {
            _toggleBool = !_toggleBool;
            Debug.Log($"toggle: {_toggleBool}");
        }

        // toolbar һ��button��ÿ��ֻ��һ��button���ڼ�����
        _toolBarIndex = GUI.Toolbar(new Rect(25, 250, 100, 30), _toolBarIndex, _toolBarNames);

        // selection grid ����toolbar
        _selectionGridIndex = GUI.SelectionGrid(new Rect(25, 300, 100, 50), _selectionGridIndex, _selectionGridNames, 2);

        // horizontal slider ˮƽ��ť
        _sliderValue = GUI.HorizontalSlider(new Rect(125, 0, 100, 30), _sliderValue, 0, 10f);

        // vertical slider ��ֱ��ť
        _sliderValue = GUI.VerticalSlider(new Rect(175, 50, 30, 50), _sliderValue, 0, 10f);

        // scroll view ��������ؼ����ϵĿ�������, ��һ��rect��λ�ã��ڶ���rect�ǰ����Ŀռ��С
        _scrollViewVector = GUI.BeginScrollView(new Rect(125, 125, 100, 100), _scrollViewVector, new Rect(0, 0, 400, 400));
        _innerText = GUI.TextArea(new Rect(0, 0, 400, 400), _innerText);
        GUI.EndScrollView();

        // window ���϶��Ŀؼ��������ɻ�ú�ʧȥ����
        _windowRect = GUI.Window(0, _windowRect, WindowFunc, "Window");

        // ����GUI�����С
        GUIStyle style = GUI.skin.GetStyle("label");
        style.fontSize = (int)(20.0f + 10.0f * Mathf.Sin(Time.time));
        GUI.Label(new Rect(225, 25, 100, 30), "Hello World!", style);

        // �Զ�����
        GUILayout.Button("this is button");

        // ����һ����
        GUI.BeginGroup(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 100));
        GUI.Box(new Rect(0, 0, 100, 100), "Group is here");
        GUI.Button(new Rect(10, 40, 80, 30), "Click me");
        GUI.EndGroup();

        // ������ָ������
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 50, 100, 100));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Click me"))
        {

        }
        GUILayout.Box("Value");
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        // ���û���GUI��ִ�����κβ���(�����ť���϶���������)���൱�ڼ�������, �����߼��������ж�
        if (GUI.changed)
        {
            Debug.Log("changed!!!");
        }
    }

    void WindowFunc(int id)
    {
        // �������߼�����
    }
}
