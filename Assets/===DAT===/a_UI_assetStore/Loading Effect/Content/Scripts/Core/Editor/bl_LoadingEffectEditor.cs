using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(bl_LoadingEffect))]
[CanEditMultipleObjects]
public class bl_LoadingEffectEditor : Editor {

    private ReorderableList list;
    private bl_LoadingEffect myTarget;

    private void OnEnable()
    {
        myTarget = (bl_LoadingEffect)target;
        list = new ReorderableList(serializedObject,serializedObject.FindProperty("LoadingUI"),true, true, true, true);
        list.drawElementCallback = OnDrawList;
        list.drawHeaderCallback = HeaderList;
        list.draggable = (myTarget.ShowList) ? true : false;
        list.elementHeight = (myTarget.ShowList) ? 70 : 0;
    }

    void HeaderList(Rect rect)
    {
        rect.x += -5;
        rect.width += 10;
        string t = (myTarget.ShowList) ? "Hide List" : "Show List";
        if (GUI.Button(rect, t, EditorStyles.toolbarDropDown))
        {
            myTarget.ShowList = !myTarget.ShowList;
            list.elementHeight = (myTarget.ShowList) ? 70 : 0;
            list.draggable = (myTarget.ShowList) ? true : false;
        }
    }

    void OnDrawList(Rect rect,int index,bool ative,bool focus)
    {
        if (myTarget.ShowList)
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty UIElement = element.FindPropertyRelative("UI");
            SerializedProperty Typ = element.FindPropertyRelative("m_Type");

            GUI.Box(rect, "", "box");
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + 5, 100, EditorGUIUtility.singleLineHeight), UIElement, GUIContent.none);
           

            EditorGUI.LabelField(new Rect(rect.x + 105, rect.y + 5, 45, EditorGUIUtility.singleLineHeight), "Type", EditorStyles.toolbarButton);
            EditorGUI.PropertyField(new Rect(rect.x + 150, rect.y + 5, 160, EditorGUIUtility.singleLineHeight), Typ, GUIContent.none);

            EditorGUI.LabelField(new Rect(rect.x + 105, rect.y + 25, 45, EditorGUIUtility.singleLineHeight), "Speed", EditorStyles.toolbarButton);
            element.FindPropertyRelative("Speed").floatValue = EditorGUI.Slider(new Rect(rect.x + 150, rect.y + 25, 160, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Speed").floatValue, 1, 100);

            if (Typ.enumValueIndex == (int)LoadingEffectType.Rotate)
            {
                EditorGUI.LabelField(new Rect(rect.x + 105, rect.y + 45, 45, EditorGUIUtility.singleLineHeight), "Axis", EditorStyles.toolbarButton);
                element.FindPropertyRelative("Axis").vector3Value = EditorGUI.Vector3Field(new Rect(rect.x + 150, rect.y + 45, 160, EditorGUIUtility.singleLineHeight), "", element.FindPropertyRelative("Axis").vector3Value);
            }

            if (Typ.enumValueIndex == (int)LoadingEffectType.Filled)
            {
                element.FindPropertyRelative("PingPong").boolValue = EditorGUI.ToggleLeft(new Rect(rect.x + 105, rect.y + 45, 200, EditorGUIUtility.singleLineHeight), "PingPong",element.FindPropertyRelative("PingPong").boolValue, EditorStyles.toolbarButton);

                EditorGUI.PropertyField(new Rect(rect.x, rect.y + 25, 100, 22), element.FindPropertyRelative("Curve"), GUIContent.none);
            }

            EditorGUI.LabelField(new Rect(rect.x, rect.y + 45, 40, EditorGUIUtility.singleLineHeight), "Delay", EditorStyles.toolbarButton);
            element.FindPropertyRelative("Delay").floatValue = EditorGUI.FloatField(new Rect(rect.x + 40, rect.y + 45, 28, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Delay").floatValue);
        }
    }

    public override void OnInspectorGUI()
    {
        myTarget = (bl_LoadingEffect)target;
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        myTarget.ID = EditorGUILayout.IntField("ID", myTarget.ID);
        myTarget.isLoading = EditorGUILayout.ToggleLeft("Is Loading", myTarget.isLoading, EditorStyles.toolbarButton);
        myTarget.FadeSpeed = EditorGUILayout.Slider("Fade Speed", myTarget.FadeSpeed, 1, 15);
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(myTarget);
        }
    }
}