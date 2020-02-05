using System;
using System.Collections.Generic;
using System.Configuration;

namespace DigialMaketingLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            var actions = ConfigurationManager.GetSection("actions") as List<ActionParam>;

            foreach (var action in actions)
            {
                LaunchDailyAction(action);
            }

            var ch = string.Empty;
            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.Read() != 'q') ;
        }

        static void LaunchDailyAction(ActionParam actionParam)
        {
             TaskScheduler.Instance.ScheduleTask(actionParam.ExecHour, actionParam.ExecMinute, 0.02, () => Batch.Execute(actionParam.Url));
        }
    }
}
