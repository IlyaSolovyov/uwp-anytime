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
using Template10.Controls;
using Template10.Services;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace AnyTimeT10.ViewModels
{
    class LoginPageViewModel : ViewModelBase
    {
        DatabaseContext db = new DatabaseContext();
        ObservableCollection<User> users = new ObservableCollection<User>();

        public DelegateCommand<object> RegisterCommand { get; set; }
        public DelegateCommand<object> LoginCommand { get; set; }
        public DelegateCommand<User> RemoveCommand { get; set; }     

        User selectedUser = null;
        bool hasSelection = false;

        public LoginPageViewModel()
        {
            FetchModels();
            RegisterCommand = new DelegateCommand<object>(
            i => ExecuteRegister(), i => CanRegister());
            LoginCommand = new DelegateCommand<object>(
            i => ExecuteLogin(SelectedUser), i => CanLogin());
            RemoveCommand = new DelegateCommand<User>(
            i => ExecuteRemove(i), i => CanRemove());
           
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (SessionState.ContainsKey("User"))
            {
                SessionState.Remove("User");
            }
            await Task.CompletedTask;
        }

        private void FetchModels()
        {
            List<User> models = db.Users.ToList();
            this.users.Clear();
            foreach (var m in models)
            {
                this.users.Add(m);
            }
        }

        #region Properties
        public ObservableCollection<User> Users
        {
            get { return users; }
            set
            {
                Set(ref users, value);
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }
        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                Set(ref selectedUser, value);
                this.HasSelection = (this.selectedUser != null);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }
        public bool HasSelection
        {
            get { return hasSelection; }
            set
            {
                Set(ref hasSelection, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region Commands
        private void ExecuteRegister()
        {
            NavigationService.Navigate(typeof(Views.RegisterPage), null);
        }
        private bool CanRegister()
        {
            return (Users.Count < 5);
        }

        private void ExecuteRemove(User user)
        {
            //TODO: confirmation popup dialog
            db.Users.Remove(user);
            db.SaveChanges();
            FetchModels();
            RegisterCommand.RaiseCanExecuteChanged();
        }
        private bool CanRemove()
        {
            return true;
        }

        private void ExecuteLogin(User user)
        {
            SessionState.Add("User", user);
            User test = SessionState["User"] as User;
            Debug.WriteLine(test.Name);
            NavigationService.Navigate(typeof(Views.TasksPage), null);
        }
        private bool CanLogin()
        {
            return HasSelection;
        }
        #endregion
    }
}
