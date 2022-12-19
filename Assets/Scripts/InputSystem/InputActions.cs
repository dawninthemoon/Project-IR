using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class InputActions : PlayerActionSet
{
    public PlayerAction Left { get; set; }
    public PlayerAction Right { get; set; }
    public PlayerAction Up { get; set; }
    public PlayerAction Down { get; set; }
    public PlayerAction Jump { get; set; }
    public PlayerAction Attack { get; set; }
    public PlayerAction Throw { get; set; }
    public PlayerOneAxisAction Horizontal { get; set; }
    public PlayerOneAxisAction Vertical { get; set; }

    public static readonly string JumpActionName = "Jump";
    public static readonly string AttackActionName = "Attack";
    public static readonly string ThrowActionName = "Throw";

    public InputActions()
    {
        Left = CreatePlayerAction("Move Left");
        Right = CreatePlayerAction("Move Right");
        Up = CreatePlayerAction("Input Up");
        Down = CreatePlayerAction("Input Down");
        Jump = CreatePlayerAction(JumpActionName);
        Attack = CreatePlayerAction(AttackActionName);
        Throw = CreatePlayerAction(ThrowActionName);
        Horizontal = CreateOneAxisPlayerAction(Left, Right);
        Vertical = CreateOneAxisPlayerAction(Down, Up);
    }
}