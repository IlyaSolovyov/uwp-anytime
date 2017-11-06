using AnyTimeT10.DAL;
using AnyTimeT10.Models;
using AnyTimeT10.Services;
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
    class MainPageViewModel : ViewModelBase
    {
        DatabaseContext db = new DatabaseContext();
        ObservableCollection<DailyTask> unassignedTasks = new ObservableCollection<DailyTask>();
        ObservableCollection<TaskCategory> categories = new ObservableCollection<TaskCategory>();

        #region Don't open
        ObservableCollection<DailyTask> monday = new ObservableCollection<DailyTask>();
        ObservableCollection<DailyTask> tuesday = new ObservableCollection<DailyTask>();
        ObservableCollection<DailyTask> wednesday = new ObservableCollection<DailyTask>();
        ObservableCollection<DailyTask> thursday = new ObservableCollection<DailyTask>();
        ObservableCollection<DailyTask> friday = new ObservableCollection<DailyTask>();
        ObservableCollection<DailyTask> saturday = new ObservableCollection<DailyTask>();
        ObservableCollection<DailyTask> sunday = new ObservableCollection<DailyTask>();
        #endregion

        public DelegateCommand<object> AssignCommand { get; set; }
        public DelegateCommand<object> PreviousWeekCommand { get; set; }
        public DelegateCommand<object> NextWeekCommand { get; set; }

        User user;
        int weekOffset = 0;
        List<DateTime> weekDates;
        DateTime mondayDate;
        DateTime sundayDate;
        String sortByParameter = "Date added";
        String taskAssignmentDay = "Monday";
        DailyTask selectedUnassignedTask;     
        bool hasSelectionUnassigned;

        public MainPageViewModel()
        {
            AssignCommand = new DelegateCommand<object>(
            i => ExecuteAssign(), i => CanAssign());
            PreviousWeekCommand = new DelegateCommand<object>(
            i => ExecuteChangeWeek(false), i => true);
            NextWeekCommand = new DelegateCommand<object>(
            i => ExecuteChangeWeek(true), i => true);

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
                FetchCategories();
                FetchUnassignedTasks();
                FetchDates();
                FetchTasks();
                SortByParameter = "Date added";
                TaskAssignmentDay = "Monday";
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

        private void SortList(string sortByParameter)
        {
            List<DailyTask> orderedTasks = null;
            UnassignedTasks.Clear();
            switch (sortByParameter)
            {
                case "Date added":
                    orderedTasks = db.DailyTasks
                        .Where(p => p.UserId == User.Id)
                        .Where(p => p.Completed == 0)
                        .Where(p=>p.Date==null)
                        .OrderBy(s => s.Id)
                        .ToList();
                    break;

                case "Category":
                    orderedTasks = db.DailyTasks
                        .Where(p => p.UserId == User.Id)
                        .Where(p => p.Completed == 0)
                        .Where(p => p.Date == null)
                        .OrderBy(s => s.CategoryId)
                        .ToList();
                    break;

                case "Deadline":
                    orderedTasks = db.DailyTasks
                        .Where(p => p.UserId == User.Id)
                        .Where(p => p.Completed == 0)
                        .Where(p => p.Date == null)
                        .OrderBy(s => s.Deadline == null)
                        .ThenBy(s => s.Deadline)
                        .ToList();
                    break;
            }
            foreach (var task in orderedTasks)
            {
                unassignedTasks.Add(task);
            }
        }

        private void FetchUnassignedTasks()
        {
            List<DailyTask> taskModels = db.DailyTasks
                .Where(p => p.UserId == User.Id)
                .Where(p => p.Completed == 0)
                .Where(p => p.Date==null)
                .ToList<DailyTask>();

            UnassignedTasks.Clear();
            foreach (var m in taskModels)
            {
                UnassignedTasks.Add(m);
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
        private void FetchDates()
        {
            WeekDates = CalendarService.GetWeekDatesList(WeekOffset);
            foreach(DateTime date in WeekDates)
            {
                Debug.WriteLine(date.ToString());
            }
        }

        private void FetchTasks()
        {
            Monday.Clear();
            Tuesday.Clear();
            Wednesday.Clear();
            Thursday.Clear();
            Friday.Clear();
            Saturday.Clear();
            Sunday.Clear();
            List<DailyTask> Models = db.DailyTasks
               .Where(p => p.UserId == User.Id)
               .ToList<DailyTask>();
       
            foreach (var m in Models)
            {
                if (m.Date.HasValue)
                {
                    Debug.WriteLine("Has value!");
                    if (m.Date.Value.Date==(WeekDates[0].Date)) Monday.Add(m);
                    else if (m.Date.Value.Date==(WeekDates[1].Date)) Tuesday.Add(m);
                    else if (m.Date.Value.Date==(WeekDates[2].Date)) Wednesday.Add(m);
                    else if (m.Date.Value.Date==(WeekDates[3].Date)) Thursday.Add(m);
                    else if (m.Date.Value.Date==(WeekDates[4].Date)) Friday.Add(m);
                    else if (m.Date.Value.Date==(WeekDates[5].Date)) Saturday.Add(m);
                    else if (m.Date.Value.Date==(WeekDates[6].Date)) Sunday.Add(m);
                }
              
            }    
        }

        #region Properties
        public ObservableCollection<DailyTask> UnassignedTasks
        {
            get { return unassignedTasks; }
            set
            {
                Set(ref unassignedTasks, value);
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

        #region Don't open - Properties
        public ObservableCollection<DailyTask> Monday
        {
            get { return monday; }
            set
            {
                Set(ref monday, value);
            }
        }
        public ObservableCollection<DailyTask> Tuesday
        {
            get { return tuesday; }
            set
            {
                Set(ref tuesday, value);
            }
        }
        public ObservableCollection<DailyTask> Wednesday
        {
            get { return wednesday; }
            set
            {
                Set(ref wednesday, value);
            }
        }
        public ObservableCollection<DailyTask> Thursday
        {
            get { return thursday; }
            set
            {
                Set(ref thursday, value);
            }
        }
        public ObservableCollection<DailyTask> Friday
        {
            get { return friday; }
            set
            {
                Set(ref friday, value);
            }
        }
        public ObservableCollection<DailyTask> Saturday
        {
            get { return saturday; }
            set
            {
                Set(ref saturday, value);
            }
        }
        public ObservableCollection<DailyTask> Sunday
        {
            get { return sunday; }
            set
            {
                Set(ref sunday, value);
            }
        }
        #endregion

        public User User
        {
            get { return user; }
            set
            {
                Set(ref user, value);
            }
        }
        public int WeekOffset
        {
            get { return weekOffset; }
            set
            {
                Set(ref weekOffset, value);
                FetchDates();
                FetchTasks();
            }
        }
        public List<DateTime> WeekDates
        {
            get { return weekDates; }
            set
            {
                Set(ref weekDates, value);
                MondayDate = WeekDates.First();
                SundayDate = WeekDates.Last();
            }

        }
        public DateTime MondayDate
        {
            get { return mondayDate; }
            set
            {
                Set(ref mondayDate, value);
            }
        }
        public DateTime SundayDate
        {
            get { return sundayDate; }
            set
            {
                Set(ref sundayDate, value);
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
        public String TaskAssignmentDay
        {
            get { return taskAssignmentDay; }
            set
            {
                Set(ref taskAssignmentDay, value);
            }
        }
        public DailyTask SelectedUnassignedTask
        {
            get { return selectedUnassignedTask; }
            set
            {
                Set(ref selectedUnassignedTask, value);
                HasSelectionUnassigned = (SelectedUnassignedTask != null);
                AssignCommand.RaiseCanExecuteChanged();

            }
        }
        public bool HasSelectionUnassigned
        {
            get { return hasSelectionUnassigned; }
            set
            {
                Set(ref hasSelectionUnassigned, value);
            }
        }
        #endregion

        #region Commands
        private void ExecuteAssign()
        {
            SelectedUnassignedTask.Date = CalendarService.GetDateByDayName(TaskAssignmentDay, weekOffset);
            db.SaveChanges();
            FetchUnassignedTasks();
            FetchTasks();
        }
        private bool CanAssign()
        {
            return HasSelectionUnassigned;
        }

        private void ExecuteChangeWeek(bool next)
        {
            if (next)
            {
                WeekOffset++;
            }
            else
            {
                WeekOffset--;
            }
        }
        #endregion
    }
}
