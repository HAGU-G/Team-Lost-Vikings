using Newtonsoft.Json;
using UnityEngine;


[System.Serializable,JsonObject(MemberSerialization.OptIn)]
public abstract class Stats
{
    [JsonProperty]
    [field: SerializeField]
    public int Id { get; protected set; }

    [field: SerializeField] public string Name { get; protected set; }
    [field: SerializeField] public string AssetFileName { get; set; }

    //Parameters
    [JsonProperty]
    [field: SerializeField] 
    public Parameter HP { get; set; } = new();

    //Stats
    [field: SerializeField] public StatFloat MoveSpeed { get; protected set; } = new();
    [field: SerializeField] public StatFloat UnitSize { get; protected set; } = new();
    [field: SerializeField] public StatFloat RecognizeRange { get; protected set; } = new();
    [field: SerializeField] public StatFloat PresenseRange { get; protected set; } = new();
    [field: SerializeField] public StatFloat AttackRange { get; protected set; } = new();
    [field: SerializeField] public StatFloat AttackSpeed { get; protected set; } = new();
    [field: SerializeField] public int CombatPoint { get; protected set; }

    public Transform objectTransform = null;
    public Ellipse SizeEllipse { get; set; } = null;
    public Ellipse RecognizeEllipse { get; set; } = null;
    public Ellipse PresenseEllipse { get; set; } = null;
    public Ellipse BasicAttackEllipse { get; set; } = null;

    public float AttackTimer { get; set; }


    //Methods
    public void ResetEllipse(Transform transform)
    {
        objectTransform = transform;

        var pos = transform.position;

        SizeEllipse ??= new();
        RecognizeEllipse ??= new();
        PresenseEllipse ??= new();
        BasicAttackEllipse ??= new();

        SizeEllipse.SetAxies(UnitSize.Current, pos);
        RecognizeEllipse.SetAxies(RecognizeRange.Current, pos);
        PresenseEllipse.SetAxies(PresenseRange.Current, pos);
        BasicAttackEllipse.SetAxies(AttackRange.Current, pos);
    }

    public void UpdateEllipsePosition()
    {
        if (objectTransform == null)
            return;

        var pos = objectTransform.position;
        SizeEllipse.position = pos;
        RecognizeEllipse.position = pos;
        PresenseEllipse.position = pos;
        BasicAttackEllipse.position = pos;
    }

    public void UpdateAttackTimer()
    {
        if (AttackTimer < AttackSpeed.Current)
        {
            AttackTimer += Time.deltaTime;
        }
    }

    public void Collision(Stats other, GridMap grid = null)
    {
        var collisionDepth = SizeEllipse.CollisionDepthWith(other.SizeEllipse);
        if (collisionDepth >= 0f)
        {
            var prePos = objectTransform.position;
            objectTransform.position -= (other.objectTransform.position - objectTransform.position).normalized * collisionDepth;

            if (grid != null
                && grid.PosToIndex(objectTransform.position) == Vector2Int.one * -1)
            {
                objectTransform.position = prePos;
            }

            UpdateEllipsePosition();
        }
    }


    public virtual void ResetStats()
    {
        HP.Reset();
    }

    /// <param name="correctionFunc">null일 경우 내림 처리</param>
    public static int GetWeightedStat(int value, float weight, System.Func<float, int> correctionFunc = null)
    {
        correctionFunc ??= Mathf.FloorToInt;

        return correctionFunc(value * weight);
    }
}
