using Domain;
using Domain.Interfaces;
using Persistentie;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_StartUp(object sender, StartupEventArgs e)
        {
            IEvenementRepo evenementRepo = new EvenementRepoDb(@"Data Source=.\SQLEXPRESS;Initial Catalog=gentseDB;Integrated Security=True");
            IGebruikerRepo gebruikerRepo = new GebruikerRepoDb(@"Data Source=.\SQLEXPRESS;Initial Catalog=gentseDB;Integrated Security=True");
            IDagplanRepo dagplanRepo = new DagplanRepoDb(@"Data Source=.\SQLEXPRESS;Initial Catalog=gentseDB;Integrated Security=True");

            DomainManager dc = new DomainManager(evenementRepo, gebruikerRepo, dagplanRepo);

            Overview overviewWnd = new Overview(dc);
            overviewWnd.Show();
        }
    }
}
