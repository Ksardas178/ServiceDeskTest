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

    public override bool IsModuleVisible()
    {
      return true;
    }
  }
}
