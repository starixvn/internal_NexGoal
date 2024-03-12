using Fanex.Logging.Sentry;
using Fanex.Logging;
using Sentry;
using NexGoal.BackgroundJob.Workers;
using Fanex.Data.SqlServer;
using Fanex.Data.MySql;
using Microsoft.Extensions.Configuration;
using NexGoal.BackgroundJob;
using NexGoal.Core.Services;
using Fanex.Data.Repository;
using NexGoal.Core.Repositories;

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
var host = builder.Build();
host.Run();

