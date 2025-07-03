public class BehaviorTree
{
    public Node RootNode;
    public void Tick()
    {
        RootNode.Run();
    }
}