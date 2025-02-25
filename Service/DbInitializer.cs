namespace ExamProjectOne.Service
{
    public class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            await RoleSeeder.SeedRoles(serviceProvider);
        }
    }
}
