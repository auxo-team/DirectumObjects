using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Auxo.Common.Server
{
  public class ModuleAsyncHandlers
  {

    /// <summary>
    /// Актуализировать свойства сущности.
    /// </summary>
    /// <param name="args">Аргументы обработчика.</param>
    public virtual void UpdateEntityProperties(Auxo.Common.Server.AsyncHandlerInvokeArgs.UpdateEntityPropertiesInvokeArgs args)
    {
      var postfix = string.Format("UpdateEntityProperties_{0}", args.ObjTypeId);
      Logger.WithLogger(postfix).Debug(string.Format("Handler starting. ObjTypeId = {0}", args.ObjTypeId));
      
      
      Logger.WithLogger(postfix).Debug("Handler complite");
    }

  }
}