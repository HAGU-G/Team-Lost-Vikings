using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class UnitOnDungeon : Unit, IDamagedable
{
    private IAttackStrategy attackBehaviour = new AttackDefault();
    private IDamagedable attackTarget;

    private UNIT_STATE_TYPE state;

    #region EVENT
    public event Action OnDamaged;
    #endregion

    protected override void ResetUnit()
    {
        base.ResetUnit();

        //TESTCODE
        OnDamaged += () => { Debug.Log(stats.CurrentHP); };
        StartCoroutine(CoTest());
    }


    //TESTCODE
    public IEnumerator CoTest()
    {
        using (var request = UnityWebRequest.Get("google.com"))
        {
            var webResponseTime = DateTime.Now.Ticks;
            yield return request.SendWebRequest();
            if (request.error != null)
            {
                Debug.Log(request.error);
            }
            else
            {
                string date = request.GetResponseHeader("date");
                webResponseTime -= DateTime.Now.Ticks;
                webResponseTime *= -1;

                var dateTime = DateTime.Parse(date).ToLocalTime();
                Debug.Log($"서버 시간 {dateTime}");

                var deviceTime = DateTime.Now.ToLocalTime();
                Debug.Log($"장치 시간 {deviceTime}");
                Debug.Log($"차이 {dateTime - deviceTime}");
                Debug.Log($"보정된 시간 {dateTime.AddTicks(webResponseTime)}");
            }
        }
    }

    protected void Awake()
    {
        ResetUnit();
    }




    public void TakeDamage(int damage)
    {
        stats.CurrentHP -= damage;
        OnDamaged?.Invoke();
    }

    protected void TryAttack()
    {
        if (attackTarget == null)
            return;
        attackBehaviour.Attack(stats.AttackDamage, attackTarget);
    }


}
