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
      Functions.PriorityComparison.UpdateName(_obj);
    }

    public virtual void ImpactValueInput(avis.ServiceDesk.Client.PriorityComparisonImpactValueInputEventArgs e)
    {
      Functions.PriorityComparison.UpdateName(_obj);
    }
  }

}