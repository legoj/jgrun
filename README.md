# jgrun
Commandline program to run another program on the different process and user credential. 

## background
there are several situations where you need to run tasks in the background with no user interaction on a different user accounts e.g. task automations, testing etc. this utility was created to replace the *runas* command and allows you to specify a clear-text password or an encrypted one on the commandline. it doesnt have any GUI or other more functions like other available runas alternatives in the internet; and is designed to be made simple and small in footprint.

### Usage:

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

### Samples:
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
simple XOR operation and AES-based encryption is used to encrypt the password and the key before converting them to base64 string. it utilizes the module GUID attribute as the other 32byte encryption key. it can be modified to use other parameters like the domain and user info into the encryption flow; as well as using system unique properties like machine UUID or SID to limit its validity on that system.

also note that the released binary is not the obfuscated build. there are several .Net obfuscation tools which you can use if you need to distribute the module.
