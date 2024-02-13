using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.CompanyOnSupport;

namespace avis.ServiceDesk
{
  partial class CompanyOnSupportSharedHandlers
  {

    public virtual void CompanyChanged(avis.ServiceDesk.Shared.CompanyOnSupportCompanyChangedEventArgs e)
    {
      if (e.NewValue == null)
        return;
      
      //Автоматическое именование записи.
      _obj.Name = e.NewValue.Name;
    }

  }
}