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
        public int barScale = 1;
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
            List<ObjectPool> pools = poolsMgr.pools;
            EditorGUI.indentLevel = 1;
            EditorGUILayout.BeginHorizontal();
            isRootExpanded = EditorGUILayout.Foldout(isRootExpanded, string.Format("Pools ({0})", pools.Count));
            EditorGUILayout.EndHorizontal();
            if (isRootExpanded)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(Application.isPlaying);
                if (GUILayout.Button("New Pool", EditorStyles.toolbarButton))
                {
                    ui_action = UI_Action.add;
                }
                barScale = EditorGUILayout.IntField("bar scale", barScale);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginScrollView(Vector2.zero, GUILayout.Width(0), GUILayout.Height(0));
                for (int idx = 0; idx < pools.Count; idx++)
                {
                    EditorGUI.indentLevel = 2;
                    // item control toolbar 
                    EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                    if (GUILayout.Button("Del Pool", EditorStyles.toolbarButton))
                    {
                        actionIdx = idx;
                        ui_action = UI_Action.remove;
                    }
                    EditorGUILayout.EndHorizontal();
                    // preview
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(12);
                    if (pools[idx].prefab != null)
                    {
                        Texture prefabPreviewIcon = AssetPreview.GetAssetPreview(pools[idx].prefab);
                        DrawTexture(prefabPreviewIcon, 50, 50);
                    }
                    // set 
                    EditorGUILayout.BeginVertical(GUILayout.MinHeight(50));
                    pools[idx].prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", pools[idx].prefab, typeof(GameObject), false);
                    pools[idx].clearTime = EditorGUILayout.FloatField("clear time", pools[idx].clearTime);

                    // EditorGUILayout.BeginHorizontal();
                    pools[idx].miniSize = EditorGUILayout.IntField("pool size", pools[idx].miniSize);
                    if (pools[idx].miniSize == 0)
                        GUILayout.Box("", GUILayout.Height(5), GUILayout.Width(1));
                    else
                        GUILayout.Box("", GUILayout.Height(5), GUILayout.Width(pools[idx].miniSize / barScale));
                    // EditorGUILayout.EndHorizontal();

                    // EditorGUILayout.BeginHorizontal();
                    if (pools[idx].poolQ.Count == 0)
                        GUILayout.Box("", GUILayout.Height(5), GUILayout.Width(1));
                    else
                        GUILayout.Box("", GUILayout.Height(5), GUILayout.Width(pools[idx].poolQ.Count / barScale));
                    EditorGUILayout.LabelField("in pool", pools[idx].poolQ.Count.ToString());
                    // EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            EditorGUI.EndDisabledGroup();
            switch (ui_action)
            {
                case UI_Action.add:
                    pools.Insert(actionIdx, new ObjectPool());
                    break;
                case UI_Action.remove:
                    pools.RemoveAt(actionIdx);
                    break;
                default: break;
            }
            if (GUI.changed && (!Application.isPlaying))
            {
                EditorSceneManager.MarkSceneDirty(poolsMgr.gameObject.scene);
            }
            this.Repaint();
        }
    }
}