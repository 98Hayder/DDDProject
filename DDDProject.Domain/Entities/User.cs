namespace DDDProject.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public int RoleID { get; set; }
        public DateTime Created { get; set; }
        public UserRole UserRole { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
