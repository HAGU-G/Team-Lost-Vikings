using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;


public enum LOCATION
{
    NONE,
    VILLAGE,
    HUNTZONE
}

[JsonObject(MemberSerialization.OptIn)]
public class UnitStats
{
    public StatsData Data { get; private set; }

    [JsonProperty] public int Id { get; private set; }
    [JsonProperty] public int InstanceID { get; private set; }
    public static List<int> existIDs = new();

    public UnitStats() { }

    [JsonConstructor]
    public UnitStats(int instanceId)
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
    /// <summary>
    /// Skill ID, Skill
    /// </summary>
    [JsonProperty] public List<Skill> Skills { get; private set; } = new();
    public Dictionary<int, Buff> Buffs { get; private set; } = new();

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
            float weight;
            switch(Data.Job)
            {
                case UNIT_JOB.WARRIOR:
                    weight = GameManager.playerManager.warriorWeight.Current;
                    break;
                case UNIT_JOB.MAGICIAN:
                    weight = GameManager.playerManager.magicianWeight.Current;
                    break;
                case UNIT_JOB.ARCHER:
                    weight = GameManager.playerManager.archerWeight.Current;
                    break;
                default:
                    weight = 0f;
                    break;
            }

            return Mathf.FloorToInt((GetWeightedStat(BaseStr.Current, StrWeight.Current)
            + GetWeightedStat(BaseWiz.Current, WizWeight.Current)
            + GetWeightedStat(BaseAgi.Current, AgiWeight.Current)) * (1f + (weight / 100f)));
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




    #region MONSTER
    public bool isBoss;
    #endregion

    #region CHARACTER
    [JsonProperty] public LOCATION Location { get; private set; }
    [JsonProperty] public LOCATION NextLocation { get; private set; }
    [JsonProperty][field: SerializeField] public int HuntZoneNum { get; private set; } = -1;

    public event System.Action ArriveVillage;
    public PARAMETER_TYPE parameterType;

    public void SetUpgradeStats()
    {
        BaseStr.SetUpgrade(GameManager.playerManager.unitStr);
        BaseWiz.SetUpgrade(GameManager.playerManager.unitMag);
        BaseAgi.SetUpgrade(GameManager.playerManager.unitAgi);

        CritChance.SetUpgrade(GameManager.playerManager.unitCritChance);
        CritWeight.SetUpgrade(GameManager.playerManager.unitCritWeight);

        BaseHP.SetUpgrade(GameManager.playerManager.unitHp);
        BaseStamina.SetUpgrade(GameManager.playerManager.unitStamina);
        BaseStress.SetUpgrade(GameManager.playerManager.unitMental);
    }

    public void SetLocation(LOCATION location, LOCATION nextLocation = LOCATION.NONE)
    {
        Location = location;
        NextLocation = nextLocation;

        if (Location == LOCATION.NONE && NextLocation != LOCATION.NONE)
            GameManager.unitManager.SpawnOnNextLocation(this);
    }

    public bool SetHuntZone(int huntZoneNum)
    {
        var hm = GameManager.huntZoneManager;
        var ud = hm.UnitDeployment;

        if (hm.IsDeployed(InstanceID, huntZoneNum)
            || ud[huntZoneNum].Count >= hm.HuntZones[huntZoneNum].GetCurrentData().UnitCapacity)
            return false;

        if (HuntZoneNum != huntZoneNum && HuntZoneNum != -1)
            ud[HuntZoneNum].Remove(InstanceID);

        ud[huntZoneNum].Add(InstanceID);
        HuntZoneNum = huntZoneNum;

        if (Location == LOCATION.HUNTZONE)
        {
            var unitOnHunt = objectTransform.GetComponent<UnitOnHunt>();
            if (unitOnHunt.CurrentHuntZone.HuntZoneNum == huntZoneNum)
            {
                unitOnHunt.forceReturn = false;
                unitOnHunt.FSM.ChangeState((int)UnitOnHunt.STATE.IDLE);
            }
            else
            {
                ForceReturn();
            }

        }

        return true;
    }

    public void ResetHuntZone()
    {
        if (HuntZoneNum == -1)
            return;

        GameManager.huntZoneManager.UnitDeployment[HuntZoneNum].Remove(InstanceID);
        HuntZoneNum = -1;

        if (Location == LOCATION.VILLAGE)
        {
            var unitOnVillage = objectTransform.GetComponent<UnitOnVillage>();
            if (unitOnVillage.currentState == UnitOnVillage.STATE.GOTO)
            {
                unitOnVillage.VillageFSM.ChangeState((int)UnitOnVillage.STATE.IDLE);
            }
        }
    }

    public void ForceReturn()
    {
        if (Location != LOCATION.HUNTZONE)
            return;

        objectTransform.GetComponent<UnitOnHunt>().forceReturn = true;
    }

    public void OnArrived()
    {
        ArriveVillage?.Invoke();
    }
    #endregion



    //Methods
    public virtual void InitStats(StatsData data, bool doGacha = true)
    {
        if (doGacha)
            GachaDefaultStats(data);

        SetConstantStats(data);

        if (doGacha && data.UnitType == UNIT_TYPE.CHARACTER)
            RegisterCharacter();
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

        foreach (var skill in Skills)
        {
            skill.UpdateEllipsePosition(pos);
        }
    }

    public void UpdateTimers(float deltaTime)
    {
        if (AttackTimer < AttackSpeed.Current)
        {
            AttackTimer += deltaTime;
        }
        UpdateBuff(deltaTime);
    }

    public void Collision(GridMap gridMap, params Unit[] others)
    {
        foreach (var other in others)
        {
            if (other == null || other.stats == this)
                continue;

            var collisionDepth = SizeEllipse.CollisionDepthWith(other.stats.SizeEllipse);
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

        Skills.Clear();
        if (data.SkillpoolId1 != 0)
            Skills.Add(new(DataTableManager.skillTable.GetData(data.SkillpoolId1), this));
        if (data.SkillpoolId2 != 0)
            Skills.Add(new(DataTableManager.skillTable.GetData(data.SkillpoolId2), this));
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


    //Buff

    public void ApplyBuff(Buff buff)
    {
        if (buff.type == STAT_TYPE.NONE)
            return;

        if (Buffs.ContainsKey(buff.id))
            RemoveBuff(buff);

        Buffs.Add(buff.id, buff);

        switch (buff.type)
        {
            case STAT_TYPE.STR:
                buff.Apply(BaseStr);
                break;
            case STAT_TYPE.WIZ:
                buff.Apply(BaseWiz);
                break;
            case STAT_TYPE.AGI:
                buff.Apply(BaseAgi);
                break;
            case STAT_TYPE.VIT:
                buff.Apply(BaseVit);
                break;
            case STAT_TYPE.CRIT_CHANCE:
                buff.Apply(CritChance);
                break;
            case STAT_TYPE.CRIT_WEIGHT:
                buff.Apply(CritWeight);
                break;
            case STAT_TYPE.HP:
                buff.Apply(BaseHP);
                ResetMaxParameter();
                break;
            case STAT_TYPE.STAMINA:
                buff.Apply(BaseStamina);
                ResetMaxParameter();
                break;
            case STAT_TYPE.MENTAL:
                buff.Apply(BaseStress);
                ResetMaxParameter();
                break;
            default:
                break;
        }

    }

    private void UpdateBuff(float deltaTime)
    {
        List<Buff> remove = new();
        foreach (var buff in Buffs.Values)
        {
            buff.Update(deltaTime);
            if (buff.Timer < 0f)
                remove.Add(buff);
        }

        foreach (var buff in remove)
        {
            RemoveBuff(buff);
        }
    }

    public void RemoveBuff(Buff buff)
    {
        if (!Buffs.ContainsKey(buff.id)
            || buff.type == STAT_TYPE.NONE)
            return;

        Buffs.Remove(buff.id);

        switch (buff.type)
        {
            case STAT_TYPE.STR:
                buff.Remove(BaseStr);
                break;
            case STAT_TYPE.WIZ:
                buff.Remove(BaseWiz);
                break;
            case STAT_TYPE.AGI:
                buff.Remove(BaseAgi);
                break;
            case STAT_TYPE.VIT:
                buff.Remove(BaseVit);
                break;
            case STAT_TYPE.CRIT_CHANCE:
                buff.Remove(CritChance);
                break;
            case STAT_TYPE.CRIT_WEIGHT:
                buff.Remove(CritWeight);
                break;
            case STAT_TYPE.HP:
                buff.Remove(BaseHP);
                ResetMaxParameter();
                break;
            case STAT_TYPE.STAMINA:
                buff.Remove(BaseStamina);
                ResetMaxParameter();
                break;
            case STAT_TYPE.MENTAL:
                buff.Remove(BaseStress);
                ResetMaxParameter();
                break;
            default:
                break;
        }
    }
}
