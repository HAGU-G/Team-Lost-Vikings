using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIRenderTexture : MonoBehaviour
{
    public Camera slotCamera;
    public int layer;
    private Vector3 zeroPos;
    private Vector3 nextPos;
    public int rowsAndColumns = 20;
    private int column = 1;
    private int row = 1;
    private float size;
    private float scale;
    public Dictionary<string, Rect> PrefabRect { get; private set; } = new();

    private void Awake()
    {
        size = slotCamera.orthographicSize / (rowsAndColumns / 2f);
        scale = size * 0.65f;
        nextPos = zeroPos = new(-size * (rowsAndColumns / 2f - 0.5f), size * (rowsAndColumns / 2f - 0.85f), 1f);

        GameManager.uiManager.unitRenderTexture = this;
    }

    public Rect LoadRenderTexture(string prefabName)
    {
        if (PrefabRect.ContainsKey(prefabName))
            return PrefabRect[prefabName];
        if (row > rowsAndColumns)
            return Rect.zero;

        var currentScale = scale;
        var currentNextPos = nextPos;
        Addressables.InstantiateAsync(prefabName, transform)
            .Completed += (handle) =>
            {
                var go = handle.Result.gameObject;
                go.transform.localScale *= currentScale;
                go.transform.transform.localPosition = currentNextPos;
                ChangeLayer(go.transform);
            };

        Rect rect = new((column - 1) / (float)rowsAndColumns, 1f - row / (float)rowsAndColumns,
            1f / rowsAndColumns, 1f / rowsAndColumns);
        PrefabRect.Add(prefabName, rect);

        if (column < rowsAndColumns)
        {
            column++;
            nextPos += Vector3.right * size;
        }
        else if (row <= rowsAndColumns)
        {
            column = 1;
            row++;
            nextPos.x = zeroPos.x;
            nextPos += Vector3.down * size;
        }

        return rect;
    }

    private void ChangeLayer(Transform trans)
    {
        trans.gameObject.layer = layer;
        for (int i = 0; i < trans.childCount;i++)
        {
            ChangeLayer(trans.GetChild(i).transform);
        }
    }
}
