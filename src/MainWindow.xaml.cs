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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly AppViewModel ViewModel;

        public MainWindow()
        {
            ViewModel = new AppViewModel(this);
            InitializeComponent();

            this.AllowDrop = true;
        }

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

        private void rect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = sender as Rectangle;

            DataObject data = new DataObject(new DragDropLib.DataObject());
            data.SetDragImage(rect, e.GetPosition(rect));

            DragDrop.DoDragDrop(rect, data, DragDropEffects.Copy);
        }
    }
}

// vim: ts=4:sts=4:sw=4:et
