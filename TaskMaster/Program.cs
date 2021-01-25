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
            var bot = container.Get<TelegramBotClient>();
            var taskBot = container.Get<TelegramTaskBot>();

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
            container.Bind<string>().ToConstant("1459735372:AAGXMBsw1dxlkl30XmlG0o1Cxwu_PvY_lA4"); // <-- Вставь токен пожалуйста
            container.Bind<System.Net.Http.HttpClient>().ToConstant(new System.Net.Http.HttpClient());
            return container;
        }
    }
}
