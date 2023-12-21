using Domain.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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
using Domain;
using Domain.DTOs;
using Domain.Exceptions;

namespace Gui
{
    public partial class MaakDagplan : Window
    {
        private DomainManager _dc;
        private Dagplan _dagplan;
        private Gebruiker _gebruiker;

        public MaakDagplan(DomainManager dc, GebruikerDto gebruiker)
        {
            try
            {
                _dc = dc;
                _gebruiker = new Gebruiker(gebruiker.Voornaam, gebruiker.Achternaam, gebruiker.DagBudget, gebruiker.GebruikerId);
                InitializeComponent();
            }
            catch (GentseFeestenException ex)
            {
                MessageBox.Show(ex.Message, "FOUTMELDING", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void RefreshMaakDagplan()
        {
            if (_dagplan != null && string.IsNullOrEmpty(EventSearch.Text))
            {
                eventsListView.ItemsSource = _dc.GeefEvenementOpDatum(_dagplan.Datum);
            } else if (_dagplan != null)
            {
                eventsListView.ItemsSource = _dc.ZoekEvenementenOpTitel(_dc.GeefEvenementOpDatum(_dagplan.Datum), EventSearch.Text);
            }
        }

        private void EventSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshMaakDagplan();
        }

        private void DateEventPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateEventPicker.SelectedDate.HasValue)
            {
                _dagplan = new Dagplan(DateEventPicker.SelectedDate.Value, _gebruiker);
                RefreshMaakDagplan();
            }
        }
        private void AddEvenement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Checken of er wel evenementen geselecteerd zijn
                // Nee: gooi exceptie
                // Ja: Cast ze en voeg ze toe
                if (eventsListView.SelectedItems.Count == 0)
                {
                    throw new GentseFeestenException("Geen evenement(en) geselecteerd!");
                } else
                {
                    foreach (EvenementDto dto in eventsListView.SelectedItems)
                    {
                        Evenement ev = dto.ParseEvenementDto();
                        _dagplan.VoegEvenementToe(ev);
                    }
                    MessageBox.Show("Evenement(en) toegevoegd.", "Toevoegen geslaagd", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (GentseFeestenException ex)
            {
                MessageBox.Show(ex.Message, "FOUT", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                // Copy the error message to the clipboard
                Clipboard.SetText(ex.Message);
            }
        }

        private void SaveDagplanClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_dagplan.GeefEvenementen().Count() == 0)
                {
                    throw new GentseFeestenException("Leeg dagplan. Voeg evenementen toe!");
                } else if (_dc.DagplanExistsOnDate(DateEventPicker.SelectedDate.Value))
                {
                    throw new GentseFeestenException("Voor het gekozen datum bestaat al een dagplan!");
                } else
                {
                    _dc.SaveDagplan(_dagplan);

                    // Show success message
                    MessageBox.Show("Dagplan opslaan succesvol!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Close the window
                    Close();
                }
            }
            catch (GentseFeestenException ex)
            {
                MessageBox.Show(ex.Message, "FOUT", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
