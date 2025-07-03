public class SelectorNode : CompositeNode
{
    public SelectorNode(params Node[] nodes)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            AddChild(nodes[i]);
        }
    }

    public override NodeState Run()
    {
        // no child -> then failed (sometime designer forgot to add Leaf node in tree)
        if (Childs.Count == 0) return NodeState.Failure;
        // evaluate childs
        for (int n = 0, amount = Childs.Count; n < amount; n++)
        {
            var state = Childs[n].Run();
            // break when reach the first child with status = Sucess or Running (not Failure)
            if (state != NodeState.Failure) return state;
        }
        // no child success or running, then return failed
        return NodeState.Failure;
    }
}