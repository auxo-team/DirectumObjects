SELECT 
  eType.id
FROM 
  auxo_nccmn_objtype As eType
LEFT JOIN Sungero_System_EntityType As sType ON sType.TypeGuid::text = eType.entityguid
WHERE
	sType.TypeGuid IS NULL