using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Args;
using TaskMaster;

namespace telBot
{
    class telegramTaskBot
    {
        private static Dictionary<long, Person> users = new Dictionary<long, Person>(); // потом из баззы данных
        static void Main()
        {
            var token = "1459735372:AAGXMBsw1dxlkl30XmlG0o1Cxwu_PvY_lA4";
            var bot = new TelegramBotClient(token);
            //using (var log = new StreamWriter(@"log.txt", true)) { log.WriteLine(message); }

            bot.OnMessage += (sender, args) => RecieveMessage(args, bot);
            bot.StartReceiving();
            Console.ReadKey();
            bot.StartReceiving();
        }

        private static async void RecieveMessage(MessageEventArgs args, TelegramBotClient bot)
        {
            var id = args.Message.Chat.Id;
            //var user = args.Message.Chat.Username;
            var id2 = args.Message.From.Id;
            //var userchat = args.Message.From.Username;
            if (!(users.ContainsKey(id)))
                users.Add(id, new Person(id2, id2));
            Console.WriteLine(id + "id2 " + id2);

            string message = "ну напиши мне";
            if (args.Message.Type is MessageType.Sticker)
                message = "кто-то любит стикеры";
            if (args.Message.Type is MessageType.Text)
                message = args.Message.Text switch
                {
                    "/start" => "хаюшки",
                    "/start@TaskssMasterBot" => "хаюшки",
                    "/new" => "введите имя новой задачи",
                    "/new@TaskssMasterBot" => "Дима дай метод!!!!!!!!!!!!",
                    "/edit@TaskssMasterBot" => "Dima give me methods",
                    "/edit" => "Dima give me methods",
                    _ => "dont understand",
                };
            await bot.SendTextMessageAsync(id2, message);

        }
    }
}