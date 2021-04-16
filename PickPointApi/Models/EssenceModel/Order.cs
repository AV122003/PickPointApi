namespace PickPointApi.Models
{
    public class Order : ExternalOrder
    {
        public Order(OrderStatus status)
        {
            this.Status = status;
        }
        public new OrderStatus Status { get; }
    }

}
