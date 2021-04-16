using System.Collections.Generic;
using System.Linq;

namespace PickPointApi.Models
{
    public class OrderRepository : IOrderRepository
    {
        public OrderRepository() { }

        private List<Order> orders = new List<Order>();

        // Создание заказа
        public bool OrderAdd(Order order)
        {
            try
            {
                orders.Add(order);
                return true;
            }
            catch
            {
                return false;
            }

        }

        // Изменение заказа. Статус не меняется, пояснения к заданию ниже. 
        // Опечатки нет: статус заказа не может быть изменен текущим сервисом, так как предполагается, что статус меняется другой системой,
        // которая работает, например, при передаче курьеру в доставку или при закладке курьером в постамат.
        // В рамках данного сервиса эта операция не предусмотрена.
        public bool OrderChange(Order order)
        {
            try
            {
                Order orderOld = orders.Find(o => o.Number == order.Number);
                if (orderOld != null)
                {
                    orderOld.Products = order.Products;
                    orderOld.Cost = order.Cost;
                    orderOld.PostomatNumber = order.PostomatNumber;
                    orderOld.UserPhone = order.UserPhone;
                    orderOld.UserFIO = order.UserFIO;
                    return true;
                }
                else
                    return false;

            }
            catch
            {
                return false;
            }
        }
        // Получение информации по заказу
        public Order OrderGet(int orderNumber)
        {
            try
            {
                return orders.Find(o => o.Number == orderNumber);
            }
            catch
            {
                return null;
            }
        }
        // Отмена заказа == удаление, пояснения к заданию выше.
        public bool OrderСancel(int orderNumber)
        {
            try
            {
                Order orderOld = orders.Find(o => o.Number == orderNumber);
                return orders.Remove(orderOld);
            }
            catch
            {
                return false;
            }
        }
    }
}
