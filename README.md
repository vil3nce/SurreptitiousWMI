## Function

The purpose of this code is to establish persistance using WMI, while avoiding detection from Sysmon and Windows Security event logs. 

This is accomplished by creating an arbitrary namespace and placing the permanent event subscription which will effect persistance inside.

The technique used was originally discoverd by Matt Graeber and Lee Christenson, and [published here](https://specterops.io/assets/resources/Subverting_Sysmon.pdf)
---

## Usage

Currently the event subscription will just launch calc as a child process of scrcons.exe every time a new win32 process is started. To make use of this code you will need to:

- Change the vbscript to something usefull.
- Change the WQL filter to something practical.
- Compile, and run as admin.
