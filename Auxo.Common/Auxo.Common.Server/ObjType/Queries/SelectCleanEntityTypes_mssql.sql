SELECT 
  eType.id
FROM 
  auxo_nccmn_objtype As eType
LEFT JOIN [dbo].[Sungero_System_EntityType] As sType ON convert(nvarchar(36), sType.[TypeGuid]) = eType.entityguid
WHERE
	sType.[TypeGuid] IS NULL