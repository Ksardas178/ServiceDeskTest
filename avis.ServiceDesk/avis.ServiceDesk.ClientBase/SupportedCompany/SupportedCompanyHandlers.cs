using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.SupportedCompany;

namespace avis.ServiceDesk
{
  partial class SupportedCompanyClientHandlers
  {

    public virtual void ContactValueInput(avis.ServiceDesk.Client.SupportedCompanyContactValueInputEventArgs e)
    {
      if (e.NewValue == null) 
        return;
      
      //Подстановка организации для выбранного контактного лица.
      _obj.Company = e.NewValue.Company;
    }

  }
}