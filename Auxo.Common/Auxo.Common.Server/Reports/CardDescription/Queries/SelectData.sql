Select 
  GroupNum, PropertyName, TypeInfo, IsRequired, IsEnabled, IsVisibility, CanBeSearch, ValuesInfo
FROM 
  Auxo_ReportCardDescription
Where
  ReportSessionId = @ReportSessionId