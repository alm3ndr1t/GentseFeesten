using Domain;
using Domain.DTOs;
using Domain.Exceptions;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Gui
{
    /// <summary>
    /// Interaction logic for ToonDagplan.xaml
    /// </summary>
    public partial class ToonDagplan : Window
    {
        private DomainManager _dm;
        private GebruikerDto _gebruiker;
        public ToonDagplan( DomainManager dm, GebruikerDto gebruiker)
        {
            try
            {
                _dm = dm;
                _gebruiker = gebruiker;
                InitializeComponent();
            }
            catch (GentseFeestenException ex)
            {
                MessageBox.Show(ex.Message, "FOUTMELDING", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void ToonDagplan_Click(object sender, RoutedEventArgs e)
        {
            if (DateEventPicker.SelectedDate.HasValue)
            {
                DagplanDto dagplan = _dm.GetDagplanOnDate(DateEventPicker.SelectedDate.Value);

                eventsListView.ItemsSource = dagplan.GeefEvenementen().OrderBy(e => e.StartUur).ToList();

            } else
            {
                // fout gooien wanneer er op de button gedrukt wordt maar geen datum gekozen is
                throw new GentseFeestenException("Selecteer een geldige datum!");
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a SaveFileDialog
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "AsciiDoc Files (*.adoc)|*.adoc|All Files (*.*)|*.*",
                DefaultExt = "adoc",
                Title = "Save Dagplan Export"
            };

            // Show the SaveFileDialog
            bool? result = saveFileDialog.ShowDialog();

            // Check if the user clicked "Save"
            if (result == true)
            {
                // Get the selected file name and location
                string filePath = saveFileDialog.FileName;

                // Iterate through the ListView items and generate AsciiDoc content
                StringBuilder asciiDocContent = new StringBuilder();

                // Add AsciiDoc document title
                asciiDocContent.AppendLine("= Dagplan Export");

                // Include user data in the export
                asciiDocContent.AppendLine("== Gebruiker");
                asciiDocContent.AppendLine($"=== Naam: {_gebruiker.Voornaam} {_gebruiker.Achternaam}");

                // Iterate through ListView items
                foreach (var item in eventsListView.Items)
                {
                    if (item is EvenementDto evenement)
                    {
                        // Add horizontal rule between events (except for the first event)
                        if (asciiDocContent.Length > 0)
                        {
                            asciiDocContent.AppendLine("'''\n");
                        }

                        // Add AsciiDoc table row for each event
                        asciiDocContent.AppendLine("== Evenement");

                        asciiDocContent.AppendLine("|===");
                        asciiDocContent.AppendLine($"| Gekozen gebruiker | {_gebruiker.Voornaam} {_gebruiker.Achternaam}");
                        asciiDocContent.AppendLine($"| UniqueId | {evenement.UniqueId}");
                        asciiDocContent.AppendLine($"| Titel | {evenement.Titel}");
                        asciiDocContent.AppendLine($"| Start | {evenement.StartUur.ToString("hh:mm tt")}");
                        asciiDocContent.AppendLine($"| Einde | {evenement.EindUur.ToString("hh:mm tt")}");
                        asciiDocContent.AppendLine($"| Prijs | {evenement.Prijs}");
                        asciiDocContent.AppendLine($"| Beschrijving | {(string.IsNullOrEmpty(evenement.Beschrijving) ? "geen beschrijving beschikbaar" : evenement.Beschrijving)}");
                        asciiDocContent.AppendLine("|===");
                    }
                }

                // Save the content to the selected file
                File.WriteAllText(filePath, asciiDocContent.ToString());

                // Notify the user about the successful export
                MessageBox.Show($"Dagplan exported to {filePath}", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                // Close the window after export
                Close();
            }
        }

    }
}
