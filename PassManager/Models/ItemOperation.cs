﻿using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using Newtonsoft.Json;

namespace PassManager.Models
{
    [JsonObject]
    public class FolderItem : BindableBase
    {
        public FolderItem(bool canDelete = true)
        {
            CanDelete = canDelete;
            Title = "新しいフォルダー";
            IsExpanded = true;
            Passwords = new ObservableCollection<PasswordItem>();
            Children = new ObservableCollection<FolderItem>();
        }

        public bool CanDelete { get; set; }
        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set { SetProperty(ref isExpanded, value); }
        }
        public ObservableCollection<PasswordItem> Passwords { get; set; }
        public ObservableCollection<FolderItem> Children { get; set; }
    }

    public class PasswordItem : BindableBase
    {
        public PasswordItem()
        {
            Title = "新しいパスワード";
            UserName = "";
            Url = "";
            Password = "";
            Memos = new ObservableCollection<MemoWrapper>();
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        private string userName;
        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value); }
        }
        private string url;
        public string Url
        {
            get { return url; }
            set { SetProperty(ref url, value); }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }
        public ObservableCollection<MemoWrapper> Memos { get; set; }
    }

    public class MemoWrapper : BindableBase
    {
        private string memo;
        public string Memo
        {
            get { return memo; }
            set { SetProperty(ref memo, value); }
        }
    }

    public sealed class ItemOperation : BindableBase
    {
        public static ItemOperation Instance { get; } = new ItemOperation();

        private ItemOperation()
        {
            SelectedPasswords = new ObservableCollection<PasswordItem>();
        }

        private FolderItem selectedFoloder;
        public FolderItem SelectedFolder
        {
            get { return selectedFoloder; }
            set { SetProperty(ref selectedFoloder, value); }
        }
        private PasswordItem selectedPassword;
        public PasswordItem SelectedPassword
        {
            get { return selectedPassword; }
            set { SetProperty(ref selectedPassword, value); }
        }
        private ObservableCollection<PasswordItem> selectedPasswords;
        public ObservableCollection<PasswordItem> SelectedPasswords
        {
            get { return selectedPasswords; }
            set { SetProperty(ref selectedPasswords, value); }
        }

        /// <summary>
        /// 選択されているパスワードアイテムに引数で渡したパスワードをセットする
        /// </summary>
        /// <param name="password"></param>
        public void SetPassword(string password)
        {
            if (SelectedPassword == null) throw new NullReferenceException();
            SelectedPassword.Password = password;
        }

        /// <summary>
        /// 選択されているパスワードアイテムのメモの末尾に新しいメモ追加する
        /// </summary>
        public void AddMemos()
        {
            if (SelectedPassword == null) throw new NullReferenceException();
            SelectedPassword.Memos.Add(new MemoWrapper());
        }
    }
}
