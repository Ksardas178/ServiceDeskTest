using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.SupportedCompany;

namespace avis.ServiceDesk
{
  partial class SupportedCompanyServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      //Автоматическое заполнение полей.
      _obj.StartDate = Calendar.UserNow;
      _obj.Responsible = Sungero.Company.Employees.Current;
    }
  }


  partial class SupportedCompanyContactPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ContactFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Company == null) 
        return query;
      
      //Фильтрация контактных лиц по выбранной организации.
      return query.Where(p => p.Company == _obj.Company);
    }
  }

}