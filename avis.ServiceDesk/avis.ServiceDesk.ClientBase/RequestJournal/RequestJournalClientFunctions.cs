using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.RequestJournal;

namespace avis.ServiceDesk.Client
{
  partial class RequestJournalFunctions
  {

    /// <summary>
    /// Установка доступности полей карточки обращения.
    /// </summary>    
    public void SetAvailableRequestFields()
    {
      var alive = !Equals(_obj.RequestState, ServiceDesk.RequestJournal.RequestState.Closed);
      
      _obj.State.Properties.SolutionDate.IsEnabled = !alive;
      _obj.State.Properties.Laboriousness.IsEnabled = !alive;
      
      _obj.State.Properties.Impact.IsEnabled = alive;
      _obj.State.Properties.Urgency.IsEnabled = alive;
      _obj.State.Properties.Contact.IsEnabled = alive;
      _obj.State.Properties.Company.IsEnabled = alive;
      _obj.State.Properties.SolutionDatePlan.IsEnabled = alive;
      _obj.State.Properties.LaboriousnessPlan.IsEnabled = alive;
    }

  }
}