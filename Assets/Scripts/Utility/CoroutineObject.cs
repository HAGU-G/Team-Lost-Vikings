using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour를 상속받지 않았을 때 코루틴을 사용할 수 있도록 만든 클래스
/// CreateCoroutine을 하면 게임오브젝트를 새로 만들어서 코루틴 실행
/// </summary>
public class CoroutineObject : MonoBehaviour
{
    public static List<CoroutineObject> coroutines = new();
    public static readonly string objectName = "CorutineObject {0}";

    private bool isUsing = true;
    private IEnumerator enumerator;
    private Coroutine coroutine;
    public bool IsStopped { get; private set; } = false;

    public static CoroutineObject CreateCorutine(IEnumerator routine)
    {
        CoroutineObject coroutineOject = coroutines.Find(x => !x.isUsing);

        if (coroutineOject == null)
        {
            coroutineOject = new GameObject(string.Format(objectName, coroutines.Count), typeof(CoroutineObject))
                .GetComponent<CoroutineObject>();
            coroutines.Add(coroutineOject);
        }
        else
        {
            coroutineOject.isUsing = true;
            coroutineOject.gameObject.SetActive(true);
        }

        coroutineOject.StartCo(routine);
        return coroutineOject;

    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void FixedUpdate()
    {
        if (IsStopped)
            return;

        if (enumerator != null && enumerator.Current == null)
        {
            enumerator = null;
            IsStopped = true;
        }
    }

    /// <summary>
    /// IsStoped가 true일때만 호출하는 것을 권장함.
    /// </summary>
    public void StartCo(IEnumerator routine)
    {
        IsStopped = false;
        enumerator = routine;
        coroutine = StartCoroutine(routine);
    }

    public void StopCo()
    {
        StopCoroutine(coroutine);
        IsStopped = true;
        enumerator = null;
        coroutine = null;
    }

    public void Release()
    {
        isUsing = false;
        IsStopped = true;
        enumerator = null;
        coroutine = null;
        gameObject.SetActive(false);
    }

}