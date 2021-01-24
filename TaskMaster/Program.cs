using System;
using TelegramBot;
using Telegram.Bot;
using Ninject;
using TaskMasterBot.Report;
using TaskMasterBot.DataBaseFolder;

namespace TaskMasterBot
{
    public class Program
    {
        public static void Main()
        {
            var container = InitContainer();
            var token = "1459735372:AAGXMBsw1dxlkl30XmlG0o1Cxwu_PvY_lA4"; /* <--- вставь токен */
            var bot = new TelegramBotClient(token);
            
            var taskBot = new TelegramTaskBot(bot, container.Get<TaskMaster>(), container.Get<IReportMaker>());
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
            container.Bind<TaskMaster>().ToSelf();
            container.Bind<TelegramBotClient>().ToSelf().InSingletonScope();
            container.Bind<TelegramTaskBot>().ToSelf().InSingletonScope();

            return container;
        }
    }
}
