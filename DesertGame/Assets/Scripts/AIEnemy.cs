using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class AIEnemy : MonoBehaviour
{
    AIDestinationSetter chaser;
    AIPath aiPath;
    Transform player;
    protected enum State
    { 
        idle,
        searching,
        chasing
    }
    State currentState;

    [SerializeField] float alertRange = 10;
    [SerializeField] float chasingRange = 15;
    Vector3 posToDefend;

    private void Awake()
    {
        chaser = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
    }
    protected virtual void Start()
    {
        currentState = State.idle;
        posToDefend = transform.localPosition;
        player = GameplayController.Get().GetCurrentPlayer().transform;
    }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case State.idle:
                if (CanSeePlayer(alertRange)) Chase();
                break;
            case State.chasing:
                if (!CanSeePlayer(chasingRange)) StartSearching();
                break;
            case State.searching:
                if (CanSeePlayer(chasingRange)) Chase();
                else if (aiPath.reachedEndOfPath)
                {
                    if (aiPath.destination != posToDefend) aiPath.destination = posToDefend;// aiPath.FinalizeMovement(posToDefend, transform.rotation);
                    else SetAsIdle();
                }
                break;
        }
    }

    bool CanSeePlayer(float distanceToCheck)
    {
        return Vector2.Distance(transform.position, player.position) < distanceToCheck;
    }

    protected virtual void Chase()
    {
        currentState = State.chasing;
        chaser.target = player; //poco performante
    }

    protected virtual void StartSearching()
    {
        currentState = State.searching;
        chaser.target = null;
    }

    protected virtual void SetAsIdle()
    {
        currentState = State.idle;
    }
}
