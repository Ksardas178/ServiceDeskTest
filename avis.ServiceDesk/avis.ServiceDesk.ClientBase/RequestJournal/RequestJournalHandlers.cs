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

    public virtual void LaboriousnessValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      if (e.NewValue < 0)
        e.AddError(avis.ServiceDesk.RequestJournals.Resources.LaboriousnessNegativeError);
    }

    public virtual void LaboriousnessPlanValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      if (e.NewValue < 0)
        e.AddError(avis.ServiceDesk.RequestJournals.Resources.LaboriousnessNegativeError);
    }

    public virtual void SolutionDateValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      if (e.NewValue < _obj.RegistrationDate)
        e.AddError(avis.ServiceDesk.RequestJournals.Resources.SolutionDateError);
    }

    public virtual void SolutionDatePlanValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      if (e.NewValue < Calendar.UserNow)
        e.AddError(avis.ServiceDesk.RequestJournals.Resources.SolutionPlanDateError);
    }

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
        _obj.RequestState = ServiceDesk.RequestJournal.RequestState.Registration;
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
      {
        _obj.Priority = null;
        return;
      }
      
      //Автоматический подбор приоритета из справочника сравнения приоритетов.
      var priorityComparison = ServiceDesk.PriorityComparisons
        .GetAll()
        .Where(p => Equals(p.Urgency, urgency) && Equals(p.Impact, impact))
        .SingleOrDefault();
     
      //Не найден либо не заполнен приоритет в справочнике.
      if (priorityComparison == null || priorityComparison.Priority == null)
      {
        _obj.Priority = null;
        return;
      }
      
      //Установка приоритета.
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