# redump2xiso
Converts XBOX and XBOX360 isos from redump format to xiso format and vice versa.  

## Usage  
The input format is determined automatically and does not need to be specified.

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
