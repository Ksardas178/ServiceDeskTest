using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.RequestJournal;

namespace avis.ServiceDesk.Client
{
  partial class RequestJournalActions
  {
    
    #region [Переписка по обращению]
    
    /// <summary>
    /// Открыть окно для дополнения текста переписки.
    /// </summary>
    /// <param name="e"></param>
    public virtual void AddCommunication(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog(avis.ServiceDesk.RequestJournals.Resources.CommunicationDialogHeader);
      var text = dialog.AddMultilineString(string.Empty, true);

      var addButton = dialog.Buttons.AddCustom(avis.ServiceDesk.RequestJournals.Resources.AddCommunicationButtonName);
      dialog.Buttons.AddCancel();
      
      if (dialog.Show() == addButton)
        AddCommunicationText(text.Value);
    }
    
    public virtual bool CanAddCommunication(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }
    
    /// <summary>
    /// Дополнение текста взаимодействия с автором обращения.
    /// </summary>
    /// <param name="worksDescription">Описание работ.</param>
    private void AddCommunicationText(string worksDescription)
    {
      //Форматирование текста комментария.
      var commentText = string.Concat
        (
          GetCommunicationHeader(),
          Environment.NewLine,
          worksDescription,
          Environment.NewLine
         );
      
      _obj.Communication = commentText + _obj.Communication;
    }
    
    /// <summary>
    /// Формирование заголовка текста переписки.
    /// </summary>
    /// <returns></returns>
    private string GetCommunicationHeader()
    {
      const int MaxNameAlignmentWidth = 20;
      const int MinAsteriskSymbols = 10;
      const char filler = '*';
      
      //Имя автора.
      var person = Sungero.Company.Employees.Current.Person;
      var name = (person.MiddleName == null) ?
        person.Name :
        string.Format("{0} {1}. {2}", person.FirstName, person.MiddleName[0], person.LastName);
      
      //Тело заголовка.
      var dateString = Calendar.Now.ToString(avis.ServiceDesk.Resources.TimeFormat);
      var headerBody = string.Format(" {0} {1}.", name, dateString);
      
      //Подбор длины заполнителя.
      var astericsToAdd = Math.Max(0, MaxNameAlignmentWidth - name.Length);
      var fillerLength = MinAsteriskSymbols + (astericsToAdd / 2);
      if (astericsToAdd % 2 == 1)
        headerBody += ' ';
      
      return string.Format
        (
          "{0}{1}{0}",
          new string(filler, fillerLength),
          headerBody
         );
    }

    #endregion [Переписка по обращению]

    #region [Задачи по обращению]
    
    /// <summary>
    /// Формирование задачи для ответственного за обращение.
    /// </summary>
    /// <param name="e"></param>
    public virtual void SendTaskForResponsible(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      //Обработка отсутствующего исполнителя.
      if (_obj.Responsible == null)
      {
        e.AddError(avis.ServiceDesk.RequestJournals.Resources.ResponsibleEmptyError);
        return;
      }
      
      Sungero.Workflow.SimpleTasks.Create(_obj.Name, _obj.Responsible).Show();
    }

    public virtual bool CanSendTaskForResponsible(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }
    
    #endregion [Задачи по обращению]

    #region [Переписка по E-Mail]
    
    /// <summary>
    /// Сформировать письмо по шаблону для отправки автору обращения.
    /// </summary>
    /// <param name="e"></param>
    public virtual void SendLetter(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var mail = MailClient.CreateMail();
      var address = _obj.Contact.Email;
      if (!string.IsNullOrEmpty(address))
        mail.To.Add(address);
      mail.Subject = avis.ServiceDesk.RequestJournals.Resources.MailSubjectFormat(_obj.Number, _obj.Description);
      
      mail.Show();
    }

    public virtual bool CanSendLetter(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }

    /// <summary>
    /// Уведомление автора обращения о регистрации.
    /// </summary>
    /// <param name="e"></param>
    public virtual void NotifyRegistered(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      //Проверка наличия адреса электронной почты.
      if (string.IsNullOrWhiteSpace(_obj.Contact.Email))
      {
        e.AddError(avis.ServiceDesk.RequestJournals.Resources.ContactMailEmptyError);
        return;
      }
      
      Functions.RequestJournal.Remote.SendRegistrationNotificationLetter(_obj);
    }

    public virtual bool CanNotifyRegistered(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }
    
    #endregion [Переписка по E-Mail]

    #region [Перевод состояний обращения]
    
    /// <summary>
    /// Закрытие обращения.
    /// </summary>
    /// <param name="e"></param>
    public virtual void SetClosed(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.RequestState = ServiceDesk.RequestJournal.RequestState.Closed;
      _obj.SolutionDate = Calendar.Now;
      Functions.RequestJournal.SetAvailableRequestFields(_obj);
    }

    public virtual bool CanSetClosed(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !Equals(_obj.RequestState, ServiceDesk.RequestJournal.RequestState.Closed);
    }

    /// <summary>
    /// Установка состояния "Ожидание ответа автора".
    /// </summary>
    /// <param name="e"></param>
    public virtual void SetAwaiting(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      DropSolution();
      _obj.RequestState = ServiceDesk.RequestJournal.RequestState.Awaiting;
      Functions.RequestJournal.SetAvailableRequestFields(_obj);
    }

    public virtual bool CanSetAwaiting(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return
        !Equals(_obj.RequestState, ServiceDesk.RequestJournal.RequestState.Awaiting) &&
        !Equals(_obj.RequestState, ServiceDesk.RequestJournal.RequestState.Closed);
    }

    /// <summary>
    /// Отправка обращения в работу.
    /// </summary>
    /// <param name="e"></param>
    public virtual void SetInWork(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      DropSolution();
      _obj.RequestState = ServiceDesk.RequestJournal.RequestState.InWork;
      Functions.RequestJournal.SetAvailableRequestFields(_obj);
    }

    public virtual bool CanSetInWork(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return
        !Equals(_obj.RequestState, ServiceDesk.RequestJournal.RequestState.InWork) &&
        !Equals(_obj.RequestState, ServiceDesk.RequestJournal.RequestState.Closed);
    }
        
    /// <summary>
    /// Сброс полей с информацией о параметрах решения
    /// при смене состояния обращения.
    /// </summary>
    private void DropSolution()
    {
      if (!Equals(_obj.RequestState, ServiceDesk.RequestJournal.RequestState.Closed))
        return;
      
      _obj.SolutionDate = null;
    }
    
    #endregion [Перевод состояний обращения]

  }


}