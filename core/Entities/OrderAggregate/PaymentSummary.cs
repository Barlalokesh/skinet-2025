using System;

namespace core.Entities.OrderAggregate;

public class PaymentSummary
{
    public int Last4 { get; set; }
    public required string Brand { get; set; } 
    public required int ExpMonth { get; set; } 
    public required int ExpYear { get; set; }  

}
