# dotnet-fciv
Dotnet CLI for File Checksum Integrity Verifier.

<br/>

### Install

```powershell
dotnet tool install dotnet-fciv -g
```

### Usage

```powershell
dotnet fciv FileOrDirectory [FilePatten] [Options]
```

For example:

```powershell
dotnet fciv c:\ *.txt -md5 -sha1
```

Supported hash algorithms:

| Options | Description                                                  |
| ------- | ------------------------------------------------------------ |
| -r      | Includes the current directory and all its subdirectories in a search operation. |
| -md5    | Compute the file using the **MD5** hash algorithm.           |
| -sha1   | Compute the file using the **SHA1** hash algorithm.          |
| -sha256 | Compute the file using the **SHA256** hash algorithm.        |
| -sha384 | Compute the file using the **SHA384** hash algorithm.        |
| -sha512 | Compute the file using the **SHA512** hash algorithm.        |

