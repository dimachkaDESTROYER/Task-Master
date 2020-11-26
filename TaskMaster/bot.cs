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
        private static bool canGetName = false;
        private static bool isEdit = false;
        private static bool isStatusChanged = false;
        private static ITask task = default;

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
            var data = args.CallbackQuery.Data;    
            var id = args.CallbackQuery.Message.Chat.Id;

            try
            {
                task = users[id].OwnedTasks[Convert.ToInt32(args.CallbackQuery.Data)];
            }
            catch
            {
                Console.WriteLine("ops");
            }

            if (isStatusChanged)
            {
                ChangeState(args, bot, id);
                isStatusChanged = false;
            }

            if (isEdit)
            {
                EditTask(args, bot, task);
                isEdit = false;
            }

            //foreach (var options in typeof(ITask).GetMethods())
            //if (data == options.Name)
            //  options.Invoke(task, str);
            //надо как то передать ему задачу и на что хочет поменять

            if (data == "Выполнено")
            {
                if (task.TryPerform(users[id]))
                    await bot.SendTextMessageAsync(id, "Задача выполнена!");
                else
                    await bot.SendTextMessageAsync(id, "Задача не может быть выполнена!");
            }

            if (data == "Взять себе")
            {
                if (task.TryTake(users[id]))
                    await bot.SendTextMessageAsync(id, "Задача присвоена");
                else
                    await bot.SendTextMessageAsync(id, "Что-то пошло не так");
            }
            else if (data == "Удалить")
            {
                users[id].TakenTasks.Remove(task);
                await bot.SendTextMessageAsync(id, "Задача удалена из вашего списка!");
            }

            //else
            //    try
            //    {
            //        var task = users[id].OwnedTasks[Convert.ToInt32(args.CallbackQuery.Data)];
            //        if (change_status)
            //            ChangeState(args, bot);
            //        if (is_edit)
            //            EditTask(args, bot, task);
            //        foreach (var options in typeof(ITask).GetMethods())
            //            if (data == options.Name)
            //                options.Invoke(task, str);
            //        надо как то передать ему задачу и на что хочет поменять
            //    }
            //    catch
            //    {
            //        Console.WriteLine("smth is wrong");
            //    }

        }

        private static async void ChangeState(CallbackQueryEventArgs args, TelegramBotClient bot, long id)
        {
            var listButtons = new List<InlineKeyboardButton>();
            listButtons.Add(InlineKeyboardButton.WithCallbackData("Удалить"));
            listButtons.Add(InlineKeyboardButton.WithCallbackData("Выполнено"));
            listButtons.Add(InlineKeyboardButton.WithCallbackData("Взять себе"));
            var keyboard = new InlineKeyboardMarkup(listButtons.ToArray());

            await bot.SendTextMessageAsync(id, "Что сделать?", replyMarkup: keyboard);
        }

        private static async void ShowYourTask(MessageEventArgs args, TelegramBotClient bot, long id)
        {
            var listTasks = new List<InlineKeyboardButton>();
            foreach (var task in users[id].OwnedTasks)
                listTasks.Add(InlineKeyboardButton.WithCallbackData(task.Topic, task.Id.ToString()));
            var keyboard = new InlineKeyboardMarkup(listTasks.ToArray());

            await bot.SendTextMessageAsync(id, "Список ваших задач", replyMarkup: keyboard);
        }

        private static async void EditTask(CallbackQueryEventArgs args, TelegramBotClient bot, ITask task)
        {
            var tasksOptions = new List<InlineKeyboardButton>();
            foreach (var options in typeof(ITask).GetMethods())
                tasksOptions.Add(InlineKeyboardButton.WithCallbackData(options.Name));
            var keyboard = new InlineKeyboardMarkup(tasksOptions.ToArray());

            await bot.SendTextMessageAsync(args.CallbackQuery.Message.Chat.Id, "Что хотите изменить?", replyMarkup: keyboard);
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

            string message = "Введите команду";
            if (args.Message.Type is MessageType.Sticker)
                message = "Кто-то любит стикеры";
            if (args.Message.Type is MessageType.Text)
                switch (args.Message.Text)
                {
                    case "new task":
                        {
                            canGetName = true;
                            await bot.SendTextMessageAsync(id, "придумай название задачи");
                            break;
                        }
                    case "edit task":
                        {
                            ShowYourTask(args, bot, id);
                            isEdit = true;
                        }
                        break;
                    case "show tasks":
                        ShowYourTask(args, bot, id);
                        break;
                    case "delete/done tasks":
                        {
                            ShowYourTask(args, bot, id);
                            isStatusChanged = true;
                        }
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
                                                    new KeyboardButton("show tasks"),
                                                    new KeyboardButton("delete/done tasks")
                                                }
                                            },
                            ResizeKeyboard = true
                        };
                        await bot.SendTextMessageAsync(id, "Выбери команду", replyMarkup: keyboard);
                        break;

                    default:
                        if (canGetName)
                        {
                            ((IOwner)users[id]).AddTask(new SimpleTask(users[id], args.Message.Text, "description"));
                            canGetName = false;
                        }
                        else
                            await bot.SendTextMessageAsync(id, message);
                        break;
                };
        }
    }
}
