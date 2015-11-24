devopsflex
==========

DevOps support for TFS driven teams using TFS Build and Deploying to Azure.

Although not it's primary purpose, it also contains a solution with a module for deployment commandlets that facilitate CRM deployments.

## Commandlets in the main module

### Push-CloudServices
- Deploys Azure Cloud Services in parallel.
- Supports VIP swaps during deployment and supports the deletion of the staging deployment after swapping.
- Can apply WAD definitions automatically to deployments, even if there's no deployment present.

### Push-DevOpsFlexConfiguration
Provisions PaaS resources on the fly based on a system spec defined in an EntityFramework database, a system configuration and a development branch.

The idea is to automate PaaS provisioning during common development activities like branching in order to facilitate the creation of continuous deployment release pipelines.

Works for:

- Azure Cloud Services (supports provisioning of ReservedIPs)
- Azure SQL Databases (supports adding database users during provisioning)
- Azure Storage Containers (creates specific access keys during provisioning)
- Azure Service Bus namespaces
- Azure Web Apps

## Commandlets in the CRM module

### New-XrmOrganisation

Re-creates a CRM organisation. If it previously exists, before recreating it will:

- Disable the Organisation
- Delete the Organisation
- Delete the Organisation database in the SQL server

## Roslyn Analyzers

These are Roslyn analyzers that focus on problems that code creates for DevOps teams.
They are not in any way best coding practices.

### NuGet Packages consolidation

This analyzer tests two things in the same pass:

- That the assembly only contains references to a single packages folder.
- That all references in the assembly only contain a unique version in the packages folder.