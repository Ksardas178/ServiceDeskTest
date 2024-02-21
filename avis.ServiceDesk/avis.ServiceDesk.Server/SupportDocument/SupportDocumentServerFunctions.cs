using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.SupportDocument;

namespace avis.ServiceDesk.Server
{
  partial class SupportDocumentFunctions
  {

    /// <summary>
    /// Создание задачи на рассмотрение документа.
    /// </summary>
    [Remote]
    public avis.ServiceDesk.ISupportDocumentReviewTask CreateReviewTask()
    {
      var task = ServiceDesk.SupportDocumentReviewTasks.Create();
      task.Attachments.Add(_obj);
      task.Subject = avis.ServiceDesk.SupportDocuments.Resources.SuportDocumentReviewTaskThemeFormat(_obj.Request.Name);
      
      return task;
    }

  }
}