﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ApiPortVS.Resources;
using ApiPortVS.ViewModels;
using Microsoft.Fx.Portability;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ApiPortVS.Views
{
    /// <summary>
    /// Interaction logic for OptionPageControl.xaml
    /// </summary>
    public partial class OptionsPageControl : UserControl
    {
        private readonly IVsStatusbar _statusBar;

        private OptionsViewModel ViewModel { get { return DataContext as OptionsViewModel; } }

        public OptionsPageControl(OptionsViewModel viewModel, IVsStatusbar statusBar)
        {
            InitializeComponent();
            DataContext = viewModel;

            _statusBar = statusBar;

            Loaded += async (s, e) => await UpdateModelAsync(false).ConfigureAwait(false);
            Unloaded += (s, e) => viewModel.Save();
        }

        private void NavigateToPrivacyModel(object sender, RequestNavigateEventArgs e) => Process.Start(DocumentationLinks.About.OriginalString);

        private void NavigateToMoreInformation(object sender, RequestNavigateEventArgs e) => Process.Start(DocumentationLinks.About.OriginalString);

        private async void RefreshRequested(object sender, RoutedEventArgs e) => await UpdateModelAsync(true).ConfigureAwait(false);

        private async Task UpdateModelAsync(bool force)
        {
            _statusBar.SetText(LocalizedStrings.RefreshingPlatforms);

            await ViewModel.UpdateAsync(force: force).ConfigureAwait(false);

            if (ViewModel.HasError)
            {
                _statusBar.SetText(ViewModel.ErrorMessage);
            }
            else
            {
                _statusBar.SetText(LocalizedStrings.RefreshingPlatformsComplete);
            }
        }

        private void UpdateDirectoryClick(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = ViewModel.OutputDirectory;

                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    ViewModel.OutputDirectory = dialog.SelectedPath;
                }
            }
        }
    }
}
