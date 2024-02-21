using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.SupportDocumentApprovalRole;

namespace avis.ServiceDesk.Server
{
  partial class SupportDocumentApprovalRoleFunctions
  {
    public static Sungero.Company.IEmployee GetRolePerformer(
        Enumeration responsible, 
        avis.ServiceDesk.ISupportDocumentReviewTask task)
    {
      if (!Equals(responsible, ServiceDesk.SupportDocumentApprovalRole.Type.ReqResponsible))
        return null;
      
      var document = task.Attachments.FirstOrDefault();
      var supportDocument = SupportDocuments.As(document);
      if (supportDocument != null)
        return supportDocument.Request.Responsible;
      
      return null;
    }
  }
}