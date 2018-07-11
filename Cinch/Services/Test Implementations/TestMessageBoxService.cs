namespace CinchExtended.Services
{
    using System;
    using System.Collections.Generic;

    using CinchExtended.Services.Interfaces;

    /// <summary>
    /// This class implements the IMessageBoxService for Unit testing purposes.
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// 
    ///        //Queue up the response we expect for our given TestMessageBoxService
    ///        //for a given ICommand/Method call within the test ViewModel
    ///        testMessageBoxService.ShowYesNoResponders.Enqueue
    ///            (() =>
    ///                {
    ///
    ///                    return CustomDialogResults.Yes;
    ///                }
    ///            );
    /// ]]>
    /// </example>
    public class TestMessageBoxService : IMessageBoxService
    {
        #region Data

        /// <summary>
        /// Queue of callback delegates for the ShowYesNo methods expected
        /// for the item under test
        /// </summary>
        public Queue<Func<CustomDialogResults>> ShowYesNoResponders { get; set; }

        /// <summary>
        /// Queue of callback delegates for the ShowYesNoCancel methods expected
        /// for the item under test
        /// </summary>
        public Queue<Func<CustomDialogResults>> ShowYesNoCancelResponders { get; set; }

        /// <summary>
        /// Queue of callback delegates for the ShowOkCancel methods expected
        /// for the item under test
        /// </summary>
        public Queue<Func<CustomDialogResults>> ShowOkCancelResponders { get; set; }


        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        public TestMessageBoxService()
        {
            ShowYesNoResponders = new Queue<Func<CustomDialogResults>>();
            ShowYesNoCancelResponders = new Queue<Func<CustomDialogResults>>();
            ShowOkCancelResponders = new Queue<Func<CustomDialogResults>>();
        }
        #endregion

        #region IMessageBoxService Members

        /// <summary>
        /// Does nothing, as nothing required for testing
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public void ShowError(string message)
        {
            //Nothing to do, as there will never be a UI
            //as we are testing the VMs
        }

        /// <summary>
        /// Shows an error message with a custom caption
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="caption">The caption</param>
        public void ShowError(string message, string caption)
        {
            //Nothing to do, as there will never be a UI
            //as we are testing the VMs
        }

        /// <summary>
        /// Does nothing, as nothing required for testing
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public void ShowInformation(string message)
        {
            //Nothing to do, as there will never be a UI
            //as we are testing the VMs
        }

        /// <summary>
        /// Shows an information message with a custom caption
        /// </summary>
        /// <param name="message">The information message</param>
        /// <param name="caption">The caption</param>
        public void ShowInformation(string message, string caption)
        {
            //Nothing to do, as there will never be a UI
            //as we are testing the VMs
        }

        /// <summary>
        /// Does nothing, as nothing required for testing
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public void ShowWarning(string message)
        {
            //Nothing to do, as there will never be a UI
            //as we are testing the VMs        
        }

        /// <summary>
        /// Shows an warning message with a custom caption
        /// </summary>
        /// <param name="message">The warning message</param>
        /// <param name="caption">The caption</param>
        public void ShowWarning(string message, string caption)
        {
            //Nothing to do, as there will never be a UI
            //as we are testing the VMs      
        }

        /// <summary>
        /// Returns the next Dequeue ShowYesNo response expected. See the tests for 
        /// the Func callback expected values
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        public CustomDialogResults ShowYesNo(string message, CustomDialogIcons icon)
        {
            if (ShowYesNoResponders.Count == 0)
                throw new ApplicationException(
                    "TestMessageBoxService ShowYesNo method expects a Func<CustomDialogResults> callback \r\n" +
                    "delegate to be enqueued for each Show call");
            else
            {
                Func<CustomDialogResults> responder = ShowYesNoResponders.Dequeue();
                return responder();
            }
        }

        /// <summary>
        /// Displays a Yes/No dialog with a custom caption, and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="caption">The caption</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        public CustomDialogResults ShowYesNo(string message, string caption, CustomDialogIcons icon)
        {
            if (ShowYesNoResponders.Count == 0)
                throw new ApplicationException(
                    "TestMessageBoxService ShowYesNo method expects a Func<CustomDialogResults> callback \r\n" +
                    "delegate to be enqueued for each Show call");
            else
            {
                Func<CustomDialogResults> responder = ShowYesNoResponders.Dequeue();
                return responder();
            }
        }

        /// <summary>
        /// Displays a Yes/No dialog with a default button selected, and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="caption">The caption of the message box window</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <param name="defaultResult">Default result for the message box</param>
        /// <returns>User selection.</returns>
        public CustomDialogResults ShowYesNo(string message, string caption, CustomDialogIcons icon, CustomDialogResults defaultResult)
        {
            if (ShowYesNoResponders.Count == 0)
                throw new ApplicationException(
                    "TestMessageBoxService ShowYesNo method expects a Func<CustomDialogResults> callback \r\n" +
                    "delegate to be enqueued for each Show call");
            else
            {
                Func<CustomDialogResults> responder = ShowYesNoResponders.Dequeue();
                return responder();
            }
        }

        /// <summary>
        /// Returns the next Dequeue ShowYesNoCancel response expected. See the tests for 
        /// the Func callback expected values
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        public CustomDialogResults ShowYesNoCancel(string message, CustomDialogIcons icon)
        {
            if (ShowYesNoCancelResponders.Count == 0)
                throw new ApplicationException(
                    "TestMessageBoxService ShowYesNoCancel method expects a Func<CustomDialogResults> callback \r\n" +
                    "delegate to be enqueued for each Show call");
            else
            {
                Func<CustomDialogResults> responder = ShowYesNoCancelResponders.Dequeue();
                return responder();
            }
        }

        /// <summary>
        /// Displays a Yes/No/Cancel dialog with a custom caption and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="caption">The caption</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        public CustomDialogResults ShowYesNoCancel(string message, string caption, CustomDialogIcons icon)
        {
            if (ShowYesNoCancelResponders.Count == 0)
                throw new ApplicationException(
                    "TestMessageBoxService ShowYesNoCancel method expects a Func<CustomDialogResults> callback \r\n" +
                    "delegate to be enqueued for each Show call");
            else
            {
                Func<CustomDialogResults> responder = ShowYesNoCancelResponders.Dequeue();
                return responder();
            }
        }

        /// <summary>
        /// Displays a Yes/No/Cancel dialog with a default button selected, and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="caption">The caption of the message box window</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// /// <param name="defaultResult">Default result for the message box</param>
        /// <returns>User selection.</returns>
        public CustomDialogResults ShowYesNoCancel(string message, string caption, CustomDialogIcons icon, CustomDialogResults defaultResult)
        {
            if (ShowYesNoCancelResponders.Count == 0)
                throw new ApplicationException(
                    "TestMessageBoxService ShowYesNoCancel method expects a Func<CustomDialogResults> callback \r\n" +
                    "delegate to be enqueued for each Show call");
            else
            {
                Func<CustomDialogResults> responder = ShowYesNoCancelResponders.Dequeue();
                return responder();
            }
        }

        /// <summary>
        /// Returns the next Dequeue ShowOkCancel response expected. See the tests for 
        /// the Func callback expected values
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        public CustomDialogResults ShowOkCancel(string message, CustomDialogIcons icon)
        {
            if (ShowOkCancelResponders.Count == 0)
                throw new ApplicationException(
                    "TestMessageBoxService ShowOkCancel method expects a Func<CustomDialogResults> callback \r\n" +
                    "delegate to be enqueued for each Show call");
            else
            {
                Func<CustomDialogResults> responder = ShowOkCancelResponders.Dequeue();
                return responder();
            }
        }

        /// <summary>
        /// Displays a OK/Cancel dialog with a custom caption and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="caption">The caption</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        public CustomDialogResults ShowOkCancel(string message, string caption, CustomDialogIcons icon)
        {
            if (ShowOkCancelResponders.Count == 0)
                throw new ApplicationException(
                    "TestMessageBoxService ShowOkCancel method expects a Func<CustomDialogResults> callback \r\n" +
                    "delegate to be enqueued for each Show call");
            else
            {
                Func<CustomDialogResults> responder = ShowOkCancelResponders.Dequeue();
                return responder();
            }
        }

        /// <summary>
        /// Displays a OK/Cancel dialog with a default button selected, and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="caption">The caption of the message box window</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <param name="defaultResult">Default result for the message box</param>
        /// <returns>User selection.</returns>
        public CustomDialogResults ShowOkCancel(string message, string caption, CustomDialogIcons icon, CustomDialogResults defaultResult)
        {
            if (ShowOkCancelResponders.Count == 0)
                throw new ApplicationException(
                    "TestMessageBoxService ShowOkCancel method expects a Func<CustomDialogResults> callback \r\n" +
                    "delegate to be enqueued for each Show call");
            else
            {
                Func<CustomDialogResults> responder = ShowOkCancelResponders.Dequeue();
                return responder();
            }
        }
        #endregion
    }
}
