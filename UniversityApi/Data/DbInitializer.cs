using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityApi.Models;

namespace UniversityApi.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            string[] roleNames = { "Admin", "Teacher", "Student" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminEmail = "admin@univ.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createAdmin = await userManager.CreateAsync(adminUser, "Admin123!");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            var teacher1 = await EnsureUserAsync(
                userManager, dbContext, "teacher1@univ.com", "Teacher123!",
                "Teacher", "Иван", "Иванов");
            var teacher2 = await EnsureUserAsync(
                userManager, dbContext, "teacher2@univ.com", "Teacher123!",
                "Teacher", "Петр", "Петров");
            var student1 = await EnsureUserAsync(
                userManager, dbContext, "student1@univ.com", "Student123!",
                "Student", "Анна", "Смирнова");
            var student2 = await EnsureUserAsync(
                userManager, dbContext, "student2@univ.com", "Student123!",
                "Student", "Олег", "Кузнецов");

            if (!await dbContext.Courses.AnyAsync())
            {
                var course1 = new Course
                {
                    Name = "Программирование",
                    Description = "Основы программирования",
                    Credits = 4,
                    TeacherId = teacher1.TeacherId!.Value
                };
                var course2 = new Course
                {
                    Name = "Базы данных",
                    Description = "Проектирование реляционных баз данных",
                    Credits = 3,
                    TeacherId = teacher2.TeacherId!.Value
                };
                dbContext.Courses.AddRange(course1, course2);
                await dbContext.SaveChangesAsync();

                dbContext.Enrollments.Add(new Enrollment
                {
                    CourseId = course1.Id,
                    StudentId = student1.StudentId!.Value
                });
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task<ApplicationUser> EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            string email,
            string password,
            string role,
            string firstName,
            string lastName)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Не удалось создать seed-пользователя {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }

            if (role == "Teacher" && !user.TeacherId.HasValue)
            {
                var teacher = await dbContext.Teachers.FirstOrDefaultAsync(t => t.Email == email);
                if (teacher == null)
                {
                    teacher = new Teacher
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                        Department = "Общий факультет"
                    };
                    dbContext.Teachers.Add(teacher);
                    await dbContext.SaveChangesAsync();
                }

                user.TeacherId = teacher.Id;
                await userManager.UpdateAsync(user);
            }
            else if (role == "Student" && !user.StudentId.HasValue)
            {
                var student = await dbContext.Students.FirstOrDefaultAsync(s => s.Email == email);
                if (student == null)
                {
                    student = new Student
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email
                    };
                    dbContext.Students.Add(student);
                    await dbContext.SaveChangesAsync();
                }

                user.StudentId = student.Id;
                await userManager.UpdateAsync(user);
            }

            return user;
        }
    }
}