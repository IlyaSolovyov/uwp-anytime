using AnyTimeT10.DAL;
using AnyTimeT10.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace AnyTimeT10.ViewModels
{
    class RegisterPageViewModel : ViewModelBase
    {
        DatabaseContext db = new DatabaseContext();
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        public DelegateCommand<object> RegisterCommand { get; private set; }
        public DelegateCommand<object> UploadAvatarCommand { get; private set; }

        String username;
        String password;
        String avatarFilename;
        String selectedAvatar = "ms-appx:///Assets/Avatars/EmptyAvatar.png";

        public RegisterPageViewModel()
        {
            RegisterCommand = new DelegateCommand<object>(
            i => RegisterAsync(new User(Username, Password, AvatarFilename)), i => CanRegister());
            UploadAvatarCommand = new DelegateCommand<object>(
            i => UploadAvatarAsync(), i => CanUpload());
        }

        #region Properties
        public String Username
        {
            get { return username; }
            set
            {
                Set(ref username, value);
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }
        public String Password
        {
            get { return password; }
            set
            {
                Set(ref password, value);
            }
        }
        public String AvatarFilename
        {
            get { return avatarFilename; }
            set
            {
                Set(ref avatarFilename, value);
            }
        }
        public String SelectedAvatar
        {
            get { return selectedAvatar; }
            set
            {
                Set(ref selectedAvatar, value);
            }
        }
        #endregion

        #region Commands
        private bool CanRegister()
        {
            return !String.IsNullOrWhiteSpace(Username);
        }
        private async void RegisterAsync(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
            ContentDialog dlg = new ContentDialog()
            {
                Title = "Profile registered",
                Content = "User " + Username + " has been succefully added!",
                PrimaryButtonText = "Great!"
            };
            await dlg.ShowAsync();
            NavigationService.Navigate(typeof(Views.LoginPage));
        }

        private bool CanUpload()
        {
            return true;
        }
        private async void UploadAvatarAsync()
        {

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            var file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                StorageFolder avatarFolder = await localFolder.GetFolderAsync("Avatars");
                StorageFile newFile = await file.CopyAsync(avatarFolder, file.Name, NameCollisionOption.GenerateUniqueName);
                AvatarFilename = newFile.Name;
                SelectedAvatar = newFile.Path;

            }
        }
        #endregion

    }
}
