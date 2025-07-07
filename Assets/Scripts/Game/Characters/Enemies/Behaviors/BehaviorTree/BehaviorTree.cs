public class BehaviorTree
{
    public Node RootNode;
    public virtual void Tick()
    {
        RootNode.Run();
    }
}