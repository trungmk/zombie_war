using System;
using System.Collections.Generic;

public abstract class CompositeNode : Node
{
    public List<Node> Childs = new List<Node>();

    public void AddChild(Node child)
    {
        Childs.Add(child);
    }

    public override NodeState Run()
    {
        // will be implement on derivered classes
        throw new NotImplementedException();
    }
}