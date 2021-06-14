# azure-blob-storage-gallery
Simple app in .NET Core and Angular presenting using of Azure Blob Storage api.

Example command to add new migration (type in from project root directory location):
```
dotnet ef migrations add NewEntitiesMigration --project src/Web --startup-project src/Web --output-dir Migrations
```

Update database schema or create while first time execution:
```
dotnet ef database update --project src/Web --startup-project src/Web
```