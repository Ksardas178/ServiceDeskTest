using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.PriorityComparison;

namespace avis.ServiceDesk
{
  partial class PriorityComparisonServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.Name = avis.ServiceDesk.PriorityComparisons.Resources.DefaultNameHintFormat
        (
          avis.ServiceDesk.PriorityComparisons.Resources.DefaultUrgencyHint,
          avis.ServiceDesk.PriorityComparisons.Resources.DefaultImpactHint
         );
    }
  }

}