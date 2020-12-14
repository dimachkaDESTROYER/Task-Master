using System;
using TelegramBot;
using Telegram.Bot;
using Ninject;
using TaskMaster.Report;
using TaskMaster.DataBaseFolder;

namespace TaskMaster
{
    public class Program
    {
        public static void Main()
        {
            var container = InitContainer();
            var token = ""; /* <--- вставь токен */
            var bot = new TelegramBotClient(token);
            
            var taskBot = new TelegramTaskBot(bot, container.Get<TaskMasters>(), container.Get<IReportMaker>());
            bot.OnMessage += (sender, args) => taskBot.RecieveMessage(args, bot);
            bot.OnCallbackQuery += (sender, args) => taskBot.RecieveKeyButton(args, bot);

            bot.StartReceiving();
            Console.ReadKey();
            bot.StopReceiving();
        }

        private static StandardKernel InitContainer()
        {
            var container = new StandardKernel();
            container.Bind<IReportMaker>().To<ExcelReportMaker>();
            container.Bind<IDataBase>().To<DataBase>();
            container.Bind<TaskMasters>().ToSelf();
            container.Bind<TelegramBotClient>().ToSelf().InSingletonScope();
            container.Bind<TelegramTaskBot>().ToSelf().InSingletonScope();

            return container;
        }
    }
}
