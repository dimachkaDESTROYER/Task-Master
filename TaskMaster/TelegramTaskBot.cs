using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Args;
using TaskMasterBot;
using Telegram.Bot.Types.ReplyMarkups;
using TaskMasterBot.Report;
using Telegram.Bot.Types.InputFiles;
using TaskMasterBot.Domain.Tasks;

namespace TelegramBot
{
    public enum State
    {
        Nothing,
        CreateNewTask,
        ShowTask,
        EditTask,
        ChooseEdition,
        ChangeStatus,
        CreateSubTask
    }

    public class TelegramTaskBot
    {
        private Dictionary<long, State> usersState = new Dictionary<long, State>();
        private Dictionary<long, ITask> usersTask = new Dictionary<long, ITask>();
        private Dictionary<long, string> userParam = new Dictionary<long, string>();
        private Dictionary<long, List<ITask>> tasksToReport = new Dictionary<long, List<ITask>>();
        private readonly TaskMaster taskMaster;
        private readonly IReportMaker reportMaker;

        public TelegramTaskBot(TelegramBotClient bot, TaskMaster taskMaster, IReportMaker reportMaker)
        {
            this.reportMaker = reportMaker;
            this.taskMaster = taskMaster;          
        }

        public async void RecieveKeyButton(CallbackQueryEventArgs args, TelegramBotClient bot)
        {
            var data = args.CallbackQuery.Data;
            var chatId = args.CallbackQuery.Message.Chat.Id;
            var personId = args.CallbackQuery.From.Id;

            if (data == "В процессе")
            {
                var tasks = taskMaster.GetTakenTasks(chatId);
                ShowYourTask(bot, args, chatId, tasks, data);
            }
            else if (data == "Свободные")
            {
                var tasks = taskMaster.GetOwnedTasks(chatId);
                ShowYourTask(bot, args, chatId, tasks, data);
            }
            else if (data == "Решённые")
            {
                var tasks = taskMaster.GetDoneTasks(chatId);
                ShowYourTask(bot, args, chatId, tasks, data);
            }
            else if (data == "Выполнить")
            {
                if (taskMaster.TryPerformTask(usersTask[chatId], chatId, personId))
                    await bot.SendTextMessageAsync(chatId, $"Задача '{usersTask[chatId].Topic}' выполнена!");
                else if (((Person)usersTask[chatId].Performer) != null)
                    await bot.SendTextMessageAsync(chatId, $"Вы не можете выполнить задачу, её выполняет {((Person)usersTask[chatId].Performer).Name}!");
                else
                    await bot.SendTextMessageAsync(chatId, $"Вы не можете выполнить задачу, которую не взяли!");
            }
            else if (data == "Взять себе")
            {
                if (taskMaster.TryTakeTask(usersTask[chatId], personId))
                    await bot.SendTextMessageAsync(chatId, $"Задача '{usersTask[chatId].Topic}' присвоена");
                else
                    await bot.SendTextMessageAsync(chatId, $"Вы не можете взять задачу '{usersTask[chatId].Topic}'");
            }
            else if (data == "Удалить")
            {
                if (taskMaster.TryDeleteTask(chatId, usersTask[chatId]))
                    await bot.SendTextMessageAsync(chatId, $"Задача '{usersTask[chatId].Topic}' удалена из вашего списка!");
                else
                    await bot.SendTextMessageAsync(chatId, $"Вы не можете удалить задачу '{usersTask[chatId].Topic}'!");
            }
            else if (data == "\u270d\ud83c\udffb Отчёт")
            {
                using (var stream = File.OpenRead(reportMaker.CreateTasksReport(tasksToReport[chatId])))
                {
                    InputOnlineFile iof = new InputOnlineFile(stream)
                    {
                        FileName = "Report.xlsx"
                    };
                    var send = bot.SendDocumentAsync(chatId, iof, "Ваш отчёт");
                }
            }
            else if (data == "\u21a9\ufe0f")
            {
                if (usersState[chatId] == State.ChooseEdition)
                    usersState[chatId] = State.EditTask;

                var keyboard = GetKeyboardListTasks();
                await bot.EditMessageTextAsync(chatId, args.CallbackQuery.Message.MessageId, "Выберите список", replyMarkup: keyboard);
            }

            else if (usersState[chatId] == State.ShowTask)
            {
                var idTask = int.Parse(args.CallbackQuery.Data);
                var task = taskMaster.GetTask(idTask);

                var message = taskMaster.GetTask(idTask).ToString();
                await bot.SendTextMessageAsync(chatId, message);
                if (task is BranchedTask branchedTask)
                {
                    var subs = new List<List<InlineKeyboardButton>>();
                    foreach(var t in branchedTask.SubTasks)
                    {
                        var taskButton = new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithCallbackData(t.Topic, t.Id.ToString())
                        };
                        subs.Add(taskButton);
                    }
                    var keyboard = new InlineKeyboardMarkup(subs);
                    await bot.SendTextMessageAsync(chatId, $"Подзадачи '{branchedTask.Topic}':", replyMarkup: keyboard);
                }
            }
            else if (usersState[chatId] == State.ChooseEdition)
            {
                var param = args.CallbackQuery.Data;
                userParam[chatId] = param;
                await bot.SendTextMessageAsync(chatId, $"Введите новое значение {param}");
            }
            else if (usersState[chatId] == State.EditTask)
            {
                var idTask = int.Parse(args.CallbackQuery.Data);
                usersState[chatId] = State.ChooseEdition;
                usersTask[chatId] = taskMaster.GetTask(idTask);
                EditTask(args, bot, usersTask[chatId]);
            }
            else if (usersState[chatId] == State.CreateSubTask)
            {
                var idTask = int.Parse(args.CallbackQuery.Data);
                usersTask[chatId] = taskMaster.GetTask(idTask);
                await bot.SendTextMessageAsync(chatId, "Опишите подзадачу в формате: 'Название, Описание, Дедлайн (DD.MM.YYYY)'");
            }
            else if (usersState[chatId] == State.ChangeStatus)
            {
                var idTask = int.Parse(args.CallbackQuery.Data);
                usersTask[chatId] = taskMaster.GetTask(idTask);
                ChangeState(args, bot, chatId, usersTask[chatId].Topic);
            }
            await bot.AnswerCallbackQueryAsync(args.CallbackQuery.Id);
        }

        private async void ChangeState(CallbackQueryEventArgs args, TelegramBotClient bot, long id, string taskName)
        {
            var listButtons = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("Взять себе"),
                InlineKeyboardButton.WithCallbackData("Выполнить"),
                InlineKeyboardButton.WithCallbackData("Удалить")
            };
            var keyboard = new InlineKeyboardMarkup(listButtons.ToArray());
            await bot.EditMessageTextAsync(id, args.CallbackQuery.Message.MessageId, $"Что сделать с задачей '{taskName}'?", replyMarkup: keyboard);
        }

        private async void ShowYourTask(TelegramBotClient bot, CallbackQueryEventArgs args, long id, List<ITask> tasks, string tasksName)
        {
            if (!tasks.Any())
            {
                await bot.SendTextMessageAsync(id, $"Ой, в списке '{tasksName}' пусто!");
                return;
            }

            var listTasks = new List<List<InlineKeyboardButton>>();
            tasksToReport[id] = new List<ITask>();
            foreach (var task in tasks)
            {
                var taskButton = new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(task.Topic, task.Id.ToString())
                };
                listTasks.Add(taskButton);
            }
            var visualButton = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("\u270d\ud83c\udffb Отчёт")
            };
            var backButton = new List<InlineKeyboardButton> 
            {
                InlineKeyboardButton.WithCallbackData("\u21a9\ufe0f") 
            };
            tasksToReport[id] = tasks;

            listTasks.Add(visualButton);
            listTasks.Add(backButton);
            var keyboard = new InlineKeyboardMarkup(listTasks);
            await bot.EditMessageTextAsync(id, args.CallbackQuery.Message.MessageId, $"'{tasksName}':", replyMarkup: keyboard);
        }

        private async void EditTask(CallbackQueryEventArgs args, TelegramBotClient bot, ITask task)
        {
            var tasksOptions = new List<List<InlineKeyboardButton>>();
            var options = task.GetType()
                              .GetProperties()
                              .Where(p => p.PropertyType == typeof(string) || p.PropertyType == typeof(DateTime));
            foreach (var option in options)
            {
                var optionButton = new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(option.Name)
                };
                tasksOptions.Add(optionButton);
            }
            var backButton = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("\u21a9\ufe0f")
            };
            tasksOptions.Add(backButton);
            var keyboard = new InlineKeyboardMarkup(tasksOptions.ToArray());
            await bot.EditMessageTextAsync(args.CallbackQuery.Message.Chat.Id,
                                           args.CallbackQuery.Message.MessageId,
                                           $"Что изменить в {task.Topic}?", replyMarkup: keyboard);
        }

        private InlineKeyboardMarkup GetKeyboardListTasks()
        {
            var buttons = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("Свободные"),
                InlineKeyboardButton.WithCallbackData("В процессе"),
                InlineKeyboardButton.WithCallbackData("Решённые")
            };
            return new InlineKeyboardMarkup(buttons.ToArray());
        }

        public async void RecieveMessage(MessageEventArgs args, TelegramBotClient bot)
        {
            var id = args.Message.Chat.Id;
            var chatName = args.Message.Chat.Title;
            var personName = args.Message.From.FirstName;

            if (id < 0)
            {
                var personId = args.Message.From.Id;
                taskMaster.MakeTeam(id, personId, personName, chatName);
            }
            else
                taskMaster.MakePerson(id, personName);

            Console.WriteLine($"{id}");

            if (!usersState.ContainsKey(id))
            {
                usersState.Add(id, State.Nothing);
                usersTask.Add(id, null);
            }

            string message = "Введите команду";

            if (args.Message.Type is MessageType.Text)
                switch (args.Message.Text)
                {
                    case "Создать новую задачу":
                        {
                            usersState[id] = State.CreateNewTask;
                            await bot.SendTextMessageAsync(id, "Опишите задачу в формате: 'Название, Описание, Дедлайн (DD.MM.YYYY)'");
                            break;
                        }

                    case "Изменить задачу":
                        {
                            usersState[id] = State.EditTask;
                            var keyboard = GetKeyboardListTasks();
                            await bot.SendTextMessageAsync(args.Message.Chat.Id, "Выберите список", replyMarkup: keyboard);
                            break;
                        }

                    case "Показать список задач":
                        {
                            usersState[id] = State.ShowTask;
                            var keyboard = GetKeyboardListTasks();
                            await bot.SendTextMessageAsync(args.Message.Chat.Id, "Выберите список", replyMarkup: keyboard);
                            break;
                        }

                    case "Добавить подзадачу":
                        {
                            usersState[id] = State.CreateSubTask;
                            var keyboard = GetKeyboardListTasks();
                            await bot.SendTextMessageAsync(args.Message.Chat.Id, "Выберите список", replyMarkup: keyboard);
                            break;
                        }

                    case "Взять/Выполнить/Удалить задачу":
                        {
                            usersState[id] = State.ChangeStatus;
                            var keyboard = GetKeyboardListTasks();
                            await bot.SendTextMessageAsync(args.Message.Chat.Id, "Выберите список", replyMarkup: keyboard);
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
                            try
                            {
                                var data = args.Message.Text.Split(',');
                                var deadline = data[2].Split('.').Select(c => int.Parse(c)).ToArray();
                                taskMaster.CreateSimpleTask(id, data[0], data[1],
                                                             new DateTime(deadline[2], deadline[1], deadline[0]));
                                await bot.SendTextMessageAsync(id, "Задача добавлена");
                            }
                            catch
                            {
                                await bot.SendTextMessageAsync(id, "Задача введена в неверном формате!");
                            }
                        }
                        else if (usersState[id] == State.CreateSubTask)
                        {
                            try
                            {
                                var data = args.Message.Text.Split(',');
                                var deadline = data[2].Split('.').Select(c => int.Parse(c)).ToArray();

                                taskMaster.CreateBranchedTask(usersTask[id], id, data[0], data[1],
                                                           new DateTime(deadline[2], deadline[1], deadline[0]));
                                await bot.SendTextMessageAsync(id, "Задача добавлена");
                            }
                            catch
                            {
                                await bot.SendTextMessageAsync(id, "Задача введена в неверном формате!");
                            }
                        }
                        else if (usersState[id] == State.ChooseEdition && userParam[id].Any())
                        {
                            try
                            {
                                taskMaster.EditTask(usersTask[id], id, userParam[id], args.Message.Text);
                                await bot.SendTextMessageAsync(id, $"У задачи '{usersTask[id].Topic}' изменен параметр '{userParam[id]}'");
                            }
                            catch
                            {
                                await bot.SendTextMessageAsync(id, $"Неверный формат для {userParam[id]}!");
                            }
                            finally
                            {
                                userParam.Clear();
                            }

                        }
                        else
                            await bot.SendTextMessageAsync(id, message);
                        break;
                };
        }

        private async Task MakeStartKeyboard(TelegramBotClient bot, long id)
        {
            var keyboard = new ReplyKeyboardMarkup()
            {
                Keyboard = new[]
                {
                    new[]
                    {
                        new KeyboardButton("Создать новую задачу"),
                        new KeyboardButton("Изменить задачу")
                    },
                    new[]
                    {
                        new KeyboardButton("Показать список задач"),
                        new KeyboardButton("Взять/Выполнить/Удалить задачу")
                    },
                    new[]
                    {
                        new KeyboardButton("Добавить подзадачу")
                    },
                },
                ResizeKeyboard = true
            };
            await bot.SendTextMessageAsync(id, "Выбери команду", replyMarkup: keyboard);
        }
    }
}