using Fanex.Data.MySql;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Sentry;
using NexGoal.BackgroundJob;
using NexGoal.BackgroundJob.Workers;
using NexGoal.Core.Repositories;
using NexGoal.Core.Services;
using Sentry;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<Fanex.Logging.ILogger, Logger>();
builder.Services.AddSingleton<IBaseService, BaseService>();
builder.Services.AddSingleton<IDynamicRepository, DynamicRepository>();
builder.Services.AddSingleton<IBaseRepository, BaseRepository>();

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

builder.Services.Configure<List<JobSetting>>(builder.Configuration.GetSection("JobSettings"));
builder.Services.AddHostedService<SyncRedmineDataWorker>();
builder.Services.AddHostedService<SyncProbationDataWorker>();
builder.Services.AddHostedService<SyncPeriodDataWorker>();
builder.Services.AddHostedService<SyncPADataWorker>();
builder.Services.AddHostedService<SyncPAFocusDataWorker>();
builder.Services.AddHostedService<SyncNominationDataWorker>();
var host = builder.Build();
host.Run();

