using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.RequestJournal;

namespace avis.ServiceDesk
{
  partial class RequestJournalCreatingFromServerHandler
  {

    public override void CreatingFrom(Sungero.Domain.CreatingFromEventArgs e)
    {
      base.CreatingFrom(e);
      
      e.Without(_info.Properties.SolutionDate);
      e.Without(_info.Properties.Laboriousness);
    }
  }

  partial class RequestJournalServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      //Попытка присвоить среднюю срочность.
      _obj.Urgency = ServiceDesk.Urgencies.GetAll().Where(u => u.Name == "Средняя").SingleOrDefault();
      //Попытка присвоить среднее воздействие.
      _obj.Impact = ServiceDesk.Impacts.GetAll().Where(i => i.Name == "Среднее").SingleOrDefault();
      _obj.Responsible = Sungero.Company.Employees.Current;
      _obj.RegistrationDate = Calendar.UserNow;
    }
  }

  partial class RequestJournalContactPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ContactFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      //Фильтрация по организациям на поддержке.
      var supportedCompanies = ServiceDesk.SupportedCompanies.GetAll();
      //Фильтрация по выбранной организации (если есть).
      var matchingCompanies = (_obj.Company != null) ? 
        supportedCompanies.Where(c => Equals(c.Company, _obj.Company)) :
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