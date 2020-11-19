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
using Telegram.Bot.Types.ReplyMarkups;
using TaskMaster.Domain;

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
            bot.OnCallbackQuery += (sender, args) => RecieveKeyButton(args, bot);
            bot.StartReceiving();
            Console.ReadKey();
            bot.StartReceiving();
        }

        private static async void RecieveKeyButton(CallbackQueryEventArgs args, TelegramBotClient bot)
        {
            var id = args.CallbackQuery.From.Id;
            switch (args.CallbackQuery.Data)
            {
                case "cool":
                    await bot.SendTextMessageAsync(id, "you are cool");
                    break;
                case "create":
                    await bot.SendTextMessageAsync(id, "Дима Колосов, придумай название задачи");
                    bot.OnMessage += (sender, args) =>
                    {
                        var taskName = CheckInputSomeInf(args, bot);
                        ((IOwner)users[id]).AddTask(new SimpleTask(users[id], taskName, "description"));
                    };
                    break;
                case "edit":
                    ShowYourTask(args, bot);
                    //пока только показывает
                    break;
                case "done":
                    await bot.SendTextMessageAsync(id, "Вот когда сделаешь методы, тогда и done");
                    break;

            }
        }
        
        private static async void ShowYourTask(CallbackQueryEventArgs args, TelegramBotClient bot)
        {
            var id = args.CallbackQuery.From.Id;
            var listTasks = new List<InlineKeyboardButton>();
            foreach (var task in users[id].OwnedTasks)
                listTasks.Add(InlineKeyboardButton.WithCallbackData(task.Topic, "choose"));

            var keyboard = new InlineKeyboardMarkup(listTasks.ToArray());
            await bot.SendTextMessageAsync(id, "тыкай", replyMarkup: keyboard);
        }

        private static string CheckInputSomeInf(MessageEventArgs args, TelegramBotClient bot)
        {
            var id = args.Message.Chat.Id;
            var id2 = args.Message.From.Id;
            if (args.Message.Type is MessageType.Text)
                return args.Message.Text;
            return "";
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
                switch (args.Message.Text)
                    {
                    case "/start":
                        message = "хаюшки";
                        break;
                    case "/start@TaskssMasterBot":
                        message = "хаюшки";
                        break;
                    case "/new":
                        message = "введите имя новой задачи";
                        break;
                    case "/new@TaskssMasterBot":
                        message = "введите имя новой задачи";
                        break;
                    case "/edit@TaskssMasterBot":
                        message = "Dima give me methods";
                        break;
                    case "/edit":
                        message = "Dima give me methods";
                        break;
                    case "/keyboard":
                        var keyboard = new InlineKeyboardMarkup ( new[]
                        { 
                            new[]
                            {
                            InlineKeyboardButton.WithCallbackData("создать", "create"),
                            InlineKeyboardButton.WithCallbackData("редактировать", "edit"),

                            },

                            new[]
                            {
                            InlineKeyboardButton.WithCallbackData("сделать", "done"),
                            InlineKeyboardButton.WithCallbackData("молодец", "cool"),
                            }
                        });
                        await bot.SendTextMessageAsync(id, "тыкай", replyMarkup: keyboard);
                        break;
                        
                    default:
                        await bot.SendTextMessageAsync(id, message);
                        break;
                
                };

            

        }
    }
}