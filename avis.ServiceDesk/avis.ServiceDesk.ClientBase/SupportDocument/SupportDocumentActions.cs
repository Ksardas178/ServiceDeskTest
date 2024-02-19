using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ServiceDesk.SupportDocument;

namespace avis.ServiceDesk.Client
{
  partial class SupportDocumentActions
  {
    public virtual void SendForReview(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      
    }

    public virtual bool CanSendForReview(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}