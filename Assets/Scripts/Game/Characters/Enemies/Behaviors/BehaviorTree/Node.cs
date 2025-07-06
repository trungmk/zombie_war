public enum NodeState
{
    Success,
    Failure,
    Running
}

public abstract class Node
{
    public Node Parent { get; set; }

    public abstract NodeState Run();
}