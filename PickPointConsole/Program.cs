using System;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace PickPointConsole
{
    class Program
    {
        readonly static string host = "https://localhost:44309";

        static void Main(string[] args)
        {
            ExternalOrder[] orders = new ExternalOrder[]
            {
                new ExternalOrder()
                {
                    Number = 1,
                    Status = OrderStatus.Зарегистрирован,
                    Products = new string[] { "продкут1", "продукт2" },
                    UserPhone = @"+7123-456-78-90",
                    PostomatNumber = 1
                },
                new ExternalOrder()
                {
                    Number = 2,
                    Status = OrderStatus.Зарегистрирован,
                    Products = new string[] { "продкут1", "продукт2" },
                    UserPhone = @"+7123-456-78-90",
                    PostomatNumber = 2
                },
                new ExternalOrder()
                {
                    Number = 3,
                    Status = OrderStatus.Зарегистрирован,
                    Products = new string[] { "продкут1", "продукт2" },
                    UserPhone = @"+7123-456-78-90",
                    PostomatNumber = 3
                }
            };

            // Создание заказа
            foreach (var order in orders)
            {
                var code1 = OrderAdd(order).GetAwaiter().GetResult();
            }
            // Изменение заказа
            orders[0].Products = new string[] { "продкут1", "продукт3" };
            orders[0].UserPhone = @"+7123-456-78-00";
            var code2 = OrderChange(orders[0]).GetAwaiter().GetResult();

            // Отмена заказа
            code2 = OrderCancel(1).GetAwaiter().GetResult();

            // Получение информации по заказу
            ExternalOrder externalOrder = OrderGet(2).GetAwaiter().GetResult();
        }

        private static async Task<System.Net.HttpStatusCode> OrderAdd(ExternalOrder externalOrder)
        {
            System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.OK;
            var jsonOrder = await Task.Run(() => JsonSerializer.Serialize(externalOrder));
            var httpContent = new StringContent(jsonOrder, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.PostAsync(host + "/api/order/OrderAdd", httpContent);
                statusCode = httpResponse.StatusCode;
                if (statusCode == System.Net.HttpStatusCode.OK && httpResponse.Content != null)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    bool ok = JsonSerializer.Deserialize<bool>(responseContent);
                }
            }

            return await Task.Run(() =>
            {
                return statusCode;
            });
        }

        private static async Task<System.Net.HttpStatusCode> OrderChange(ExternalOrder externalOrder)
        {
            System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.OK;
            var jsonOrder = await Task.Run(() => JsonSerializer.Serialize(externalOrder));
            var httpContent = new StringContent(jsonOrder, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.PostAsync(host + "/api/order/OrderChange", httpContent);
                statusCode = httpResponse.StatusCode;
                if (statusCode == System.Net.HttpStatusCode.OK && httpResponse.Content != null)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    bool ok = JsonSerializer.Deserialize<bool>(responseContent);
                }
            }

            return await Task.Run(() =>
            {
                return statusCode;
            });
        }

        private static async Task<System.Net.HttpStatusCode> OrderCancel(int orderNumber)
        {
            System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.OK;
            var jsonOrder = await Task.Run(() => JsonSerializer.Serialize(orderNumber));
            var httpContent = new StringContent(jsonOrder, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.PostAsync(host + "/api/order/OrderCancel", httpContent);
                statusCode = httpResponse.StatusCode;
                if (statusCode == System.Net.HttpStatusCode.OK && httpResponse.Content != null)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    bool ok = JsonSerializer.Deserialize<bool>(responseContent);
                }
            }

            return await Task.Run(() =>
            {
                return statusCode;
            });
        }

        private static async Task<ExternalOrder> OrderGet(int orderNumber)
        {
            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.GetAsync(host + "/api/order/OrderGet?orderNumber=" + orderNumber.ToString());
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK && httpResponse.Content != null)
                {
                    string responseContent = await httpResponse.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = true
                    };

                    ExternalOrder externalOrder = JsonSerializer.Deserialize<ExternalOrder>(responseContent, options);

                    return await Task.Run(() =>
                    {
                        return externalOrder;
                    });
                }
            }
            return await Task.Run(() =>
            {
                return new ExternalOrder();
            });
        }
    }

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

    [Serializable]
    public class Postomat
    {
        public Postomat(string number)
        {
            this.Number = number;
        }
        // Номер
        public string Number { get; }
        // Адрес постамата
        public string Address { get; set; }
        // Статус постамата(bool, Рабочий = true, иначе закрыт)
        public bool Status { get; set; }
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
