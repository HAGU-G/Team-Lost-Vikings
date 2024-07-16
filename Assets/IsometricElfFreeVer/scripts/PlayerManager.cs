using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Transform ItenPoint;//アイテ?の?示開始?
    public Transform ShotPoint;//射出武器の開始?
    public GameObject ItemPrefab;//アイテ?のPrefabスロット
    public GameObject ThrowPrefab;//投適用のPrefabスロット
    public GameObject BowPrefab;//?（矢）のPrefabスロット
    Rigidbody2D rb;
    Animator animator;
    public float moveSpeed = 1f;

    [SerializeField]
    private Transform shotPointTransform = null;
   


    void Start()
    {
        Application.targetFrameRate = 60;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()//方向キ?で向きを決めて押し続けたらWalkに?示
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = (x == 0) ? Input.GetAxisRaw("Vertical") : 0.0f;
            
        if (x != 0 || y != 0)
        {
            animator.SetFloat("x", x);
            animator.SetFloat("y", y);
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }

        StartCoroutine(Action());
        StartCoroutine(Shot());
    }
    IEnumerator Action()//各行動をキ?で再生
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            animator.SetTrigger("Slash");
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            animator.SetTrigger("Guard");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            animator.SetTrigger("Item");//各方向で動きに合わせて薬瓶を上にあげる
            Instantiate(ItemPrefab, ItenPoint.position, transform.rotation);
        }
                              
        if (Input.GetKeyDown(KeyCode.N))
        {
            animator.SetTrigger("Damage");
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            animator.SetTrigger("Dead");
            this.transform.position = new Vector2(0f, -0.12f);
            for (var i = 0; i < 64; i++)
            {
                yield return null;
            }
            this.transform.position = Vector2.zero;
        }
    }
    IEnumerator Shot()//射出武器の選択と?示
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetTrigger("Throw");
            for (var i = 0; i < 30; i++)
            {
                // コル??ン
                yield return null;
            }
            Instantiate(ThrowPrefab, Vector2.zero, Quaternion.identity, shotPointTransform);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.SetTrigger("Bow");
            for (var i = 0; i < 40; i++)
            {
                yield return null;
            }
            Instantiate(BowPrefab, Vector2.zero, Quaternion.identity, shotPointTransform);
        }  
       
    }
}




