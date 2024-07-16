using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    private float speed = 5.0f;
   [SerializeField] Transform target;

    public float timer = 1f;

    void Update()// オブジェクトを目標?まで移動させる
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        Destroy(gameObject, timer);
    }
    
}

