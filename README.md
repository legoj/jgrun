# jgrun

### introduction
*jgrun* utility is a commandline program to run another program on different user credential, i.e. as a *runas* command alternative. runas command doesnt allow you to specify the user's password and thus couldnt be used for task automations. jgrun allows you to specify a clear-text password or an encrypted one.

*Usage:*

>`$>jgrun [exePath] [options] [domain] [username] [password] [encKey]`
> 
> Parameters:
>  - exePath  - complete path of the file to be executed 
>  - options  - commandline arguments 
>  - domain   - user domain  
>  - username - user name or id 
>  - password - user password. if encrypted, specify the generated encKey 
>  - encKey   - base64-string encryption key used to encrypt the password 
  
*jgrun* has the function to encrypt the password with your specified 32byte (256bit) AES key and returns the both the encrypted password and key in base64 string format which will be used as the password and encKey parameters. to generate the encrypted password and key, follow the usage below:

>`$>jgrun [password] [encKey]`
> 
> Parameters:
>  - password  - clear-type password of the user 
>  - encKey    - 256bit (32byte) AES key

*Sample:*
- Generating the encrypted password and key
> `$>jgrun  myPassW0rd My32BytePersonalEncryptionAESKey`
>
> this command returns the following:
>
> `WdWRzosAZpi0xj0XYtaVuw== LEkFVHNARQEyVRFAW1xSDydWAEIaQ0MNWV51dWByVEE=`

- Running a powershell script (c:\scripts\mytask.ps1) using the encrypted password and key for domain user id *foo/taro*
> `$>jgrun "powershell" "c:\scripts\mytask.ps1" "foo" "taro" "WdWRzosAZpi0xj0XYtaVuw==" "LEkFVHNARQEyVRFAW1xSDydWAEIaQ0MNWV51dWByVEE=" `
>
> if the user id is a local user account, specify the computername or use the env variable %COMPUTERNAME% as your domain name.

### security matters: 
simple XOR operation and AES-based encryption is used to encrypt the password and the key before converting them to base64 string. it utilizes the module GUID attribute as the other 32byte encryption key.  
