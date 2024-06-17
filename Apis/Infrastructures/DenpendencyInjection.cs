using Application;
using Application.Interfaces;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Infrastructures.Mappers;
using Infrastructures.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructures
{
    public static class DenpendencyInjection
    {
        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ILearnerService, LearnerService>();
            services.AddScoped<IMentorService, MentorService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IContentService, ContentService>();
            services.AddScoped<IRequirementService, RequirementService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IChapterService, ChapterService>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ILearnerRepository, LearnerRepository>();
            services.AddScoped<IMentorRepository, MentorRepository>();
            services.AddScoped<IContentRepository, ContentRepository>();
            services.AddScoped<IRequirementRepository, RequirementRepository>();
            services.AddScoped<ICourseTagRepository, CourseTagRepository>();
            services.AddScoped<IChapterRepository, ChapterRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IFirebaseService, FirebaseService>();

            services.AddSingleton<ICurrentTime, CurrentTime>();
            services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(GetConnection(configuration, env),
                        builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            services.AddIdentity<ApplicationUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();
            services.AddAutoMapper(typeof(MapperConfigurationsProfile));
            services.Configure<IdentityOptions>(options => options.SignIn.RequireConfirmedEmail = true);
            return services;
        }
        private static string GetConnection(IConfiguration configuration, IWebHostEnvironment env)
        {
#if DEVELOPMENT
        return configuration.GetConnectionString("DefaultConnection") 
            ?? throw new Exception("DefaultConnection not found");
#else
            return configuration[$"ConnectionStrings:{env.EnvironmentName}"]
                ?? throw new Exception($"ConnectionStrings:{env.EnvironmentName} not found");
#endif
        }
    }


}
