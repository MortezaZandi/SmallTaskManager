using Autofac;
using SmallTask.Repositories;
using SmallTask.Services;

namespace SmallTask;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {

        builder.RegisterType<ProjectRepository>().As<IProjectRepository>().InstancePerLifetimeScope();
        builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();
        builder.RegisterType<GroupRepository>().As<IGroupRepository>().InstancePerLifetimeScope();
        builder.RegisterType<TaskRepository>().As<ITaskRepository>().InstancePerLifetimeScope();
        builder.RegisterType<LabelRepository>().As<ILabelRepository>().InstancePerLifetimeScope();
        builder.RegisterType<CommentRepository>().As<ICommentRepository>().InstancePerLifetimeScope();
        builder.RegisterType<AttachmentRepository>().As<IAttachmentRepository>().InstancePerLifetimeScope();
        builder.RegisterType<ActivityLogRepository>().As<IActivityLogRepository>().InstancePerLifetimeScope();

        builder.RegisterType<ProjectService>().As<IProjectService>().InstancePerLifetimeScope();
        builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
        builder.RegisterType<GroupService>().As<IGroupService>().InstancePerLifetimeScope();
        builder.RegisterType<TaskService>().As<ITaskService>().InstancePerLifetimeScope();
        builder.RegisterType<LabelService>().As<ILabelService>().InstancePerLifetimeScope();
        builder.RegisterType<CommentService>().As<ICommentService>().InstancePerLifetimeScope();
        builder.RegisterType<AttachmentService>().As<IAttachmentService>().InstancePerLifetimeScope();
        builder.RegisterType<ActivityLogService>().As<IActivityLogService>().InstancePerLifetimeScope();
    }
}
