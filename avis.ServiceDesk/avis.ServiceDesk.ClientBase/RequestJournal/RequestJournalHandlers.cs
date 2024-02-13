using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.RequestJournal;

namespace avis.ServiceDesk
{
  partial class RequestJournalClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      //Выдача прав на редактирование полей для Администраторов SD.
      var currentUser = Sungero.Company.Employees.Current;
      var adminsSD = Groups.GetAll(g => g.Name == avis.ServiceDesk.RequestJournals.Resources.SDAdmins).FirstOrDefault();
      if (adminsSD != null && currentUser.IncludedIn(adminsSD))
      {
        _obj.State.Properties.Communication.IsEnabled = true;
        _obj.State.Properties.SolutionCourse.IsEnabled = true;
      }
      
      //Установка состояния на "регистрацию" для новых и скопированных обращений.
      if (_obj.State.IsCopied || _obj.State.IsInserted)
        _obj.State = ServiceDesk.RequestJournal.RequestState.Registration;
    }

    public virtual void UrgencyValueInput(avis.ServiceDesk.Client.RequestJournalUrgencyValueInputEventArgs e)
    {
      UpdatePriority(e.NewValue, _obj.Impact);
    }

    public virtual void ImpactValueInput(avis.ServiceDesk.Client.RequestJournalImpactValueInputEventArgs e)
    {
      UpdatePriority(_obj.Urgency, e.NewValue);
    }
    
    private void UpdatePriority(IUrgency urgency, IImpact impact)
    {
      if (urgency == null || impact == null)
        return;
      
      //Автоматический подбор приоритета из справочника сравнения приоритетов.
      var priorityComparison = ServiceDesk.PriorityComparisons
        .GetAll()
        .Where(p => Equals(p.Urgency, urgency) && Equals(p.Impact, impact))
        .SingleOrDefault();
     
      if (priorityComparison == null || priorityComparison.Priority == null)
        return;
      
      _obj.Priority = priorityComparison.Priority;
      
      //Определение плановой даты выполнения.
      var reactionTime = _obj.Priority.ReactionTime;
      if (reactionTime != null)
        _obj.SolutionDatePlan = _obj.RegistrationDate.Value.AddDays(reactionTime.Value);
    }

    public virtual void ContactValueInput(avis.ServiceDesk.Client.RequestJournalContactValueInputEventArgs e)
    {
      //Автоматическая подстановка организации.
      if (_obj.Company == null)
        _obj.Company = e.NewValue.Company;
    }

  }
}