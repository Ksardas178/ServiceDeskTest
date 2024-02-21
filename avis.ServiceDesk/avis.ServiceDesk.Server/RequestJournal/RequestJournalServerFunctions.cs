using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.RequestJournal;

namespace avis.ServiceDesk.Server
{
  partial class RequestJournalFunctions
  {
    /// <summary>
    /// Формирование данных о ФИО.
    /// </summary>
    /// <param name="fulName">Полное имя.</param>
    /// <returns></returns>
    public static Structures.RequestJournal.NameData GetNameData(string fullName)
    {
      var parts = fullName.Trim().Split(' ');
      
      if (parts.Length < 2 || parts.Length > 3) 
        throw new ArgumentException("Person initials count mismatch.");
      
      var nameData = Structures.RequestJournal.NameData.Create();
      
      nameData.LastName = parts[0];
      nameData.FirstName = parts[1];
      if (parts.Length == 3)
        nameData.MiddleName = parts[2];
      
      return nameData;
    }

    /// <summary>
    /// Уведомление автора обращения о регистрации.
    /// </summary>
    [Remote (IsPure = true)]
    public void SendRegistrationNotificationLetter()
    {
      var addressee = _obj.Contact;
      if (addressee.Email == null)
        return;
      
      var mail = Mail.CreateMailMessage();
      mail.From = avis.ServiceDesk.RequestJournals.Resources.SupportMail;
      mail.To.Add(addressee.Email);
      mail.Subject = avis.ServiceDesk.RequestJournals.Resources.MailSubjectFormat(_obj.Number, _obj.Description);      
      mail.Body = GetRegistrationMailBody(_obj);
      
      Mail.Send(mail);
    }

    /// <summary>
    /// Формирование тела письма с уведомлением о регистрации.
    /// </summary>
    /// <param name="_obj">Обращение (запись журнала).</param>
    /// <returns>Текст письма.</returns>
    private string GetRegistrationMailBody(IRequestJournal _obj)
    {
      const int MaxThemeAsterics = 50;
      const char filler = '*';
      
      //Формирование темы уведомления.
      var fillerLength = Math.Min(MaxThemeAsterics, _obj.Description.Length);
      var fillerLine = new string(filler, fillerLength);
      var requestName = string.Join(Environment.NewLine, fillerLine, _obj.Description, fillerLine);
      var theme = avis.ServiceDesk.RequestJournals.Resources.MailThemeFormat
        (
          GetAddresseeName(_obj.Contact),
          GetGreeting(),
          requestName
         );
      
      //Формирование общей информации по обращению.
      var requestInfo = avis.ServiceDesk.RequestJournals.Resources.RequestInfoFormat
        (
          _obj.Number,
          _obj.RequestType.DisplayValue,
          _obj.Urgency == null ? string.Empty : _obj.Urgency.DisplayValue,
          _obj.Impact == null ? string.Empty : _obj.Impact.DisplayValue,
          _obj.SolutionDatePlan == null ? string.Empty : _obj.SolutionDatePlan.Value.ToString(avis.ServiceDesk.Resources.TimeFormat),
          _obj.LaboriousnessPlan == null ? string.Empty : _obj.LaboriousnessPlan.Value.ToString()
         );
      
      //Формирование реквизитов отправителя.
      var contacts = string.Join
        (
          Environment.NewLine,
          avis.ServiceDesk.RequestJournals.Resources.CompanyAddress,
          avis.ServiceDesk.RequestJournals.Resources.CompanyNumber,
          avis.ServiceDesk.RequestJournals.Resources.CompanySite
         );
      var credentials = avis.ServiceDesk.RequestJournals.Resources.MailCredentialsFormat(contacts);
      
      return string.Concat
        (
          theme, Environment.NewLine,
          requestInfo, fillerLine,
          credentials
         );
    }
    
    /// <summary>
    /// Формирование имени адресата.
    /// </summary>
    /// <param name="person">Адресат.</param>
    /// <returns>ИО либо полное имя при отсутствии отчества.</returns>
    private string GetAddresseeName(Sungero.Parties.IContact contact)
    {
      var nameData = Functions.RequestJournal.GetNameData(contact.Name);
      
      return (nameData.MiddleName == null) ?
        nameData.FullName :
        string.Format("{0} {1}", nameData.FirstName, nameData.MiddleName);
    }
    
    /// <summary>
    /// Формирование приветствия.
    /// </summary>
    /// <returns>Приветственная фраза.</returns>
    private string GetGreeting()
    {
      var hour = Calendar.UserNow.Hour;
      return (hour >= 12 && hour < 17) ?
        avis.ServiceDesk.RequestJournals.Resources.GoodAfternoon :
        avis.ServiceDesk.RequestJournals.Resources.Greetings;
    }
  }
}