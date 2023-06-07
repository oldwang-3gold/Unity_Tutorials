using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGGBT
{
    public enum EBTNodeType
    {
        EntryNodeType = 1,      // 根节点
        ActionNodeType,         // 行为节点
        CompositeNodeType,      // 组合节点
        ConditionNodeType,      // 条件节点
        DecorationNodeType,     // 装饰节点
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
        /// 唯一标识Id
        /// </summary>
        public int uid;

        /// <summary>
        /// 节点显示框大小
        /// </summary>
        public Rect nodeDisplayRect;

        /// <summary>
        /// 节点索引
        /// </summary>
        public int nodeIndex;

        /// <summary>
        /// 节点名字
        /// </summary>
        public string nodeName;

        /// <summary>
        /// 节点参数
        /// </summary>
        public string nodeParams;

        /// <summary>
        /// 节点类型
        /// </summary>
        public int nodeType;

        /// <summary>
        /// 父节点uid
        /// </summary>
        public int parentNodeUID;

        /// <summary>
        /// 子节点uid列表
        /// </summary>
        public List<int> childrenNodeUID;

        /// <summary>
        /// 节点所属行为树图表
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
            Debug.LogError("重复添加节点");
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

