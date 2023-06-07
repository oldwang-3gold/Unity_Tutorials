using UnityEngine;
using UnityEditor;

public class MyWindow : EditorWindow
{
    private string m_TextFieldString = "Hello World";
    private bool m_GroupEnabled;
    private bool m_ToggleBool = true;
    private float m_SliderValue = 0f;

    private bool m_Foldout;

    public enum EValue
    {
        Value1,
        Value2,
        Value3
    }
    private EValue m_Value;

    private string m_FilePath;
    private string m_FolderPath;

    // 快捷键 m_t => T    %t => ctrl + T   #t => shift + T    &t => alt + t 
    [MenuItem("Window/My Window #t")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MyWindow));
    }

    private void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        m_TextFieldString = EditorGUILayout.TextField("TextField", m_TextFieldString);

        // 开关组
        m_GroupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", m_GroupEnabled);
        m_ToggleBool = EditorGUILayout.Toggle("Toggle", m_ToggleBool);
        m_SliderValue = EditorGUILayout.Slider("Slider", m_SliderValue, 0, 1);
        EditorGUILayout.EndToggleGroup();

        // 折叠列表
        m_Foldout = EditorGUILayout.Foldout(m_Foldout, "列表");
        if (m_Foldout)
        {
            EditorGUILayout.LabelField("姓名:XXX");
            EditorGUILayout.LabelField("手机号:XXX");
        }

        // 下拉框
        m_Value = (EValue)EditorGUILayout.EnumPopup("选择", m_Value);

        // 再次确认框
        if (GUILayout.Button("弹出确认框", GUILayout.Width(100), GUILayout.Height(50)))
        {
            if (EditorUtility.DisplayDialog("再次确定", "是否确认", "确定", "取消"))
            {
                Debug.Log("yes");
            }
            else
            {
                Debug.Log("no");
            }
        }

        // 进度条
        if (GUILayout.Button("进度条"))
        {
            EditorUtility.DisplayProgressBar("进度条", "当前进度", m_SliderValue);
            //EditorUtility.ClearProgressBar();
        }

        // 提示信息
        if (GUILayout.Button("提示"))
        {
             ShowNotification(new GUIContent("this is a notification"), 2f);
        }

        // 帮助信息
        EditorGUILayout.HelpBox("帮助信息", MessageType.None);
        EditorGUILayout.HelpBox("警告提示", MessageType.Warning);

        // 选择文件/文件夹
        if (GUILayout.Button("选择存储位置"))
        {
            m_FilePath = EditorUtility.OpenFilePanel("选择文件", m_FilePath, "lua");
            m_FolderPath = EditorUtility.OpenFolderPanel("选择路径", m_FilePath, "");
        }

        //Debug.Log(Event.current.mousePosition);
    }

    /// <summary>
    /// 选择hierarchy面板内对象时
    /// </summary>
    private void OnSelectionChange()
    {
        foreach(var item in Selection.transforms)
        {
            Debug.Log($"Select: {item.name}");
        }
        foreach(var guid in Selection.assetGUIDs)
        {
            var path = AssetDatabase.GUIDToAssetPath(new GUID(guid));
            Debug.Log($"guid: {guid}, path: {path}");
            
        }
        
    }

    /// <summary>
    /// 窗口关闭时
    /// </summary>
    private void OnDestroy()
    {
        Debug.Log("Close Window");
    }
}
