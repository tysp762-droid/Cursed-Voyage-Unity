using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private bool shouldChase = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
       
        {
            agent.SetDestination(player.position);
        }
    }

    // Deze functie wordt aangeroepen wanneer je op de button klikt
    public void StartChasing()
    {
        shouldChase = true;
    }
}