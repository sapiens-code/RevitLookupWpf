﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using RevitLookupWpf.InstanceTree;
using RevitLookupWpf.PropertySys;
using System.Linq;
using RevitLookupWpf.View;

namespace RevitLookupWpf.ViewModel
{
    public class LookupWindowViewModel : LookupViewModel
    {
        #region Fields
        private string _title = "LookupWindow";
        private RelayCommand _closeCommand;

        private RelayCommand _selectedItemChangedCommand;
        private ObservableCollection<LookupViewModel> _items;
        private readonly LookupWindow _lookupWindow;
        #endregion

        #region Ctor

        public LookupWindowViewModel(LookupWindow lookupWindow)
        {
            LookupData = this;
            Items = new ObservableCollection<LookupViewModel>(GetAllSnoopItems());

            Messenger.Default.Register<RvtObjectMessage>(this, OnNavigation);
            _lookupWindow = lookupWindow;
        }
        #endregion

        #region Properties
        public string Title { get => _title; set => Set(ref _title, value); }

        public Action CloseAction { get; set; }

        public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(CloseAction));

        public ObservableCollection<LookupViewModel> Items { get => _items; set => Set(ref _items , value); }
        #endregion

        #region Public Methods
        public bool SetRvtInstance<TRvtObject>(TRvtObject rvtObject)
        {
            var root = InstanceNode.Create<TRvtObject>(rvtObject);
            root.IsSelected = true;

            LookupData.Roots = new ObservableCollection<InstanceNode>() { root };

            LookupData.PropertyList = GetSelectedNode()?.PropertyList;

            return LookupData.Roots.Any();
        }

        public ICommand SelectedItemChangedCommand
        {
            get
            {
                if (_selectedItemChangedCommand == null)
                {
                    _selectedItemChangedCommand = new RelayCommand(PerformSelectedItemChanged);
                }

                return _selectedItemChangedCommand;
            }
        }

        protected IEnumerable<LookupViewModel> GetAllSnoopItems()
        {
            LookupViewModel current = this;
            yield return current;
            while (current.Next != null)
            {
                current = current.Next;
                yield return current;
            }
        }

        private void PerformSelectedItemChanged()
        {
            if (LookupData.Roots == null)
            {
                return;
            }

            var selectedNode = GetSelectedNode();
            if (selectedNode != null)
            {
                selectedNode.Snoop();
                LookupData.PropertyList = selectedNode.PropertyList;
            }

        }

        private void OnNavigation(RvtObjectMessage objectMessage)
        {
            if (!_lookupWindow.IsActive)
            {
                return;
            }

            var vm = new LookupViewModel();

            var root = InstanceNode.Create(objectMessage.RvtObject);

            vm.Roots = new ObservableCollection<InstanceNode>() { root };

            vm.PropertyList = vm.GetSelectedNode()?.PropertyList;

            //Navigate to the next object
            if (vm.Roots.Any())
            {
                LookupData.Next = vm;
                LookupData = vm;
                Items = new ObservableCollection<LookupViewModel>( GetAllSnoopItems());
            }

            root.IsSelected = true;
            root.IsExpanded = true;
        }

        #endregion

        protected override void LookupDataChanged()
        {
            Items = new ObservableCollection<LookupViewModel>(GetAllSnoopItems());
            //if (Items?.Any() != true)
            //    return;

            //var startIndex = Items.IndexOf(LookupData)+1;
            //for (int i = startIndex; i < Items.Count; i++)
            //{
            //    Items.RemoveAt(i);
            //}
        }
    }
}
