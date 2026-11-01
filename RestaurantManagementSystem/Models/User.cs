namespace RestaurantManagementSystem.Models
{
    public abstract class User
    {
        public string Name { get; private set; }
        public abstract string Role { get; }

        protected User(string name)
        {
            Name = name;
        }
    }

    public class AdminUser : User
    {
        public override string Role
        {
            get { return "Administrator"; }
        }

        public AdminUser(string name) : base(name)
        {
        }
    }

    public class Customer : User
    {
        public override string Role
        {
            get { return "Customer"; }
        }

        public Customer(string name) : base(name)
        {
        }
    }
}
