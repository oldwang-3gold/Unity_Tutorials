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
        /// ��Ϊ�������ļ���
        /// </summary>
        public string exportFileName;

        /// <summary>
        /// ��Ϊ�����ڵ�uid
        /// </summary>
        public int rootNodeUID;

        /// <summary>
        /// ��Ϊ�����нڵ�����
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
            Debug.Log($"��������ӿջ�UID:{node.uid}���ظ��ڵ�");
            return false;
        }
    }
}


