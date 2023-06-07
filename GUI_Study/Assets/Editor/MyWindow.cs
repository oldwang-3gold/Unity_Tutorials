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

    // ��ݼ� m_t => T    %t => ctrl + T   #t => shift + T    &t => alt + t 
    [MenuItem("Window/My Window #t")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MyWindow));
    }

    private void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        m_TextFieldString = EditorGUILayout.TextField("TextField", m_TextFieldString);

        // ������
        m_GroupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", m_GroupEnabled);
        m_ToggleBool = EditorGUILayout.Toggle("Toggle", m_ToggleBool);
        m_SliderValue = EditorGUILayout.Slider("Slider", m_SliderValue, 0, 1);
        EditorGUILayout.EndToggleGroup();

        // �۵��б�
        m_Foldout = EditorGUILayout.Foldout(m_Foldout, "�б�");
        if (m_Foldout)
        {
            EditorGUILayout.LabelField("����:XXX");
            EditorGUILayout.LabelField("�ֻ���:XXX");
        }

        // ������
        m_Value = (EValue)EditorGUILayout.EnumPopup("ѡ��", m_Value);

        // �ٴ�ȷ�Ͽ�
        if (GUILayout.Button("����ȷ�Ͽ�", GUILayout.Width(100), GUILayout.Height(50)))
        {
            if (EditorUtility.DisplayDialog("�ٴ�ȷ��", "�Ƿ�ȷ��", "ȷ��", "ȡ��"))
            {
                Debug.Log("yes");
            }
            else
            {
                Debug.Log("no");
            }
        }

        // ������
        if (GUILayout.Button("������"))
        {
            EditorUtility.DisplayProgressBar("������", "��ǰ����", m_SliderValue);
            //EditorUtility.ClearProgressBar();
        }

        // ��ʾ��Ϣ
        if (GUILayout.Button("��ʾ"))
        {
             ShowNotification(new GUIContent("this is a notification"), 2f);
        }

        // ������Ϣ
        EditorGUILayout.HelpBox("������Ϣ", MessageType.None);
        EditorGUILayout.HelpBox("������ʾ", MessageType.Warning);

        // ѡ���ļ�/�ļ���
        if (GUILayout.Button("ѡ��洢λ��"))
        {
            m_FilePath = EditorUtility.OpenFilePanel("ѡ���ļ�", m_FilePath, "lua");
            m_FolderPath = EditorUtility.OpenFolderPanel("ѡ��·��", m_FilePath, "");
        }

        //Debug.Log(Event.current.mousePosition);
    }

    /// <summary>
    /// ѡ��hierarchy����ڶ���ʱ
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
    /// ���ڹر�ʱ
    /// </summary>
    private void OnDestroy()
    {
        Debug.Log("Close Window");
    }
}
