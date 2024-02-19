using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.ServiceDesk.Structures.RequestJournal
{

  /// <summary>
  /// Имя персоны.
  /// </summary>
  partial class NameData
  {
    /// Имя.
    public string FirstName { get; set; }
    /// Отчество.
    public string MiddleName { get; set; }
    /// Фамилия.
    public string LastName { get; set; }
    /// ФИО.
    public string FullName 
    {
      get 
      {
        var initials = new string[] {LastName, FirstName, MiddleName};
        return string.Join(" ", initials.Select(i => string.IsNullOrEmpty(i)));
      }
      set { FullName = value; }
    }
  }

}