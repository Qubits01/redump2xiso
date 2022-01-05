# redump2xiso
Converts OG xbox isos from redump format to xiso format and vice versa

## Usage
**redump -> xiso**
```
redump2xiso.exe <.iso file>
```
If you want to create a file of the video partition (needed to convert back to redump) specify the output name as a 2nd parameter:
```
redump2xiso.exe <.iso file> <video partition file>
```
**xiso -> redump**
You have to explicitly enter the input xiso file and the video partition file.
```
redump2xiso.exe <.iso file> <video partition file>
```
