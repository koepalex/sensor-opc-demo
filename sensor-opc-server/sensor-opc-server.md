# Precondition 

**On Raspberry Pi**
* Install vs debugger
  * curl -sSSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l ~/vsdbg
* Install .NET Core 3.1 
  * .NET 5.0 currently not support remote debugging, will be fixed with .NET 5.02
  
_Hint:_ Using `root` with password as described in many guides is not required anymore.

**On Windows**
* Install Putty on Windows
* Set environment variable `raspberry_pwd` to password of `pi` user 

# Debug Workflow

* cross compile assembly for arm linux
* publish arm linux assemblies
* using scp via ssh to copy content of publish folder to raspbery (folder /home/pi/app)
* launch remote debugger via ssh and start the app under /home/pi/app
  * path to dotnet 3.1: /home/pi/.dotnet/dotnet
  * path to dotnet 5.0: /opt/dotnet/dotnet