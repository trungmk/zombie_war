public enum PlayerState : byte
{
    Idle,
    Move,
    Shoot,
    ThrowGrenade,
    Die,
    MoveAndShoot,
    MoveAndGrenade
}

public enum PlayerAnimationState : byte
{
    Idle = 0,
    Move = 1,
    Shoot = 2,
    ThrowGrenade = 3,
    Die = 4
}