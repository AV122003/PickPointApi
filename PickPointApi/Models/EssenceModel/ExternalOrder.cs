using System;

namespace PickPointApi.Models
{
    [Serializable]
    public class ExternalOrder
    {
        // Номер заказа
        public int Number { get; set; }
        // Статус заказа
        public OrderStatus Status { get; set; }
        // Состав заказа: массив товаров
        public string[] Products { get; set; }
        // Стоимость заказа
        public decimal Cost { get; set; }
        // Номер постамата доставки(int)
        public int PostomatNumber { get; set; }
        // Номер телефона получателя(string)
        public string UserPhone { get; set; }
        // ФИО получателя(string)
        public string UserFIO { get; set; }
    }
    public enum OrderStatus : int
    {
        Зарегистрирован = 1,
        Принят_на_складе = 2,
        Выдан_курьеру = 3,
        Доставлен_в_постамат = 4,
        Доставлен_получателю = 5,
        Отменен = 6
    }
}
