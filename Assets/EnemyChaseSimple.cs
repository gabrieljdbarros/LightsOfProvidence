using UnityEngine;



public class EnemyChaseSimple3D : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 10f;
    public float moveSpeed = 3.5f;
    public float stopDistance = 1.2f;
    public float turnSpeed = 10f;
private Animator anim;

void Start()
{
    if (!player)
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }


    anim = GetComponentInChildren<Animator>(true);

    if (!anim)
        Debug.LogWarning($"{name}: Nenhum Animator encontrado em filhos.");
    else if (anim.runtimeAnimatorController == null)
        Debug.LogWarning($"{name}: Animator encontrado ('{anim.name}'), mas sem Controller ativo!");
}


   void Update()
    {
        if (!player) return;

        Vector3 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;

        bool isMoving = dist <= detectionRadius && dist > stopDistance;

        if (isMoving)
        {
            Vector3 dir = new Vector3(toPlayer.x, 0f, toPlayer.z).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
        }

        if (anim)
        {
            float speedParam = isMoving ? moveSpeed : 0f;
            anim.SetFloat("Speed", speedParam);
        }

        if (anim && anim.runtimeAnimatorController != null)
{
        float speedParam = isMoving ? moveSpeed : 0f;
        anim.SetFloat("Speed", speedParam);
}

    
}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}