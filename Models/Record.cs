using Prism.Mvvm;
using System;
using System.Threading;
using System.Windows;

namespace Boysenberry.Models
{
    class Record : BindableBase
    {
        private string _userId;
        private string _nickname;
        private int _count;
        private DateTime _updateTime;

        private bool _isFuncEnable = true;
        private bool _isStopEnable = false;
        private bool _isOpenEnable = true;
        private Style _funcIconStyle = Application.Current.FindResource("ToolbarButtonIcon") as Style;
        private Style _stopIconStyle = Application.Current.FindResource("ToolbarButtonIconDisable") as Style;
        private Style _openIconStyle = Application.Current.FindResource("ToolbarButtonIcon") as Style;
        private Style _progressIconStyle = Application.Current.FindResource("DataGridProgressIconDisable") as Style;

        private CancellationTokenSource _cancellationTokenSource;



        public string UserId
        {
            get { return _userId; }
            set { SetProperty(ref _userId, value); }
        }
        public string Nickname
        {
            get { return _nickname; }
            set { SetProperty(ref _nickname, value); }
        }
        public int Count
        {
            get { return _count; }
            set { SetProperty(ref _count, value); }
        }
        public DateTime UpdateTime
        {
            get { return _updateTime; }
            set { SetProperty(ref _updateTime, value); }
        }

        public bool IsFuncEnable
        {
            get { return _isFuncEnable; }
            set { SetProperty(ref _isFuncEnable, value); }
        }
        public bool IsStopEnable
        {
            get { return _isStopEnable; }
            set { SetProperty(ref _isStopEnable, value); }
        }
        public bool IsOpenEnable
        {
            get { return _isOpenEnable; }
            set { SetProperty(ref _isOpenEnable, value); }
        }
        public Style FuncIconStyle
        {
            get { return _funcIconStyle; }
            set { SetProperty(ref _funcIconStyle, value); }
        }
        public Style StopIconStyle
        {
            get { return _stopIconStyle; }
            set { SetProperty(ref _stopIconStyle, value); }
        }
        public Style OpenIconStyle
        {
            get { return _openIconStyle; }
            set { SetProperty(ref _openIconStyle, value); }
        }

        public Style ProgressIconStyle
        {
            get { return _progressIconStyle; }
            set { SetProperty(ref _progressIconStyle, value); }
        }

        public CancellationTokenSource CancellationTokenSource
        {
            get { return _cancellationTokenSource; }
            set { SetProperty(ref _cancellationTokenSource, value); }
        }

        public Record()
        {
        }

        public Record(string userId, string nickname, int count, DateTime updateTime)
        {
            _userId = userId;
            _nickname = nickname;
            _count = count;
            _updateTime = updateTime;
        }
    }
}
