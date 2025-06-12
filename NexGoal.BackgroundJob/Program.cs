using Fanex.Data.MySql;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Sentry;
using NexGoal.BackgroundJob;
using NexGoal.BackgroundJob.Services;
using NexGoal.BackgroundJob.Services.Interface;
using NexGoal.BackgroundJob.Workers;
using NexGoal.Core.Models;
using NexGoal.Core.Repositories;
using NexGoal.Core.Services;
using Sentry;

var builder = Host.CreateApplicationBuilder(args);

var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
builder.Services.AddSingleton(appSettings);

var mailConfigs = builder.Configuration.GetSection("MailConfigs").Get<MailConfigs>();
builder.Services.AddSingleton(mailConfigs);

var chatbotConfigs = builder.Configuration.GetSection("ChatbotConfigs").Get<ChatbotConfigs>();
builder.Services.AddSingleton(chatbotConfigs);

var jobSettings = builder.Configuration.GetSection("JobSettings").Get<List<JobSetting>>();
builder.Services.AddSingleton(jobSettings);

var syncHrmApiSettings = builder.Configuration.GetSection("SyncHrmApiSettings").Get<SyncHrmSetting>();
builder.Services.AddSingleton(syncHrmApiSettings);

builder.Services.AddHttpClient();

builder.Services.AddSingleton<Fanex.Logging.ILogger, Logger>();
builder.Services.AddSingleton<IDynamicRepository, DynamicRepository>();
builder.Services.AddSingleton<IBaseRepository, BaseRepository>();
builder.Services.AddSingleton<IChatbotService, ChatbotService>();
builder.Services.AddSingleton<IBaseMailService, BaseMailService>();
builder.Services.AddSingleton<IBaseService, BaseService>();
builder.Services.AddSingleton<ICryptoService, CryptoService>();
builder.Services.AddSingleton<IPeriodService, PeriodService>();
builder.Services.AddSingleton<IPAService, PAService>();
builder.Services.AddSingleton<IPAFocusService, PAFocusService>();
builder.Services.AddSingleton<INominationService, NominationService>();
builder.Services.AddSingleton<IProbationService, ProbationService>();

builder.Services.AddSingleton<IHttpClientService, HttpClientService>();
builder.Services.AddSingleton<IEmployeeService, EmployeeService>();
builder.Services.AddSingleton<IGroupService, GroupService>();
builder.Services.AddSingleton<IEmployeeAttendanceService, EmployeeAttendanceService>();
builder.Services.AddSingleton<IEmployeeCalendarService, EmployeeCalendarService>();

LogManager
    .SetDefaultLogCategory(builder.Configuration.GetSection("LogCategory").Value)
    .Use(new List<ILogEngine>
    {
        new SentryLogging(new SentryEngineOptions
        {
            Dsn = new Dsn(builder.Configuration.GetSection("SentryEngineOptions:Dsn").Value),
            FrontEndDsn = new Dsn(builder.Configuration.GetSection("SentryEngineOptions:FrontEndDsn").Value)
        })
    });

builder.Services.AddFanexSql(builder.Configuration);
builder.Services.AddFanexMySql(builder.Configuration);

builder.Services.AddHostedService<SyncEmployeeWorker>();
builder.Services.AddHostedService<SyncGroupWorker>();
builder.Services.AddHostedService<SyncEmployeeAttendanceWorker>();
builder.Services.AddHostedService<SyncEmployeeCalendarWorker>();
builder.Services.AddHostedService<SyncRedmineDataWorker>();
builder.Services.AddHostedService<ProbationWorker>();
builder.Services.AddHostedService<PeriodWorker>();
builder.Services.AddHostedService<PAWorker>();
builder.Services.AddHostedService<PAFocusWorker>();
builder.Services.AddHostedService<NominationWorker>();







var host = builder.Build();
await host.RunAsync();