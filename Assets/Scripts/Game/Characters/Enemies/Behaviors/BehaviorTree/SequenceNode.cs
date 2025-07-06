public class SequenceNode : CompositeNode
{
    public SequenceNode(params Node[] nodes)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            AddChild(nodes[i]);
        }
    }

    public override NodeState Run()
    {
        // no child -> then failed (sometime designer forgot to add Leaf nodes)
        if (Childs.Count == 0) return NodeState.Failure;
        // evaluate childs
        for (int n = 0, amount = Childs.Count; n < amount; n++)
        {
            var state = Childs[n].Run();
            // break when reach any child that return Failure or Running
            if (state != NodeState.Success) return state;
        }
        // all child are Success, then return Success
        return NodeState.Success;
    }
}