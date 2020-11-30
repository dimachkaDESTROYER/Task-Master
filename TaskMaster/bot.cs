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
    enum State
    {
        Nothing,
        CreateNewTask,
        ShowTask,
        EditTask,
        ChangeStatus
    }

    class telegramTaskBot
    {
        private static Dictionary<long, Person> users = new Dictionary<long, Person>();
        private static Dictionary<long, State> usersState = new Dictionary<long, State>();
        private static Dictionary<long, ITask> usersTask = new Dictionary<long, ITask>();
        // потом из баззы данных
        static void Main()
        {
            var token = ""; //вставь токен
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
            if (usersState[id] == State.ShowTask)
            {
                usersTask[id] = users[id].OwnedTasks[Convert.ToInt32(args.CallbackQuery.Data)];
                await bot.SendTextMessageAsync(id, usersTask[id].GetType().GetCustomAttributes().ToString());
                usersState[id] = State.Nothing;
                usersTask[id] = null;
            }

            else if (usersState[id] == State.EditTask)
            {
                usersTask[id] = users[id].OwnedTasks[Convert.ToInt32(args.CallbackQuery.Data)];
                EditTask(args, bot, usersTask[id]);
                usersState[id] = State.Nothing;
                usersTask[id] = null;
            }
            
            else if (usersState[id] == State.ChangeStatus)
            {
                usersTask[id] = users[id].OwnedTasks[Convert.ToInt32(args.CallbackQuery.Data)];
                ChangeState(args, bot, id, usersTask[id].Topic);
                usersState[id] = State.Nothing;
            }

            else if (data == "Выполнено")
            {
                if (usersTask[id].TryPerform(users[id]))
                    await bot.SendTextMessageAsync(id, "Задача выполнена!");
                else
                    await bot.SendTextMessageAsync(id, "Задача не может быть выполнена!");
                usersTask[id] = null;
            }

            else if (data == "Взять себе")
            {
                if (usersTask[id].TryTake(users[id]))
                    await bot.SendTextMessageAsync(id, "Задача присвоена");
                else
                    await bot.SendTextMessageAsync(id, "Что-то пошло не так");
                usersTask[id] = null;
            }
            else if (data == "Удалить")
            {
                users[id].TakenTasks.Remove(usersTask[id]);
                await bot.SendTextMessageAsync(id, "Задача удалена из вашего списка!");
                usersTask[id] = null;
            }
            await bot.AnswerCallbackQueryAsync(args.CallbackQuery.Id);


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

    

    private static async void ChangeState(CallbackQueryEventArgs args, TelegramBotClient bot, long id, string taskName)
    {
        var listButtons = new List<InlineKeyboardButton>();
        listButtons.Add(InlineKeyboardButton.WithCallbackData("Удалить"));
        listButtons.Add(InlineKeyboardButton.WithCallbackData("Выполнено"));
        listButtons.Add(InlineKeyboardButton.WithCallbackData("Взять себе"));
        var keyboard = new InlineKeyboardMarkup(listButtons.ToArray());
        await bot.SendTextMessageAsync(id, "что сделать с задачей " + taskName, replyMarkup: keyboard);
    }


    private static async void ShowYourTask(MessageEventArgs args, TelegramBotClient bot, long id)
    {
        var listTasks = new List<InlineKeyboardButton>();
        foreach (var task in users[id].OwnedTasks)
            listTasks.Add(InlineKeyboardButton.WithCallbackData(task.Topic, task.Id.ToString()));
        var keyboard = new InlineKeyboardMarkup(listTasks.ToArray());
        await bot.SendTextMessageAsync(id, "список ваших задач", replyMarkup: keyboard);
    }

    private static async void EditTask(CallbackQueryEventArgs args, TelegramBotClient bot, ITask task)
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
            if (!(users.ContainsKey(id)))
            {
                users.Add(id, new Person((ulong)id));
                usersState.Add(id, State.Nothing);
                usersTask.Add(id, null);
            }
            Console.WriteLine(id);

            string message = "введите команду";
            if (args.Message.Type is MessageType.Sticker)
                message = "кто-то любит стикеры";
            if (args.Message.Type is MessageType.Text)
                switch (args.Message.Text)
                {
                    case "new task":
                        {
                            usersState[id] = State.CreateNewTask;
                            await bot.SendTextMessageAsync(id, "придумай название задачи");
                            break;
                        }
                    case "edit task":
                        {
                            usersState[id] = State.EditTask;
                            ShowYourTask(args, bot, id);
                        }
                        break;
                    case "show tasks":
                        usersState[id] = State.ShowTask;
                        ShowYourTask(args, bot, id);
                        break;
                    case "delete/done tasks":
                        {
                            usersState[id] = State.ChangeStatus;
                            ShowYourTask(args, bot, id);
                        }
                        break;
                    case "/start":
                        await MakeStartKeyboard(bot, id);
                        break;
                    case "/start@TaskssMasterBot":
                        await MakeStartKeyboard(bot, id);
                        break;

                    default:
                        if (usersState[id] == State.CreateNewTask)
                        {
                            ((IOwner)users[id]).AddTask(new SimpleTask(users[id], args.Message.Text, "description"));
                            usersState[id] = State.Nothing;
                        }
                        else
                            await bot.SendTextMessageAsync(id, message);
                        break;

                };

        }

        private static async Task MakeStartKeyboard(TelegramBotClient bot, long id)
        {
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
        }
    }
}
