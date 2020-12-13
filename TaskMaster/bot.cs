﻿using System;
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
using TaskMaster.DataBaseFolder;

namespace telBot
{
    public enum State
    {
        Nothing,
        CreateNewTask,
        ShowTask,
        EditTask,
        ChangeStatus
    }

    class telegramTaskBot
    {
        private static Dictionary<long, State> usersState = new Dictionary<long, State>();
        private static Dictionary<long, ITask> usersTask = new Dictionary<long, ITask>();

        static void Main()
        {
            var token = ""; // <--- вставь токен
            var bot = new TelegramBotClient(token);
            bot.OnMessage += (sender, args) => RecieveMessage(args, bot);
            bot.OnCallbackQuery += (sender, args) => RecieveKeyButton(args, bot);
            bot.StartReceiving();
            Console.ReadKey();
            bot.StartReceiving();
        }

        private static async void RecieveKeyButton(CallbackQueryEventArgs args, TelegramBotClient bot)
        {
            var data = args.CallbackQuery.Data;
            var name = args.CallbackQuery.Message.Chat.FirstName;
            var namePerson = args.CallbackQuery.Message.From.FirstName;
            var id = args.CallbackQuery.Message.Chat.Id;
            var idPerson = args.CallbackQuery.Message.From.Id;

            if (data == "taken")
            {
                var tasks = TaskMasters.GetTakenTasks(id, name);
                ShowYourTask(bot, id, tasks);
            }
            else if (data == "owned")
            {
                var tasks = TaskMasters.GetOwnedTasks(id, name);
                ShowYourTask(bot, id, tasks);
            }
            else if (data == "done")
            {
                var tasks = TaskMasters.GetDoneTasks(id);
                ShowYourTask(bot, id, tasks);
            }
            else if (data == "Выполнено")
            {
                if (TaskMasters.TryPerformTask(usersTask[id], id))
                    await bot.SendTextMessageAsync(id, "Задача выполнена!");
                else
                    await bot.SendTextMessageAsync(id, "Задача не может быть выполнена!");
            }
            else if (data == "Взять себе")
            {
                if (TaskMasters.TryTakeTask(usersTask[id], id))
                    await bot.SendTextMessageAsync(id, "Задача присвоена");
                else
                    await bot.SendTextMessageAsync(id, "Что-то пошло не так");
            }
            else if (data == "Удалить")
            {
                TaskMasters.DeleteTask(id, usersTask[id]);
                await bot.SendTextMessageAsync(id, "Задача удалена из вашего списка!");
            }
            else if (usersState[id] == State.ShowTask)
            {
                var idTask = int.Parse(args.CallbackQuery.Data);
                var message = TaskMasters.GetTask(idTask).ToString();
                await bot.SendTextMessageAsync(id, message);
            }
            else if (usersState[id] == State.EditTask)
            {
                var idTask = int.Parse(args.CallbackQuery.Data);
                EditTask(args, bot, TaskMasters.GetTask(idTask));
            }
            else if (usersState[id] == State.ChangeStatus)
            {
                var idTask = int.Parse(args.CallbackQuery.Data);
                usersTask[id] = TaskMasters.GetTask(idTask);
                ChangeState(args, bot, id, usersTask[id].Topic);
            }
            await bot.AnswerCallbackQueryAsync(args.CallbackQuery.Id);
        }

        private static async void ChangeState(CallbackQueryEventArgs args, TelegramBotClient bot, long id, string taskName)
        {
            var listButtons = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("Удалить"),
                InlineKeyboardButton.WithCallbackData("Выполнено"),
                InlineKeyboardButton.WithCallbackData("Взять себе")
            };
            var keyboard = new InlineKeyboardMarkup(listButtons.ToArray());
            await bot.SendTextMessageAsync(id, $"Что сделать с задачей {taskName}?", replyMarkup: keyboard);
        }

        private static async void ShowYourTask(TelegramBotClient bot, long id, List<ITask> tasks)
        {
            if (!tasks.Any())
            {
                await bot.SendTextMessageAsync(id, "Ой, тут ничего нет (.о.)");
                /* Енто Дима */
                await bot.SendStickerAsync(id, "CAACAgIAAxkBAAJ-TF_T2AIMCKCBcZDGN54UlP7N4hwGAAJKAANA9G8az5-i1PHrnOoeBA");
                return;
            }

            var listTasks = new List<List<InlineKeyboardButton>>();
            foreach (var task in tasks)
            {
                var taskButton = new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(task.Topic, task.Id.ToString())
                };
                listTasks.Add(taskButton);
            }
            var keyboard = new InlineKeyboardMarkup(listTasks);
            await bot.SendTextMessageAsync(id, "Список ваших задач: ", replyMarkup: keyboard);
        }

        private static async void EditTask(CallbackQueryEventArgs args, TelegramBotClient bot, ITask task)
        {
            var tasksOptions = new List<List<InlineKeyboardButton>>();
            foreach (var options in typeof(SimpleTask).GetProperties())
            {
                var optionButton = new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(options.Name)
                };
                tasksOptions.Add(optionButton);
            }
            var keyboard = new InlineKeyboardMarkup(tasksOptions.ToArray());
            await bot.SendTextMessageAsync(args.CallbackQuery.Message.Chat.Id, "Что изменить?", replyMarkup: keyboard);
        }

        private static async void ChooseListTask(MessageEventArgs args, TelegramBotClient bot)
        {
            var butons = new List<InlineKeyboardButton>();
            butons.Add(InlineKeyboardButton.WithCallbackData("owned"));
            if (args.Message.Chat.Id > 0)
                butons.Add(InlineKeyboardButton.WithCallbackData("taken"));
            butons.Add(InlineKeyboardButton.WithCallbackData("done"));

            var keyboard = new InlineKeyboardMarkup(butons.ToArray());
            await bot.SendTextMessageAsync(args.Message.Chat.Id, "Выберите список", replyMarkup: keyboard);
        }

        private static async void RecieveMessage(MessageEventArgs args, TelegramBotClient bot)
        {
            var id = args.Message.Chat.Id;
            var name = args.Message.Chat.FirstName;

            if (!TaskMasters.db.ContainsPerson(id)) TaskMasters.db.AddPerson(new Person(id, name));

            Console.WriteLine($"{id} {name}");
            if (!usersState.ContainsKey(id))
            {
                usersState.Add(id, State.Nothing);
                usersTask.Add(id, null);
            }
            string message = "Введите команду";

            if (args.Message.Type is MessageType.Sticker)
            {
                message = "Кто-то любит стикеры";
                await bot.SendTextMessageAsync(id, message);
            }
                
            if (args.Message.Type is MessageType.Text)
                switch (args.Message.Text)
                {
                    case "New task":
                        {
                            usersState[id] = State.CreateNewTask;
                            await bot.SendTextMessageAsync(id, "Опишите задачу в формате: 'Название, Описание, Дедлайн (DD.MM.YYYY)'");
                            break;
                        }

                    case "Edit task":
                        {
                            usersState[id] = State.EditTask;
                            ChooseListTask(args, bot);
                            break;
                        }

                    case "Show tasks":
                        {
                            usersState[id] = State.ShowTask;
                            ChooseListTask(args, bot);
                            break;
                        }

                    case "Delete/Done/Take tasks":
                        {
                            usersState[id] = State.ChangeStatus;
                            ChooseListTask(args, bot);
                            break;
                        }

                    case "/start":
                        await MakeStartKeyboard(bot, id);
                        break;

                    case "/start@TaskssMasterBot":
                        await MakeStartKeyboard(bot, id);
                        break;

                    default:  
                        if (usersState[id] == State.CreateNewTask)
                        {
                            var data = args.Message.Text.Split(',');
                            var deadline = data[2].Split('.').Select(c => int.Parse(c)).ToArray();
                            TaskMasters.CreateSimpleTask(id, data[0], data[1], new DateTime(deadline[2], deadline[1], deadline[0]));
                            await bot.SendTextMessageAsync(id, "Задача добавлена");
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
                Keyboard = new[] 
                {
                    new[] /* row 1 */
                    {
                        new KeyboardButton("New task"),
                        new KeyboardButton("Edit task")
                    },
                    new[] /* row 2 */
                    {
                        new KeyboardButton("Show tasks"),
                        new KeyboardButton("Delete/Done/Take tasks")
                    }
                },
                ResizeKeyboard = true
            };
            await bot.SendTextMessageAsync(id, "Выбери команду", replyMarkup: keyboard);
        }
    }
}