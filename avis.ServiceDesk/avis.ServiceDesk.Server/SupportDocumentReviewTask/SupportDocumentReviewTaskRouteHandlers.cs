using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using avis.ServiceDesk.SupportDocumentReviewTask;

namespace avis.ServiceDesk.Server
{
  partial class SupportDocumentReviewTaskRouteHandlers
  {

    public virtual void StartBlock3(avis.ServiceDesk.Server.SupportDocumentReviewAssignmentArguments e)
    {
      var reviewer = Functions.SupportDocumentApprovalRole.GetRolePerformer(
        avis.ServiceDesk.SupportDocumentApprovalRole.Type.ReqResponsible,
        _obj);
      if (reviewer != null)
        e.Block.Performers.Add(reviewer);
    }

  }
}