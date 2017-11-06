
using AnyTimeT10.DAL;
using AnyTimeT10.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AnyTimeT10.ViewModels
{
    class TasksPageViewModel : ViewModelBase
    {
        DatabaseContext db = new DatabaseContext();
        ObservableCollection<DailyTask> tasks = new ObservableCollection<DailyTask>();
        ObservableCollection<TaskCategory> categories = new ObservableCollection<TaskCategory>();

        public DelegateCommand<object> UpdateTaskListCommand { get; set; }
        public DelegateCommand<object> ChangeModeCommand { get; set; }
        public DelegateCommand<object> RemoveCommand { get; private set; }

        User user;
        DateTime todayDate = DateTime.Now;

        DailyTask temporaryTask = new DailyTask();
        TaskCategory taskCategory = new TaskCategory();
        String taskName = null;
        String sortByParameter;


        DailyTask selectedTask;
        String sidepanelLabel;
        String sidepanelButtonText;
        String changeModeButtonText;
        bool hasSelection = false;
        bool isEditMode;


        public TasksPageViewModel()
        {
            UpdateTaskListCommand = new DelegateCommand<object>(
            i => ExecuteUpdateAsync(), i => CanUpdate());
            ChangeModeCommand = new DelegateCommand<object>(
            i => ExecuteChangeMode(), i => CanChangeMode());
            RemoveCommand = new DelegateCommand<object>(
            i => ExecuteRemove(), i => CanRemove());


        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            User = await GetUserFromSessionStateAsync();
            if (User == null)
            {
                NavigationService.Navigate(typeof(Views.LoginPage), null);
            }
            else
            {
                sortByParameter = "Date added";
                FetchCategories();
                FetchTasks();
                EnterCreateMode();
            }
            await Task.CompletedTask;
        }
        private async Task<User> GetUserFromSessionStateAsync()
        {
            if (SessionState.ContainsKey("User"))
            {
                return SessionState["User"] as User;
            }
            else
            {
                ContentDialog dlg = new ContentDialog()
                {
                    Title = "Access denied",
                    PrimaryButtonText = "I'm sorry!",
                    Content = "You haven't logged in yet. Please select a user profile before proceding."
                };
                await dlg.ShowAsync();
                return null;

            }
        }

        private void FetchCategories()
        {
            List<TaskCategory> categoryModels = db.TaskCategories.ToList<TaskCategory>();
            Categories.Clear();
            foreach (var m in categoryModels)
            {
                Categories.Add(m);
            }
        }
        private void FetchTasks()
        {
            List<DailyTask> taskModels = db.DailyTasks.Where(p => p.UserId == User.Id).Where(p => p.Completed == 0).ToList<DailyTask>();
            Tasks.Clear();
            foreach (var m in taskModels)
            {
                Tasks.Add(m);
            }
        }
        private void SortList(string sortByParameter)
        {
            List<DailyTask> orderedTasks = null;
            Tasks.Clear();
            switch (sortByParameter)
            {
                case "Date added":
                    orderedTasks = db.DailyTasks
                        .Where(p => p.UserId == User.Id)
                        .Where(p=>p.Completed==0)
                        .OrderBy(s => s.Id)
                        .ToList();
                    break;

                case "Category":
                    orderedTasks = db.DailyTasks
                        .Where(p => p.UserId == User.Id)
                        .Where(p => p.Completed == 0)
                        .OrderBy(s => s.CategoryId)
                        .ToList();
                    break;

                case "Deadline":
                    orderedTasks = db.DailyTasks
                        .Where(p => p.UserId == User.Id)
                        .Where(p => p.Completed == 0)
                        .OrderBy(s => s.Deadline == null)
                        .ThenBy(s => s.Deadline)
                        .ToList();
                    break;
            }
            foreach (var task in orderedTasks)
            {
                Tasks.Add(task);
            }
        }

        private void EnterCreateMode()
        {
            SidepanelLabel = "Add new task";
            SidepanelButtonText = "Add to task list";
            ChangeModeButtonText = "Edit task";
            isEditMode = false;
            ResetFields();
        }
        private void ResetFields()
        {

            TemporaryTask = new DailyTask();
            TaskName = null;
            TaskCategory = Categories.First();
        }
        private void EnterEditMode()
        {
            SidepanelLabel = "Edit task";
            SidepanelButtonText = "Save changes";
            ChangeModeButtonText = "Cancel edit";
            TemporaryTask = SelectedTask;
            TaskName = TemporaryTask.Name;
            TaskCategory = TemporaryTask.Category;
            isEditMode = true;
        }

        #region Properties
        public ObservableCollection<DailyTask> Tasks
        {
            get { return tasks; }
            set
            {
                Set(ref tasks, value);
            }
        }
        public ObservableCollection<TaskCategory> Categories
        {
            get { return categories; }
            set
            {
                Set(ref categories, value);
            }
        }

        public User User
        {
            get { return user; }
            set
            {
                Set(ref user, value);
            }
        }
        public DateTime TodayDate
        {
            get { return todayDate; }
            set
            {
                Set(ref todayDate, value);
            }
        }

        public DailyTask TemporaryTask
        {
            get { return temporaryTask; }
            set
            {
                Set(ref temporaryTask, value);
            }
        }
        public TaskCategory TaskCategory
        {
            get { return taskCategory; }
            set
            {
                Set(ref taskCategory, value);
                if (TemporaryTask != null)
                {
                    TemporaryTask.Category = TaskCategory;
                }
            }
        }
        public String TaskName
        {
            get { return taskName; }
            set
            {
                Set(ref taskName, value);
                TemporaryTask.Name = TaskName;
                UpdateTaskListCommand.RaiseCanExecuteChanged();
            }
        }
        public String SortByParameter
        {
            get { return sortByParameter; }
            set
            {
                Set(ref sortByParameter, value);
                SortList(SortByParameter);
            }
        }

        public DailyTask SelectedTask
        {
            get { return selectedTask; }
            set
            {
                Set(ref selectedTask, value);
                HasSelection = (SelectedTask != null);
                ChangeModeCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
            }
        }
        public String SidepanelLabel
        {
            get { return sidepanelLabel; }
            set
            {
                Set(ref sidepanelLabel, value);
            }
        }
        public String SidepanelButtonText
        {
            get { return sidepanelButtonText; }
            set
            {
                Set(ref sidepanelButtonText, value);
            }
        }
        public String ChangeModeButtonText
        {
            get { return changeModeButtonText; }
            set
            {
                Set(ref changeModeButtonText, value);
            }
        }
        public bool HasSelection
        {
            get { return hasSelection; }
            set
            {
                Set(ref hasSelection, value);
            }
        }
        #endregion

        #region Commands
        private void ExecuteRemove()
        {
            db.DailyTasks.Remove(SelectedTask);
            db.SaveChanges();
            FetchTasks();
        }
        private bool CanRemove()
        {
            return HasSelection;
        }

        private void ExecuteChangeMode()
        {
            if (!isEditMode)
            {
                EnterEditMode();
            }
            else { EnterCreateMode(); }

        }
        private bool CanChangeMode()
        {
            return HasSelection;
        }

        private async void ExecuteUpdateAsync()
        {
            ContentDialog dlg = new ContentDialog()
            {
                Title = "Task list updated",
                PrimaryButtonText = "Great!"
            };
            if (isEditMode)
            {
                db.SaveChanges();
                dlg.Content = "Task has been successfully updated.";
            }
            else
            {
                TemporaryTask.UserId = User.Id;
                db.DailyTasks.Add(TemporaryTask);
                db.SaveChanges();
                dlg.Content = "Task has been successfully added.";
            }
            EnterCreateMode();
            await dlg.ShowAsync();
            FetchTasks();
        }
        private bool CanUpdate()
        {
            return !String.IsNullOrWhiteSpace(TaskName);
        }
        #endregion
    }
}