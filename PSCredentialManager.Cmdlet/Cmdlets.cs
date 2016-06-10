using System;
using System.Text;
using System.Management.Automation;
using PSCredentialManager.Common.Enum;
using PSCredentialManager.Api;
using PSCredentialManager.Common;
using PSCredentialManager.Api.Extensions;
using PSCredentialManager.Common.Exceptions;
using System.Collections.Generic;
using System.Security;
using PSCredentialManager.Cmdlet.Extensions;

namespace PSCredentialManager.Cmdlet
{
    /// <summary>
    /// <para type="synopsis">Gets stored credentials from the Windows Credential Store/Vault</para>
    /// <para type="description">Gets stored credentials from the Windows Credential Store/Vault and returns as either a PSCredential object or as a Credential Object</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\> Get-StoredCredential -Target Server01</code>
    /// <para>Returns credentials for Server01 as a PSCredential object</para>
    /// <para>UserName                      Password</para>
    /// <para>--------                      --------</para>
    /// <para>test-user System.Security.SecureString</para>
    /// </example>
    /// <example>
    /// <code>PS C:\> Get-StoredCredential -Target Server01 -AsCredentialObject</code>
    /// <para>Returns credentials for Server01 as a Credential object</para>
    /// <para>Flags          : 0</para>
    /// <para>Type           : GENERIC</para>
    /// <para>TargetName     : server01</para>
    /// <para>Comment        : </para>
    /// <para>LastWritten    : 23/04/2016 10:01:37</para>
    /// <para>PaswordSize    : 18</para>
    /// <para>Password       : Password1</para>
    /// <para>Persist        : ENTERPRISE</para>
    /// <para>AttributeCount : 0</para>
    /// <para>Attributes     : 0</para>
    /// <para>TargetAlias    : </para>
    /// <para>UserName       : test-user</para>
    /// </example>
    /// <para type="link" uri="https://github.com/davotronic5000/PowerShell_Credential_Manager/wiki/Get-StoredCredential">Online Version</para>
    [Cmdlet(VerbsCommon.Get, "StoredCredential")]
    [OutputType(typeof(PSCredential))]
    [OutputType(typeof(Credential), ParameterSetName = new[]{"CredentialObject Output"})]
    public class GetStoredCredential : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The command will only return credentials with the specified target</para>
        /// </summary>
        [Parameter(ValueFromPipeline = true)]
        [ValidateLength(1, 337)]
        public string Target;

        /// <summary>
        /// <para type="description">Specifies the type of credential to return, possible values are [GENERIC, DOMAIN_PASSWORD, DOMAIN_CERTIFICATE, DOMAIN_VISIBLE_PASSWORD, GENERIC_CERTIFICATE,   DOMAIN_EXTENDED, MAXIMUM, MAXIMUM_EX]</para>
        /// </summary>
        [Parameter()]
        public CredType Type = CredType.Generic;

        /// <summary>
        /// <para type="description">Switch to return the credentials as Credential objects instead of the default PSObject</para>
        /// </summary>
        [Parameter(ParameterSetName = "CredentialObject Output")]
        public SwitchParameter AsCredentialObject;

        protected override void BeginProcessing()
        {
            
        }

        protected override void ProcessRecord()
        {
        CredentialManager credentialManager = new CredentialManager();

        if (Target == null)
            {
                //If no target is specified get all available credentials from the store
                IEnumerable<Credential> credential;
                try
                {
                    credential = credentialManager.ReadCred();

                    if (!AsCredentialObject)
                    {                     

                        //if AsPSCredential is specified create PS credential object and write to pipeline
                        WriteVerbose("Converting returned credentials to PSCredential Object");
                            
                        foreach (Credential cred in credential)
                        {

                            try
                            {
                                PSCredential psCredential = cred.ToPsCredential();
                                WriteObject(psCredential);
                            }
                            catch
                            {
                                WriteWarning("Unable to convert Credential object without username or password to PSCredential object");
                            }
                        }
                    }
                    else
                    {
                        //write credential object to pipeline
                        WriteObject(credential);
                    }
                }
                catch (Exception exception)
                {
                    ErrorRecord errorRecord = new ErrorRecord(exception, "1", ErrorCategory.InvalidOperation, Target);
                    WriteError(errorRecord);
                }
            }
            else
            {
                Credential credential = new Credential();
                bool shouldContinue = true;
                try
                {
                    //Retrieve credential from Cred Store
                    WriteVerbose("Retrieving requested credential from Windows Credential Manager");
                    credential = credentialManager.ReadCred(Target, Type);
                }
                catch (CredentialNotFoundException exception)
                {
                    WriteDebug(exception.Message);
                    shouldContinue = false;
                }
                catch (Exception exception)
                {
                    ErrorRecord errorRecord = new ErrorRecord(exception, "1", ErrorCategory.InvalidOperation, Target);
                    WriteError(errorRecord);
                }

                if (shouldContinue)
                {
                    if (!AsCredentialObject)
                    {
                        //if AsPSCredential is specified create PS credential object and write to pipeline
                        WriteVerbose("Converting returned credential blob to PSCredential Object");
                        try
                        {
                            PSCredential psCredential = credential.ToPsCredential();
                            WriteObject(psCredential);
                        }
                        catch
                        {
                            WriteWarning("Unable to convert Credential object without username or password to PSCredential object");
                        }
                    }
                    else
                    {
                        //write credential object to pipeline
                        WriteObject(credential);
                    }
                }

            }
        }

        protected override void EndProcessing() { }
    }

    /// <summary>
    /// <para type="synopsis">Create a new credential in the Windows Credential Store/Vault</para>
    /// <para type="description">Create a new credential in the Windows Credential Store/Vault</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\> New-StoredCredential -Target server01 -UserName test-user -Password Password1</code>
    /// <para>creates a credential for server01 with the username test-user and password Password1</para>
    /// <para>Flags          : 0</para>
    /// <para>Type           : GENERIC</para>
    /// <para>TargetName     : server01</para>
    /// <para>Comment        : Updated by: Dave on: 23/04/2016</para>
    /// <para>LastWritten    : 23/04/2016 10:48:56</para>
    /// <para>PaswordSize    : 18</para>
    /// <para>Password       : Password1</para>
    /// <para>Persist        : SESSION</para>
    /// <para>AttributeCount : 0</para>
    /// <para>Attributes     : 0</para>
    /// <para>TargetAlias    : </para>
    /// <para>UserName       : test-user</para>
    /// </example>
    /// <example>
    /// <code>PS C:\> Get-Credential -UserName test-user -Message "Password please" | New-StoredCredential -Target Server01</code>
    /// <para>Creates a credential for Server01 with the username and password provided in the PSCredential object from Get-Credential</para>
    /// </example>
    /// <para type="link" uri="https://github.com/davotronic5000/PowerShell_Credential_Manager/wiki/New-StoredCredential">Online Version</para>
    [Cmdlet(VerbsCommon.New, "StoredCredential")]
    [OutputType(typeof(Credential))]
    public class NewStoredCredential : PSCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the target of the credentials being added.</para>
        /// </summary>
        [Parameter()]
        [ValidateLength(1, 337)]
        public string Target = Environment.MachineName;

        /// <summary>
        /// <para type="description">specified the username to be used for the credentials, cannot be used in conjunction with Credentials parameter.</para>
        /// </summary>
        [Parameter(ParameterSetName = "Plain Text")]
        [Parameter(ParameterSetName = "Secure String")]
        public string UserName = Environment.UserName;

        /// <summary>
        /// <para type="description">Specifies the password in plain text, cannot be used in conjunction with SecurePassword or Credential parameters.</para>
        /// </summary>
        [Parameter(ParameterSetName = "Plain Text")]
        public string Password = System.Web.Security.Membership.GeneratePassword(10, 2);

        /// <summary>
        /// <para type="description">Specifies the password as a secure string, cannot be used in conjunction with SecurePassword or Credential parameters.</para>
        /// </summary>
        [Parameter(ParameterSetName = "Secure String")]
        public SecureString SecurePassword;

        /// <summary>
        /// <para type="description">Provides a comment to identify the credentials in the store</para>
        /// <para type="description"></para>
        /// </summary>
        [Parameter()]
        public string Comment = "Updated by: " + Environment.UserName + " on: " + DateTime.Now.ToShortDateString();

        /// <summary>
        /// <para type="description">Type of credential to store, possible values are [GENERIC, DOMAIN_PASSWORD, DOMAIN_CERTIFICATE, DOMAIN_VISIBLE_PASSWORD, GENERIC_CERTIFICATE,   DOMAIN_EXTENDED, MAXIMUM, MAXIMUM_EX]</para>
        /// </summary>
        [Parameter()]
        public CredType Type = CredType.Generic;

        /// <summary>
        /// <para type="description">sets the persistence settings of the credential, possible values are [SESSION, LOCAL_MACHINE, ENTERPRISE]</para>
        /// </summary>
        [Parameter()]
        public CredPersist Persist = CredPersist.Session;

        /// <summary>
        /// <para type="description"></para>
        /// </summary>
        [Parameter(ValueFromPipeline = true, ParameterSetName = "PSCredentialObject")]
        public PSCredential Credentials;

        /// <summary>
        /// <para type="description"></para>
        /// </summary>
        protected override void BeginProcessing()
        {
            //Check if Password or SecurePassword is set and validate the password is valid.

            if (MyInvocation.BoundParameters.ContainsKey("Password"))
            {
                //Argument validation
                //Check password does not exceed 512 bytes
                byte[] byteArray = Encoding.Unicode.GetBytes(Password);
                if (byteArray.Length > 512)
                {
                    Exception exception = new ArgumentOutOfRangeException($"Password", "The specified password has exceeded 512 bytes");
                    ErrorRecord error = new ErrorRecord(exception, "1", ErrorCategory.InvalidArgument, Password);
                    WriteError(error);
                }
            }

            if (MyInvocation.BoundParameters.ContainsKey("SecurePassword"))
            {
                //Argument validation
                //Check password does not exceed 512 bytes
                byte[] byteArray = Encoding.Unicode.GetBytes(Password);
                if (byteArray.Length > 512)
                {
                    Exception exception = new ArgumentOutOfRangeException($"SecurePassword", "The specified password has exceeded 512 bytes");
                    ErrorRecord error = new ErrorRecord(exception, "1", ErrorCategory.InvalidArgument, Password);
                    WriteError(error);
                }
                Password = SecurePassword.ToInsecureString();
            }
        }

        protected override void ProcessRecord()
        {
            if (MyInvocation.BoundParameters.ContainsKey("Credentials"))
            {
                UserName = Credentials.UserName;
                Password = Credentials.GetNetworkCredential().Password;
            }

            //Create credential object
            Credential credential = new Credential()
            {
                TargetName = Target,
                Password = Password,
                PaswordSize = (UInt32)Encoding.Unicode.GetBytes(Password).Length,
                AttributeCount = 0,
                Attributes = IntPtr.Zero,
                Comment = Comment,
                TargetAlias = null,
                Type = Type,
                Persist = Persist,
                UserName = UserName,
                LastWritten = DateTime.Now,                    
            };
            
                

            //Convert credential to native credential
            NativeCredential nativeCredential = credential.ToNativeCredential();
            CredentialManager credentialManager = new CredentialManager();

            try
            {
                //Write credential to Windows Credential manager
                WriteVerbose("Writing credential to Windows Credential Manager");
                credentialManager.WriteCred(nativeCredential);
                WriteObject(credential);
            }
            catch (Exception exception)
            {
                ErrorRecord errorRecord = new ErrorRecord(exception, "1", ErrorCategory.InvalidOperation, credential);
                WriteError(errorRecord);
            }
        }

        protected override void EndProcessing() { }
    }

    /// <summary>
    /// <para type="synopsis">Deletes a credentials from the Windows Credential Store/Vault</para>
    /// <para type="description">Deletes a credentials from the Windows Credential Store/Vault</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\> Remove-StoredCredential -Target Server01 -Type GENERIC</code>
    /// <para>Deletes a generic credential with the target Server01</para>
    /// </example>
    /// <para type="link" uri="https://github.com/davotronic5000/PowerShell_Credential_Manager/wiki/Remove-StoredCredential">Online Version</para>
    [Cmdlet(VerbsCommon.Remove, "StoredCredential")]
    public class RemoveStoredCredential : PSCmdlet
    {
        /// <summary>
        /// <para type="description">specifies a target to identitfy the credential to be deleted</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string Target;

        /// <summary>
        /// <para type="description">Specifies the type of credential to be deleted, possible values are [GENERIC, DOMAIN_PASSWORD, DOMAIN_CERTIFICATE, DOMAIN_VISIBLE_PASSWORD, GENERIC_CERTIFICATE,   DOMAIN_EXTENDED, MAXIMUM, MAXIMUM_EX]</para>
        /// </summary>
        [Parameter()]
        public CredType Type = CredType.Generic;


        protected override void BeginProcessing() { }

        protected override void ProcessRecord()
        {
        CredentialManager credentialManager = new CredentialManager();

        //Delete credential from store
        try
        {
                WriteVerbose("Deleting requested credential from Windows Credential Manager");
                credentialManager.DeleteCred(Target, Type);
            }
            catch (Exception exception)
            {
                ErrorRecord errorRecord = new ErrorRecord(exception, "1", ErrorCategory.InvalidOperation, Target);
                WriteError(errorRecord);
            }
        }

        protected override void EndProcessing() { }
    }

    /// <summary>
    /// <para type="synopsis">Generates a strong password</para>
    /// <para type="description">Generates a strong password based on the parameters provided</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\> Get-StrongPassword</code>
    /// <para>Generates a password 10 characters long with 3 special characters</para>
    /// <para>QTJ(T?wwe)</para>
    /// </example>
    /// <example>
    /// <code>PS C:\> Get-StrongPassword  -Length 20 -NumberOfSpecialCharacters 5</code>
    /// <para>Generates a password 20 characters long with 5 special characters</para>
    /// <para>zPN&gt;C%.f/(l1aGq)n3Ze</para>
    /// </example>
    /// <list type="alertSet">
    /// <item>
    /// <description>This cmdlet does not do a lot currently, it only really generates a password based on System.Web.Security.Membership.GeneratePassword().</description>
    /// </item>
    /// </list>
    /// <para type="link" uri="https://github.com/davotronic5000/PowerShell_Credential_Manager/wiki/Get-StrongPassword">Online Version</para>
    [Cmdlet(VerbsCommon.Get, "StrongPassword")]
    [OutputType(typeof(string))]
    public class GetStrongPassword : PSCmdlet
    {
        /// <summary>
        /// <para type="description">Length in Characters for the generated password to be.</para>
        /// </summary>
        [Parameter()]
        public int Length = 10;

        /// <summary>
        /// <para type="description">Number of special characters to include in the password, must be less than the length of the password</para>
        /// </summary>
        [Parameter()]
        public int NumberOfSpecialCharacters = 3;

        protected override void BeginProcessing() { }

        protected override void ProcessRecord()
        {
            WriteObject(System.Web.Security.Membership.GeneratePassword(Length, NumberOfSpecialCharacters));
        }

        protected override void EndProcessing() { }
    }
}