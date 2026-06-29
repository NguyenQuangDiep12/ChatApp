using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using ChatApp.Core.UnitOfWorks;
using ChatApp.Repository.Repositories;
using ChatApp.Repository.UnitOfWorks;
using ChatApp.Service.DTOs;
using ChatApp.Service.Security;
using ChatApp.Service.Services;
using ChatApp.Core.Services;
using ChatApp.API.Services;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Configure Mapster: User → UserDto (UserStatus enum → Status string)
            TypeAdapterConfig<User, UserDto>.NewConfig()
                .Map(dest => dest.Status, src => src.UserStatus.ToString());

            services.AddHttpContextAccessor();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<IInviteTokenRepository, InviteTokenRepository>();
            services.AddScoped<IMessageReadRepository, MessageReadRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoomMemberRepository, RoomMemberRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();
            services.AddScoped<IUserSystemRoleRepository, UserSystemRoleRepository>();

            // Security
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            // Services
            services.AddScoped(typeof(IService<>), typeof(Service<>));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IUserSettingsService, UserSettingsService>();
            services.AddScoped<IUserSystemRoleService, UserSystemRoleService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IRoomMemberService, RoomMemberService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IMessageReadService, MessageReadService>();
            services.AddScoped<IRealTimeMessageSender, RealTimeMessageSender>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRolePermissionService, RolePermissionService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IInviteTokenService, InviteTokenService>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();

            return services;
        }
    }
}
