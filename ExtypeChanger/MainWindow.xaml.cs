using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace ExtypeChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        List<string> filesList;

        private void findFilesButton_Click(object sender, RoutedEventArgs e)
        {
            if (locationTextBox.Text.Length != 0 && extentionFindTypeChoice.Text.Length != 0)
            {
                if (FindFilesInDirectory(ref filesList, extentionFindTypeChoice.Text))
                {
                    if (filesList.Count != 0)
                        DisplayStatus("Only '" + extentionFindTypeChoice.Text + "' files found. (" + filesList.Count + ")");
                    else if (filesList.Count == 0)
                        DisplayStatus("No items found!");
                }
                DisplayFilesInListBox(filesList);
            }
            else if (locationTextBox.Text.Length != 0)
            {
                if (FindFilesInDirectory(ref filesList, "*") && filesList.Count != 0)
                    DisplayStatus("All files found. (" + filesList.Count + ")");
                else if (filesList.Count == 0)
                    DisplayStatus("No items found!");

                DisplayFilesInListBox(filesList);
            }
            else itemsFoundList.Items.Clear();
        }

        /// <summary>
        /// Gets files paths and places them to filesList. 
        /// </summary>
        /// <param name="filesList"></param>
        /// <param name="extentionString"></param>
        private bool FindFilesInDirectory(ref List<string> filesList, string extentionString)
        {
            try
            {
                filesList = Directory.GetFiles(locationTextBox.Text, "*" + extentionString).ToList();
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Wrong directory!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        /// <summary>
        /// Clears itemFoundList and displays files names stored in filesList.
        /// </summary>
        /// <param name="filesList"></param>
        private void DisplayFilesInListBox(List<string> filesList)
        {
            itemsFoundList.Items.Clear();
            foreach (var item in filesList)
            {
                itemsFoundList.Items.Add(System.IO.Path.GetFileName(item.ToString()));
            }
        }

        private void itemsFoundList_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }

        private void oneFileChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (newExtentionTextBox.Text.Length == 0) DisplayStatus("No new extetntion specified.");
            else if (itemsFoundList.Items.Count == 0 || itemsFoundList.SelectedItem == null) DisplayStatus("No items found or file is not selected.");
            else if (!newExtentionTextBox.Text.StartsWith(".")) DisplayStatus("Extention starts from dot.");
            else
            {
                string fullFilePath = FindSelectedFileLocalization();
                ChangeExtention(fullFilePath, IndicateNewPath(fullFilePath));

                findFilesButton_Click(sender, e);
                DisplayStatus("Name change completed!");
            }
        }

        private void allFilesChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (newExtentionTextBox.Text.Length == 0) DisplayStatus("No new extetntion specified.");
            else if (itemsFoundList.Items.Count == 0) DisplayStatus("No items found.");
            else if (!newExtentionTextBox.Text.StartsWith(".")) DisplayStatus("Extention starts from dot.");
            else
            {
                foreach (var item in filesList)
                {
                    string fullFilePath = item.ToString();
                    ChangeExtention(fullFilePath, IndicateNewPath(fullFilePath));
                }
                findFilesButton_Click(sender, e);
                DisplayStatus("All names change completed!");
            }
        }

        /// <summary>
        /// Finds full directory from selected file in itemFoundList.
        /// </summary>
        /// <returns></returns>
        private string FindSelectedFileLocalization()
        {
            foreach (var item in filesList)
            {
                if (item.ToString().Contains(itemsFoundList.SelectedItem.ToString())) return item.ToString();
            }
            return null;
        }

        /// <summary>
        /// Function to change old name or directory to new one.
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <param name="newExtentionPath"></param>
        private void ChangeExtention(string fullFilePath, string newExtentionPath)
        {
            try
            {
                File.Move(fullFilePath, newExtentionPath);
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find/change file.", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Indicates new file path depends on new extention in newExtetntionTextBox.
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns></returns>
        private string IndicateNewPath(string fullFilePath)
        {
            string directoryName, fileName, newExtentionPath;

            directoryName = System.IO.Path.GetDirectoryName(fullFilePath);
            fileName = System.IO.Path.GetFileNameWithoutExtension(fullFilePath);
            newExtentionPath = directoryName + System.IO.Path.DirectorySeparatorChar + fileName + newExtentionTextBox.Text;

            return newExtentionPath;
        }

        /// <summary>
        /// Clears statusTextBlock and displays messages given in arg.
        /// </summary>
        /// <param name="messageText"></param>
        private void DisplayStatus(string messageText)
        {
            statusTextBlock.Text = string.Empty;
            statusTextBlock.Text = messageText;
        }

    }
}
