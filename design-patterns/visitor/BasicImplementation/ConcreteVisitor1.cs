namespace DesignPatterns.Visitor.BasicImplementation;

public class ConcreteVisitor1 : Visitor
{
    public void Visit(ConcreteElement1 element1)
    {
        string message = element1.Operation1();
        Console.WriteLine($"Concrete visitor 1: '{message}'");
    }

    public void Visit(ConcreteElement2 element2)
    {
        string message = element2.Operation2();
        Console.WriteLine($"Concrete visitor 1: '{message}'");
    }
}