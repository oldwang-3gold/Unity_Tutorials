using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGGBT
{
    [Serializable]
    public class BTGraph
    {
        /// <summary>
        /// 行为树导出文件名
        /// </summary>
        public string exportFileName;

        /// <summary>
        /// 行为树根节点uid
        /// </summary>
        public int rootNodeUID;

        /// <summary>
        /// 行为树所有节点数据
        /// </summary>
        public List<BTNode> allNodesList;

        public BTNode RootNode
        {
            get;
            private set;
        }

        public BTGraph(string fileName)
        {
            exportFileName = fileName;
            allNodesList = new List<BTNode>();
        }

        public void SetRootNode(BTNode root)
        {
            RootNode = root;
            rootNodeUID = root.uid;
            allNodesList.Add(root);
        }

        public BTNode FindNodeByUID(int uid)
        {
            var findnode = allNodesList.Find((btnode) =>
            {
                return btnode.uid == uid;
            });
            return findnode;
        }

        public bool AddNode(BTNode node)
        {
            if (node != null && !allNodesList.Contains(node))
            {
                allNodesList.Add(node);
                return true;
            }
            Debug.Log($"不允许添加空或UID:{node.uid}的重复节点");
            return false;
        }
    }
}


