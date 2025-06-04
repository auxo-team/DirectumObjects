using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Auxo.Common.ObjProperty;
using Sungero.Domain.Shared;
using Sungero.Metadata;

namespace Auxo.Common.Server
{
  partial class ObjPropertyFunctions
  {
    
    /// <summary>
    /// Получить все свойства сущности.
    /// </summary>
    /// <param name="objType">Тип сущности.</param>
    /// <returns>Свойства документа.</returns>
    [Public, Remote(IsPure=true)]
    public static IQueryable<IObjProperty> GetPropertiesByEntityType(IObjType objType)
    {
      if (objType == null)
        return null;
      
      return ObjProperties.GetAll()
        .Where(_ => Equals(_.ObjType, objType))
        .Where(_ => _.Status == Common.ObjProperty.Status.Active);
    }
    
    
    #region Логика создания и обновления записей
    
    /// <summary>
    /// Актуализировать свойства сущности.
    /// </summary>
    /// <param name="objType">Тип сущности.</param>
    [Public]
    public static void UpdateEntityProperties(IObjType objType)
    {
      if (objType == null)
        return;
      
      var metadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(Guid.Parse(objType.EntityGuid)).GetFinal();
      if (metadata == null)
        return;
      
      var createdProperties = ObjProperties.GetAll().Where(_ => Equals(_.ObjType, objType)).ToList();
      var properties = metadata.Properties;
      
      foreach (var propertyMetadata in properties)
      {
        var objProperty = createdProperties.Where(_ => _.PropertyGuid == propertyMetadata.NameGuid.ToString()).FirstOrDefault();
        
        if (objProperty != null)
          UpdateProperty(ref objProperty, propertyMetadata);
        else
          objProperty = CreateProperty(objType, propertyMetadata, null);
        
        if (propertyMetadata.PropertyType == Sungero.Metadata.PropertyType.Collection)
        {
          var collectionMetadata = (Sungero.Metadata.CollectionPropertyMetadata)propertyMetadata;
          var collectionProperties = collectionMetadata.InterfaceMetadata.Properties;
          foreach (var collectionPropertyMetadata in collectionProperties)
          {
            var collectionDocProperty = createdProperties.Where(_ => _.PropertyGuid == collectionPropertyMetadata.NameGuid.ToString()).FirstOrDefault();
            if (collectionDocProperty != null)
              UpdateProperty(ref collectionDocProperty, collectionPropertyMetadata);
            else
              collectionDocProperty = CreateProperty(objType, collectionPropertyMetadata, objProperty);
          }
          
          var collectionClosedProperties = createdProperties.Where(_ => !collectionProperties.Any(p => _.PropertyGuid == p.NameGuid.ToString())).Where(_ => _.Parent != null);
          foreach (var collectionClosedProperty in collectionClosedProperties)
            CloseProperty(collectionClosedProperty);
        }
      }
      
      var closedProperties = createdProperties.Where(_ => !properties.Any(p => _.PropertyGuid == p.NameGuid.ToString())).Where(_ => _.Parent == null);
      foreach (var closedProperty in closedProperties)
        CloseProperty(closedProperty);
    }
    
    /// <summary>
    /// Создать свойство.
    /// </summary>
    /// <param name="objType">Тип сущности.</param>
    /// <param name="propertyMetadata">Метаданные свойства.</param>
    /// <param name="parent">Родитель.</param>
    /// <returns>Свойство сущности.</returns>
    private static IObjProperty CreateProperty(IObjType objType,
                                               Sungero.Metadata.PropertyMetadata propertyMetadata,
                                               IObjProperty parent)
    {
      var localizedName = propertyMetadata.GetLocalizedName();
      var isNavigation = propertyMetadata is Sungero.Metadata.NavigationPropertyMetadata;
      if (parent != null && isNavigation && localizedName.ToString().StartsWith("Property_"))
        return null;
      
      var objProperty = ObjProperties.Create();
      objProperty.ObjType = objType;
      objProperty.PropertyGuid = propertyMetadata.NameGuid.ToString();
      objProperty.Name = propertyMetadata.Name;
      objProperty.LocalizedName = localizedName;
      objProperty.Parent = parent;
      objProperty.Status = objType.Status == Common.ObjType.Status.Active ? Common.ObjProperty.Status.Active : Common.ObjProperty.Status.Closed;
      
      //Параметры
      objProperty.IsRequired = propertyMetadata.IsRequired;
      objProperty.IsUnique = propertyMetadata.IsUnique;
      objProperty.IsEnabled = propertyMetadata.IsEnabled;
      objProperty.IsVisibility = propertyMetadata.IsVisibility;
      objProperty.IsShowedInList = propertyMetadata.IsShowedInList;
      objProperty.CanBeSearch = propertyMetadata.CanBeSearch;
      objProperty.IsSystem = !propertyMetadata.IsAvailableInEditorForm;
      objProperty.IsAvailableInIntegration = propertyMetadata.IsAvailableInIntegration;
      objProperty.IsDisplayValue = propertyMetadata.IsDisplayValue;
      
      if (propertyMetadata is Sungero.Metadata.StringPropertyMetadata)
      {
        var stringMetadata = (Sungero.Metadata.StringPropertyMetadata)propertyMetadata;
        objProperty.Length = stringMetadata.Length;
      }
      
      if (propertyMetadata is Sungero.Metadata.DateTimePropertyMetadata)
      {
        var dateTimeMetadata = (Sungero.Metadata.DateTimePropertyMetadata)propertyMetadata;
        var dateTimeFormat = dateTimeMetadata.DateTimeFormat.ToString();
        objProperty.PropertyType = new Enumeration(string.Format("Ax{0}", dateTimeFormat));
      }
      else
        objProperty.PropertyType = new Enumeration(string.Format("Ax{0}", propertyMetadata.PropertyType));

      if (isNavigation)
        objProperty.NavigationGuid = ((Sungero.Metadata.NavigationPropertyMetadata)propertyMetadata).EntityGuid.ToString();

      Functions.ObjProperty.FillEnumValues(objProperty, propertyMetadata);
      Functions.ObjProperty.FillAdditionalInfo(objProperty, propertyMetadata);
      
      objProperty.Save();
      return objProperty;
    }
    
    /// <summary>
    /// Обновить свойство.
    /// </summary>
    /// <param name="objProperty">Свойство сущности.</param>
    /// <param name="propertyMetadata">Метаданные свойства.</param>
    private static void UpdateProperty(ref IObjProperty objProperty, Sungero.Metadata.PropertyMetadata propertyMetadata)
    {
      if (objProperty.ObjTypeName != objProperty.ObjType.Name)
        objProperty.ObjTypeName = objProperty.ObjType.Name;
      
      if (objProperty.Name != propertyMetadata.Name)
        objProperty.Name = propertyMetadata.Name;
      
      var localizedName = propertyMetadata.GetLocalizedName();
      if (objProperty.LocalizedName != localizedName)
        objProperty.LocalizedName = localizedName;
      
      Sungero.Core.Enumeration? type = null;
      if (propertyMetadata is Sungero.Metadata.DateTimePropertyMetadata)
      {
        var dateTimeMetadata = (Sungero.Metadata.DateTimePropertyMetadata)propertyMetadata;
        var dateTimeFormat = dateTimeMetadata.DateTimeFormat.ToString();
        type = new Enumeration(string.Format("Ax{0}", dateTimeFormat));
      }
      else
        type = new Enumeration(string.Format("Ax{0}", propertyMetadata.PropertyType));

      if (objProperty.PropertyType != type)
        objProperty.PropertyType = type;
      
      if (propertyMetadata is Sungero.Metadata.StringPropertyMetadata)
        objProperty.Length = ((Sungero.Metadata.StringPropertyMetadata)propertyMetadata).Length;
      
      var entityGuidStr = string.Empty;
      if (propertyMetadata is Sungero.Metadata.NavigationPropertyMetadata)
        entityGuidStr = ((Sungero.Metadata.NavigationPropertyMetadata)propertyMetadata).EntityGuid.ToString();

      if (objProperty.NavigationGuid != entityGuidStr)
        objProperty.NavigationGuid = entityGuidStr;
      
      if (objProperty.IsRequired != propertyMetadata.IsRequired)
        objProperty.IsRequired = propertyMetadata.IsRequired;
      
      if (objProperty.IsUnique != propertyMetadata.IsUnique)
        objProperty.IsUnique = propertyMetadata.IsUnique;
      
      if (objProperty.IsEnabled != propertyMetadata.IsEnabled)
        objProperty.IsEnabled = propertyMetadata.IsEnabled;
      
      if (objProperty.IsVisibility != propertyMetadata.IsVisibility)
        objProperty.IsVisibility = propertyMetadata.IsVisibility;
      
      if (objProperty.IsShowedInList != propertyMetadata.IsShowedInList)
        objProperty.IsShowedInList = propertyMetadata.IsShowedInList;
      
      if (objProperty.CanBeSearch != propertyMetadata.CanBeSearch)
        objProperty.CanBeSearch = propertyMetadata.CanBeSearch;

      if (objProperty.IsAvailableInIntegration != propertyMetadata.IsAvailableInIntegration)
        objProperty.IsAvailableInIntegration = propertyMetadata.IsAvailableInIntegration;
      
      if (objProperty.IsDisplayValue != propertyMetadata.IsDisplayValue)
        objProperty.IsDisplayValue = propertyMetadata.IsDisplayValue;
      
      var status = objProperty.ObjType.Status == Common.ObjType.Status.Active ? Common.ObjProperty.Status.Active : Common.ObjProperty.Status.Closed;
      if (objProperty.Status != status)
        objProperty.Status = status;
      
      Functions.ObjProperty.FillEnumValues(objProperty, propertyMetadata);
      Functions.ObjProperty.FillAdditionalInfo(objProperty, propertyMetadata);
      
      if (objProperty.State.IsChanged)
        objProperty.Save();
    }
    
    /// <summary>
    /// Заполнить дополнительную информацию.
    /// </summary>
    /// <param name="propertyMetadata">Метаданные свойства.</param>
    public virtual void FillAdditionalInfo(Sungero.Metadata.PropertyMetadata propertyMetadata)
    {
      //TODO: Метод расширения функционала.
    }
    
    /// <summary>
    /// Закрыть свойство.
    /// </summary>
    /// <param name="objProperty">Свойство сущности.</param>
    private static void CloseProperty(IObjProperty objProperty)
    {
      objProperty.Status = Common.ObjProperty.Status.Closed;
      objProperty.Save();
      
      var childProperties = Functions.ObjProperty.GetChildProperties(objProperty);
      foreach (var childProperty in childProperties)
        CloseProperty(childProperty);
    }
    
    /// <summary>
    /// Получить дочерние свойства.
    /// </summary>
    /// <returns>Дочерние свойства.</returns>
    [Public, Remote(IsPure=true)]
    public virtual IQueryable<IObjProperty> GetChildProperties()
    {
      return ObjProperties.GetAll()
        .Where(_ => _.Status == Common.ObjProperty.Status.Active)
        .Where(_ => Equals(_.Parent, _obj));
    }

    /// <summary>
    /// Заполнить значения свойства с типом перечисление.
    /// </summary>
    /// <param name="propertyMetadata">Метаданные свойства.</param>
    public virtual void FillEnumValues(Sungero.Metadata.PropertyMetadata propertyMetadata)
    {
      if (propertyMetadata.PropertyType != Sungero.Metadata.PropertyType.Enumeration)
        return;
      
      var enumPropertyMetadata = (Sungero.Metadata.EnumPropertyMetadata)propertyMetadata;
      var directValues = enumPropertyMetadata.Values;
      foreach (var enumeration in directValues)
      {
        var line = _obj.EnumValues.Where(_ => _.Name == enumeration.Name).FirstOrDefault();
        if (line == null)
        {
          line = _obj.EnumValues.AddNew();
          line.Name = enumeration.Name;
          line.LocalizedValue = EnumLocalization.GetLocalizedValue(enumeration);
        }
        else
        {
          var localizedValue = EnumLocalization.GetLocalizedValue(enumeration);
          if (line.LocalizedValue != localizedValue)
            line.LocalizedValue = localizedValue;
        }
      }
      
      var removeNames = _obj.EnumValues.Select(_ => _.Name).Except(directValues.Select(_ => _.Name)).ToList();
      foreach (var removeName in removeNames)
      {
        var line = _obj.EnumValues.Where(_ => _.Name == removeName).FirstOrDefault();
        if (line != null)
          _obj.EnumValues.Remove(line);
      }
    }
    
    #endregion
    
  }
}