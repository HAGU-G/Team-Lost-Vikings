using System.Collections.Generic;

public class Monster : CombatUnit, ISubject<Monster>
{
    public List<IObserver<Monster>> observer = new();

    public override bool IsNeedReturn => false;


    public void ResetMonster(HuntZone huntZone, bool isBoss = false)
    {
        CurrentHuntZone = huntZone;
        stats ??= new();

        ResetEvents();
        stats.InitStats(isBoss ? huntZone.GetCurrentBoss() : huntZone.GetCurrentMonster());
        stats.ResetStats();
        stats.SetLocation(LOCATION.HUNTZONE);
        stats.isBoss = isBoss;
        attackTarget = null;

        ResetUnit(stats);

        Enemies = CurrentHuntZone.Units;
        Allies = CurrentHuntZone.Monsters;

        FSM.ResetFSM();
    }


    public void Subscribe(IObserver<Monster> observer)
    {
        if (this.observer.Contains(observer))
            return;

        this.observer.Add(observer);
    }

    public void UnSubscrive(IObserver<Monster> observer)
    {
        this.observer.Remove(observer);
    }

    public void SendNotification(NOTIFY_TYPE type, bool removeObservers = false)
    {
        if (observer.Count == 0)
            return;

        for (int i = observer.Count - 1; i >= 0; i--)
        {
            observer[i].ReceiveNotification(this, type);
            if (removeObservers)
                observer.RemoveAt(i);

        }
    }

    public override void RemoveUnit()
    {
        base.RemoveUnit();
        SendNotification(NOTIFY_TYPE.REMOVE, true);
        GameManager.huntZoneManager.ReleaseMonster(this);
    }

    public void DropItem()
    {
        if (stats.Data.DropId == 0)
            return;

        var dropData = DataTableManager.dropTable.GetData(stats.Data.DropId);
        var im = GameManager.itemManager;

        GameManager.playerManager.Exp += dropData.DropExp;
        foreach (var item in dropData.DropItem())
        {
            im.AddItem(item.Key, item.Value);
        }
    }
}
