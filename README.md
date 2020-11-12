## Purpose

This tool creates a WMI permanent event subscription to establish persistance, while avoiding detection from Sysmon and Windows Security event logs. 

This is accomplished by creating an arbitrary namespace and placing the permanent event subscription which will effect persistance inside.

The technique used was originally discoverd by Matt Graeber and Lee Christenson, and [published here](https://specterops.io/assets/resources/Subverting_Sysmon.pdf)

---

## Usage

Currently the event subscription will just launch calc as a child process of scrcons.exe every time a new win32 process is started. To make use of this code you will need to:

- Change the vbscript to something usefull.
- Change the WQL filter to something practical.
- You'll probably want to remove the console printing
- Compile, and run as admin.

## ToDo

- <strike>Check for the existance of a namespace with the same name before I try to create one
- Check for permanent event subscription componants with the same names before I try to create new ones
- Make ActiveScriptConsumer class creation an indpendant function</strike>
- Make function(s) to derive other event consumers
- Store the vbscript in a way that is not human readable
- Add lateral movement capabilities
