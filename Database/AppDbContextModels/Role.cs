namespace Database.AppDbContextModels
{
    public class Role : AuditableEntity
    {
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public bool IsSystem { get; set; }
    }
}
