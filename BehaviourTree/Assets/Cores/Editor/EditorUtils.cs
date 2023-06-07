using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GGGBT.Editor
{
    public static class EditorUtils
    {
        public static void DisplayDIYGUILable(string title, Color color, float space, float width, float height)
        {
            var precolor = GUI.color;
            GUI.color = color;
            GUILayout.Space(space);
            GUILayout.Label(title, "box", GUILayout.Width(width), GUILayout.Height(height));
            GUI.color = precolor;
        }
    }
}

