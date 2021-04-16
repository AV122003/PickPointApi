namespace PickPointApi.Models
{
    public interface IOrderRepository
    {
        // Создание заказа
        public bool OrderAdd(Order order);
        // Изменение заказа
        public bool OrderChange(Order order);
        // Получение информации по заказу
        public Order OrderGet(int orderNumber);
        // Отмена заказа
        public bool OrderСancel(int orderNumber);
    }
}