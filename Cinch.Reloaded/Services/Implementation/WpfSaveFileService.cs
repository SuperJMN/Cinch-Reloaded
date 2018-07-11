using System;
using System.ComponentModel.Composition;
using System.Windows;
using Cinch.Reloaded.Services.Interfaces;
using MEFedMVVM.ViewModelLocator;
using Microsoft.Win32;

namespace Cinch.Reloaded.Services.Implementation
{
    /// <summary>
    /// This class implements the ISaveFileService for WPF purposes.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.Shared)]
    [ExportService(ServiceType.Both, typeof(ISaveFileService))]
    public class WpfSaveFileService : ISaveFileService
    {
        #region Data

        /// <summary>
        /// Embedded SaveFileDialog to pass back correctly selected
        /// values to ViewModel
        /// </summary>
        private SaveFileDialog sfd = new SaveFileDialog(); 

        #endregion

        #region ISaveFileService Members
        /// <summary>
        /// This method should show a window that allows a file to be selected
        /// </summary>
        /// <param name="owner">The owner window of the dialog</param>
        /// <returns>A bool from the ShowDialog call</returns>
        public bool? ShowDialog(Window owner)
        {
            //Set embedded SaveFileDialog.Filter
            if (!String.IsNullOrEmpty(this.Filter))
                sfd.Filter = this.Filter;

            //Set embedded SaveFileDialog.InitialDirectory
            if (!String.IsNullOrEmpty(this.InitialDirectory))
                sfd.InitialDirectory = this.InitialDirectory;

            //Set embedded SaveFileDialog.OverwritePrompt
            sfd.OverwritePrompt = this.OverwritePrompt;

            //return results
            return sfd.ShowDialog(owner);
        }

        /// <summary>
        /// FileName : Simply use embedded SaveFileDialog.FileName
        /// Also allow users to set new FileName, which sets SaveFileDialog.FileName
        /// </summary>
        public string FileName
        {
            get { return sfd.FileName; }
            set { sfd.FileName = value; }
        }

        /// <summary>
        /// Filter : Simply use embedded SaveFileDialog.Filter
        /// </summary>
        public string Filter
        {
            get { return sfd.Filter; }
            set { sfd.Filter = value; }
        }

        /// <summary>
        /// Filter : Simply use embedded SaveFileDialog.InitialDirectory
        /// </summary>
        public string InitialDirectory
        {
            get { return sfd.InitialDirectory; }
            set { sfd.InitialDirectory = value; }
        }

        /// <summary>
        /// OverwritePrompt : Simply use embedded SaveFileDialog.OverwritePrompt
        /// </summary>
        public bool OverwritePrompt
        {
            get { return sfd.OverwritePrompt; }
            set { sfd.OverwritePrompt = value; }
        }
        #endregion
    }
}
