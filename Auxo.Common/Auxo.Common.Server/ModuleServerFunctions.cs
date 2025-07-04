using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Shared;
using Sungero.Domain.SessionExtensions;

namespace Auxo.Common.Server
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Отправить уведомление.
    /// </summary>
    /// <param name="subject">Тема.</param>
    /// <param name="text">Текст.</param>
    /// <param name="recipient">Список адресатов.</param>
    /// <param name="attachment">Список вложений.</param>
    [Public, Remote]
    public static long? SendNotice(string subject,
                                  string text,
                                  List<Sungero.CoreEntities.IRecipient> recipients,
                                  List<Sungero.Domain.Shared.IEntity> attachments)
    {
      long? taskID = null;
      
      if (!recipients.Any())
        return taskID;
      
      if (subject.Length > 250)
        subject = subject.Substring(0, 250);
      
      Sungero.Workflow.ISimpleTask task;
      if (attachments != null && attachments.Any() && !attachments.Any(a => a == null))
        task = Sungero.Workflow.SimpleTasks.CreateWithNotices(subject, recipients, attachments.ToArray());
      else
        task = Sungero.Workflow.SimpleTasks.CreateWithNotices(subject, recipients.ToArray());
      
      task.ActiveText = text;
      
      try
      {
        task.Save();
        task.Start();
        taskID = task.Id;
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("SendNotice: {0}", ex.Message);
      }
      
      return taskID;
    }
    
    /// <summary>
    /// Получить объект по идентификатору.
    /// </summary>
    /// <param name="typeGuid">Guid типа объекта.</param>
    /// <param name="entityId">Идентификатор объекта.</param>
    /// <returns>Объект.</returns>
    [Public, Remote(IsPure = true)]
    public static Sungero.Domain.Shared.IEntity GetEntityById(Guid typeGuid, long entityId)
    {
      var entityType = Sungero.Domain.Shared.TypeExtension.GetTypeByGuid(typeGuid);
      using (var session = new Sungero.Domain.Session())
      {
        return session.GetAll(entityType).Where(_ => _.Id == entityId).FirstOrDefault();
      }
    }
    
    /// <summary>
    /// Получить объекты по типу.
    /// </summary>
    /// <param name="typeGuid">Guid типа объекта.</param>
    /// <returns>Список объектов.</returns>
    [Public, Remote(IsPure = true)]
    public static IQueryable<Sungero.Domain.Shared.IEntity> GetEntityByTypeGuid(Guid typeGuid)
    {
      var entityType = Sungero.Domain.Shared.TypeExtension.GetTypeByGuid(typeGuid);
      using (var session = new Sungero.Domain.Session())
      {
        return session.GetAll(entityType);
      }
    }
    
    /// <summary>
    /// Просклонять имя объекта.
    /// </summary>
    /// <param name="objectName">Имя объекта.</param>
    /// <param name="declensionCase">Склонение.</param>
    /// <returns>Имя объекта в заданном склонении.</returns>
    [Public, Remote(IsPure = true)]
    public virtual string DeclensionObject(string objectName, string declensionCase, string objectType)
    {
      if (string.IsNullOrWhiteSpace(objectName))
        return string.Empty;
      
      switch (declensionCase)
      {
          //Винительный
        case "Accusative":
          {
            if (objectType == "JobTitles")
              return Sungero.Content.Server.ElectronicDocumentFunctions.AccusativeJobTitle(objectName);
            else if (objectType == "Departments")
              return Sungero.Content.Server.ElectronicDocumentFunctions.AccusativeDepartmentTitle(objectName);
            else if (objectType == "People")
              return Sungero.Content.Server.ElectronicDocumentFunctions.AccusativePersonalFullName(objectName);
            else
              return Sungero.Content.Server.ElectronicDocumentFunctions.Accusative(objectName);
          }
          //Дательный
        case "Dative":
          {
            if (objectType == "JobTitles")
              return Sungero.Content.Server.ElectronicDocumentFunctions.DativeJobTitle(objectName);
            else if (objectType == "Departments")
              return Sungero.Content.Server.ElectronicDocumentFunctions.DativeDepartmentTitle(objectName);
            else if (objectType == "People")
              return Sungero.Content.Server.ElectronicDocumentFunctions.DativePersonalFullName(objectName);
            else
              return Sungero.Content.Server.ElectronicDocumentFunctions.Dative(objectName);
          }
          //Предложный
        case "Prepositional":
          {
            if (objectType == "JobTitles")
              return Sungero.Content.Server.ElectronicDocumentFunctions.PrepositionalJobTitle(objectName);
            else if (objectType == "Departments")
              return Sungero.Content.Server.ElectronicDocumentFunctions.PrepositionalDepartmentTitle(objectName);
            else if (objectType == "People")
              return Sungero.Content.Server.ElectronicDocumentFunctions.PrepositionalPersonalFullName(objectName);
            else
              return Sungero.Content.Server.ElectronicDocumentFunctions.Prepositional(objectName);
          }
          //Родительный
        case "Genitive":
          {
            if (objectType == "JobTitles")
              return Sungero.Content.Server.ElectronicDocumentFunctions.GenitiveJobTitle(objectName);
            else if (objectType == "Departments")
              return Sungero.Content.Server.ElectronicDocumentFunctions.GenitiveDepartmentTitle(objectName);
            else if (objectType == "People")
              return Sungero.Content.Server.ElectronicDocumentFunctions.GenitivePersonalFullName(objectName);
            else
              return Sungero.Content.Server.ElectronicDocumentFunctions.Genitive(objectName);
          }
          //Творительный
        case "Ablative":
          {
            if (objectType == "JobTitles")
              return Sungero.Content.Server.ElectronicDocumentFunctions.AblativeJobTitle(objectName);
            else if (objectType == "Departments")
              return Sungero.Content.Server.ElectronicDocumentFunctions.AblativeDepartmentTitle(objectName);
            else if (objectType == "People")
              return Sungero.Content.Server.ElectronicDocumentFunctions.AblativePersonalFullName(objectName);
            else
              return Sungero.Content.Server.ElectronicDocumentFunctions.Ablative(objectName);
          }
        default:
          return objectName;
      }
    }
        
  }
}