using System;

namespace PickPointApi.Models
{
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
}
