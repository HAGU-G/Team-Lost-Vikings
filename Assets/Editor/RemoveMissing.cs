using UnityEngine;
using UnityEditor;

public class RemoveMissing : Editor
{
    [MenuItem("Window/미싱 스크립트 제거", false, 1231)]
    private static void RemoveMissings()
    {
        var select = Selection.gameObjects;
        int componentCount = 0;
        int gameObjectCount = 0;

        foreach (var gameObject in select)
        {
            var missingScriptCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject);

            if (missingScriptCount > 0)
            {
                Undo.RegisterCompleteObjectUndo(gameObject, "미싱 크스립트 제거");
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);

                componentCount += missingScriptCount;
                gameObjectCount++;
            }
        }

        Debug.Log($"{gameObjectCount}개의 게임 오브젝트에서 {componentCount}개의 미싱스크립트 제거 완료");
    }
}
