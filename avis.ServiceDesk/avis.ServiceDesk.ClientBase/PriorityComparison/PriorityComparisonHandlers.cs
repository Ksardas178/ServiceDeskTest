using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.PriorityComparison;

namespace avis.ServiceDesk
{

  partial class PriorityComparisonClientHandlers
  {

    public virtual void UrgencyValueInput(avis.ServiceDesk.Client.PriorityComparisonUrgencyValueInputEventArgs e)
    {
      UpdateName(e.NewValue, _obj.Impact);
    }

    public virtual void ImpactValueInput(avis.ServiceDesk.Client.PriorityComparisonImpactValueInputEventArgs e)
    {
      UpdateName(_obj.Urgency, e.NewValue);
    }
    
    /// <summary>
    /// Обновление наименования записи справочника.
    /// </summary>
    private void UpdateName(IUrgency urgency, IImpact impact)
    {
      _obj.Name = avis.ServiceDesk.PriorityComparisons.Resources.DefaultNameHintFormat
        (
          urgency != null ? urgency.DisplayValue : avis.ServiceDesk.PriorityComparisons.Resources.DefaultUrgencyHint,
          impact != null ? impact.DisplayValue : avis.ServiceDesk.PriorityComparisons.Resources.DefaultImpactHint
         );
    }
  }

}