using UnityEngine;

public class StoneManager : MonoBehaviour
{
    private float speed = 2.5f;
   [SerializeField] Transform target;

    public float timer = 1f;

    void Update()// �I�u�W�F�N�g��ڕW?�܂ňړ�������
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        Destroy(gameObject, timer);
    }
    
}

