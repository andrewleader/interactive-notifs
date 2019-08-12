﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace InteractiveNotifs.HubApp.Shared.ViewModels
{
    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetProperty<T>(ref T storage, T value, [CallerMemberName]string name = "")
        {
            if (object.Equals(storage, value))
            {
                return;
            }

            storage = value;
            NotifyPropertyChanged(name);
        }

        public void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
