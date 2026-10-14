using SchoolManager.Data.Entities;
using SchoolManager.Helpers;

namespace SchoolManager.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(
            DataContext context,
            IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await CreateRolesAsync();
            await CreateAdminAsync();
            await CreateSecretariaAsync();
            await CreateProfessorAsync();
            await CreateUtilizadorAsync();
        }

        private async Task CreateRolesAsync()
        {
            string[] roles =
            {
                "Admin",
                "Secretaria",
                "Professor",
                "Utilizador"
            };

            foreach (var role in roles)
            {
                if (!await _userHelper.CheckRoleAsync(role))
                {
                    await _userHelper.AddRoleAsync(role);
                }
            }
        }

        private async Task CreateAdminAsync()
        {
            const string email = "admin@schoolmanager.pt";

            var user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    FirstName = "Administrador",
                    LastName = "SchoolManager",
                    Email = email,
                    UserName = email
                };

                var result = await _userHelper.AddUserAsync(
                    user,
                    "Admin123!");

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Não foi possível criar o utilizador administrador.");
                }
            }

            if (!await _userHelper.IsUserInRoleAsync(user, "Admin"))
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }
        }
        private async Task CreateSecretariaAsync()
        {
            var email = "secretaria@schoolmanager.pt";

            var user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    FirstName = "Secretaria",
                    LastName = "SchoolManager",
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true
                };

                var result = await _userHelper.AddUserAsync(
                    user,
                    "Secretaria123!");

                if (result.Succeeded)
                {
                    await _userHelper.AddUserToRoleAsync(
                        user,
                        "Secretaria");
                }
            }
        }
        private async Task CreateProfessorAsync()
        {
            var email = "professor@schoolmanager.pt";

            var user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    FirstName = "Professor",
                    LastName = "SchoolManager",
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true
                };

                var result = await _userHelper.AddUserAsync(
                    user,
                    "Professor123!");

                if (result.Succeeded)
                {
                    await _userHelper.AddUserToRoleAsync(
                        user,
                        "Professor");
                }
            }
        }
        private async Task CreateUtilizadorAsync()
        {
            var email = "utilizador@schoolmanager.pt";

            var user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    FirstName = "Utilizador",
                    LastName = "SchoolManager",
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true
                };

                var result = await _userHelper.AddUserAsync(
                    user,
                    "Utilizador123!");

                if (result.Succeeded)
                {
                    await _userHelper.AddUserToRoleAsync(
                        user,
                        "Utilizador");
                }
            }
        }
    }
}
