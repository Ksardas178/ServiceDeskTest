using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.PriorityComparison;

namespace avis.ServiceDesk.Client
{
  partial class PriorityComparisonFunctions
  {

    /// <summary>
    /// Обновление наименования записи справочника.
    /// </summary>   
    public void UpdateName()
    {
      _obj.Name = string.Format("{0}_{1}", _obj.Urgency.DisplayValue, _obj.Priority.DisplayValue);
    }

  }
}