# api-workflow-demo

This project has been **developed and tested only** in a **RelativityOne** instance. 


To enter your **RelativityOne Instance details**, follow the below instructions:
- Enter **Tenant** name, **Relativity Admin username** and **password** in the **Constants.cs** class under **E2EEDRM.Helpers** project
  
To use the **Transfer REST API feature**, follow the below instructions:
- Contact Relativity Support to setup **Blobfuse** in your RelativityOne instance. 
- Enter the Azure Blob storage details under **AzureSecrets** class in in the **Constants.cs** class under **E2EEDRM.Helpers** project

To use the **Export Production feature**, follow the below instructions: 
- This feature is currently only compatible for 10.2.99.73 and above
- In this project we have provided the RDC installer for 10.2.99.73
- If you are using a version higher than this contact support@relativity.com to get the installer for the RDC
- If a previous version of the Relativity Desktop Client (RDC) is installed on your machine then uninstall that version
- In the project, double click kCura.WinEDDS64.Setup.msi executeable
    - Follow the instructions on the Setup Wizard to complete the install

If using this workflow on a **Dev VM or an on-prem** Relativity Instance, please keep in mind the following:
- You don't have to use the Transfer API feature, you can copy the files directly to the Processing Source location.
- You don't have to setup Blobfuse.



