using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridInfo))]
public class GridInfoEditor : Editor
{
    SerializedProperty useSprites;
    SerializedProperty row;
    SerializedProperty col;
    SerializedProperty minRow;
    SerializedProperty minCol;
    SerializedProperty cellSize;
    SerializedProperty images;
    SerializedProperty fixImages;
    SerializedProperty defaultTileSprite;
    SerializedProperty unusableTileSprite;

    private void OnEnable()
    {
        useSprites = serializedObject.FindProperty("useSprites");
        row = serializedObject.FindProperty("row");
        col = serializedObject.FindProperty("col");
        minRow = serializedObject.FindProperty("minRow");
        minCol = serializedObject.FindProperty("minCol");
        cellSize = serializedObject.FindProperty("cellSize");
        images = serializedObject.FindProperty("images");
        fixImages = serializedObject.FindProperty("fixImages");
        defaultTileSprite = serializedObject.FindProperty("defaultTileSprite");
        unusableTileSprite = serializedObject.FindProperty("unusableTileSprite");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GridInfo gridInfo = (GridInfo)target;

        MakeLabelBox("버튼");
        if (GUILayout.Button("변경점 저장"))
        {
            EditorUtility.SetDirty(target);
        }
        EditorGUILayout.Space();

        MakeLabelBox("사용할 타일 이미지");
        EditorGUILayout.PropertyField(useSprites);
        EditorGUILayout.Space();

        MakeLabelBox("최대 타일 수");
        EditorGUILayout.PropertyField(row);
        EditorGUILayout.PropertyField(col);
        EditorGUILayout.Space();

        MakeLabelBox("최소 타일 수");
        EditorGUILayout.PropertyField(minRow);
        EditorGUILayout.PropertyField(minCol);
        EditorGUILayout.Space();

        MakeLabelBox("타일 크기");
        EditorGUILayout.PropertyField(cellSize);
        EditorGUILayout.Space();

        MakeLabelBox("기본 타일 이미지 설정");
        EditorGUIUtility.labelWidth = 60;
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(defaultTileSprite);
        EditorGUILayout.PropertyField(unusableTileSprite);
        //EditorGUILayout.BeginHorizontal();
        //gridInfo.defaultTileSprite = (Sprite)EditorGUILayout.ObjectField(
        //        "확장 타일",
        //        gridInfo.defaultTileSprite,
        //        typeof(Sprite), true);

        //gridInfo.unusableTileSprite = (Sprite)EditorGUILayout.ObjectField(
        //        "미확장 타일",
        //        gridInfo.unusableTileSprite,
        //        typeof(Sprite), true);
        //EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        MakeLabelBox("지정 타일 이미지 설정");
        EditorGUILayout.PropertyField(images);
        EditorGUILayout.PropertyField(fixImages);

        //EditorGUIUtility.labelWidth = 60;
        //ShowImageList(gridInfo);

        //EditorGUILayout.BeginHorizontal();

        //SerializedProperty dataList = serializedObject.FindProperty("images").FindPropertyRelative("data");
        //if (GUILayout.Button("이미지 추가"))
        //{
        //    dataList.arraySize++;
        //    SerializedProperty newElement = dataList.GetArrayElementAtIndex(dataList.arraySize - 1);
        //    newElement.FindPropertyRelative("key").vector2IntValue = Vector2Int.zero;  // 기본 키 값 초기화
        //    newElement.FindPropertyRelative("value").FindPropertyRelative("usingImage").objectReferenceValue = null;
        //    newElement.FindPropertyRelative("value").FindPropertyRelative("unUsingImage").objectReferenceValue = null;
        //}

        //if (GUILayout.Button("이미지 제거") && dataList.arraySize > 0)
        //{
        //    dataList.DeleteArrayElementAtIndex(dataList.arraySize - 1);
        //}
        //EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        HashSet<Vector2Int> keySet = new HashSet<Vector2Int>();

        foreach (var item in gridInfo.images.data)
        {
            if (!keySet.Add(item.key))
            {
                EditorGUILayout.HelpBox($"중복된 인덱스를 입력했습니다.: {item.key}", MessageType.Error);
            }

            if (item.key.x >= gridInfo.row
                || item.key.y >= gridInfo.col)
            {
                EditorGUILayout.HelpBox($"유효하지 않은 인덱스를 입력했습니다.: {item.key}", MessageType.Error);
            }

            if ((item.value.usingImage != null && !AssetDatabase.GetAssetPath(item.value.usingImage).Contains("Isometric_Fantasy_Tiles"))
                || (item.value.unUsingImage != null && !AssetDatabase.GetAssetPath(item.value.unUsingImage).Contains("Isometric_Fantasy_Tiles")))
                EditorGUILayout.HelpBox($"어어 그거 아니다: {item.key}", MessageType.Warning);
        }
        EditorUtility.SetDirty(target);
    }

    private void MakeLabelBox(string label)
    {
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
    }

    private void ShowImageList(GridInfo gridInfo)
    {
        SerializedProperty dataList = serializedObject.FindProperty("images").FindPropertyRelative("data");

        for (int i = 0; i < dataList.arraySize; i++)
        {
            SerializedProperty element = dataList.GetArrayElementAtIndex(i);
            SerializedProperty key = element.FindPropertyRelative("key");
            SerializedProperty value = element.FindPropertyRelative("value");

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.PropertyField(key, new GUIContent("Key"));

            EditorGUILayout.BeginHorizontal();
            value.FindPropertyRelative("usingImage").objectReferenceValue = (Sprite)EditorGUILayout.ObjectField(
                "확장타일", value.FindPropertyRelative("usingImage").objectReferenceValue, typeof(Sprite), allowSceneObjects: true);

            value.FindPropertyRelative("unUsingImage").objectReferenceValue = (Sprite)EditorGUILayout.ObjectField(
                "미확장타일", value.FindPropertyRelative("unUsingImage").objectReferenceValue, typeof(Sprite), allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();



            EditorGUILayout.EndVertical();
        }
    }
}