using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.RequestJournal;

namespace avis.ServiceDesk
{
  partial class RequestJournalFilteringServerHandler<T>
  {

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      if (_filter == null)
        return query;
      
      if (_filter.Company != null)
        query = query.Where(c => Equals(c.Company, _filter.Company));
      
      if (_filter.Responsible != null)
        query = query.Where(c => Equals(c.Responsible, _filter.Responsible));
      
      //Фильтрация по статусу.
      if (_filter.InWork || _filter.Registration || _filter.Awaiting || _filter.Closed)
        query = query.Where(c =>
                            _filter.InWork       && Equals(c.RequestState, ServiceDesk.RequestJournal.RequestState.InWork)       ||
                            _filter.Registration && Equals(c.RequestState, ServiceDesk.RequestJournal.RequestState.Registration) ||
                            _filter.Awaiting     && Equals(c.RequestState, ServiceDesk.RequestJournal.RequestState.Awaiting)     ||
                            _filter.Closed       && Equals(c.RequestState, ServiceDesk.RequestJournal.RequestState.Closed));
      
      //Фильтрация по датам.
      if (!_filter.All)
      {
        var currentMonth = Calendar.BeginningOfMonth(Calendar.UserNow);
        var previousMonth = Calendar.PreviousMonth(Calendar.UserNow);
        
        var fromTime = _filter.PeriodRangeFrom;
        var toTime = _filter.PeriodRangeTo;
        
        query = query.Where(c =>
                            _filter.CurentMonth && c.SolutionDate >= currentMonth ||
                            _filter.PreviousMonth && c.SolutionDate >= previousMonth && c.SolutionDate < currentMonth ||
                            _filter.Period && c.SolutionDate >= fromTime && c.SolutionDate <= toTime);
      }
      
      return query;
    }
  }

  partial class RequestJournalCreatingFromServerHandler
  {

    public override void CreatingFrom(Sungero.Domain.CreatingFromEventArgs e)
    {
      base.CreatingFrom(e);
      
      e.Without(_info.Properties.Name);
      e.Without(_info.Properties.Number);
      e.Without(_info.Properties.SolutionDate);
      e.Without(_info.Properties.Laboriousness);
    }
  }

  partial class RequestJournalServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.Number = avis.ServiceDesk.RequestJournals.Resources.RequestNumberFormat(_obj.Id);
      _obj.RegistrationDate = Calendar.UserNow;
      _obj.Name = avis.ServiceDesk.RequestJournals.Resources.RequestNameFormat(_obj.Number, _obj.RegistrationDate.Value.ToString(avis.ServiceDesk.Resources.TimeFormat));
      //Попытка присвоить среднюю срочность.
      _obj.Urgency = ServiceDesk.Urgencies.GetAll().Where(u => u.Name == avis.ServiceDesk.RequestJournals.Resources.DefaultUrgencyName).SingleOrDefault();
      //Попытка присвоить среднее воздействие.
      _obj.Impact = ServiceDesk.Impacts.GetAll().Where(i => i.Name == avis.ServiceDesk.RequestJournals.Resources.DefaultImpactName).SingleOrDefault();
      _obj.Responsible = Sungero.Company.Employees.Current;
    }
  }

  partial class RequestJournalContactPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ContactFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      //Фильтрация по организациям на поддержке.
      var supportedCompanies = ServiceDesk.SupportedCompanies.GetAll().Select(c => c.Company);
      //Фильтрация по выбранной организации (если есть).
      var matchingCompanies = (_obj.Company != null) ?
        supportedCompanies.Where(c => Equals(c, _obj.Company)) :
        supportedCompanies;
      //Выбор ID подходящих организаций.
      var matchingIDs = matchingCompanies.Select(c => c.Id);
      
      //Выбор контактных лиц из организаций с подходящими ID.
      return query.Where(c => matchingIDs.Contains(c.Company.Id)); 
    }
  }

  partial class RequestJournalCompanyPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> CompanyFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      //Фильтрация по организациям на поддержке.
      var matchingIDs = ServiceDesk.SupportedCompanies
        .GetAll()
        .Where(c => c.Company != null)
        .Select(c => c.Company.Id);
      
      return query.Where(c => matchingIDs.Contains(c.Id));
    }
  }

}