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
using System.Reflection;

namespace telBot
{
    class telegramTaskBot
    {
        private static Dictionary<long, Person> users = new Dictionary<long, Person>(); // потом из баззы данных
        private static bool get_name = false;
        private static bool is_edit = false;
        private static bool try_perform = false;
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
            try
            {
                var task = users[id].OwnedTasks[Convert.ToInt32(args.CallbackQuery.Data)];
                if (try_perform)
                    task.TryPerform(users[id]);
                if (is_edit)
                    EditTask(args, bot, task);
            }
            catch
            {
                Console.WriteLine("smth is wrong");
            }
                switch (args.CallbackQuery.Data)
            {

                case "edit":
                    break;
                case "done":
                    await bot.SendTextMessageAsync(id, "Вот когда сделаешь методы, тогда и done");
                    break;

            }
        }

        private static async void ShowYourTask(MessageEventArgs args, TelegramBotClient bot, long id)
        {

            var listTasks = new List<InlineKeyboardButton>();
            foreach (var task in users[id].OwnedTasks)
                listTasks.Add(InlineKeyboardButton.WithCallbackData(task.Topic, task.Id.ToString()));

            var keyboard = new InlineKeyboardMarkup(listTasks.ToArray());
            await bot.SendTextMessageAsync(id, "список ваших задач", replyMarkup: keyboard);
        }

        private static async void EditTask(CallbackQueryEventArgs args, TelegramBotClient bot, ITask task )
        {
            
            var tasksOptions = new List<InlineKeyboardButton>();
            foreach (var options in typeof(ITask).GetMethods())
                tasksOptions.Add(InlineKeyboardButton.WithCallbackData(options.Name));

            var keyboard = new InlineKeyboardMarkup(tasksOptions.ToArray());
            await bot.SendTextMessageAsync(args.CallbackQuery.Message.Chat.Id, "Что хотите изменить", replyMarkup: keyboard);
        }





        private static async void RecieveMessage(MessageEventArgs args, TelegramBotClient bot)
        {
            var id = args.Message.Chat.Id;
            //var user = args.Message.Chat.Username;
            var id2 = args.Message.From.Id;
            //var userchat = args.Message.From.Username;
            if (!(users.ContainsKey(id)))
                users.Add(id, new Person((ulong)id));
            Console.WriteLine(id + "id2 " + id2);

            string message = "введите команду";
            if (args.Message.Type is MessageType.Sticker)
                message = "кто-то любит стикеры";
            if (args.Message.Type is MessageType.Text)
                switch (args.Message.Text)
                {
                    case "new task":
                        {
                            get_name = true;
                            await bot.SendTextMessageAsync(id, "придумай название задачи");
                            break;
                        }
                    case "edit task":
                        {
                            ShowYourTask(args, bot, id);
                            is_edit = true;
                        }
                        break;
                    case "show task":
                        ShowYourTask(args, bot, id);
                        break;
                    case "/start":
                        var keyboard = new ReplyKeyboardMarkup()
                        {
                            Keyboard = new[] {
                                                new[] // row 1
                                                {
                                                    new KeyboardButton("new task"),
                                                    new KeyboardButton("edit task")
                                                },
                                                new[] // row 2
                                                {
                                                    new KeyboardButton("show task"),
                                                    new KeyboardButton("smth")
                                                }
                                            },
                            ResizeKeyboard = true
                        };
                        await bot.SendTextMessageAsync(id, "Выбери команду", replyMarkup: keyboard);
                        break;

                    default:
                        if (get_name)
                        {
                            ((IOwner)users[id]).AddTask(new SimpleTask(users[id], args.Message.Text, "description"));
                            get_name = false;
                        }
                        else
                            await bot.SendTextMessageAsync(id, message);
                        break;

                };



        }
    }
}