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
      FillCatalogs();
    }

    /// <summary>
    /// Заполнение справочников.
    /// </summary>
    private void FillCatalogs()
    {
      var requestTypeNames = new List<string> {"Консультация", "Инцидент", "Запрос на изменение" };
      FillImpactCatalog(requestTypeNames);
      
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
      foreach (var requestTypeName in requestTypeNames)
      {
        if (ServiceDesk.RequestTypes.GetAll(t => t.Name == requestTypeName).Any())
        {
          var requestType = ServiceDesk.RequestTypes.Create();
          requestType.Name = requestTypeName;
          requestType.Save();
        }
      }
    }
    
    /// <summary>
    /// Заполнение справочника "Воздействия".
    /// </summary>
    /// <param name="impactNames">Список наименований.</param>
    private void FillImpactCatalog(List<string> impactNames)
    {
      foreach (var impactName in impactNames)
      {
        if (ServiceDesk.Impacts.GetAll(i => i.Name == impactName).Any())
        {
          var impact = ServiceDesk.Impacts.Create();
          impact.Name = impactName;
          impact.Save();
        }
      }
    }
    
    /// <summary>
    /// Заполнение справочника "Приоритеты".
    /// </summary>
    /// <param name="priorityNames">Список наименований.</param>
    private void FillPriorityCatalog(List<string> priorityNames)
    {
      foreach (var priorityName in priorityNames)
      {
        if (ServiceDesk.Priorities.GetAll(p => p.Name == priorityName).Any())
        {
          var priority = ServiceDesk.Priorities.Create();
          priority.Name = priorityName;
          priority.Save();
        }
      }
    }
    
    public override bool IsModuleVisible()
    {
      return true;
    }
  }
}
