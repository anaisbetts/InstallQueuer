using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace InstallQueuer.Ui
{
    [Export("AppView")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {            
            InitializeComponent();
            this.AllowDrop = true;
        }

        [Import]
        AppViewModel ViewModel {
            get { return (AppViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(AppViewModel), typeof(MainWindow));

        protected override void OnDragEnter(DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;            
            DropTargetHelper.DragEnter(this, e.Data, e.GetPosition(this), e.Effects);
            e.Handled = true;
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            DropTargetHelper.DragLeave();
            e.Handled = true;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;                        
            DropTargetHelper.DragOver(e.GetPosition(this), e.Effects);
            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            DropTargetHelper.Drop(e.Data, e.GetPosition(this), e.Effects);
            e.Handled = true;
        }
    }
}

// vim: ts=4:sts=4:sw=4:et
