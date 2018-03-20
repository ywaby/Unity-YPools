using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

namespace YPools
{
    [CustomEditor(typeof(ObjectPoolsMgr))]
    public class ObjectPoolsMgr_UI : Editor
    {
        private ObjectPoolsMgr poolsMgr;
        private bool isRootExpanded = true;

        public static void DrawTexture(Texture tex)
        {
            if (tex == null)
            {
                Debug.LogWarning("GUI texture is missing !");
                return;
            }

            Rect rect = GUILayoutUtility.GetRect(0f, 0f);
            rect.width = tex.width;
            rect.height = tex.height;
            GUILayout.Space(rect.height);
            GUI.DrawTexture(rect, tex);
        }

        public static void DrawTexture(Texture tex, float optionalWidth, float optionalHeight)
        {
            if (tex == null)
            {
                Debug.LogWarning("GUI texture is missing !");
                return;
            }

            Rect rect = GUILayoutUtility.GetRect(0f, 0f);
            rect.width = optionalWidth;
            rect.height = optionalHeight;
            GUILayout.Space(rect.height);
            GUI.DrawTexture(rect, tex);
        }

        enum UI_Action
        {
            none,
            remove,
            add,
            retag
        };
        public override void OnInspectorGUI()
        {
            UI_Action ui_action = UI_Action.none;
            int actionIdx = 0;
            EditorGUI.indentLevel = 0;
            poolsMgr = (ObjectPoolsMgr)target;
            List<ObjectPoolsMgr.ObjectPool> poolsSet = poolsMgr.poolsSet;
            EditorGUI.indentLevel = 1;
            EditorGUILayout.BeginHorizontal();
            isRootExpanded = EditorGUILayout.Foldout(isRootExpanded, string.Format("Pools ({0})", poolsSet.Count));
            if (!Application.isPlaying) //During Editor
            {
                if (GUILayout.Button("Add", EditorStyles.toolbarButton, GUILayout.Width(32)))
                {
                    ui_action = UI_Action.add;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (isRootExpanded)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginScrollView(Vector2.zero, GUILayout.Width(0), GUILayout.Height(0));
                for (int idx = 0; idx < poolsSet.Count; idx++)
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                    if (!Application.isPlaying) //During Editor
                    {
                        if (GUILayout.Button("Del", EditorStyles.toolbarButton, GUILayout.Width(32)))
                        {
                            actionIdx = idx;
                            ui_action = UI_Action.remove;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(12);
                    if (poolsSet[idx].prefab != null)
                    {
                        Texture prefabPreviewIcon = null;
                        prefabPreviewIcon = AssetPreview.GetAssetPreview(poolsSet[idx].prefab);
                        DrawTexture(prefabPreviewIcon, 50, 50);
                    }
                    EditorGUILayout.BeginVertical(GUILayout.MinHeight(50));
                    if (!Application.isPlaying) //During Editor
                    {
                        poolsSet[idx].tag = EditorGUILayout.TextField("tag", poolsSet[idx].tag);
                        poolsSet[idx].prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", poolsSet[idx].prefab, typeof(GameObject), false);
                        poolsSet[idx].miniSize = EditorGUILayout.IntField("pool size", poolsSet[idx].miniSize);
                        poolsSet[idx].clearTime = EditorGUILayout.FloatField("clear time", poolsSet[idx].clearTime);
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            switch (ui_action)
            {
                case UI_Action.add:
                    poolsSet.Insert(actionIdx, new ObjectPoolsMgr.ObjectPool());
                    break;
                case UI_Action.remove:
                    poolsSet.RemoveAt(actionIdx);
                    break;
                default: break;
            }
            if (GUI.changed)
            {
                EditorSceneManager.MarkSceneDirty(poolsMgr.gameObject.scene);
            }
            this.Repaint();
        }
    }
}