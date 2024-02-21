using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace avis.ServiceDesk.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      InitializationLogger.Debug("Инициализация модуля \"ServiceDesk\"");
      
      FillCatalogs();
      CreateDocumentTypes();
      CreateDocumentKinds();
      CreateRoles();
      CreateApprovalRole(ServiceDesk.SupportDocumentApprovalRole.Type.ReqResponsible, avis.ServiceDesk.Resources.ResponsibleForRequest);
      GrantRights();
    }
    
    #region [Выдача прав]
    
    private static void GrantRights()
    {
      InitializationLogger.Debug("Init: grant rights on catalogs");
      
      var sDAdmins = Roles.GetAll(r => r.Sid == ServiceDesk.Constants.Module.SDAdminsRoleGuid).First();
      var sDUsers = Roles.GetAll(r => r.Sid == ServiceDesk.Constants.Module.SDUsersRoleGuid).First();
      
      GrantRights(sDAdmins, DefaultAccessRightsTypes.FullAccess);
      GrantRights(sDUsers, DefaultAccessRightsTypes.Read);      
      ServiceDesk.RequestJournals.AccessRights.Grant(sDAdmins, DefaultAccessRightsTypes.FullAccess);
      ServiceDesk.RequestJournals.AccessRights.Grant(sDUsers, DefaultAccessRightsTypes.FullAccess);
      
      InitializationLogger.Debug("Init: grant rights on documents");
      
      ServiceDesk.SupportDocuments.AccessRights.Grant(sDAdmins, DefaultAccessRightsTypes.Create);
      ServiceDesk.SupportDocuments.AccessRights.Grant(sDUsers, DefaultAccessRightsTypes.Create);
    }
    
    private static void GrantRights(Sungero.CoreEntities.IRole role, Guid rightsType)
    {
      ServiceDesk.Impacts.AccessRights.Grant(role, rightsType);
      ServiceDesk.Urgencies.AccessRights.Grant(role, rightsType);
      ServiceDesk.Priorities.AccessRights.Grant(role, rightsType);
      ServiceDesk.PriorityComparisons.AccessRights.Grant(role, rightsType);
      ServiceDesk.SupportedCompanies.AccessRights.Grant(role, rightsType);
    }
    
    #endregion [Выдача прав]

    #region [Создание документов]
    
    /// <summary>
    /// Создание типов документов.
    /// </summary>
    private void CreateDocumentTypes()
    {
      var name = ServiceDesk.SupportDocuments.Info.LocalizedName;
      InitializationLogger.DebugFormat("Init: create document type '{0}'", name);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType
        (
          name,
          SupportDocument.ClassTypeGuid,
          Sungero.Docflow.DocumentType.DocumentFlow.Inner,
          true
         );
    }

    /// <summary>
    /// Создание видов документов.
    /// </summary>
    private void CreateDocumentKinds()
    {
      InitializationLogger.Debug("Init: create document kinds");
      
      var name = avis.ServiceDesk.Resources.SupportDocumentKindName;
      InitializationLogger.DebugFormat("Init: create '{0}' document kind", name);
      
      var notNumerable = Sungero.Docflow.DocumentKind.NumberingType.NotNumerable;
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind
        (
          name,
          avis.ServiceDesk.Resources.SupportDocumentKindShortName,
          notNumerable,
          Sungero.Docflow.DocumentType.DocumentFlow.Inner,
          false,
          false,
          SupportDocument.ClassTypeGuid,
          new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
          ServiceDesk.Constants.Module.SupportDocumentKindGuid
         );
    }
    
    #endregion [Создание документов]
    
    #region [Заполнение справочников]
    
    /// <summary>
    /// Заполнение справочников.
    /// </summary>
    private void FillCatalogs()
    {
      InitializationLogger.Debug("Init: fill catalogs.");
      
      var requestTypeNames = new List<string> {"Консультация", "Инцидент", "Запрос на изменение" };
      FillRequestTypeCatalog(requestTypeNames);
      
      var impactNames = new List<string> {"Низкое", "Среднее", "Высокое" };
      FillImpactCatalog(impactNames);
      
      var priorityNames = new List<string> {"Низкое", "Среднее", "Высокое" };
      FillPriorityCatalog(priorityNames);
    }
    
    /// <summary>
    /// Заполнение справочника "Типы обращений".
    /// </summary>
    /// <param name="requestTypenames">Список наименований.</param>
    private void FillRequestTypeCatalog(List<string> requestTypeNames)
    {
      InitializationLogger.Debug("Init: fill request types catalog.");
      foreach (var requestTypeName in requestTypeNames)
      {
        if (ServiceDesk.RequestTypes.GetAll(t => t.Name == requestTypeName).Any())
          continue;
        
        var requestType = ServiceDesk.RequestTypes.Create();
        requestType.Name = requestTypeName;
        requestType.Save();
      }
    }
    
    /// <summary>
    /// Заполнение справочника "Воздействия".
    /// </summary>
    /// <param name="impactNames">Список наименований.</param>
    private void FillImpactCatalog(List<string> impactNames)
    {
      InitializationLogger.Debug("Init: fill impacts catalog.");
      foreach (var impactName in impactNames)
      {
        if (ServiceDesk.Impacts.GetAll(i => i.Name == impactName).Any())
          continue;

        var impact = ServiceDesk.Impacts.Create();
        impact.Name = impactName;
        impact.Save();
      }
    }
    
    /// <summary>
    /// Заполнение справочника "Приоритеты".
    /// </summary>
    /// <param name="priorityNames">Список наименований.</param>
    private void FillPriorityCatalog(List<string> priorityNames)
    {
      InitializationLogger.Debug("Init: fill priorities catalog.");
      foreach (var priorityName in priorityNames)
      {
        if (ServiceDesk.Priorities.GetAll(p => p.Name == priorityName).Any())
          continue;
        
        var priority = ServiceDesk.Priorities.Create();
        priority.Name = priorityName;
        priority.Save();
      }
    }
    
    #endregion [Заполнение справочников]
    
    #region [Создание ролей]
    
    /// <summary>
    /// Создание ролей.
    /// </summary>
    private static void CreateRoles()
    {
      InitializationLogger.Debug("Init: create roles");
      
      Sungero.Docflow.Server.ModuleInitializer.CreateRole(ServiceDesk.Resources.SDAdmins, ServiceDesk.Resources.SDAdminsDescription, ServiceDesk.Constants.Module.SDAdminsRoleGuid);
      Sungero.Docflow.Server.ModuleInitializer.CreateRole(ServiceDesk.Resources.SDUsers, ServiceDesk.Resources.SDUsersDescription, ServiceDesk.Constants.Module.SDUsersRoleGuid);
    }
    
    /// <summary>
    /// Создание роли согласования.
    /// </summary>
    private static void CreateApprovalRole(Enumeration roleType, string description)
    {
      var role = SupportDocumentApprovalRoles
        .GetAll()
        .Where(r => Equals(r.Type, roleType))
        .FirstOrDefault();
      
      if (role == null)
      {
        role = SupportDocumentApprovalRoles.Create();
        role.Type = roleType;
      }
      role.Description = description;
      role.Save();
      InitializationLogger.DebugFormat
        (
          "Init: create role '{0}'",
          SupportDocumentApprovalRole.TypeItems.GetLocalizedValue(roleType, System.Globalization.CultureInfo.CurrentUICulture)
         );
    }
    
    #endregion [Создание ролей]
    
    public override bool IsModuleVisible()
    {
      return 
        Users.Current.IncludedIn(ServiceDesk.Constants.Module.SDAdminsRoleGuid) ||
        Users.Current.IncludedIn(ServiceDesk.Constants.Module.SDUsersRoleGuid);
    }
  }
}
