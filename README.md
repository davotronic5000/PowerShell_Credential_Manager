# PowerShell Credential Manager
PowerShell Module to Read and Write Credentials from the Windows Credential Manager

## Ongoing Development and Support
I am no longer working on this project or PowerShell much at all.  If anyone else wants to take a fork and continue supporting this project. I would be happy to link to that project from here to guide people in the right direction.

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

### v2.0
- Implemented pipeline support for Get-StoredCredential.
- Implemented pipeline support for New-StoredCredential.
- Implemented pipeline support for Remove-StoredCredential.
- Improved error handling to respect the Error Action Preference in PowerShell.
- Changed AsPSCredential to a Switch parameter and renamed to AsCredentialObject on Get-StoredCredential to make it easier to use.
- Added Credentials parameter to New-StoredCredential which accepts a PSCredential object instead of User name and Password.
- Added SecuserPassword parameter to New-StoredCredential which accepts a SecureString as the password.
- Credential object now returns LastWritten as a DateTime instead of a ComType.FILETIME
- Changing license to MIT from GPL
- General refactoring and bug fixes. 

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


This software is licensed under the [The MIT License (MIT)](http://opensource.org/licenses/MIT).

	Copyright (C) 2016 Dave Garnar

	Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
