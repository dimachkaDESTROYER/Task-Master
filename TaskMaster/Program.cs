using System;
using System.Collections.Generic;
using System.Text;
using telBot;
using Telegram.Bot;
using Ninject;
using TaskMaster.Report;
using TaskMaster.DataBaseFolder;

namespace TaskMaster
{
    class Program
    {
        public static void Main()
        {
            var container = InitContainer();
            //var bot = container.Get<TelegramBotClient>();
            //var bot = new TelegramBotClient("1459735372:AAGXMBsw1dxlkl30XmlG0o1Cxwu_PvY_lA4");

            //var ourBot = container.Get<TelegramTaskBot>();
            var bot = new TelegramBotClient("1459735372:AAGXMBsw1dxlkl30XmlG0o1Cxwu_PvY_lA4");
            
            var ourBot = new TelegramTaskBot(bot,
                container.Get<TaskMasters>(),
                container.Get<IReportMaker>());
            bot.OnMessage += (sender, args) => ourBot.RecieveMessage(args, bot);
            bot.OnCallbackQuery += (sender, args) => ourBot.RecieveKeyButton(args, bot);
            bot.StartReceiving();

            //bot.StartReceiving();
            Console.ReadKey();
        }

        private static StandardKernel InitContainer()
        {
            var container = new StandardKernel();
            container.Bind<IReportMaker>().To<ExcelReportMaker>();
            container.Bind<IDataBase>().To<DataBase>();
            //container.Bind<string>().ToConstant("1459735372:AAGXMBsw1dxlkl30XmlG0o1Cxwu_PvY_lA4");// <--- вставь токен
            container.Bind<TaskMasters>().ToSelf();
            container.Bind<TelegramBotClient>().ToSelf().InSingletonScope();
            container.Bind<TelegramTaskBot>().ToSelf().InSingletonScope();

            return container;
        }
    }
}
