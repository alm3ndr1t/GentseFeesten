using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IDagplanRepo
    {
        public void SaveDagplan(Dagplan dagplan);
        public Dagplan GeefDagplanOpDatum(DateTime datum);
        bool DagplanExistsOnDate(DateTime date);
    }
}
