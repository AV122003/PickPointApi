namespace PickPointApi.Models
{
    public interface IPostomatRepository
    {
        // Получение информации о постамате
        public Postomat PostomatGet(string number);
    }
}