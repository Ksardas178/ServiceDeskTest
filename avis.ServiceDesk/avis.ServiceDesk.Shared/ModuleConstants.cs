using System;
using Sungero.Core;

namespace avis.ServiceDesk.Constants
{
  public static class Module
  {
    /// GUID роли "Администраторы SD".
    public static readonly Guid SDAdminsRoleGuid = Guid.Parse("F040EF00-B58E-4664-A8A5-C05C8BDCF760");
 
    /// GUID роли "Пользователи SD".
    public static readonly Guid SDUsersRoleGuid = Guid.Parse("A98C3C6E-D524-45B8-A479-C0100E8BF145");
    
    /// GUID типа документа "Документ по поддержке".
    public static readonly Guid SupportDocumentKindGuid = Guid.Parse("5324E434-358A-4A99-951E-A95F34840803");

  }
}