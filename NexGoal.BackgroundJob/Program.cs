using Fanex.Data.MySql;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Sentry;
using NexGoal.BackgroundJob;
using NexGoal.BackgroundJob.Services;
using NexGoal.BackgroundJob.Services.Interfaces;
using NexGoal.BackgroundJob.Workers;
using NexGoal.Core.Models;
using NexGoal.Core.Repositories;
using NexGoal.Core.Services;
using Sentry;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
services.AddSingleton(appSettings);

var mailConfigs = configuration.GetSection("MailConfigs").Get<MailConfigs>();
services.AddSingleton(mailConfigs);

var chatbotConfigs = configuration.GetSection("ChatbotConfigs").Get<ChatbotConfigs>();
services.AddSingleton(chatbotConfigs);

var jobSettings = configuration.GetSection("JobSettings").Get<List<JobSetting>>();
services.AddSingleton(jobSettings);

var syncHrmApiSettings = configuration.GetSection("SyncHrmApiSettings").Get<SyncHrmSetting>();
services.AddSingleton(syncHrmApiSettings);

services.AddHttpClient();

services.AddSingleton<Fanex.Logging.ILogger, Logger>();
services.AddSingleton<IDynamicRepository, DynamicRepository>();
services.AddSingleton<IBaseRepository, BaseRepository>();
services.AddSingleton<IChatbotService, ChatbotService>();
services.AddSingleton<IBaseMailService, BaseMailService>();
services.AddSingleton<IBaseService, BaseService>();
services.AddSingleton<ICryptoService, CryptoService>();
services.AddSingleton<IPeriodService, PeriodService>();
services.AddSingleton<IPAService, PAService>();
services.AddSingleton<IPAFocusService, PAFocusService>();
services.AddSingleton<INominationService, NominationService>();
services.AddSingleton<IProbationService, ProbationService>();

services.AddSingleton<IHttpClientService, HttpClientService>();
services.AddSingleton<IEmployeeService, EmployeeService>();
services.AddSingleton<IGroupService, GroupService>();
services.AddSingleton<IEmployeeAttendanceService, EmployeeAttendanceService>();
services.AddSingleton<IEmployeeCalendarService, EmployeeCalendarService>();
services.AddSingleton<IEmployeeOvertimeService, EmployeeOvertimeService>();
services.AddSingleton<ITOEICService, TOEICService>();
services.AddSingleton<IJobTitleService, JobTitleService>();
services.AddSingleton<IWorkingTimeRequestService, WorkingTimeRequestService>();

LogManager
    .SetDefaultLogCategory(configuration.GetSection("LogCategory").Value)
    .Use(new List<ILogEngine>
    {
        new SentryLogging(new SentryEngineOptions
        {
            Dsn = new Dsn(configuration.GetSection("SentryEngineOptions:Dsn").Value),
            FrontEndDsn = new Dsn(configuration.GetSection("SentryEngineOptions:FrontEndDsn").Value)
        })
    });

services.AddFanexSql(configuration);
services.AddFanexMySql(configuration);

services.AddHostedService<SyncGroupWorker>();
services.AddHostedService<SyncEmployeeWorker>();
services.AddHostedService<SyncEmployeeAttendanceWorker>();
services.AddHostedService<SyncEmployeeCalendarWorker>();
services.AddHostedService<SyncEmployeeOvertimeWorker>();
services.AddHostedService<SyncTOEICWorker>();
services.AddHostedService<JobTitleSyncWorker>();
services.AddHostedService<SyncWorkingTimeRequestWorker>();
services.AddHostedService<SyncRedmineDataWorker>();
services.AddHostedService<ProbationWorker>();
services.AddHostedService<PeriodWorker>();
services.AddHostedService<PAWorker>();
services.AddHostedService<PAFocusWorker>();
services.AddHostedService<NominationWorker>();

var host = builder.Build();
await host.RunAsync();