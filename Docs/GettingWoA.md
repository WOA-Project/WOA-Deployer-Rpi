# Getting Windows 10 ARM
1. Go to this site: https://uup.rg-adguard.net/
2. Enter these options:
![image](https://user-images.githubusercontent.com/3109851/51803240-f5528780-2252-11e9-92e8-2d80169e1131.png)
	* Please, notice the we selected the **ARM64** architecture (not to confuse with AMD64)
    * You can select a newer build if you wish. Take into account, however, that each build can have some issues. It's recommended to select builds from the R5+ branch. Build numbers like 18XXX are usually OK.
3. Click the link on the right. It will appear when you've done selecting the options. It will download a .zip file.
4. Extract the .zip file to a Folder, preferably one that doesn't contain spaces in the path, like "c:\temp\W10IsoScripts"
5. Execute the script **creatingISO.cmd** and wait for it to complete.
6. When the script has finished, you will find a **.iso ** inside, as a result of the process.
7. Mount the .iso file with Windows Explorer by double clicking it.
8. Navigate to the folder x:\sources, where **x:** is the drive letter of the mounted .iso file.
9. Inside this "sources" folder you will find the **install.wim** that WoA Installer needs to deploy Windows 😃
