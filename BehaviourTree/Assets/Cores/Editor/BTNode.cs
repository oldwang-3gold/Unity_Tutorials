using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGGBT
{
    public enum EBTNodeType
    {
        EntryNodeType = 1,      // ���ڵ�
        ActionNodeType,         // ��Ϊ�ڵ�
        CompositeNodeType,      // ��Ͻڵ�
        ConditionNodeType,      // �����ڵ�
        DecorationNodeType,     // װ�νڵ�
    }

    public interface IRecycle
    {
        void OnCreate();
        void OnDispose();
    }

    [Serializable]
    public class BTNode : IRecycle
    {
        /// <summary>
        /// Ψһ��ʶId
        /// </summary>
        public int uid;

        /// <summary>
        /// �ڵ���ʾ���С
        /// </summary>
        public Rect nodeDisplayRect;

        /// <summary>
        /// �ڵ�����
        /// </summary>
        public int nodeIndex;

        /// <summary>
        /// �ڵ�����
        /// </summary>
        public string nodeName;

        /// <summary>
        /// �ڵ����
        /// </summary>
        public string nodeParams;

        /// <summary>
        /// �ڵ�����
        /// </summary>
        public int nodeType;

        /// <summary>
        /// ���ڵ�uid
        /// </summary>
        public int parentNodeUID;

        /// <summary>
        /// �ӽڵ�uid�б�
        /// </summary>
        public List<int> childrenNodeUID;

        /// <summary>
        /// �ڵ�������Ϊ��ͼ��
        /// </summary>
        public BTGraph OwnerGraph
        {
            get;
            set;
        }

        public BTNode() { }

        public BTNode(BTGraph ownerGraph, Rect nodeRect, int nodeindex, string nodename, EBTNodeType nodetype, BTNode parentnode)
        {
            uid = BTUtilities.GetNodeUID();
            nodeDisplayRect = nodeRect;
            nodeIndex = nodeindex;
            nodeName = nodename;
            nodeParams = String.Empty;
            nodeType = (int)nodetype;
            parentNodeUID = parentnode != null ? parentnode.uid : 0;
            childrenNodeUID = new List<int>();
        }

        public bool AddChildNode(int childNodeUID, BTGraph btgraph, int? insertindex = null)
        {
            if (insertindex == null)
            {
                insertindex = childrenNodeUID.Count;
            }
            if (!childrenNodeUID.Contains(childNodeUID))
            {
                childrenNodeUID.Insert((int)insertindex, childNodeUID);
                return true;
            }
            Debug.LogError("�ظ���ӽڵ�");
            return false;
        }

        public bool DeleteChildNode(int childNodeUID, BTGraph btgraph)
        {
            if (childrenNodeUID.Remove(childNodeUID))
            {
                return true;
            }
            return false;
        }

        public bool IsRootNode()
        {
            return parentNodeUID == 0;
        }

        public bool CheckNodeType(EBTNodeType nodetype)
        {
            return nodeType == (int)nodetype;
        }

        public virtual void OnCreate()
        {

        }

        public virtual void Dispose()
        {
            uid = 0;
            nodeIndex = -1;
            nodeName = null;
            nodeType = 0;
            parentNodeUID = 0;
            childrenNodeUID = null;
            OwnerGraph = null;
        }

        public void OnDispose()
        {
            uid = 0;
            nodeIndex = -1;
            nodeName = null;
            nodeType = 0;
            parentNodeUID = 0;
            childrenNodeUID = null;
            OwnerGraph = null;
        }

        public virtual void OnPause(bool isPause)
        {

        }

        public virtual void Reset()
        {

        }

        protected virtual void OnEnter()
        {

        }

        protected virtual void OnExit()
        {
            Reset();
        }
    }
}

