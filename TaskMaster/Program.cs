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
            //InitContainer();
            var token = "1459735372:AAGXMBsw1dxlkl30XmlG0o1Cxwu_PvY_lA4"; // <--- вставь токен
            var bot = new TelegramBotClient(token);
            bot.StartReceiving();
            var ourBot = new TelegramTaskBot(bot);
            Console.ReadKey();
        }

        private static void InitContainer()
        {
            var container = new StandardKernel();
            container.Bind<IReportMaker>().To<ExcelReportMaker>();
            container.Bind<IDataBase>().To<DataBase>();
            container.Bind<string>().ToConstant("1459735372:AAGXMBsw1dxlkl30XmlG0o1Cxwu_PvY_lA4");
            container.Bind<TelegramBotClient>().ToSelf();
            container.Bind<TelegramTaskBot>().ToSelf();


            var bot = container.Get<TelegramTaskBot>();
        }
    }
}
