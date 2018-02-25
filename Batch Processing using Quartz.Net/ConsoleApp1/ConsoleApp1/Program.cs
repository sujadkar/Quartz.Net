using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using System.Threading;
namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
             Main();
        }
        public static async Task Main()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = await schedulerFactory.GetScheduler();
            
            IJobDetail job = JobBuilder.Create<HelloJob>()
                                        .WithIdentity("name","group")
                                        .UsingJobData("Name","Bob")
                                        .UsingJobData("Count",1)
                                        .Build();

            ITrigger trigger = TriggerBuilder.Create().WithSimpleSchedule(s => s.WithIntervalInSeconds(2).WithRepeatCount(5)).WithIdentity("name","group").StartNow().Build();

            ITrigger trigger1 = TriggerBuilder.Create().WithCronSchedule("0 0/1 * 1/1 * ? *").StartNow().Build();
            await scheduler.Start();
            await scheduler.ScheduleJob(job, trigger1);

            Thread.Sleep(TimeSpan.FromMinutes(5));
            await scheduler.Shutdown();
        }
    }
    //http://www.cronmaker.com/
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class HelloJob : IJob
    {
        //public string Name { get; set; }
        public Task Execute(IJobExecutionContext context)
        {
            var count = context.JobDetail.JobDataMap.GetInt("Count");
            context.JobDetail.JobDataMap.Put("Count", ++count);
            Console.WriteLine("Hello "+context.JobDetail.JobDataMap.GetString("Name"));
            Console.WriteLine("Job Started at " + DateTime.Now + " Count "+ count);
            Thread.Sleep(TimeSpan.FromSeconds(5));
            Console.WriteLine("Job Finished at "+ DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
