using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Interfqce
public class BaseState
{
    public string name;
    protected StateMachine stateMachine;

    public BaseState(string name, StateMachine stateMachine)
    {
        this.name = name;
        this.stateMachine = stateMachine;
    }


    public virtual void Enter() { }
    public virtual void UpdateLogic() { }
    public virtual void UpdatePhysics() { }
    public virtual void Exit() { }
}


// States
public class Idle : BaseState
{
    private MovementSM _sm;
    private Vector2 _dir;
    public Idle(MovementSM stateMachine) : base("Idle", stateMachine)
    {
        _sm = stateMachine;
    }


    public override void Enter()
    {
        base.Enter();
        _dir.x = 0;
        _dir.y = 0;
    }


    public override void UpdateLogic()
    {
        base.UpdateLogic();
        _dir.x = Input.GetAxis("Horizontal");
        _dir.y = Input.GetAxis("Vertical");
        if (Mathf.Abs(_dir.x) > Mathf.Epsilon || Mathf.Abs(_dir.y) > Mathf.Epsilon)
        {
            _sm.ChangeState(_sm.movingState);
        }
    }

}

public class Moving : BaseState
{

    private MovementSM _sm;
    

    private Vector2 _dir;


    public Moving(MovementSM stateMachine) : base("Moving", stateMachine)
    {
        _sm = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        _dir.x = 0;
        _dir.y = 0;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        _dir.x = Input.GetAxis("Horizontal");
        _dir.y = Input.GetAxis("Vertical");
        if (Mathf.Abs(_dir.x) < Mathf.Epsilon && Mathf.Abs(_dir.y) < Mathf.Epsilon)
        {
            _sm.ChangeState(_sm.idleState);
        }
    }


    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        _dir.x = Input.GetAxis("Horizontal");
        _dir.y = Input.GetAxis("Vertical");
        _sm.rgbd.velocity = _dir * _sm.speed;

    }

}




// State Handler Logic
public class StateMachine : MonoBehaviour
{
    BaseState currentState;

    void Start()
    {
        currentState = GetInitialState();
        if (currentState != null)
            currentState.Enter();
    }

    void Update()
    {
        if (currentState != null)
            currentState.UpdateLogic();
    }

    void LateUpdate()
    {
        if (currentState != null)
            currentState.UpdatePhysics();
    }

    public void ChangeState(BaseState newState)
    {
        currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    protected virtual BaseState GetInitialState()
    {
        return null;
    }


}


//State Handler
public class MovementSM : StateMachine
{
    [HideInInspector]
    public Rigidbody2D rgbd;

    [SerializeField]
    public float speed = 5f;
    [HideInInspector]
    public Idle idleState;
    [HideInInspector]
    public Moving movingState;

    private void Awake()
    {
        rgbd = GetComponent<Rigidbody2D>();
        idleState = new Idle(this);
        movingState = new Moving(this);
    }

    protected override BaseState GetInitialState()
    {
        return idleState;
    }
}