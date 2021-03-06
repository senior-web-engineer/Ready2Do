/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
:r .\Prepop\Prepop.Test.TipologieClienti.sql
:r .\Prepop\Prepop.Test.TipologieImmagini.sql
:r .\Prepop\Prepop.Test.Clienti.sql
:r .\Prepop\Prepop.Test.MailTemplates.sql
:r .\Prepop\Prepop.Test.TipologieNotifiche.sql

:r .\Prepop\Prepop.Security.Roles.sql
:r .\Prepop\Prepop.StatiPagamenti.sql

:r .\Prepop\Prepop.Utils.Date.sql