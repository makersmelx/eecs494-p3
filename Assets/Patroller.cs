using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Patroller : MonoBehaviour
{

    public GameObject Player;
    private Transform Target;
    public float AttackRadius;

    [SerializeField] float Timer;
    public float WaitTime;
    public float ChaseTime;

    public Transform[] Destinations;
    private int CurrentPoint = 0;

    private NavMeshAgent NavMesh;
    private bool InRange = false;
    private bool InAggro = true;


    void Start()
    {
        NavMesh = GetComponent<NavMeshAgent>();
    }


    private void FixedUpdate()
    {
        Target = Player.transform;
        float DistToTarget = Vector3.Distance(Target.position, transform.position);

        // Aim the player, lock the player for 3 seconds 
        if (DistToTarget <= AttackRadius && InAggro)
        {
            Timer += Time.deltaTime;
            
            // Drop aggro when chasing for enough time
            if (Timer > ChaseTime)
            {
                StartCoroutine(DropAggro(10f));
            }

            // Dash towards player
            if (Timer > WaitTime)
            {
                InRange = true;
                transform.LookAt(Target);
                Vector3 MoveTo = Vector3.MoveTowards(transform.position, Target.position, 100f);
                NavMesh.destination = MoveTo;

                // Attack player when close enough
                if (DistToTarget < 1f)
                {
                    Player.GetComponent<KnockBack>().KnockBackPlayer(Vector3.Normalize(MoveTo), 30f);
                    StartCoroutine(DropAggro(10f));
                }

            }
        }

        else
        {
            InRange = false;
            BackToPath();
        }
    }

    void BackToPath()
    {
        if (!InRange && NavMesh.remainingDistance < 0.5f)
        {
            NavMesh.destination = Destinations[CurrentPoint].position;
            UpdateCurrentPoint();
        }
    }

    void UpdateCurrentPoint()
    {
        CurrentPoint++;
        CurrentPoint %= Destinations.Length;
    }

    IEnumerator DropAggro(float Delay)
    {
        Debug.Log("Dropped Aggro");
        Timer = 0f;
        InRange = false;
        InAggro = false;
        yield return new WaitForSeconds(Delay);
        InAggro = true;
    }
  

  
}
