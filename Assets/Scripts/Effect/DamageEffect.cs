using TMPro;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(SortingLayer))]
public class DamageEffect : MonoBehaviour
{
    public enum TYPE
    {
        NONE,
        DEFAULT,
    }

    public IObjectPool<DamageEffect> pool;

    public Transform textRoot;
    public TextMeshPro text;

    public Animator animator;
    public TYPE type = TYPE.DEFAULT;
    private readonly static int paramType = Animator.StringToHash("Type");
    private readonly static int triggerPlay = Animator.StringToHash("Play");

    private void OnEnable()
    {
        animator.SetFloat(paramType, (int)type);
        animator.SetTrigger(triggerPlay);
    }

    private void OnDisable()
    {
        text.color = Color.white;
        type = TYPE.DEFAULT;

        if (pool == null)
            Destroy(gameObject);
        else
            pool.Release(this);
    }

    private void OnEffectEnd()
    {
        gameObject.SetActive(false);
        textRoot.position = Vector3.zero;
    }

    public void Set(string text, Vector3 position, Color color) => Set(text, position, color, TYPE.DEFAULT);
    public void Set(string text, Vector3 position, Color color, TYPE type)
    {
        transform.position = position;
        this.text.text = text;
        this.text.color = color;
        this.type = type;
        animator.SetFloat(paramType, (int)type);
    }
}
