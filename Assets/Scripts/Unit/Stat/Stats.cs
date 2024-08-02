using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;


[JsonObject(MemberSerialization.OptIn)]
public abstract class Stats
{
    public StatsData Data { get; private set; }

    [JsonProperty] public int Id { get; private set; }
    [JsonProperty] public int InstanceID { get; private set; }
    public static List<int> existIDs = new();

    public Stats() { }

    [JsonConstructor]
    public Stats(int instanceId)
    {
        InstanceID = instanceId;
        existIDs.Add(instanceId);
    }

    public void RegisterCharacter()
    {
        InstanceID = System.DateTime.Now.GetHashCode();
        while (existIDs.Contains(InstanceID))
        {
            InstanceID++;
        }
        existIDs.Add(InstanceID);
    }

    //Parameter
    [JsonProperty] public Parameter HP { get; private set; } = new();
    [JsonProperty] public Parameter Stamina { get; private set; } = new();
    [JsonProperty] public Parameter Stress { get; private set; } = new();
    public StatInt BaseHP { get; private set; } = new();
    public StatInt BaseStamina { get; private set; } = new();
    public StatInt BaseStress { get; private set; } = new();

    //Stat
    [JsonProperty] public StatInt BaseVit { get; private set; } = new();
    [JsonProperty] public StatInt BaseStr { get; private set; } = new();
    [JsonProperty] public StatInt BaseWiz { get; private set; } = new();
    [JsonProperty] public StatInt BaseAgi { get; private set; } = new();
    public StatFloat VitWeight { get; private set; } = new();
    public StatFloat StrWeight { get; private set; } = new();
    public StatFloat WizWeight { get; private set; } = new();
    public StatFloat AgiWeight { get; private set; } = new();

    public StatFloat CritChance { get; private set; } = new();
    public StatFloat CritWeight { get; private set; } = new();

    public StatInt PhysicalDef { get; private set; } = new();
    public StatInt MagicalDef { get; private set; } = new();
    public StatInt SpecialDef { get; private set; } = new();


    //Skill
    [JsonProperty] public int SkillId1 { get; private set; }
    [JsonProperty] public int SkillId2 { get; private set; }


    //Ellipse
    public StatFloat SizeRange { get; private set; } = new();
    public StatFloat PresenseRange { get; private set; } = new();
    public StatFloat RecognizeRange { get; private set; } = new();
    public StatFloat BasicAttackRange { get; private set; } = new();
    public Ellipse SizeEllipse { get; set; } = null;
    public Ellipse PresenseEllipse { get; set; } = null;
    public Ellipse RecognizeEllipse { get; set; } = null;
    public Ellipse BasicAttackEllipse { get; set; } = null;

    public Transform objectTransform = null;


    //Combat
    public StatFloat MoveSpeed { get; private set; } = new();
    public StatFloat AttackSpeed { get; private set; } = new();
    public int CombatPoint
    {
        get
        {
            return GetWeightedStat(BaseStr.Current, StrWeight.Current)
            + GetWeightedStat(BaseWiz.Current, WizWeight.Current)
            + GetWeightedStat(BaseAgi.Current, AgiWeight.Current);
        }
    }
    public float AttackTimer { get; set; }


    //Etc
    public UNIT_GRADE UnitGrade
    {
        get
        {
            var overroll = BaseStr.defaultValue
                + BaseWiz.defaultValue
                + BaseAgi.defaultValue
                + BaseVit.defaultValue;

            return overroll switch
            {
                _ when (overroll >= GameSetting.Instance.overrollUltraRare) => UNIT_GRADE.ULTRA_RARE,
                _ when (overroll >= GameSetting.Instance.overrollSuperRare) => UNIT_GRADE.SUPER_RARE,
                _ when (overroll >= GameSetting.Instance.overrollRare) => UNIT_GRADE.RARE,
                _ when (overroll >= GameSetting.Instance.overrollNormal) => UNIT_GRADE.NORMAL,
                _ => UNIT_GRADE.COMMON,
            };
        }
    }


    //Methods
    public virtual void InitStats(StatsData data, bool doGacha = true)
    {
        if (doGacha)
            GachaDefaultStats(data);

        SetConstantStats(data);
    }

    public virtual void ResetStats()
    {
        ResetMaxParameter();
        HP.Reset();
        Stamina.Reset();
        Stress.Reset();
    }


    public void ResetEllipse(Transform transform)
    {
        objectTransform = transform;

        var pos = transform.position;

        SizeEllipse ??= new();
        RecognizeEllipse ??= new();
        PresenseEllipse ??= new();
        BasicAttackEllipse ??= new();

        SizeEllipse.SetAxies(SizeRange.Current, pos);
        RecognizeEllipse.SetAxies(RecognizeRange.Current, pos);
        PresenseEllipse.SetAxies(PresenseRange.Current, pos);
        BasicAttackEllipse.SetAxies(BasicAttackRange.Current, pos);
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

    public void Collision(GridMap gridMap, params Unit[] others)
    {
        foreach (var other in others)
        {
            if (other == null || other.GetStats == this)
                continue;

            var collisionDepth = SizeEllipse.CollisionDepthWith(other.GetStats.SizeEllipse);
            if (collisionDepth >= 0f)
            {
                var prePos = objectTransform.position;
                objectTransform.position -= (other.transform.position - objectTransform.position).normalized * collisionDepth;

                if (gridMap != null
                    && gridMap.PosToIndex(objectTransform.position) == Vector2Int.one * -1)
                {
                    objectTransform.position = prePos;
                }

                UpdateEllipsePosition();
            }
        }
    }

    /// <param name="correctionFunc">null일 경우 내림 처리</param>
    public static int GetWeightedStat(int value, float weight, System.Func<float, int> correctionFunc = null)
    {
        correctionFunc ??= Mathf.FloorToInt;

        return correctionFunc(value * weight);
    }

    public void ResetMaxParameter()
    {
        HP.max = BaseHP.Current + GetWeightedStat(BaseVit.Current, VitWeight.Current);
        if (HP.Current > HP.max)
            HP.Current = HP.max;

        Stamina.max = BaseStamina.Current;
        if (Stamina.Current > Stamina.max)
            Stamina.Current = Stamina.max;

        Stress.max = BaseStress.Current;
        if (Stress.Current > Stress.max)
            Stress.Current = Stress.max;
    }


    //Gacha
    protected void GachaDefaultStats(StatsData data)
    {
        BaseVit.defaultValue = Random.Range(data.MinBaseVit, data.MaxBaseVit + 1);
        BaseStr.defaultValue = Random.Range(data.MinBaseStr, data.MaxBaseStr + 1);
        BaseWiz.defaultValue = Random.Range(data.MinBaseWiz, data.MaxBaseWiz + 1);
        BaseAgi.defaultValue = Random.Range(data.MinBaseAgi, data.MaxBaseAgi + 1);
        SkillId1 = data.SkillpoolId1;
        SkillId2 = data.SkillpoolId2;
    }

    protected void SetConstantStats(StatsData data)
    {
        Data = data;
        Id = data.Id;

        //Stats
        BaseHP.defaultValue = data.BaseHP;
        BaseStamina.defaultValue = data.BaseStamina;
        BaseStress.defaultValue = data.BaseMental;

        VitWeight.defaultValue = data.VitWeight;
        StrWeight.defaultValue = data.StrWeight;
        WizWeight.defaultValue = data.WizWeight;
        AgiWeight.defaultValue = data.AgiWeight;

        CritChance.defaultValue = data.CritChance;
        CritWeight.defaultValue = data.CritWeight;

        PhysicalDef.defaultValue = data.PhysicalDef;
        MagicalDef.defaultValue = data.MagicalDef;
        SpecialDef.defaultValue = data.SpecialDef;

        SizeRange.defaultValue = data.SizeRange;
        RecognizeRange.defaultValue = data.RecognizeRange;
        PresenseRange.defaultValue = data.PresenseRange;
        BasicAttackRange.defaultValue = data.BasicAttackRange;

        MoveSpeed.defaultValue = data.MoveSpeed;
        AttackSpeed.defaultValue = data.AttackSpeed;


        ResetMaxParameter();
    }



}
