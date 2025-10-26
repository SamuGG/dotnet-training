namespace RazorTemplate.Common;

public sealed record SalesOrder(string OrderId, DateTime OrderDate, OrderCustomer Customer, IReadOnlyCollection<OrderLine> OrderLines);
public sealed record OrderCustomer(string FirstName, string LastName);
public sealed record OrderLine(OrderItem OrderItem, int Quantity, decimal Subtotal);
public sealed record OrderItem(string Title, string Subheading, decimal UnitPrice);