﻿using System;
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
        }
    }
}

// vim: ts=4:sts=4:sw=4:et
