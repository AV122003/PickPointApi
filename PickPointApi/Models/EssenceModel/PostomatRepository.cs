using System.Collections.Immutable;

namespace PickPointApi.Models
{
    public class PostomatRepository : IPostomatRepository
    {
        public PostomatRepository()
        {
            Postomat[] p =
                {
                    new Postomat ("1") { Address = @"Адрес1", Status = true },
                    new Postomat ("2") { Address = @"Адрес2", Status = true },
                    new Postomat ("3") { Address = @"Адрес3", Status = false }
                };

            postomats = p.ToImmutableList();
        }

        private ImmutableList<Postomat> postomats;

        public Postomat PostomatGet(string number)
        {
            try
            {
                return postomats.Find(o => o.Number == number);
            }
            catch
            {
                return null;
            }
        }
    }
}
