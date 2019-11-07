using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public class State
    {
        public Vector3 position;
        public Vector3 velocity;
    }

    public CharacterController chaCtrl;
    public float speed = 5f;

    protected State _state = new State();

    public State Move(MotorCommand cmd)
    {
        float h = 0,  v = 0;

        if (cmd.Input.Forward ^ cmd.Input.Backward)
            v = cmd.Input.Forward ? 1 : -1;

        if (cmd.Input.Right ^ cmd.Input.Left)
            h = cmd.Input.Right ? 1 : -1;

        _state.velocity = new Vector3(h, 0, v).normalized * speed;
        _state.position = transform.position + _state.velocity * BoltNetwork.FrameDeltaTime;

        chaCtrl.Move(_state.position - transform.position);

        return _state;
    }
    public void SetState(MotorCommand cmd)
    {
        _state.velocity = cmd.Result.Velocity;
        _state.position = cmd.Result.Position;

        chaCtrl.transform.position = _state.position;
    }  
}
