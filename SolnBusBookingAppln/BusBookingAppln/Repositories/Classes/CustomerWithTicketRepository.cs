using BusBookingAppln.Contexts;
using BusBookingAppln.Models.DBModels;

namespace BusBookingAppln.Repositories.Classes
{
    public class CustomerWithTicketRepository : BaseRepository<int, User>
    {
        public CustomerWithTicketRepository(BusBookingContext context) : base(context)
        {
        }

        public override Task<IList<User>> GetAll()
        {
            return base.GetAll();
        }
        public override Task<User> Delete(int key)
        {
            throw new NotImplementedException();
        }

        public override Task<User> GetById(int key)
        {
            throw new NotImplementedException();
        }

        public override Task<User> Update(User entity, int key)
        {
            throw new NotImplementedException();
        }
    }
}
