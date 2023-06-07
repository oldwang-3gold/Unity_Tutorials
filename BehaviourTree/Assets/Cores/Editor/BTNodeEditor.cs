using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace GGGBT.Editor
{
    public class BTNodeEditor : EditorWindow
    {
        /// <summary>
        /// �༭���ڵ������Ϣ
        /// </summary>
        public class CreateNodeInfo
        {
            /// <summary>
            /// ��Ϊ��ͼ
            /// </summary>
            public BTGraph Graph { get; set; }

            /// <summary>
            /// �����ڵ�
            /// </summary>
            public BTNode OperateNode { get; set; }

            /// <summary>
            /// �ڵ���
            /// </summary>
            public string NodeName { get; set; }

            public CreateNodeInfo(BTGraph graph, BTNode operateNode, string nodeName)
            {
                Graph = graph;
                OperateNode = operateNode;
                NodeName = nodeName;
            }
        }
        #region ���������
        /// <summary>
        /// �������
        /// </summary>
        private string[] m_ToolBarNames = { "�����", "�������", "��̬����" };
        private int m_ToolBarSelectedIndex;
        
        /// <summary>
        /// ����ѡ���������
        /// </summary>
        private Rect m_ToolBarRect;

        private const float m_ToolBarWidth = 250.0f;
        private const float m_ToolBarHeight = 20.0f;

        /// <summary>
        /// ����ѡ�������������
        /// </summary>
        private Rect m_InspectRect;

        private float m_InspectWindowWidth = m_ToolBarWidth;
        private float m_InspectWindowHeight = 2000f;
        #endregion
        #region �ڵ��������
        private const float m_NodeAreaWindowWidth = 4000f;
        private const float m_NodeAreaWindowHeight = 2000f;
        private Rect m_NodeWindowDragableRect;

        private const float m_NodeWindowWidth = 150f;
        private const float m_NodeWindowHeight = 150f;

        private const string m_DefaultBTGraphFileName = "DefaultBT";

        private Vector2 m_WindowScrollPos;

        public enum EBTMenuType
        {
            EmptyAreaMenu = 1,      // �������������˵�
            RootNodeAreaMenu,       // ������ڵ�Ĳ˵�
            ChildNodeAreaMenu,      // ����ӽڵ�Ĳ˵�
            ActionLeafNodeAreaMenu, // �����ΪҶ�ӽڵ�Ĳ˵�
        }

        /// <summary>
        /// �ڵ�˵�ӳ��
        /// </summary>
        private Dictionary<EBTMenuType, GenericMenu> m_NodeMenuMap;

        /// <summary>
        /// �ڵ����Ͷ�Ӧ��ɫ
        /// </summary>
        private Dictionary<EBTNodeType, Color> m_NodeTypeColorMap;

        /// <summary>
        /// �ڵ����Ͷ�Ӧ����
        /// </summary>
        private Dictionary<EBTNodeType, string> m_NodeTypeNameMap;

        /// <summary>
        /// �հ��������˵�
        /// </summary>
        private GenericMenu m_EmptyAreaMenu;

        private GenericMenu m_RootNodeAreaMenu;
        private GenericMenu m_ChildNodeAreaMenu;
        private GenericMenu m_ActionLeafNodeAreaMenu;

        private GUIStyle m_LabelAlignMiddleStyle;
        private GUIStyle m_LabelAlignLeftStyle;

        /// <summary>
        /// ��ͨ������ɫ
        /// </summary>
        private Color m_NormalCurveColor;

        public enum ECreateNodeStragey
        {
            ToEnd = 1,      // ���뵽β��
            ToStart = 2,    // ���뵽ͷ
        }

        private ECreateNodeStragey m_CurrentCreateNodeStragey = ECreateNodeStragey.ToEnd;

        #endregion


        private BTGraph m_CurrentSelectionBTGraph;

        /// <summary>
        /// ��ǰ����Ľڵ�
        /// </summary>
        private BTNode m_CurrentClickNode;

        [MenuItem("TinyWang/AI/��Ϊ���༭��")]
        static void ShowEditor()
        {
            BTNodeEditor btNodeEditor = EditorWindow.GetWindow<BTNodeEditor>("��Ϊ���༭��");
            btNodeEditor.Show();
            btNodeEditor.Init();
        }

        private void Init()
        {
            CreateNewBTAsset();
        }

        private void OnEnable()
        {
            Debug.Log("BTNodeEditor OnEnable");
            m_NodeTypeColorMap = new Dictionary<EBTNodeType, Color>();
            m_NodeTypeColorMap.Add(EBTNodeType.EntryNodeType, Color.white);
            m_NodeTypeColorMap.Add(EBTNodeType.ActionNodeType, Color.red);
            m_NodeTypeColorMap.Add(EBTNodeType.CompositeNodeType, Color.blue);
            m_NodeTypeColorMap.Add(EBTNodeType.ConditionNodeType, Color.yellow);
            m_NodeTypeColorMap.Add(EBTNodeType.DecorationNodeType, Color.cyan);

            m_NodeTypeNameMap = new Dictionary<EBTNodeType, string>();
            m_NodeTypeNameMap.Add(EBTNodeType.EntryNodeType, "���ڵ�");
            m_NodeTypeNameMap.Add(EBTNodeType.ActionNodeType, "��Ϊ�ڵ�");
            m_NodeTypeNameMap.Add(EBTNodeType.CompositeNodeType, "��Ͻڵ�");
            m_NodeTypeNameMap.Add(EBTNodeType.ConditionNodeType, "�����ڵ�");
            m_NodeTypeNameMap.Add(EBTNodeType.DecorationNodeType, "װ�νڵ�");

            m_ToolBarRect = new Rect(0, 0, m_ToolBarWidth, m_ToolBarHeight);
            m_ToolBarSelectedIndex = 0;
            m_InspectRect = new Rect(0, m_ToolBarHeight, m_InspectWindowWidth, m_InspectWindowHeight);
            m_NodeWindowDragableRect = new Rect(0, 0, m_NodeWindowWidth, 20f);
            m_NormalCurveColor = Color.blue;
            InitMenu();
        }

        private void OnGUI()
        {
            m_InspectWindowHeight = position.height;
            m_InspectRect.height = m_InspectWindowHeight - m_ToolBarRect.height;
            m_LabelAlignMiddleStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            m_LabelAlignLeftStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperLeft };

            if (m_CurrentSelectionBTGraph != null && m_CurrentSelectionBTGraph.rootNodeUID != 0)
            {
                DrawOperationPanel();
                DrawBTNode();
            }
            else
            {
                Debug.LogError("δѡ����Ч��Ϊ���ڵ���ļ�������");
            }
        }

        /// <summary>
        /// ���Ʋ������
        /// </summary>
        private void DrawOperationPanel()
        {
            // ������ť��ҳ
            GUILayout.BeginArea(m_ToolBarRect, EditorStyles.toolbar);
            m_ToolBarSelectedIndex = GUILayout.Toolbar(m_ToolBarSelectedIndex, m_ToolBarNames, EditorStyles.toolbarButton);
            GUILayout.EndArea();

            // ����������
            GUILayout.BeginArea(m_InspectRect);
            DrawSelectToolBarInspector();
            GUILayout.EndArea();

        }
        
        /// <summary>
        /// ���Ƶ�ǰѡ��Ĳ����������
        /// </summary>
        private void DrawSelectToolBarInspector()
        {
            switch(m_ToolBarSelectedIndex)
            {
                case 0: DrawUserOperationPanel();
                    break;
                case 1: DrawParamsPanel();
                    break;
                case 2: DrawVariablePanel();
                    break;
            }
        }

        private void DrawUserOperationPanel()
        {
            var halfToolbarWidth = m_ToolBarWidth / 2 - 10f;
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorUtils.DisplayDIYGUILable("�������", Color.yellow, 0, m_ToolBarWidth - 10f, 20.0f);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("��Ϊ����:", GUILayout.Width(halfToolbarWidth), GUILayout.Height(20.0f));
            m_CurrentSelectionBTGraph.exportFileName = EditorGUILayout.TextField(m_CurrentSelectionBTGraph.exportFileName, GUILayout.Width(halfToolbarWidth), GUILayout.Height(20f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            if (!Application.isPlaying)
            {
                if (m_CurrentSelectionBTGraph != null)
                {
                    if (GUILayout.Button("����", GUILayout.Width(m_ToolBarWidth - 10), GUILayout.Height(20f)))
                    {

                    }
                }
                else
                {
                    if (GUILayout.Button("����", GUILayout.Width(m_ToolBarWidth - 10), GUILayout.Height(20f)))
                    {
                        if (!string.IsNullOrEmpty(m_CurrentSelectionBTGraph.exportFileName))
                        {

                        }
                    }
                }
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.EndVertical();
        }

        private void DrawParamsPanel()
        {
            var halfToolbarWidth = m_ToolBarWidth / 2 - 10f;
            if (m_CurrentClickNode != null)
            {
                EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("�ڵ���", GUILayout.Width(halfToolbarWidth - 40f), GUILayout.Height(20f));
                GUILayout.Label(m_CurrentClickNode != null ? m_CurrentClickNode.nodeName : String.Empty, "textarea", GUILayout.Width(halfToolbarWidth + 40f), GUILayout.Height(20f));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("�ڵ����", GUILayout.Width(m_ToolBarWidth - 10), GUILayout.Height(20f));
                m_CurrentClickNode.nodeParams = GUILayout.TextField(m_CurrentClickNode.nodeParams, GUILayout.Width(m_ToolBarWidth - 10), GUILayout.Height(20f));
                EditorGUILayout.LabelField("�ڵ����˵��", GUILayout.Width(m_ToolBarWidth - 10), GUILayout.Height(20f));
                var nodeParamIntroduction = "test";
                GUILayout.Label(nodeParamIntroduction, "textarea", GUILayout.Width(m_ToolBarWidth - 10), GUILayout.Height(40f));
                EditorGUILayout.EndVertical();
            }

        }

        private void DrawVariablePanel()
        {

        }



        /// <summary>
        /// ������Ϊ���ڵ�
        /// </summary>
        private void DrawBTNode()
        {
            m_WindowScrollPos = GUI.BeginScrollView(
                new Rect(m_InspectWindowHeight, 0, position.width - m_InspectWindowHeight, position.height),
                m_WindowScrollPos,
                new Rect(0, 0, m_NodeAreaWindowWidth, m_NodeAreaWindowHeight)
            );
            if (m_CurrentSelectionBTGraph != null)
            {
                DrawNodeCurves(m_CurrentSelectionBTGraph, m_CurrentSelectionBTGraph.RootNode);
                BeginWindows();
                DrawNodes(m_CurrentSelectionBTGraph, m_CurrentSelectionBTGraph.RootNode);
                EndWindows();
            }
            GUI.EndScrollView();
        }

        /// <summary>
        /// ���ƽڵ��������
        /// </summary>
        /// <param name="btgraph"></param>
        /// <param name="node"></param>
        private void DrawNodeCurves(BTGraph btgraph, BTNode node)
        {
            if (node != null && node.childrenNodeUID != null)
            {
                for (int i = 0; i < node.childrenNodeUID.Count; ++i)
                {
                    var childNode = btgraph.FindNodeByUID(node.childrenNodeUID[i]);
                    if (childNode != null)
                    {
                        var curveColor = m_NormalCurveColor;
                        DrawCurve(node.nodeDisplayRect, childNode.nodeDisplayRect, curveColor);
                        DrawNodeCurves(btgraph, childNode);
                    }
                }
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="curveColor"></param>
        private void DrawCurve(Rect start, Rect end, Color curveColor)
        {
            Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height, 0);
            Vector3 endPos = new Vector3(end.x + end.width / 2, end.y, 0);
            Vector3 startTangent = startPos + Vector3.up * 40;
            Vector3 endTangent = endPos + Vector3.down * 40;
            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, curveColor, null, 2);
        }

        /// <summary>
        /// ���ƽڵ�
        /// </summary>
        private void DrawNodes(BTGraph btgraph, BTNode node)
        {
            if (node != null)
            {
                var title = node.IsRootNode() ? "Root" : $"No.{node.nodeIndex}";
                GUI.color = GetNodeTypeColor((EBTNodeType)node.nodeType);
                var preNodeDisplayRect = node.nodeDisplayRect;
                node.nodeDisplayRect = GUI.Window(node.uid, node.nodeDisplayRect, DrawNodeWindow, new GUIContent(title));
                if (!node.nodeDisplayRect.Equals(preNodeDisplayRect))
                {
                    node.nodeDisplayRect.x = Mathf.Clamp(node.nodeDisplayRect.x, 0, m_NodeAreaWindowWidth - m_NodeWindowWidth);
                    node.nodeDisplayRect.y = Mathf.Clamp(node.nodeDisplayRect.y, 0, m_NodeAreaWindowHeight - m_NodeWindowHeight);
                }
                GUI.color = Color.white;
                for (int i = 0; i < node.childrenNodeUID.Count; ++i)
                {
                    var childnode = btgraph.FindNodeByUID(node.childrenNodeUID[i]);
                    if (childnode != null)
                    {
                        DrawNodes(btgraph, childnode);
                    }
                }
            }
        }

        /// <summary>
        /// ���ƽڵ㴰��
        /// </summary>
        /// <param name="uid"></param>
        private void DrawNodeWindow(int uid)
        {
            var customVariableWidth = m_NodeWindowWidth / 2f;
            var btNode = m_CurrentSelectionBTGraph.FindNodeByUID(uid);
            if (btNode == null)
            {
                Debug.Log("no node" + uid);
                return;
            }
            // �������ڵ�
            if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
            {
                m_CurrentClickNode = btNode;
            }
            else if (Event.current.button == 1 && Event.current.type == EventType.MouseDown)
            {
                m_CurrentClickNode = btNode;
                HandleNodeInteraction();
            }

            
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"{btNode.nodeName}", m_LabelAlignMiddleStyle, GUILayout.Width(m_NodeWindowWidth - 20f), GUILayout.Height(20.0f));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"{GetNodeTypeName((EBTNodeType)btNode.nodeType)}", m_LabelAlignLeftStyle, GUILayout.Width(m_NodeWindowWidth - 20f), GUILayout.Height(20f));
            EditorGUILayout.EndVertical();

            if (!Application.isPlaying)
            {
                // ����Խڵ㴰�ڽ�����ק
                GUI.DragWindow(m_NodeWindowDragableRect);
            }
        }

        /// <summary>
        /// ����ڵ㽻��
        /// </summary>
        private void HandleNodeInteraction()
        {
            if (!Application.isPlaying)
            {
                if (m_CurrentClickNode == null)
                {
                    Debug.Log("��ʾ�հ�����˵�");
                    UpdateMenu();
                    var menu = GetNodeTypeMenu(EBTMenuType.EmptyAreaMenu);
                    menu.ShowAsContext();
                }
                else if (m_CurrentClickNode != null && m_CurrentClickNode.IsRootNode())
                {
                    Debug.Log("��ʾ���ڵ�����˵�");
                    UpdateMenu();
                    var menu = GetNodeTypeMenu(EBTMenuType.RootNodeAreaMenu);
                    menu.ShowAsContext();
                }
                else if (m_CurrentClickNode != null && !m_CurrentClickNode.IsRootNode() && (!m_CurrentClickNode.CheckNodeType(EBTNodeType.ActionNodeType)) && !m_CurrentClickNode.CheckNodeType(EBTNodeType.ConditionNodeType))
                {
                    Debug.Log("��ʾ�ӽڵ�����˵�");
                    UpdateMenu();
                    var menu = GetNodeTypeMenu(EBTMenuType.ChildNodeAreaMenu);
                    menu.ShowAsContext();
                }
            }
        }

        private GenericMenu GetNodeTypeMenu(EBTMenuType menuType)
        {
            return m_NodeMenuMap[menuType];
        }


        private Color GetNodeTypeColor(EBTNodeType nodeType)
        {
            if (m_NodeTypeColorMap.ContainsKey(nodeType))
            {
                return m_NodeTypeColorMap[nodeType];
            }
            return Color.white;
        }

        /// <summary>
        /// �½���Ϊ��Asset
        /// </summary>
        private void CreateNewBTAsset()
        {
            m_CurrentSelectionBTGraph = new BTGraph(m_DefaultBTGraphFileName);

            var node = new BTNode(m_CurrentSelectionBTGraph, GetNodeRect(new Vector2(250f, 50f)), 0, "Root", EBTNodeType.EntryNodeType, null);
            m_CurrentSelectionBTGraph.SetRootNode(node);
        }

        private Rect GetNodeRect(Vector2 pos)
        {
            var rect = new Rect();
            rect.x = pos.x;
            rect.y = pos.y;
            rect.width = m_NodeWindowWidth;
            rect.height = m_NodeWindowHeight;
            return rect;
        }

        private string GetNodeTypeName (EBTNodeType nodeType)
        {
            return m_NodeTypeNameMap[nodeType];
        }

        #region �˵�����
        private void InitMenu()
        {
            m_NodeMenuMap = new Dictionary<EBTMenuType, GenericMenu>();
        }

        private void UpdateMenu()
        {
            m_EmptyAreaMenu = new GenericMenu();
            m_EmptyAreaMenu.AddItem(new GUIContent("�����"), false, null, m_CurrentSelectionBTGraph);
            m_RootNodeAreaMenu = new GenericMenu();
            AddAllAvalibleBTNodeMenu(m_RootNodeAreaMenu, m_CurrentClickNode);

            m_ChildNodeAreaMenu = new GenericMenu();
            AddAllAvalibleBTNodeMenu(m_ChildNodeAreaMenu, m_CurrentClickNode);
            
            m_NodeMenuMap.Clear();
            m_NodeMenuMap.Add(EBTMenuType.EmptyAreaMenu, m_EmptyAreaMenu);
            m_NodeMenuMap.Add(EBTMenuType.RootNodeAreaMenu, m_RootNodeAreaMenu);
            m_NodeMenuMap.Add(EBTMenuType.ChildNodeAreaMenu, m_ChildNodeAreaMenu);

        }

        private void AddAllAvalibleBTNodeMenu(GenericMenu menu, BTNode operationnode)
        {
            foreach(var compositeNodeName in BTNodeData.BTCompositeNodeNameArray)
            {
                var nodeinfo = new CreateNodeInfo(m_CurrentSelectionBTGraph, operationnode, compositeNodeName);
                menu.AddItem(new GUIContent($"����ӽڵ�/��Ͻڵ�/{compositeNodeName}"), false, OnCreateBTCompositeNode, nodeinfo);
            }
        }

        private void OnCreateBTCompositeNode(object createNodeInfo)
        {
            var nodeInfo = createNodeInfo as CreateNodeInfo;
            if (nodeInfo.OperateNode.CheckNodeType(EBTNodeType.DecorationNodeType) && nodeInfo.OperateNode.childrenNodeUID.Count > 0)
            {
                Debug.LogError("װ�νڵ�ֻ������һ���ӽڵ�");
            }
            else if (nodeInfo.OperateNode.IsRootNode() && nodeInfo.OperateNode.childrenNodeUID.Count > 0)
            {
                Debug.LogError("���ڵ㲻����������ӽڵ�");
            }
            else if (!nodeInfo.OperateNode.IsRootNode() || (nodeInfo.OperateNode.IsRootNode() && nodeInfo.OperateNode.childrenNodeUID.Count == 0))
            {
                var childNodePosX = nodeInfo.OperateNode.nodeDisplayRect.x;
                bool toEnd = m_CurrentCreateNodeStragey == ECreateNodeStragey.ToEnd;
                var childIndex = toEnd ? nodeInfo.OperateNode.childrenNodeUID.Count : 0;
                if (nodeInfo.OperateNode.childrenNodeUID.Count > 0)
                {
                    BTNode lastChildNode = toEnd ? nodeInfo.Graph.FindNodeByUID(nodeInfo.OperateNode.childrenNodeUID[nodeInfo.OperateNode.childrenNodeUID.Count - 1]) : nodeInfo.Graph.FindNodeByUID(nodeInfo.OperateNode.childrenNodeUID[0]);
                    childNodePosX = toEnd ? lastChildNode.nodeDisplayRect.x + m_NodeWindowWidth : lastChildNode.nodeDisplayRect.x - m_NodeWindowWidth;
                }
                var childNode = new BTNode(
                    nodeInfo.Graph,
                    GetNodeRect(new Vector2(childNodePosX, nodeInfo.OperateNode.nodeDisplayRect.y + m_NodeWindowHeight)),
                    childIndex,
                    nodeInfo.NodeName,
                    EBTNodeType.CompositeNodeType,
                    nodeInfo.OperateNode
                );
                if (m_CurrentSelectionBTGraph.AddNode(childNode))
                {
                    nodeInfo.OperateNode.AddChildNode(childNode.uid, nodeInfo.Graph, childNode.nodeIndex);
                }
                Debug.Log("OnCreateBTCompositeNode");
                Debug.Log($"ParentNodeName:{nodeInfo.OperateNode.nodeName}, ChildNodeName:{childNode.nodeName}");
            }
            else
            {
                Debug.LogError("��֧�ֵĴ�����ʽ");
            }
        }

        #endregion

    }
}

