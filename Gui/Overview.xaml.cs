using Domain;
using Domain.DTOs;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
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

namespace Gui
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Overview : Window
    {
        DomainManager _dc;

        public Overview(DomainManager dc)
        {
            _dc = dc;
            InitializeComponent();
            RefreshGebruikerOverzicht();
        }

        // toont alle gebruikers, indien searchbar leeg is = alle gebruikers | searchbar met text/naam = zoek naar naam
        private void RefreshGebruikerOverzicht()
        {
            if (searchTextBox.Text == "")
            {
                userListView.ItemsSource = _dc.GetGebruikers();
            } else
            {
                userListView.ItemsSource = _dc.ZoekGebruikerViaNaam(searchTextBox.Text);
            }
        }

        private void userListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                GebruikerDto selectedGebruiker = (GebruikerDto)userListView.SelectedItem;
            }
        }

        // refresh userlistview met ingegeven text/naam in searchbar
        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.RefreshGebruikerOverzicht();
        }

        // Button MAAK DAGPLAN die een nieuw window toont om een dagplan te maken
        // voor de geselecteerde gebruiker
        private void CreateDagplan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (userListView.SelectedItem == null)
                {
                    throw new GentseFeestenException("Geen gebruiker geselecteerd!");
                } else
                {
                    MaakDagplan maakDagplanWnd = new(_dc, (GebruikerDto)userListView.SelectedItem);
                    maakDagplanWnd.ShowDialog();
                }
            }
            catch (GentseFeestenException ex)
            {
                MessageBox.Show(ex.Message, "FOUT", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void ViewDagplan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (userListView.SelectedItem == null)
                {
                    throw new GentseFeestenException("Geen gebruiker geselecteerd!");
                } else
                {
                    ToonDagplan toonDagplan = new(_dc, (GebruikerDto)userListView.SelectedItem);
                    toonDagplan.ShowDialog();
                }
            }
            catch (GentseFeestenException ex)
            {
                MessageBox.Show(ex.Message, "FOUT", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
