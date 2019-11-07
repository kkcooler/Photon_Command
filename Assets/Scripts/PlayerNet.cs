using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class PlayerNet : Bolt.EntityBehaviour<IPlayerNetState>
{
    public Renderer render;
    public PlayerMotor motor;

    public bool enableMotorCommand = true;
    public bool enableFlashCommand = true;


    bool _forward;
    bool _backward;
    bool _left;
    bool _right;

    bool _flash;



    public override void Attached()
    {
        // This couples the Transform property of the State with the GameObject Transform
        state.SetTransforms(state.Transform, transform);
    }

    public override void SimulateController()
    {
        PollKeys();

        GenerateMotorCommand();
        GenerateFlashCommand();

    }
    public override void ExecuteCommand(Command command, bool resetState)
    {
        ExecuteMotorCommand(command, resetState);
        ExecuteFlashCommand(command, resetState);
    }


    void PollKeys()
    {
        _forward = Input.GetKey(KeyCode.W);
        _backward = Input.GetKey(KeyCode.S);
        _left = Input.GetKey(KeyCode.A);
        _right = Input.GetKey(KeyCode.D);

        _flash = Input.GetKey(KeyCode.Space);
    }

    void GenerateMotorCommand()
    {
        if (!enableMotorCommand)
            return;

        IMotorCommandInput input = MotorCommand.Create();

        input.Forward = _forward;
        input.Backward = _backward;
        input.Left = _left;
        input.Right = _right;
        entity.QueueInput(input);
    }
    void GenerateFlashCommand()
    {
        if (!enableFlashCommand)
            return;

        var input = FlashCommand.Create();

        input.DoFlash = _flash;

        entity.QueueInput(input);

    }

    void ExecuteMotorCommand(Command command, bool resetState)
    {
        if (command is MotorCommand)
        {
            var cmd = (MotorCommand)command;

            if (resetState)
            {
                // we got a correction from the server, reset (this only runs on the client)
                motor.SetState(cmd);
            }
            else
            {
                // apply movement (this runs on both server and client)
                PlayerMotor.State motorState = motor.Move(cmd);

                // copy the motor state to the commands result (this gets sent back to the client)
                cmd.Result.Position = motorState.position;
                cmd.Result.Velocity = motorState.velocity;
            }
        }
    }
    void ExecuteFlashCommand(Command command, bool resetState)
    {
        if (command is FlashCommand)
        {
            var cmd = (FlashCommand)command;

            render.sharedMaterial.color = cmd.Input.DoFlash ? Color.red : Color.green;
        }
    }
}
