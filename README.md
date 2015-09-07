# PowerShell Credential Manager
PowerShell Module to Read and Write Credentials from the Windows Credential Manager

## Installation
###PowerShell Gallery Installation
The module is available on the PowerShell Gallery: https://www.powershellgallery.com/packages/CredentialManager/1.0.0.

1. PS> Save-Module -Name CredentialManager -Path <path>
2. PS> Install-Module -Name CredentialManager

###Manual Installation

1. Dowload the latest verion of the module code from https://github.com/davotronic5000/PowerShell_Credential_Manager/releases
2. Unzip CredentialManager.zip and copy the contents to you preferred module path. Usually C:\Users\$UserName\Documents\WindowsPowerShell\Modules.
3. In your PowerShell session run the command Import-Module CredentialManager

## Usage

Import the module in to your PowerShell session and full help is available in the module with Get-Help

## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## History

### v1.1 Bug Fix
Fixed a bug where the username specified in the -UserName parameter was not being used to create the credential in the store. The username for the logged on user was being used instead. Issue logged https://github.com/davotronic5000/PowerShell_Credential_Manager/issues/8


### v1.0 Initial Release
Implementing basic functionality
- Get-StoredCredential - Gets one or more credentials from the Windows Credential Manager.
- New-Stored Credential - Adds a new credential to the Windows Credential Manager.
- Remove-StoredCredential - Deletes a credential from the Windows Credential Manager.
- Get-StrongPassword - Randomly generates a new password.

## Credits

Written by Dave Garnar (@davotronic5000)
http://blog.davotronic5000.co.uk

## License

GNU GENERAL PUBLIC LICENSE
