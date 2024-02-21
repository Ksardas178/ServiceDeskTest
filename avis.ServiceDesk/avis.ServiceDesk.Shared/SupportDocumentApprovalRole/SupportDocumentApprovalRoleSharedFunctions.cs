using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.SupportDocumentApprovalRole;

namespace avis.ServiceDesk.Shared
{
  partial class SupportDocumentApprovalRoleFunctions
  {
    public override List<Sungero.Docflow.IDocumentKind> Filter(List<Sungero.Docflow.IDocumentKind> kinds)
    {
      var query = base.Filter(kinds);
      if (_obj.Type == ServiceDesk.SupportDocumentApprovalRole.Type.ReqResponsible)
        query = query.Where(k => k.DocumentType.DocumentTypeGuid == "a017bbf2-d7b5-449e-8d42-6630a7b667cc").ToList();
      
      return query;
    }
  }
}