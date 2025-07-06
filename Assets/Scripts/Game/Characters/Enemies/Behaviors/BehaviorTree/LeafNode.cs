public class LeafNode : Node
{
    /// Declare the delegate, LeafNode will have a reference to the our script's method, to invoke in Run() func
    public delegate NodeState LeafNodeDelegate();

    /// Reference to our function, will be set by framework
    public LeafNodeDelegate function;

    public LeafNode(LeafNodeDelegate func)
    {
        SetFunction(func);
    }

    public void SetFunction(LeafNodeDelegate func)
    {
        this.function = func;
    }

    public override NodeState Run()
    {
        return function();
    }
}