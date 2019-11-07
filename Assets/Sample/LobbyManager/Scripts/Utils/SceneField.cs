﻿using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bolt.Samples.Photon.Lobby.Utilities
{
    [Serializable]
    public class SceneField
    {
        [SerializeField] private Object sceneAsset;
        [SerializeField] private string sceneName = "";

        public Object SceneAsset
        {
            get { return sceneAsset; }
        }

        public string SceneName
        {
            get { return sceneName; }
        }

        public string SimpleSceneName
        {
            get {
                string[] data = sceneName.Split(Path.AltDirectorySeparatorChar);
                return data[data.Length - 1];
            }
        }

        public int SceneIndex
        {
            get { return SceneManager.GetSceneByName(SimpleSceneName).buildIndex; }
        }

        public bool IsLoaded
        {
            get
            {
                Debug.LogFormat( "Current Scene: {0}, local: {1}", SceneManager.GetActiveScene().buildIndex, SceneIndex);
                
                return SceneManager.GetActiveScene().buildIndex == SceneIndex;
            }
        }

        // makes it work with the existing Unity methods (LoadLevel/LoadScene)
        public static implicit operator string(SceneField sceneField)
        {
            return sceneField.SceneName;
        }
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            var sceneAsset = property.FindPropertyRelative("sceneAsset");
            var sceneName = property.FindPropertyRelative("sceneName");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (sceneAsset != null)
            {
                EditorGUI.BeginChangeCheck();
                var value = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
                if (EditorGUI.EndChangeCheck())
                {
                    sceneAsset.objectReferenceValue = value;
                    if (sceneAsset.objectReferenceValue != null)
                    {
                        var scenePath = AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue);
                        var assetsIndex = scenePath.IndexOf("Assets", StringComparison.Ordinal) + 7;
                        var extensionIndex = scenePath.LastIndexOf(".unity", StringComparison.Ordinal);
                        scenePath = scenePath.Substring(assetsIndex, extensionIndex - assetsIndex);
                        sceneName.stringValue = scenePath;
                    }
                }
            }
            EditorGUI.EndProperty();
        }
    }
#endif
}
