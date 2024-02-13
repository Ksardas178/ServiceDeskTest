using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.SupportedCompany;

namespace avis.ServiceDesk
{
  partial class SupportedCompanySharedHandlers
  {

    public virtual void CompanyChanged(avis.ServiceDesk.Shared.SupportedCompanyCompanyChangedEventArgs e)
    {
      if (e.NewValue == null)
        return;
      
      //Автоматическое именование записи.
      _obj.Name = e.NewValue.Name;
    }

  }
}