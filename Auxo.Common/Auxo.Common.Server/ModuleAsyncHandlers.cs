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
      args.Retry = false;
      var postfix = string.Format("UpdateEntityProperties_{0}", args.ObjTypeId);
      Logger.WithLogger(postfix).Debug(string.Format("Handler starting. ObjTypeId = {0}", args.ObjTypeId));
      
      var isClosedNullTypes = Sungero.Docflow.PublicFunctions.Module.Remote.GetDocflowParamsStringValue(Constants.Module.ClosedNullTypesParamName);
      if (isClosedNullTypes == true.ToString() && args.RetryIteration <= Constants.Module.MaxRetryIteration)
      {
        args.Retry = true;
        args.NextRetryTime = Calendar.Now.AddMinutes(Constants.Module.DelayedRetryMinutes);
        Logger.WithLogger(postfix).Debug(string.Format("Delayed update restart {0}. Closing of inactive types has been started", args.ObjTypeId));
      }
        
      var objType = ObjTypes.GetAll()
        .Where(_ => _.Id == args.ObjTypeId)
        .Where(_ => _.Status == Common.ObjType.Status.Active)
        .FirstOrDefault();
      
      if (objType == null)
      {
        Logger.WithLogger(postfix).Error(string.Format("Unable to find record with ID {0}", args.ObjTypeId));
        return;
      }
      
      try
      {
        Functions.ObjProperty.UpdateEntityProperties(objType);
        Logger.WithLogger(postfix).Debug("Handler complite");
      }
      catch (Exception ex)
      {
        Logger.WithLogger(postfix).Error(ex, "An error occurred while updating the object type properties.");
      }
    }

  }
}